using Fabiq.SmartFactory.Api.Contracts;
using Fabiq.SmartFactory.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineStatusController : ControllerBase
{
    private readonly FactoryEventProcessor _processor;

    public MachineStatusController(FactoryEventProcessor processor)
    {
        _processor = processor;
    }

    [HttpPost]
    public async Task<ActionResult> UpdateMachineStatus(CreateMachineStatusRequest request)
    {
        try
        {
            var result = await _processor.UpdateMachineStatusAsync(new MachineStatusMessage(
                request.MachineId,
                request.Status,
                request.CurrentWorkOrderId,
                request.Notes,
                request.ReportedAt
            ));

            return Ok(new
            {
                message = "Machine status updated.",
                machineId = result.MachineId,
                lineId = result.LineId,
                status = result.Status,
                currentWorkOrderId = result.CurrentWorkOrderId,
                lastUpdatedAt = result.LastUpdatedAt
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
    }
}

public record CreateMachineStatusRequest(
    string MachineId,
    string Status,
    string? CurrentWorkOrderId,
    string? Notes,
    DateTimeOffset? ReportedAt
);