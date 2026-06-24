using Fabiq.SmartFactory.Api.Data;
using Fabiq.SmartFactory.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachinesController : ControllerBase
{
    private readonly SmartFactoryDbContext _db;

    public MachinesController(SmartFactoryDbContext db)
    {
        _db = db;
    }

    [HttpGet("status")]
    public async Task<ActionResult<IEnumerable<MachineStatusDto>>> GetMachineStatus()
    {
        var machines = await _db.Machines
            .OrderBy(m => m.MachineId)
            .Select(m => new MachineStatusDto(
                m.MachineId,
                m.LineId,
                m.Status,
                m.CurrentWorkOrderId,
                m.GoodCount,
                m.ScrapCount,
                m.LastUpdatedAt
            ))
            .ToListAsync();

        return Ok(machines);
    }

    [HttpPost("seed")]
    public async Task<ActionResult> SeedMachines()
    {
        if (await _db.Machines.AnyAsync())
        {
            return Ok(new
            {
                message = "Machines already exist. Seed skipped."
            });
        }

        var machines = new List<Machine>
        {
            new()
            {
                MachineId = "SMT-01",
                LineId = "LINE-A",
                Status = "RUNNING",
                CurrentWorkOrderId = "WO-2026-0001",
                GoodCount = 120,
                ScrapCount = 2,
                LastUpdatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                MachineId = "AOI-01",
                LineId = "LINE-A",
                Status = "RUNNING",
                CurrentWorkOrderId = "WO-2026-0001",
                GoodCount = 118,
                ScrapCount = 4,
                LastUpdatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                MachineId = "TEST-01",
                LineId = "LINE-A",
                Status = "DOWN",
                CurrentWorkOrderId = "WO-2026-0001",
                GoodCount = 90,
                ScrapCount = 6,
                LastUpdatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                MachineId = "PACKING-01",
                LineId = "LINE-A",
                Status = "IDLE",
                CurrentWorkOrderId = "WO-2026-0001",
                GoodCount = 88,
                ScrapCount = 0,
                LastUpdatedAt = DateTimeOffset.UtcNow
            }
        };

        _db.Machines.AddRange(machines);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Seed machines created.",
            count = machines.Count
        });
    }
}

public record MachineStatusDto(
    string MachineId,
    string LineId,
    string Status,
    string? CurrentWorkOrderId,
    int GoodCount,
    int ScrapCount,
    DateTimeOffset LastUpdatedAt
);