using Fabiq.SmartFactory.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraceabilityController : ControllerBase
{
    private readonly SmartFactoryDbContext _db;

    public TraceabilityController(SmartFactoryDbContext db)
    {
        _db = db;
    }

    [HttpGet("part/{partId}")]
    public async Task<ActionResult<PartTraceabilityDto>> GetPartTraceability(string partId)
    {
        var events = await _db.ProductionEvents
            .Where(e => e.PartId == partId)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();

        if (!events.Any())
        {
            return NotFound(new { message = $"No production history found for part {partId}." });
        }

        var workOrderIds = events
            .Select(e => e.WorkOrderId)
            .Distinct()
            .ToList();

        var workOrders = await _db.WorkOrders
            .Where(w => workOrderIds.Contains(w.WorkOrderId))
            .ToDictionaryAsync(w => w.WorkOrderId);

        var route = events
            .Select(e => new PartTraceabilityStepDto(
                e.EventId,
                e.MachineId,
                e.LineId,
                e.WorkOrderId,
                workOrders.TryGetValue(e.WorkOrderId, out var workOrder) ? workOrder.ProductCode : null,
                e.EventType,
                e.Quantity,
                e.DefectType,
                e.Notes,
                e.CreatedAt
            ))
            .ToList();

        var finalStatus = events.Any(e => e.EventType == "SCRAP_PART")
            ? "SCRAPPED"
            : "GOOD";

        var latestEvent = events.Last();

        return Ok(new PartTraceabilityDto(
            PartId: partId,
            FinalStatus: finalStatus,
            CurrentMachineId: latestEvent.MachineId,
            CurrentLineId: latestEvent.LineId,
            LastSeenAt: latestEvent.CreatedAt,
            Route: route
        ));
    }

    [HttpGet("work-order/{workOrderId}")]
    public async Task<ActionResult<WorkOrderTraceabilityDto>> GetWorkOrderTraceability(
        string workOrderId,
        [FromQuery] int limit = 200)
    {
        limit = Math.Clamp(limit, 1, 1000);

        var workOrder = await _db.WorkOrders
            .FirstOrDefaultAsync(w => w.WorkOrderId == workOrderId);

        if (workOrder is null)
        {
            return NotFound(new { message = $"Work order {workOrderId} was not found." });
        }

        var events = await _db.ProductionEvents
            .Where(e => e.WorkOrderId == workOrderId && e.PartId != null)
            .OrderByDescending(e => e.CreatedAt)
            .Take(limit)
            .ToListAsync();

        var parts = events
            .GroupBy(e => e.PartId!)
            .Select(g =>
            {
                var latestEvent = g.OrderByDescending(e => e.CreatedAt).First();
                var goodQuantity = g.Where(e => e.EventType == "GOOD_PART").Sum(e => e.Quantity);
                var scrapQuantity = g.Where(e => e.EventType == "SCRAP_PART").Sum(e => e.Quantity);

                return new WorkOrderPartSummaryDto(
                    PartId: g.Key,
                    FinalStatus: scrapQuantity > 0 ? "SCRAPPED" : "GOOD",
                    GoodQuantity: goodQuantity,
                    ScrapQuantity: scrapQuantity,
                    LastMachineId: latestEvent.MachineId,
                    LastEventType: latestEvent.EventType,
                    LastSeenAt: latestEvent.CreatedAt
                );
            })
            .OrderByDescending(p => p.LastSeenAt)
            .ToList();

        return Ok(new WorkOrderTraceabilityDto(
            WorkOrderId: workOrder.WorkOrderId,
            ProductCode: workOrder.ProductCode,
            LineId: workOrder.LineId,
            PlannedQuantity: workOrder.PlannedQuantity,
            GoodCount: workOrder.GoodCount,
            ScrapCount: workOrder.ScrapCount,
            Status: workOrder.Status,
            PartCount: parts.Count,
            Parts: parts
        ));
    }
}

public record PartTraceabilityDto(
    string PartId,
    string FinalStatus,
    string CurrentMachineId,
    string CurrentLineId,
    DateTimeOffset LastSeenAt,
    IReadOnlyList<PartTraceabilityStepDto> Route
);

public record PartTraceabilityStepDto(
    string EventId,
    string MachineId,
    string LineId,
    string WorkOrderId,
    string? ProductCode,
    string EventType,
    int Quantity,
    string? DefectType,
    string? Notes,
    DateTimeOffset CreatedAt
);

public record WorkOrderTraceabilityDto(
    string WorkOrderId,
    string ProductCode,
    string LineId,
    int PlannedQuantity,
    int GoodCount,
    int ScrapCount,
    string Status,
    int PartCount,
    IReadOnlyList<WorkOrderPartSummaryDto> Parts
);

public record WorkOrderPartSummaryDto(
    string PartId,
    string FinalStatus,
    int GoodQuantity,
    int ScrapQuantity,
    string LastMachineId,
    string LastEventType,
    DateTimeOffset LastSeenAt
);