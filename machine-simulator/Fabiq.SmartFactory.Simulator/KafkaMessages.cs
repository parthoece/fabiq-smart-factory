namespace Fabiq.SmartFactory.Simulator;

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

public sealed record WorkOrderDto(
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