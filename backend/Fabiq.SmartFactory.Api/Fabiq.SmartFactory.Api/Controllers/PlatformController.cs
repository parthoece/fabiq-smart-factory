using System.Reflection;
using Fabiq.SmartFactory.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformController : ControllerBase
{
    private readonly IOptions<KafkaOptions> _kafkaOptions;

    public PlatformController(IOptions<KafkaOptions> kafkaOptions)
    {
        _kafkaOptions = kafkaOptions;
    }

    [HttpGet("services")]
    public IActionResult GetServices()
    {
        var options = _kafkaOptions.Value;

        return Ok(new[]
        {
            new PlatformServiceInfo("api", "running", "/health", null),
            new PlatformServiceInfo("database", "configured", "Postgres", null),
            new PlatformServiceInfo("kafka", "configured", options.BootstrapServers, null),
            new PlatformServiceInfo("simulator", "configured", options.MachineStatusTopic, "machine.status"),
            new PlatformServiceInfo("anomaly-worker", "configured", options.TelemetryTopic, "machine.telemetry")
        });
    }

    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        var assembly = Assembly.GetEntryAssembly() ?? typeof(PlatformController).Assembly;

        return Ok(new
        {
            service = "Fabiq Smart Factory API",
            version = assembly.GetName().Version?.ToString() ?? "unknown",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        });
    }
}

public sealed record PlatformServiceInfo(
    string Name,
    string Status,
    string Endpoint,
    string? Topic
);