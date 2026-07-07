using System.Net.Http.Json;
using System.Text.Json;
using Confluent.Kafka;

var baseUrl = Environment.GetEnvironmentVariable("SIMULATOR_BASE_URL")
    ?? Environment.GetEnvironmentVariable("BACKEND_BASE_URL")
    ?? "http://localhost:5078";

var bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")
    ?? "localhost:9092";

var machineStatusTopic = Environment.GetEnvironmentVariable("KAFKA_MACHINE_STATUS_TOPIC")
    ?? "machine.status";

var productionTopic = Environment.GetEnvironmentVariable("KAFKA_PRODUCTION_TOPIC")
    ?? "production.counts";

var downtimeTopic = Environment.GetEnvironmentVariable("KAFKA_DOWNTIME_TOPIC")
    ?? "downtime.events";

var telemetryTopic = Environment.GetEnvironmentVariable("KAFKA_TELEMETRY_TOPIC")
    ?? "machine.telemetry";

var intervalMs = ReadInt("SIMULATOR_INTERVAL_MS", 4000);
var iterations = ReadInt("SIMULATOR_ITERATIONS", 0);

var seedFirst = !string.Equals(
    Environment.GetEnvironmentVariable("SIMULATOR_SKIP_SEED"),
    "true",
    StringComparison.OrdinalIgnoreCase);

ValidateConfiguration(
    baseUrl,
    bootstrapServers,
    machineStatusTopic,
    productionTopic,
    downtimeTopic,
    telemetryTopic,
    intervalMs);

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cts.Cancel();
};

AppDomain.CurrentDomain.ProcessExit += (_, _) =>
{
    cts.Cancel();
};

var stoppingToken = cts.Token;

using var http = new HttpClient
{
    BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
    Timeout = TimeSpan.FromSeconds(10)
};

using var producer = new ProducerBuilder<Null, string>(new ProducerConfig
{
    BootstrapServers = bootstrapServers,
    AllowAutoCreateTopics = true,
    Acks = Acks.All,
    EnableIdempotence = true,
    MessageTimeoutMs = 10000,
    RequestTimeoutMs = 5000
}).Build();

Console.WriteLine($"Smart Factory simulator targeting {http.BaseAddress}");
Console.WriteLine($"Kafka bootstrap servers: {bootstrapServers}");

