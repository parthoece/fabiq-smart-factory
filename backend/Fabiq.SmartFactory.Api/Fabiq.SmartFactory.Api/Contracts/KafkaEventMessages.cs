namespace Fabiq.SmartFactory.Api.Contracts;

public sealed record MachineStatusMessage(
    string MachineId,
    string Status,
    string? CurrentWorkOrderId,
    string? Notes,
    DateTimeOffset? ReportedAt
);

public sealed record ProductionEventMessage(
    string MachineId,
    string WorkOrderId,
    string EventType,
    int Quantity,
    string? PartId,
    string? DefectType,
    string? Notes
);

public sealed record DowntimeEventMessage(
    string MachineId,
    string? WorkOrderId,
    string ReasonCode,
    int DurationMinutes,
    DateTimeOffset? StartedAt,
    string? Notes
);

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