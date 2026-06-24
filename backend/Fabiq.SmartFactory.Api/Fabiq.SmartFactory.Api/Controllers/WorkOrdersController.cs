using Fabiq.SmartFactory.Api.Data;
using Fabiq.SmartFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly SmartFactoryDbContext _db;

    public WorkOrdersController(SmartFactoryDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetWorkOrders()
    {
        var workOrders = await _db.WorkOrders
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WorkOrderDto(
                w.WorkOrderId,
                w.ProductCode,
                w.LineId,
                w.PlannedQuantity,
                w.GoodCount,
                w.ScrapCount,
                w.Status,
                w.CreatedAt,
                w.StartedAt,
                w.CompletedAt
            ))
            .ToListAsync();

        return Ok(workOrders);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetActiveWorkOrders()
    {
        var workOrders = await _db.WorkOrders
            .Where(w => w.Status == "RUNNING")
            .OrderBy(w => w.LineId)
            .Select(w => new WorkOrderDto(
                w.WorkOrderId,
                w.ProductCode,
                w.LineId,
                w.PlannedQuantity,
                w.GoodCount,
                w.ScrapCount,
                w.Status,
                w.CreatedAt,
                w.StartedAt,
                w.CompletedAt
            ))
            .ToListAsync();

        return Ok(workOrders);
    }

    [HttpGet("{workOrderId}")]
    public async Task<ActionResult<WorkOrderDto>> GetWorkOrder(string workOrderId)
    {
        var workOrder = await _db.WorkOrders
            .Where(w => w.WorkOrderId == workOrderId)
            .Select(w => new WorkOrderDto(
                w.WorkOrderId,
                w.ProductCode,
                w.LineId,
                w.PlannedQuantity,
                w.GoodCount,
                w.ScrapCount,
                w.Status,
                w.CreatedAt,
                w.StartedAt,
                w.CompletedAt
            ))
            .FirstOrDefaultAsync();

        if (workOrder is null)
        {
            return NotFound(new
            {
                message = $"Work order {workOrderId} was not found."
            });
        }

        return Ok(workOrder);
    }

    [HttpPost("seed")]
    public async Task<ActionResult> SeedWorkOrders()
    {
        if (await _db.WorkOrders.AnyAsync())
        {
            return Ok(new
            {
                message = "Work orders already exist. Seed skipped."
            });
        }

        var workOrders = new List<WorkOrder>
        {
            new()
            {
                WorkOrderId = "WO-2026-0001",
                ProductCode = "PCB-A100",
                LineId = "LINE-A",
                PlannedQuantity = 500,
                GoodCount = 120,
                ScrapCount = 6,
                Status = "RUNNING",
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-2),
                StartedAt = DateTimeOffset.UtcNow.AddHours(-1)
            },
            new()
            {
                WorkOrderId = "WO-2026-0002",
                ProductCode = "PCB-B200",
                LineId = "LINE-B",
                PlannedQuantity = 300,
                GoodCount = 0,
                ScrapCount = 0,
                Status = "PLANNED",
                CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-30)
            },
            new()
            {
                WorkOrderId = "WO-2026-0003",
                ProductCode = "MODULE-C300",
                LineId = "LINE-A",
                PlannedQuantity = 200,
                GoodCount = 200,
                ScrapCount = 8,
                Status = "COMPLETED",
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-8),
                StartedAt = DateTimeOffset.UtcNow.AddHours(-7),
                CompletedAt = DateTimeOffset.UtcNow.AddHours(-3)
            }
        };

        _db.WorkOrders.AddRange(workOrders);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Seed work orders created.",
            count = workOrders.Count
        });
    }
}

public record WorkOrderDto(
    string WorkOrderId,
    string ProductCode,
    string LineId,
    int PlannedQuantity,
    int GoodCount,
    int ScrapCount,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt
);