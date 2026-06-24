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