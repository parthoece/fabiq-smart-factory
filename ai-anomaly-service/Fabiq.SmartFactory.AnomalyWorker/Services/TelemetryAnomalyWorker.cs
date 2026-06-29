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

        _bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "localhost:9092";
        _groupId = Environment.GetEnvironmentVariable("KAFKA_GROUP_ID") ?? "fabiq-anomaly-worker";
        _telemetryTopic = Environment.GetEnvironmentVariable("KAFKA_TELEMETRY_TOPIC") ?? "machine.telemetry";
        _alertTopic = Environment.GetEnvironmentVariable("KAFKA_ALERT_TOPIC") ?? "maintenance.alerts";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
            AllowAutoCreateTopics = true
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        consumer.Subscribe(_telemetryTopic);

        _logger.LogInformation(
            "AI anomaly worker started. Consuming {TelemetryTopic}, publishing {AlertTopic}",
            _telemetryTopic,
            _alertTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var payload = consumeResult.Message.Value;

                var telemetry = JsonSerializer.Deserialize<MachineTelemetryMessage>(
                    payload,
                    JsonOptions());

                if (telemetry is null)
                {
                    _logger.LogWarning("Skipping empty telemetry payload.");
                    consumer.Commit(consumeResult);
                    continue;
                }

                var alerts = _anomalyDetectionService.Analyze(telemetry);

                foreach (var alert in alerts)
                {
                    var alertPayload = JsonSerializer.Serialize(alert, JsonOptions());

                    await producer.ProduceAsync(
                        _alertTopic,
                        new Message<string, string>
                        {
                            Key = alert.MachineId,
                            Value = alertPayload
                        },
                        stoppingToken);

                    _logger.LogWarning(
                        "Published anomaly alert {AlertType} {Severity} for {MachineId}",
                        alert.AlertType,
                        alert.Severity,
                        alert.MachineId);
                }

                consumer.Commit(consumeResult);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected anomaly worker error.");
            }
        }

        consumer.Close();
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}