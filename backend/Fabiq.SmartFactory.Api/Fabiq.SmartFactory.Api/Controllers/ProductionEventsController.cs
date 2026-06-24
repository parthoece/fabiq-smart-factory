using Fabiq.SmartFactory.Api.Contracts;
using Fabiq.SmartFactory.Api.Services;
using Fabiq.SmartFactory.Api.Data;
using Fabiq.SmartFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionEventsController : ControllerBase
{
    private readonly SmartFactoryDbContext _db;
    private readonly FactoryEventProcessor _processor;

    public ProductionEventsController(SmartFactoryDbContext db, FactoryEventProcessor processor)
    {
        _db = db;
        _processor = processor;
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<ProductionEventDto>>> GetRecentEvents()
    {
        var events = await _db.ProductionEvents
            .OrderByDescending(e => e.CreatedAt)
            .Take(50)
            .Select(e => new ProductionEventDto(
                e.EventId,
                e.MachineId,
                e.WorkOrderId,
                e.LineId,
                e.EventType,
                e.Quantity,
                e.PartId,
                e.DefectType,
                e.Notes,
                e.CreatedAt
            ))
            .ToListAsync();

        return Ok(events);
    }

    [HttpPost]
    public async Task<ActionResult> CreateProductionEvent(CreateProductionEventRequest request)
    {
        try
        {
            var result = await _processor.RecordProductionEventAsync(new ProductionEventMessage(
                request.MachineId,
                request.WorkOrderId,
                request.EventType,
                request.Quantity,
                request.PartId,
                request.DefectType,
                request.Notes
            ));

            return Ok(new
            {
                message = result.Message,
                eventId = result.EventId,
                machineId = result.MachineId,
                workOrderId = result.WorkOrderId,
                machineGoodCount = result.MachineGoodCount,
                machineScrapCount = result.MachineScrapCount,
                workOrderGoodCount = result.WorkOrderGoodCount,
                workOrderScrapCount = result.WorkOrderScrapCount,
                workOrderStatus = result.WorkOrderStatus
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

    [HttpPost("demo-good")]
    public async Task<ActionResult> CreateDemoGoodEvent()
    {
        var request = new CreateProductionEventRequest(
            MachineId: "SMT-01",
            WorkOrderId: "WO-2026-0001",
            EventType: "GOOD_PART",
            Quantity: 10,
            PartId: $"PCB-{Random.Shared.Next(100000, 999999)}",
            DefectType: null,
            Notes: "Demo good production event"
        );

        return await CreateProductionEvent(request);
    }

    [HttpPost("demo-scrap")]
    public async Task<ActionResult> CreateDemoScrapEvent()
    {
        var request = new CreateProductionEventRequest(
            MachineId: "AOI-01",
            WorkOrderId: "WO-2026-0001",
            EventType: "SCRAP_PART",
            Quantity: 2,
            PartId: $"PCB-{Random.Shared.Next(100000, 999999)}",
            DefectType: "SOLDER_BRIDGE",
            Notes: "Demo AOI inspection failure"
        );

        return await CreateProductionEvent(request);
    }
}

public record CreateProductionEventRequest(
    string MachineId,
    string WorkOrderId,
    string EventType,
    int Quantity,
    string? PartId,
    string? DefectType,
    string? Notes
);

public record ProductionEventDto(
    string EventId,
    string MachineId,
    string WorkOrderId,
    string LineId,
    string EventType,
    int Quantity,
    string? PartId,
    string? DefectType,
    string? Notes,
    DateTimeOffset CreatedAt
);