namespace Fabiq.SmartFactory.Api.Models;

public class MachineStatusEvent
{
    public int Id { get; set; }

    public string EventId { get; set; } = Guid.NewGuid().ToString("N");

    public string MachineId { get; set; } = string.Empty;

    public string LineId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? CurrentWorkOrderId { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}