try
{
    await WaitForBackendAsync(http, stoppingToken);
    await WaitForKafkaAsync(producer, stoppingToken);

    if (seedFirst)
    {
        await ExecuteWithRetryAsync(async () =>
        {
            await EnsureSeedAsync(http, stoppingToken);
            return true;
        }, "seeding backend data", stoppingToken);
    }

    var activeWorkOrderId = await ExecuteWithRetryAsync(
        async () =>
        {
            var workOrderId = await ResolveActiveWorkOrderIdAsync(http, stoppingToken);

            if (string.IsNullOrWhiteSpace(workOrderId))
            {
                throw new InvalidOperationException("No active work order was returned by the backend.");
            }

            return workOrderId;
        },
        "reading active work order",
        stoppingToken);

    var machineIds = new[] { "SMT-01", "AOI-01", "ASSEMBLY-01", "TEST-01", "PACKING-01" };
    var cycle = 0;

    while (!stoppingToken.IsCancellationRequested && (iterations <= 0 || cycle < iterations))
    {
        cycle++;

        var partId = $"PCB-{DateTime.UtcNow:yyyyMMddHHmmss}-{cycle:000}";
        Console.WriteLine($"Cycle {cycle}: simulating {partId} on {activeWorkOrderId}");

        foreach (var machineId in machineIds)
        {
            await PublishAsync(
                producer,
                machineStatusTopic,
                new MachineStatusMessage(
                    machineId,
                    "RUNNING",
                    activeWorkOrderId,
                    "Simulator heartbeat",
                    DateTimeOffset.UtcNow),
                stoppingToken);
        }

        foreach (var machineId in machineIds)
        {
            var anomalyMode = Random.Shared.NextDouble() < 0.18;

            var telemetry = new MachineTelemetryMessage(
                EventId: Guid.NewGuid().ToString("N"),
                MachineId: machineId,
                WorkOrderId: activeWorkOrderId,
                Timestamp: DateTimeOffset.UtcNow,
                TemperatureC: anomalyMode
                    ? Random.Shared.Next(86, 98)
                    : Random.Shared.Next(55, 78),
                VibrationMmS: anomalyMode
                    ? Math.Round((decimal)(Random.Shared.NextDouble() * 3.0 + 5.2), 2)
                    : Math.Round((decimal)(Random.Shared.NextDouble() * 2.0 + 1.5), 2),
                CycleTimeSeconds: anomalyMode
                    ? Random.Shared.Next(42, 62)
                    : Random.Shared.Next(24, 36),
                ErrorCount: anomalyMode
                    ? Random.Shared.Next(3, 8)
                    : Random.Shared.Next(0, 2),
                ScrapRate: anomalyMode
                    ? Math.Round((decimal)(Random.Shared.NextDouble() * 0.08 + 0.08), 4)
                    : Math.Round((decimal)(Random.Shared.NextDouble() * 0.04), 4));

            await PublishAsync(producer, telemetryTopic, telemetry, stoppingToken);
        }

        await PublishAsync(
            producer,
            productionTopic,
            new ProductionEventMessage(
                "SMT-01",
                activeWorkOrderId,
                "GOOD_PART",
                1,
                partId,
                null,
                "SMT placement completed"),
            stoppingToken);

        var aoiScrap = Random.Shared.NextDouble() < 0.18;

        await PublishAsync(
            producer,
            productionTopic,
            aoiScrap
                ? new ProductionEventMessage(
                    "AOI-01",
                    activeWorkOrderId,
                    "SCRAP_PART",
                    1,
                    partId,
                    "SOLDER_BRIDGE",
                    "AOI inspection detected a defect")
                : new ProductionEventMessage(
                    "AOI-01",
                    activeWorkOrderId,
                    "GOOD_PART",
                    1,
                    partId,
                    null,
                    "AOI inspection passed"),
            stoppingToken);

        await PublishAsync(
            producer,
            productionTopic,
            new ProductionEventMessage(
                "ASSEMBLY-01",
                activeWorkOrderId,
                "GOOD_PART",
                1,
                partId,
                null,
                "Assembly completed"),
            stoppingToken);

        await PublishAsync(
            producer,
            productionTopic,
            new ProductionEventMessage(
                "TEST-01",
                activeWorkOrderId,
                "GOOD_PART",
                1,
                partId,
                null,
                "Functional test passed"),
            stoppingToken);

        await PublishAsync(
            producer,
            productionTopic,
            new ProductionEventMessage(
                "PACKING-01",
                activeWorkOrderId,
                "GOOD_PART",
                1,
                partId,
                null,
                "Packed for shipment"),
            stoppingToken);

        if (Random.Shared.NextDouble() < 0.25)
        {
            await PublishAsync(
                producer,
                machineStatusTopic,
                new MachineStatusMessage(
                    "TEST-01",
                    "DOWN",
                    activeWorkOrderId,
                    "Automated downtime from simulator",
                    DateTimeOffset.UtcNow),
                stoppingToken);

            await PublishAsync(
                producer,
                downtimeTopic,
                new DowntimeEventMessage(
                    "TEST-01",
                    activeWorkOrderId,
                    "EQUIPMENT_FAILURE",
                    12,
                    DateTimeOffset.UtcNow.AddMinutes(-12),
                    "Automated downtime from simulator"),
                stoppingToken);

            await PublishAsync(
                producer,
                machineStatusTopic,
                new MachineStatusMessage(
                    "TEST-01",
                    "RUNNING",
                    activeWorkOrderId,
                    "Recovered after downtime",
                    DateTimeOffset.UtcNow),
                stoppingToken);
        }

        await Task.Delay(intervalMs, stoppingToken);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Simulator shutdown requested.");
}
finally
{
    producer.Flush(TimeSpan.FromSeconds(10));
}

Console.WriteLine("Simulator finished.");

static int ReadInt(string name, int defaultValue)
{
    var value = Environment.GetEnvironmentVariable(name);
    return int.TryParse(value, out var parsed) ? parsed : defaultValue;
}

static void ValidateConfiguration(
    string? baseUrl,
    string? bootstrapServers,
    string? machineStatusTopic,
    string? productionTopic,
    string? downtimeTopic,
    string? telemetryTopic,
    int intervalMs)
{
    if (intervalMs <= 0)
    {
        throw new InvalidOperationException("SIMULATOR_INTERVAL_MS must be greater than zero.");
    }

    var requiredValues = new Dictionary<string, string?>
    {
        ["BACKEND_BASE_URL/SIMULATOR_BASE_URL"] = baseUrl,
        ["KAFKA_BOOTSTRAP_SERVERS"] = bootstrapServers,
        ["KAFKA_MACHINE_STATUS_TOPIC"] = machineStatusTopic,
        ["KAFKA_PRODUCTION_TOPIC"] = productionTopic,
        ["KAFKA_DOWNTIME_TOPIC"] = downtimeTopic,
        ["KAFKA_TELEMETRY_TOPIC"] = telemetryTopic
    };

    foreach (var item in requiredValues)
    {
        if (string.IsNullOrWhiteSpace(item.Value))
        {
            throw new InvalidOperationException($"{item.Key} is required.");
        }
    }
}

static async Task WaitForBackendAsync(HttpClient http, CancellationToken stoppingToken)
{
    await ExecuteWithRetryAsync(async () =>
    {
        using var response = await http.GetAsync("health", stoppingToken);
        response.EnsureSuccessStatusCode();
        return true;
    }, "waiting for backend health", stoppingToken);
}

static async Task WaitForKafkaAsync(IProducer<Null, string> producer, CancellationToken stoppingToken)
{
    await ExecuteWithRetryAsync(() =>
    {
        stoppingToken.ThrowIfCancellationRequested();
        producer.GetMetadata(TimeSpan.FromSeconds(10));
        return Task.FromResult(true);
    }, "waiting for Kafka", stoppingToken);
}

static async Task EnsureSeedAsync(HttpClient http, CancellationToken stoppingToken)
{
    await PostAndLogAsync(http, "api/workorders/seed", stoppingToken);
    await PostAndLogAsync(http, "api/machines/seed", stoppingToken);
}

static async Task<string?> ResolveActiveWorkOrderIdAsync(HttpClient http, CancellationToken stoppingToken)
{
    var workOrders = await http.GetFromJsonAsync<List<WorkOrderDto>>(
        "api/workorders/active",
        cancellationToken: stoppingToken);

    return workOrders?.FirstOrDefault()?.WorkOrderId;
}

static async Task<T> ExecuteWithRetryAsync<T>(
    Func<Task<T>> action,
    string description,
    CancellationToken stoppingToken,
    int maxAttempts = 60,
    int delayMilliseconds = 2000)
{
    Exception? lastException = null;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        stoppingToken.ThrowIfCancellationRequested();

        try
        {
            return await action();
        }
        catch (Exception exception) when (
            exception is HttpRequestException ||
            exception is TaskCanceledException ||
            exception is KafkaException ||
            exception is InvalidOperationException)
        {
            lastException = exception;

            Console.WriteLine(
                $"  waiting for {description} (attempt {attempt}/{maxAttempts}): {exception.Message}");

            await Task.Delay(delayMilliseconds, stoppingToken);
        }
    }

    throw new InvalidOperationException($"Timed out while {description}.", lastException);
}

