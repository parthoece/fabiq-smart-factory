using System.Text;
using Fabiq.SmartFactory.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fabiq.SmartFactory.Api.Controllers;

[ApiController]
public class MetricsController : ControllerBase
{
    private readonly ApplicationMetrics _metrics;

    public MetricsController(ApplicationMetrics metrics)
    {
        _metrics = metrics;
    }

    [HttpGet("metrics")]
    [Produces("text/plain")]
    public IActionResult GetMetrics()
    {
        var output = new StringBuilder();

        output.AppendLine("# TYPE fabiq_machine_status_events_processed_total counter");
        output.AppendLine($"fabiq_machine_status_events_processed_total {_metrics.MachineStatusEventsProcessed}");
        output.AppendLine("# TYPE fabiq_production_events_processed_total counter");
        output.AppendLine($"fabiq_production_events_processed_total {_metrics.ProductionEventsProcessed}");
        output.AppendLine("# TYPE fabiq_downtime_events_processed_total counter");
        output.AppendLine($"fabiq_downtime_events_processed_total {_metrics.DowntimeEventsProcessed}");
        output.AppendLine("# TYPE fabiq_maintenance_alerts_created_total counter");
        output.AppendLine($"fabiq_maintenance_alerts_created_total {_metrics.MaintenanceAlertsCreated}");

        return Content(output.ToString(), "text/plain", Encoding.UTF8);
    }
}