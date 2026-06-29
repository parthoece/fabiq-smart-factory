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

var http = new HttpClient
{
    BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
};

using var producer = new ProducerBuilder<Null, string>(new ProducerConfig
{
    BootstrapServers = bootstrapServers,
    AllowAutoCreateTopics = true,
    Acks = Acks.All
}).Build();

Console.WriteLine($"Smart Factory simulator targeting {http.BaseAddress}");
Console.WriteLine($"Kafka bootstrap servers: {bootstrapServers}");

await WaitForBackendAsync(http);

if (seedFirst)
{
    await ExecuteWithRetryAsync(async () =>
    {
        await EnsureSeedAsync(http);
        return true;
    }, "seeding backend data");
}

var activeWorkOrderId = await ExecuteWithRetryAsync(() => ResolveActiveWorkOrderIdAsync(http), "reading active work order") ?? "WO-2026-0001";
var machineIds = new[] { "SMT-01", "AOI-01", "ASSEMBLY-01", "TEST-01", "PACKING-01" };
var cycle = 0;

while (iterations <= 0 || cycle < iterations)
{
    cycle++;

    var partId = $"PCB-{DateTime.UtcNow:yyyyMMddHHmmss}-{cycle:000}";
    Console.WriteLine($"Cycle {cycle}: simulating {partId} on {activeWorkOrderId}");

    foreach (var machineId in machineIds)
    {
        await PublishAsync(producer, machineStatusTopic, new MachineStatusMessage(
            machineId,
            "RUNNING",
            activeWorkOrderId,
            "Simulator heartbeat",
            DateTimeOffset.UtcNow
        ));
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
                    : Math.Round((decimal)(Random.Shared.NextDouble() * 0.04), 4)
            );

            await PublishAsync(producer, telemetryTopic, telemetry);
        }

    await PublishAsync(producer, productionTopic, new ProductionEventMessage(
        "SMT-01",
        activeWorkOrderId,
        "GOOD_PART",
        1,
        partId,
        null,
        "SMT placement completed"
    ));

    var aoiScrap = Random.Shared.NextDouble() < 0.18;
    await PublishAsync(producer, productionTopic, aoiScrap
        ? new ProductionEventMessage("AOI-01", activeWorkOrderId, "SCRAP_PART", 1, partId, "SOLDER_BRIDGE", "AOI inspection detected a defect")
        : new ProductionEventMessage("AOI-01", activeWorkOrderId, "GOOD_PART", 1, partId, null, "AOI inspection passed"));

    await PublishAsync(producer, productionTopic, new ProductionEventMessage(
        "ASSEMBLY-01",
        activeWorkOrderId,
        "GOOD_PART",
        1,
        partId,
        null,
        "Assembly completed"
    ));
    await PublishAsync(producer, productionTopic, new ProductionEventMessage(
        "TEST-01",
        activeWorkOrderId,
        "GOOD_PART",
        1,
        partId,
        null,
        "Functional test passed"
    ));
    await PublishAsync(producer, productionTopic, new ProductionEventMessage(
        "PACKING-01",
        activeWorkOrderId,
        "GOOD_PART",
        1,
        partId,
        null,
        "Packed for shipment"
    ));

    if (Random.Shared.NextDouble() < 0.25)
    {
        await PublishAsync(producer, machineStatusTopic, new MachineStatusMessage(
            "TEST-01",
            "DOWN",
            activeWorkOrderId,
            "Automated downtime from simulator",
            DateTimeOffset.UtcNow
        ));

        await PublishAsync(producer, downtimeTopic, new DowntimeEventMessage(
            "TEST-01",
            activeWorkOrderId,
            "EQUIPMENT_FAILURE",
            12,
            DateTimeOffset.UtcNow.AddMinutes(-12),
            "Automated downtime from simulator"
        ));

        await PublishAsync(producer, machineStatusTopic, new MachineStatusMessage(
            "TEST-01",
            "RUNNING",
            activeWorkOrderId,
            "Recovered after downtime",
            DateTimeOffset.UtcNow
        ));
    }

    await Task.Delay(intervalMs);
}

Console.WriteLine("Simulator finished.");

static int ReadInt(string name, int defaultValue)
{
    var value = Environment.GetEnvironmentVariable(name);
    return int.TryParse(value, out var parsed) ? parsed : defaultValue;
}

static async Task EnsureSeedAsync(HttpClient http)
{
    await PostAndLogAsync(http, "api/workorders/seed");
    await PostAndLogAsync(http, "api/machines/seed");
}

static async Task WaitForBackendAsync(HttpClient http)
{
    await ExecuteWithRetryAsync(async () =>
    {
        var response = await http.GetAsync("swagger");
        response.EnsureSuccessStatusCode();
        return true;
    }, "waiting for backend");
}

static async Task<string?> ResolveActiveWorkOrderIdAsync(HttpClient http)
{
    var workOrders = await http.GetFromJsonAsync<List<WorkOrderDto>>("api/workorders/active");
    return workOrders?.FirstOrDefault()?.WorkOrderId;
}

static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, string description, int maxAttempts = 60, int delayMilliseconds = 2000)
{
    Exception? lastException = null;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            return await action();
        }
        catch (HttpRequestException exception)
        {
            lastException = exception;
            Console.WriteLine($"  waiting for {description} (attempt {attempt}/{maxAttempts}): {exception.Message}");
            await Task.Delay(delayMilliseconds);
        }
    }

    throw new InvalidOperationException($"Timed out while {description}.", lastException);
}

static async Task PostAndLogAsync(HttpClient http, string path)
{
    var response = await http.PostAsync(path, null);
    var body = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"  OK {path}: {body}");
    }
    else
    {
        Console.WriteLine($"  WARN {path}: {(int)response.StatusCode} {response.ReasonPhrase} {body}");
    }
}

static async Task PublishAsync<T>(IProducer<Null, string> producer, string topic, T payload)
{
    var serializedPayload = JsonSerializer.Serialize(payload, JsonOptions());
    var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string>
    {
        Value = serializedPayload
    });

    Console.WriteLine($"  published {topic} to {deliveryResult.TopicPartitionOffset}: {serializedPayload}");
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
