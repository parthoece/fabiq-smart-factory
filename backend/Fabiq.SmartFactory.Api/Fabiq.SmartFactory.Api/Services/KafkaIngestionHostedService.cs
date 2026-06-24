using System.Text.Json;
using Confluent.Kafka;
using Fabiq.SmartFactory.Api.Contracts;
using Fabiq.SmartFactory.Api.Options;
using Microsoft.Extensions.Options;

namespace Fabiq.SmartFactory.Api.Services;

public sealed class KafkaIngestionHostedService : BackgroundService
{
    private readonly KafkaOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaIngestionHostedService> _logger;

    public KafkaIngestionHostedService(
        IOptions<KafkaOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<KafkaIngestionHostedService> logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    // Important: allow ASP.NET Core startup to continue before entering the Kafka loop.
    await Task.Yield();

    if (string.IsNullOrWhiteSpace(_options.BootstrapServers))
    {
        _logger.LogWarning("Kafka is not configured; skipping background ingestion.");
        return;
    }

    var config = new ConsumerConfig
    {
        BootstrapServers = _options.BootstrapServers,
        GroupId = _options.GroupId,
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = false,
        EnableAutoOffsetStore = false,
        AllowAutoCreateTopics = true
    };

    using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

    consumer.Subscribe(new[]
    {
        _options.MachineStatusTopic,
        _options.ProductionTopic,
        _options.DowntimeTopic
    });

    _logger.LogInformation("Kafka ingestion started for topics: {Topics}", string.Join(", ", new[]
    {
        _options.MachineStatusTopic,
        _options.ProductionTopic,
        _options.DowntimeTopic
    }));

    try
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<Ignore, string>? consumeResult = null;

            try
            {
                consumeResult = consumer.Consume(
                    TimeSpan.FromMilliseconds(_options.PollTimeoutMilliseconds));
            }
            catch (ConsumeException ex)
            {
                _logger.LogWarning(ex, "Kafka consume error.");

                // Important: avoid tight loop when Kafka/topic is unavailable.
                await Task.Delay(1000, stoppingToken);
                continue;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected Kafka consume error.");

                // Important: avoid tight loop on unexpected errors.
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            if (consumeResult is null)
            {
                // Important: yield control when no Kafka message is available.
                await Task.Delay(250, stoppingToken);
                continue;
            }

            using var scope = _scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<FactoryEventProcessor>();

            try
            {
                await HandleMessageAsync(processor, consumeResult, stoppingToken);
                consumer.Commit(consumeResult);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to process Kafka message from {Topic} at offset {Offset}.",
                    consumeResult.Topic,
                    consumeResult.Offset);

                await Task.Delay(500, stoppingToken);
            }
        }
    }
    finally
    {
        consumer.Close();
    }
}

    private async Task HandleMessageAsync(
        FactoryEventProcessor processor,
        ConsumeResult<Ignore, string> consumeResult,
        CancellationToken stoppingToken)
    {
        var topic = consumeResult.Topic;
        var payload = consumeResult.Message.Value;

        if (topic == _options.MachineStatusTopic)
        {
            var message = JsonSerializer.Deserialize<MachineStatusMessage>(payload, JsonOptions())
                ?? throw new InvalidOperationException("Kafka machine status payload was empty.");
            await processor.UpdateMachineStatusAsync(message);
            return;
        }

        if (topic == _options.ProductionTopic)
        {
            var message = JsonSerializer.Deserialize<ProductionEventMessage>(payload, JsonOptions())
                ?? throw new InvalidOperationException("Kafka production payload was empty.");
            await processor.RecordProductionEventAsync(message);
            return;
        }

        if (topic == _options.DowntimeTopic)
        {
            var message = JsonSerializer.Deserialize<DowntimeEventMessage>(payload, JsonOptions())
                ?? throw new InvalidOperationException("Kafka downtime payload was empty.");
            await processor.RecordDowntimeEventAsync(message);
        }
    }

    private static JsonSerializerOptions JsonOptions() => new()
    {
        PropertyNameCaseInsensitive = true
    };
}