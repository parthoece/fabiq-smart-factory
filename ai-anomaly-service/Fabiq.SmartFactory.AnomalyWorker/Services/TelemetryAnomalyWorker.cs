using System.Text.Json;
using Confluent.Kafka;
using Fabiq.SmartFactory.AnomalyWorker.Models;

namespace Fabiq.SmartFactory.AnomalyWorker.Services;

public sealed class TelemetryAnomalyWorker : BackgroundService
{
    private readonly ILogger<TelemetryAnomalyWorker> _logger;
    private readonly AnomalyDetectionService _anomalyDetectionService;

    private readonly string _bootstrapServers;
    private readonly string _groupId;
    private readonly string _telemetryTopic;
    private readonly string _alertTopic;

    public TelemetryAnomalyWorker(
        ILogger<TelemetryAnomalyWorker> logger,
        AnomalyDetectionService anomalyDetectionService)
    {
        _logger = logger;
        _anomalyDetectionService = anomalyDetectionService;

        _bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")
            ?? "localhost:9092";

        _groupId = Environment.GetEnvironmentVariable("KAFKA_GROUP_ID")
            ?? "fabiq-anomaly-worker";

        _telemetryTopic = Environment.GetEnvironmentVariable("KAFKA_TELEMETRY_TOPIC")
            ?? "machine.telemetry";

        _alertTopic = Environment.GetEnvironmentVariable("KAFKA_ALERT_TOPIC")
            ?? "maintenance.alerts";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        ValidateConfiguration();

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = _groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            AllowAutoCreateTopics = true
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _bootstrapServers,
            Acks = Acks.All,
            AllowAutoCreateTopics = true,
            EnableIdempotence = true,
            MessageTimeoutMs = 10000,
            RequestTimeoutMs = 5000
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        await WaitForKafkaAsync(stoppingToken);

        consumer.Subscribe(_telemetryTopic);

        _logger.LogInformation(
            "AI anomaly worker started. Consuming {TelemetryTopic}, publishing {AlertTopic}. Kafka: {BootstrapServers}",
            _telemetryTopic,
            _alertTopic,
            _bootstrapServers);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? consumeResult = null;

                try
                {
                    consumeResult = consumer.Consume(stoppingToken);

                    await ProcessTelemetryAsync(
                        producer,
                        consumeResult,
                        stoppingToken);

                    consumer.Commit(consumeResult);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (ConsumeException ex)
                {
                    _logger.LogWarning(ex, "Kafka consume error.");
                    await Task.Delay(1000, stoppingToken);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Invalid telemetry JSON payload. The message will be skipped.");

                    if (consumeResult is not null)
                    {
                        consumer.Commit(consumeResult);
                    }
                }
                catch (ProduceException<string, string> ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to publish anomaly alert to topic {AlertTopic}.",
                        _alertTopic);

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected anomaly worker error.");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
        finally
        {
            try
            {
                consumer.Close();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while closing Kafka consumer.");
            }

            producer.Flush(TimeSpan.FromSeconds(10));

            _logger.LogInformation("AI anomaly worker stopped.");
        }
    }

    private async Task ProcessTelemetryAsync(
        IProducer<string, string> producer,
        ConsumeResult<string, string> consumeResult,
        CancellationToken stoppingToken)
    {
        var payload = consumeResult.Message.Value;

        if (string.IsNullOrWhiteSpace(payload))
        {
            _logger.LogWarning("Skipping empty telemetry payload.");
            return;
        }

        var telemetry = JsonSerializer.Deserialize<MachineTelemetryMessage>(
            payload,
            JsonOptions());

        if (telemetry is null)
        {
            _logger.LogWarning("Skipping telemetry payload that could not be deserialized.");
            return;
        }

        var alerts = _anomalyDetectionService.Analyze(telemetry);

        if (alerts.Count == 0)
        {
            _logger.LogInformation(
                "Telemetry normal. Machine={MachineId}, Temp={TemperatureC}, Vibration={VibrationMmS}, Cycle={CycleTimeSeconds}, Errors={ErrorCount}, ScrapRate={ScrapRate}",
                telemetry.MachineId,
                telemetry.TemperatureC,
                telemetry.VibrationMmS,
                telemetry.CycleTimeSeconds,
                telemetry.ErrorCount,
                telemetry.ScrapRate);

            return;
        }

        foreach (var alert in alerts)
        {
            var alertPayload = JsonSerializer.Serialize(alert, JsonOptions());

            var deliveryResult = await producer.ProduceAsync(
                _alertTopic,
                new Message<string, string>
                {
                    Key = alert.MachineId,
                    Value = alertPayload
                },
                stoppingToken);

            _logger.LogWarning(
                "Published anomaly alert {AlertType} {Severity} for {MachineId} to {TopicPartitionOffset}. Message={Message}",
                alert.AlertType,
                alert.Severity,
                alert.MachineId,
                deliveryResult.TopicPartitionOffset,
                alert.Message);
        }
    }

    private async Task WaitForKafkaAsync(CancellationToken stoppingToken)
{
    await ExecuteWithRetryAsync(() =>
    {
        stoppingToken.ThrowIfCancellationRequested();

        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = _bootstrapServers
        }).Build();

        adminClient.GetMetadata(TimeSpan.FromSeconds(10));

        return Task.FromResult(true);
    }, "waiting for Kafka", stoppingToken);
}

    private static async Task<T> ExecuteWithRetryAsync<T>(
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

    private void ValidateConfiguration()
    {
        var requiredValues = new Dictionary<string, string?>
        {
            ["KAFKA_BOOTSTRAP_SERVERS"] = _bootstrapServers,
            ["KAFKA_GROUP_ID"] = _groupId,
            ["KAFKA_TELEMETRY_TOPIC"] = _telemetryTopic,
            ["KAFKA_ALERT_TOPIC"] = _alertTopic
        };

        foreach (var item in requiredValues)
        {
            if (string.IsNullOrWhiteSpace(item.Value))
            {
                throw new InvalidOperationException($"{item.Key} is required.");
            }
        }
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}