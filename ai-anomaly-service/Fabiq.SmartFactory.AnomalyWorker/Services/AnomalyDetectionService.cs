using Fabiq.SmartFactory.AnomalyWorker.Models;

namespace Fabiq.SmartFactory.AnomalyWorker.Services;

public sealed class AnomalyDetectionService
{
    public IReadOnlyList<MaintenanceAlertMessage> Analyze(MachineTelemetryMessage telemetry)
    {
        var alerts = new List<MaintenanceAlertMessage>();

        if (telemetry.TemperatureC > 85m)
        {
            alerts.Add(CreateAlert(
                telemetry,
                "HIGH_TEMPERATURE",
                telemetry.TemperatureC >= 95m ? "CRITICAL" : "WARNING",
                $"High temperature detected: {telemetry.TemperatureC}°C"
            ));
        }

        if (telemetry.VibrationMmS > 5.0m)
        {
            alerts.Add(CreateAlert(
                telemetry,
                "HIGH_VIBRATION",
                telemetry.VibrationMmS >= 7.0m ? "CRITICAL" : "WARNING",
                $"High vibration detected: {telemetry.VibrationMmS} mm/s"
            ));
        }

        if (telemetry.CycleTimeSeconds > 40m)
        {
            alerts.Add(CreateAlert(
                telemetry,
                "SLOW_CYCLE_TIME",
                "WARNING",
                $"Slow cycle time detected: {telemetry.CycleTimeSeconds} seconds"
            ));
        }

        if (telemetry.ErrorCount >= 3)
        {
            alerts.Add(CreateAlert(
                telemetry,
                "ERROR_SPIKE",
                telemetry.ErrorCount >= 5 ? "CRITICAL" : "WARNING",
                $"Error spike detected: {telemetry.ErrorCount} errors"
            ));
        }

        if (telemetry.ScrapRate >= 0.08m)
        {
            alerts.Add(CreateAlert(
                telemetry,
                "HIGH_SCRAP_RATE",
                telemetry.ScrapRate >= 0.12m ? "CRITICAL" : "WARNING",
                $"High scrap rate detected: {telemetry.ScrapRate:P2}"
            ));
        }

        return alerts;
    }

    private static MaintenanceAlertMessage CreateAlert(
        MachineTelemetryMessage telemetry,
        string alertType,
        string severity,
        string message)
    {
        return new MaintenanceAlertMessage(
            AlertId: Guid.NewGuid().ToString("N"),
            MachineId: telemetry.MachineId,
            WorkOrderId: telemetry.WorkOrderId,
            AlertType: alertType,
            Severity: severity,
            Message: message,
            SourceEventId: telemetry.EventId,
            TemperatureC: telemetry.TemperatureC,
            VibrationMmS: telemetry.VibrationMmS,
            CycleTimeSeconds: telemetry.CycleTimeSeconds,
            ErrorCount: telemetry.ErrorCount,
            ScrapRate: telemetry.ScrapRate,
            CreatedAt: DateTimeOffset.UtcNow
        );
    }
}