using Fabiq.SmartFactory.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceAlertsController : ControllerBase
{
    private readonly SmartFactoryDbContext _db;

    public MaintenanceAlertsController(SmartFactoryDbContext db)
    {
        _db = db;
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<MaintenanceAlertDto>>> GetActiveAlerts(
        [FromQuery] int limit = 50)
    {
        limit = Math.Clamp(limit, 1, 200);

        var alerts = await _db.MaintenanceAlerts
            .AsNoTracking()
            .Where(a => a.ResolvedAt == null)
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .Select(a => new MaintenanceAlertDto(
                a.AlertId,
                a.MachineId,
                a.WorkOrderId,
                a.AlertType,
                a.Severity,
                a.Message,
                a.SourceEventId,
                a.TemperatureC,
                a.VibrationMmS,
                a.CycleTimeSeconds,
                a.ErrorCount,
                a.ScrapRate,
                a.CreatedAt,
                a.ResolvedAt
            ))
            .ToListAsync();

        return Ok(alerts);
    }

    [HttpGet("{alertId}")]
    public async Task<ActionResult<MaintenanceAlertDto>> GetAlert(string alertId)
    {
        var alert = await _db.MaintenanceAlerts
            .AsNoTracking()
            .Where(a => a.AlertId == alertId)
            .Select(a => new MaintenanceAlertDto(
                a.AlertId,
                a.MachineId,
                a.WorkOrderId,
                a.AlertType,
                a.Severity,
                a.Message,
                a.SourceEventId,
                a.TemperatureC,
                a.VibrationMmS,
                a.CycleTimeSeconds,
                a.ErrorCount,
                a.ScrapRate,
                a.CreatedAt,
                a.ResolvedAt
            ))
            .FirstOrDefaultAsync();

        if (alert is null)
        {
            return NotFound(new { message = $"Maintenance alert {alertId} was not found." });
        }

        return Ok(alert);
    }

    [HttpPost("{alertId}/resolve")]
    public async Task<IActionResult> ResolveAlert(string alertId)
    {
        var alert = await _db.MaintenanceAlerts
            .FirstOrDefaultAsync(a => a.AlertId == alertId);

        if (alert is null)
        {
            return NotFound(new { message = $"Maintenance alert {alertId} was not found." });
        }

        alert.ResolvedAt ??= DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Maintenance alert resolved.",
            alert.AlertId,
            alert.ResolvedAt
        });
    }
}

public sealed record MaintenanceAlertDto(
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
    DateTimeOffset CreatedAt,
    DateTimeOffset? ResolvedAt
);