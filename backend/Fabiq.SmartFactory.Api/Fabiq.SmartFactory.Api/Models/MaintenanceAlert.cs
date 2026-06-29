namespace Fabiq.SmartFactory.Api.Models;

public class MaintenanceAlert
{
    public int Id { get; set; }

    public string AlertId { get; set; } = Guid.NewGuid().ToString("N");

    public string MachineId { get; set; } = string.Empty;

    public string? WorkOrderId { get; set; }

    public string AlertType { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? SourceEventId { get; set; }

    public decimal? TemperatureC { get; set; }

    public decimal? VibrationMmS { get; set; }

    public decimal? CycleTimeSeconds { get; set; }

    public int? ErrorCount { get; set; }

    public decimal? ScrapRate { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ResolvedAt { get; set; }
}