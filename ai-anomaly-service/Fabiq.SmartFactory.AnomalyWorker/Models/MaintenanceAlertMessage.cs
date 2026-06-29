namespace Fabiq.SmartFactory.AnomalyWorker.Models;

public sealed record MaintenanceAlertMessage(
    string AlertId,
    string MachineId,
    string? WorkOrderId,
    string AlertType,
    string Severity,
    string Message,
    string? SourceEventId,
    decimal? TemperatureC,
    decimal? VibrationMmS,
    decimal? CycleTimeSeconds,
    int? ErrorCount,
    decimal? ScrapRate,
    DateTimeOffset CreatedAt
);