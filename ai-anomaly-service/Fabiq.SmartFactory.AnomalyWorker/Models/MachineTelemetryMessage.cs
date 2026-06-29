namespace Fabiq.SmartFactory.AnomalyWorker.Models;

public sealed record MachineTelemetryMessage(
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