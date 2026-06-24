namespace Fabiq.SmartFactory.Api.Models;

public class WorkOrder
{
    public int Id { get; set; }

    public string WorkOrderId { get; set; } = string.Empty;

    public string ProductCode { get; set; } = string.Empty;

    public string LineId { get; set; } = string.Empty;

    public int PlannedQuantity { get; set; }

    public int GoodCount { get; set; }

    public int ScrapCount { get; set; }

    public string Status { get; set; } = "PLANNED";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }
}