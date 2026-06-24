namespace Fabiq.SmartFactory.Api.Models;

public class DowntimeEvent
{
    public int Id { get; set; }

    public string EventId { get; set; } = Guid.NewGuid().ToString("N");

    public string MachineId { get; set; } = string.Empty;

    public string LineId { get; set; } = string.Empty;

    public string? WorkOrderId { get; set; }

    public string ReasonCode { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset EndedAt { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}