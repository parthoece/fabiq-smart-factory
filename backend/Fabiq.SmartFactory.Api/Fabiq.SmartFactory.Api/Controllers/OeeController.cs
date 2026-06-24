using Fabiq.SmartFactory.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OeeController : ControllerBase
{
    private readonly SmartFactoryDbContext _db;

    public OeeController(SmartFactoryDbContext db)
    {
        _db = db;
    }

    [HttpGet("line/{lineId}")]
    public async Task<ActionResult<OeeDto>> GetLineOee(
        string lineId,
        [FromQuery] double plannedProductionMinutes = 120,
        [FromQuery] double idealCycleTimeSeconds = 10)
    {
        var workOrders = await _db.WorkOrders
            .Where(w => w.LineId == lineId)
            .ToListAsync();

        if (!workOrders.Any())
        {
            return NotFound(new
            {
                message = $"No work orders found for line {lineId}."
            });
        }

        var totalGood = workOrders.Sum(w => w.GoodCount);
        var totalScrap = workOrders.Sum(w => w.ScrapCount);
        var totalProduced = totalGood + totalScrap;

        var runningMinutes = workOrders.Sum(CalculateRunningMinutes);

        var result = CalculateOee(
            scope: $"Line {lineId}",
            plannedProductionMinutes,
            runningMinutes,
            idealCycleTimeSeconds,
            totalGood,
            totalScrap
        );

        return Ok(result);
    }

    [HttpGet("work-order/{workOrderId}")]
    public async Task<ActionResult<OeeDto>> GetWorkOrderOee(
        string workOrderId,
        [FromQuery] double plannedProductionMinutes = 120,
        [FromQuery] double idealCycleTimeSeconds = 10)
    {
        var workOrder = await _db.WorkOrders
            .FirstOrDefaultAsync(w => w.WorkOrderId == workOrderId);

        if (workOrder is null)
        {
            return NotFound(new
            {
                message = $"Work order {workOrderId} was not found."
            });
        }

        var runningMinutes = CalculateRunningMinutes(workOrder);

        var result = CalculateOee(
            scope: $"Work Order {workOrder.WorkOrderId}",
            plannedProductionMinutes,
            runningMinutes,
            idealCycleTimeSeconds,
            workOrder.GoodCount,
            workOrder.ScrapCount
        );

        return Ok(result);
    }

    private static double CalculateRunningMinutes(Models.WorkOrder workOrder)
    {
        if (workOrder.StartedAt is null)
        {
            return 0;
        }

        var endTime = workOrder.CompletedAt ?? DateTimeOffset.UtcNow;
        var minutes = (endTime - workOrder.StartedAt.Value).TotalMinutes;

        return Math.Max(0, minutes);
    }

    private static OeeDto CalculateOee(
        string scope,
        double plannedProductionMinutes,
        double runningMinutes,
        double idealCycleTimeSeconds,
        int goodCount,
        int scrapCount)
    {
        var totalCount = goodCount + scrapCount;

        var availability = plannedProductionMinutes <= 0
            ? 0
            : runningMinutes / plannedProductionMinutes;

        var performance = runningMinutes <= 0
            ? 0
            : (totalCount * idealCycleTimeSeconds) / (runningMinutes * 60);

        var quality = totalCount <= 0
            ? 0
            : (double)goodCount / totalCount;

        availability = ClampPercentage(availability);
        performance = ClampPercentage(performance);
        quality = ClampPercentage(quality);

        var oee = availability * performance * quality;

        return new OeeDto(
            Scope: scope,
            PlannedProductionMinutes: Math.Round(plannedProductionMinutes, 2),
            RunningMinutes: Math.Round(runningMinutes, 2),
            IdealCycleTimeSeconds: idealCycleTimeSeconds,
            GoodCount: goodCount,
            ScrapCount: scrapCount,
            TotalCount: totalCount,
            Availability: Math.Round(availability * 100, 2),
            Performance: Math.Round(performance * 100, 2),
            Quality: Math.Round(quality * 100, 2),
            Oee: Math.Round(oee * 100, 2),
            CalculatedAt: DateTimeOffset.UtcNow
        );
    }

    private static double ClampPercentage(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return 0;
        }

        return Math.Clamp(value, 0, 1);
    }
}

public record OeeDto(
    string Scope,
    double PlannedProductionMinutes,
    double RunningMinutes,
    double IdealCycleTimeSeconds,
    int GoodCount,
    int ScrapCount,
    int TotalCount,
    double Availability,
    double Performance,
    double Quality,
    double Oee,
    DateTimeOffset CalculatedAt
);