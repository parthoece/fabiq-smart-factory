namespace Fabiq.SmartFactory.Api.Models;

public class ProductionEvent
{
    public int Id { get; set; }

    public string EventId { get; set; } = Guid.NewGuid().ToString("N");

    public string MachineId { get; set; } = string.Empty;

    public string WorkOrderId { get; set; } = string.Empty;

    public string LineId { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string? PartId { get; set; }

    public string? DefectType { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}