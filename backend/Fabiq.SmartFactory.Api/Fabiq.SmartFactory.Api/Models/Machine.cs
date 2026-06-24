namespace Fabiq.SmartFactory.Api.Models;

public class Machine
{
    public int Id { get; set; }

    public string MachineId { get; set; } = string.Empty;

    public string LineId { get; set; } = string.Empty;

    public string Status { get; set; } = "UNKNOWN";

    public string? CurrentWorkOrderId { get; set; }

    public int GoodCount { get; set; }

    public int ScrapCount { get; set; }

    public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}