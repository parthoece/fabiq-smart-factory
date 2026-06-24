namespace Fabiq.SmartFactory.Api.Options;

public sealed class KafkaOptions
{
    public string BootstrapServers { get; set; } = "localhost:9092";

    public string GroupId { get; set; } = "fabiq-smart-factory-api";

    public string MachineStatusTopic { get; set; } = "machine.status";

    public string ProductionTopic { get; set; } = "production.counts";

    public string DowntimeTopic { get; set; } = "downtime.events";

    public int PollTimeoutMilliseconds { get; set; } = 1000;
}
