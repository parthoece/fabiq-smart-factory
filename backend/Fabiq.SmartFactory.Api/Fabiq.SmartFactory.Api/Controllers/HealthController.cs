using Fabiq.SmartFactory.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    private readonly SmartFactoryDbContext _db;

    public HealthController(SmartFactoryDbContext db)
    {
        _db = db;
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "healthy",
            service = "Fabiq Smart Factory API",
            timestamp = DateTimeOffset.UtcNow
        });
    }

    [HttpGet("ready")]
    public async Task<IActionResult> GetReady()
    {
        var databaseReady = await _db.Database.CanConnectAsync();

        if (!databaseReady)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "not-ready",
                database = "unavailable",
                timestamp = DateTimeOffset.UtcNow
            });
        }

        return Ok(new
        {
            status = "ready",
            database = "available",
            timestamp = DateTimeOffset.UtcNow
        });
    }
}