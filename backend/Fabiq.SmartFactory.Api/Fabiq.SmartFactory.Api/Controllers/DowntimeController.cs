using Fabiq.SmartFactory.Api.Contracts;
using Fabiq.SmartFactory.Api.Services;
using Fabiq.SmartFactory.Api.Data;
using Fabiq.SmartFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DowntimeController : ControllerBase
{
    private readonly SmartFactoryDbContext _db;
    private readonly FactoryEventProcessor _processor;

    public DowntimeController(SmartFactoryDbContext db, FactoryEventProcessor processor)
    {
        _db = db;
        _processor = processor;
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<DowntimeEventDto>>> GetRecentDowntimeEvents(
        [FromQuery] string? lineId = null,
        [FromQuery] int limit = 50)
    {
        limit = Math.Clamp(limit, 1, 200);

        var query = _db.DowntimeEvents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(lineId))
        {
            query = query.Where(e => e.LineId == lineId);
        }

        var events = await query
            .OrderByDescending(e => e.StartedAt)
            .Take(limit)
            .Select(e => new DowntimeEventDto(
                e.EventId,
                e.MachineId,
                e.LineId,
                e.WorkOrderId,
                e.ReasonCode,
                e.DurationMinutes,
                e.StartedAt,
                e.EndedAt,
                e.Notes,
                e.CreatedAt
            ))
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("summary")]
    
public async Task<ActionResult<IEnumerable<DowntimeSummaryDto>>> GetDowntimeSummary(
    [FromQuery] string? lineId = null,
    [FromQuery] DateTimeOffset? from = null,
    [FromQuery] DateTimeOffset? to = null)
{
    var query = _db.DowntimeEvents.AsNoTracking().AsQueryable();

    if (!string.IsNullOrWhiteSpace(lineId))
    {
        query = query.Where(e => e.LineId == lineId);
    }

    if (from is not null)
    {
        query = query.Where(e => e.StartedAt >= from.Value);
    }

    if (to is not null)
    {
        query = query.Where(e => e.StartedAt <= to.Value);
    }

    var events = await query
        .Select(e => new
        {
            e.LineId,
            e.ReasonCode,
            e.DurationMinutes,
            e.MachineId,
            e.StartedAt
        })
        .ToListAsync();

    var summary = events
        .GroupBy(e => new { e.LineId, e.ReasonCode })
        .Select(g => new DowntimeSummaryDto(
            LineId: g.Key.LineId,
            ReasonCode: g.Key.ReasonCode,
            EventCount: g.Count(),
            TotalDurationMinutes: g.Sum(e => e.DurationMinutes),
            AffectedMachineCount: g.Select(e => e.MachineId).Distinct().Count(),
            LastOccurredAt: g.Max(e => e.StartedAt)
        ))
        .OrderByDescending(s => s.TotalDurationMinutes)
        .ToList();

    return Ok(summary);
}

    [HttpPost]
    public async Task<ActionResult> CreateDowntimeEvent(CreateDowntimeEventRequest request)
    {
        try
        {
            var result = await _processor.RecordDowntimeEventAsync(new DowntimeEventMessage(
                request.MachineId,
                request.WorkOrderId,
                request.ReasonCode,
                request.DurationMinutes,
                request.StartedAt,
                request.Notes
            ));

            return Ok(new
            {
                message = result.Message,
                eventId = result.EventId,
                machineId = result.MachineId,
                lineId = result.LineId,
                workOrderId = result.WorkOrderId,
                reasonCode = result.ReasonCode,
                durationMinutes = result.DurationMinutes,
                startedAt = result.StartedAt,
                endedAt = result.EndedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("demo")]
    public async Task<ActionResult> CreateDemoDowntimeEvent()
    {
        var request = new CreateDowntimeEventRequest(
            MachineId: "TEST-01",
            WorkOrderId: "WO-2026-0001",
            ReasonCode: "EQUIPMENT_FAILURE",
            DurationMinutes: 15,
            StartedAt: DateTimeOffset.UtcNow.AddMinutes(-15),
            Notes: "Demo downtime event from test station"
        );

        return await CreateDowntimeEvent(request);
    }
}

public record CreateDowntimeEventRequest(
    string MachineId,
    string? WorkOrderId,
    string ReasonCode,
    int DurationMinutes,
    DateTimeOffset? StartedAt,
    string? Notes
);

public record DowntimeEventDto(
    string EventId,
    string MachineId,
    string LineId,
    string? WorkOrderId,
    string ReasonCode,
    int DurationMinutes,
    DateTimeOffset StartedAt,
    DateTimeOffset EndedAt,
    string? Notes,
    DateTimeOffset CreatedAt
);

public record DowntimeSummaryDto(
    string LineId,
    string ReasonCode,
    int EventCount,
    int TotalDurationMinutes,
    int AffectedMachineCount,
    DateTimeOffset LastOccurredAt
);