static async Task PostAndLogAsync(HttpClient http, string path, CancellationToken stoppingToken)
{
    using var response = await http.PostAsync(path, null, stoppingToken);
    var body = await response.Content.ReadAsStringAsync(stoppingToken);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"  OK {path}: {body}");
        return;
    }

    throw new HttpRequestException(
        $"Seed endpoint failed: {path} returned {(int)response.StatusCode} {response.ReasonPhrase} {body}");
}

static async Task PublishAsync<T>(
    IProducer<Null, string> producer,
    string topic,
    T payload,
    CancellationToken stoppingToken)
{
    await ExecuteWithRetryAsync(async () =>
    {
        var serializedPayload = JsonSerializer.Serialize(payload, JsonOptions());

        var deliveryResult = await producer.ProduceAsync(
            topic,
            new Message<Null, string>
            {
                Value = serializedPayload
            },
            stoppingToken);

        Console.WriteLine($"  published {topic} to {deliveryResult.TopicPartitionOffset}: {serializedPayload}");

        return true;
    }, $"publishing to Kafka topic {topic}", stoppingToken, maxAttempts: 5, delayMilliseconds: 1000);
}

static JsonSerializerOptions JsonOptions() => new()
{
    PropertyNameCaseInsensitive = true,
    WriteIndented = false
};

public record WorkOrderDto(
    string WorkOrderId,
    string ProductCode,
    string LineId,
    int PlannedQuantity,
    int GoodCount,
    int ScrapCount,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt
);

public record MachineStatusMessage(
    string MachineId,
    string Status,
    string? CurrentWorkOrderId,
    string? Notes,
    DateTimeOffset ReportedAt
);

public record ProductionEventMessage(
    string MachineId,
    string WorkOrderId,
    string EventType,
    int Quantity,
    string? PartId,
    string? DefectType,
    string? Notes
);

public record DowntimeEventMessage(
    string MachineId,
    string? WorkOrderId,
    string ReasonCode,
    int DurationMinutes,
    DateTimeOffset StartedAt,
    string? Notes
);

public record MachineTelemetryMessage(
    string EventId,
    string MachineId,
    string? WorkOrderId,
    DateTimeOffset Timestamp,
    decimal TemperatureC,
    decimal VibrationMmS,
    decimal CycleTimeSeconds,
    int ErrorCount,
    decimal ScrapRate
);