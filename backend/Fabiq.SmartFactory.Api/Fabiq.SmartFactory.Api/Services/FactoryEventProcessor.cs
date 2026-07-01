using Fabiq.SmartFactory.Api.Contracts;
using Fabiq.SmartFactory.Api.Data;
using Fabiq.SmartFactory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabiq.SmartFactory.Api.Services;

public sealed record MachineStatusUpdateResult(
    string MachineId,
    string LineId,
    string Status,
    string? CurrentWorkOrderId,
    DateTimeOffset LastUpdatedAt
);

public sealed record ProductionEventResult(
    string Message,
    string EventId,
    string MachineId,
    string WorkOrderId,
    int MachineGoodCount,
    int MachineScrapCount,
    int WorkOrderGoodCount,
    int WorkOrderScrapCount,
    string WorkOrderStatus
);

public sealed record DowntimeEventResult(
    string Message,
    string EventId,
    string MachineId,
    string LineId,
    string? WorkOrderId,
    string ReasonCode,
    int DurationMinutes,
    DateTimeOffset StartedAt,
    DateTimeOffset EndedAt
);

public sealed record MaintenanceAlertResult(
    string Message,
    string AlertId,
    string MachineId,
    string? WorkOrderId,
    string AlertType,
    string Severity,
    DateTimeOffset CreatedAt
);

public sealed class FactoryEventProcessor
{
    private static readonly string[] AllowedMachineStatuses = ["RUNNING", "IDLE", "DOWN", "UNKNOWN"];

    private readonly SmartFactoryDbContext _db;
    private readonly ApplicationMetrics _metrics;

    public FactoryEventProcessor(SmartFactoryDbContext db, ApplicationMetrics metrics)
    {
        _db = db;
        _metrics = metrics;
    }

    public async Task<MachineStatusUpdateResult> UpdateMachineStatusAsync(MachineStatusMessage request)
    {
        if (string.IsNullOrWhiteSpace(request.MachineId))
        {
            throw new ArgumentException("MachineId is required.", nameof(request.MachineId));
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            throw new ArgumentException("Status is required.", nameof(request.Status));
        }

        var status = request.Status.Trim().ToUpperInvariant();
        if (!AllowedMachineStatuses.Contains(status))
        {
            throw new ArgumentException($"Status must be one of: {string.Join(", ", AllowedMachineStatuses)}.", nameof(request.Status));
        }

        var machine = await _db.Machines.FirstOrDefaultAsync(m => m.MachineId == request.MachineId);
        if (machine is null)
        {
            throw new KeyNotFoundException($"Machine {request.MachineId} was not found.");
        }

        machine.Status = status;
        machine.CurrentWorkOrderId = string.IsNullOrWhiteSpace(request.CurrentWorkOrderId)
            ? machine.CurrentWorkOrderId
            : request.CurrentWorkOrderId.Trim();
        machine.LastUpdatedAt = request.ReportedAt ?? DateTimeOffset.UtcNow;

        _db.MachineStatusEvents.Add(new MachineStatusEvent
        {
            MachineId = machine.MachineId,
            LineId = machine.LineId,
            Status = machine.Status,
            CurrentWorkOrderId = machine.CurrentWorkOrderId,
            Notes = request.Notes,
            CreatedAt = machine.LastUpdatedAt
        });

        await _db.SaveChangesAsync();
        _metrics.IncrementMachineStatusEventsProcessed();

        return new MachineStatusUpdateResult(
            machine.MachineId,
            machine.LineId,
            machine.Status,
            machine.CurrentWorkOrderId,
            machine.LastUpdatedAt
        );
    }

    public async Task<ProductionEventResult> RecordProductionEventAsync(ProductionEventMessage request)
    {
        if (request.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than 0.", nameof(request.Quantity));
        }

        var allowedEventTypes = new[] { "GOOD_PART", "SCRAP_PART" };
        if (!allowedEventTypes.Contains(request.EventType))
        {
            throw new ArgumentException("EventType must be GOOD_PART or SCRAP_PART.", nameof(request.EventType));
        }

        var machine = await _db.Machines.FirstOrDefaultAsync(m => m.MachineId == request.MachineId)
            ?? throw new KeyNotFoundException($"Machine {request.MachineId} was not found.");

        var workOrder = await _db.WorkOrders.FirstOrDefaultAsync(w => w.WorkOrderId == request.WorkOrderId)
            ?? throw new KeyNotFoundException($"Work order {request.WorkOrderId} was not found.");

        if (!string.Equals(workOrder.LineId, machine.LineId, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Machine {machine.MachineId} is on {machine.LineId}, but work order {workOrder.WorkOrderId} is on {workOrder.LineId}.");
        }

        var productionEvent = new ProductionEvent
        {
            MachineId = machine.MachineId,
            WorkOrderId = workOrder.WorkOrderId,
            LineId = machine.LineId,
            EventType = request.EventType,
            Quantity = request.Quantity,
            PartId = request.PartId,
            DefectType = request.DefectType,
            Notes = request.Notes,
            CreatedAt = DateTimeOffset.UtcNow
        };

        if (request.EventType == "GOOD_PART")
        {
            machine.GoodCount += request.Quantity;
            workOrder.GoodCount += request.Quantity;
        }
        else
        {
            machine.ScrapCount += request.Quantity;
            workOrder.ScrapCount += request.Quantity;
        }

        machine.CurrentWorkOrderId = workOrder.WorkOrderId;
        machine.Status = "RUNNING";
        machine.LastUpdatedAt = DateTimeOffset.UtcNow;

        if (workOrder.Status == "PLANNED")
        {
            workOrder.Status = "RUNNING";
            workOrder.StartedAt = DateTimeOffset.UtcNow;
        }

        var totalProduced = workOrder.GoodCount + workOrder.ScrapCount;
        if (totalProduced >= workOrder.PlannedQuantity)
        {
            workOrder.Status = "COMPLETED";
            workOrder.CompletedAt = DateTimeOffset.UtcNow;
        }

        _db.ProductionEvents.Add(productionEvent);
        await _db.SaveChangesAsync();
        _metrics.IncrementProductionEventsProcessed();

        return new ProductionEventResult(
            "Production event created.",
            productionEvent.EventId,
            machine.MachineId,
            workOrder.WorkOrderId,
            machine.GoodCount,
            machine.ScrapCount,
            workOrder.GoodCount,
            workOrder.ScrapCount,
            workOrder.Status
        );
    }

    public async Task<DowntimeEventResult> RecordDowntimeEventAsync(DowntimeEventMessage request)
    {
        if (string.IsNullOrWhiteSpace(request.MachineId))
        {
            throw new ArgumentException("MachineId is required.", nameof(request.MachineId));
        }

        if (string.IsNullOrWhiteSpace(request.ReasonCode))
        {
            throw new ArgumentException("ReasonCode is required.", nameof(request.ReasonCode));
        }

        if (request.DurationMinutes <= 0)
        {
            throw new ArgumentException("DurationMinutes must be greater than 0.", nameof(request.DurationMinutes));
        }

        var machine = await _db.Machines.FirstOrDefaultAsync(m => m.MachineId == request.MachineId)
            ?? throw new KeyNotFoundException($"Machine {request.MachineId} was not found.");

        WorkOrder? workOrder = null;
        if (!string.IsNullOrWhiteSpace(request.WorkOrderId))
        {
            workOrder = await _db.WorkOrders.FirstOrDefaultAsync(w => w.WorkOrderId == request.WorkOrderId)
                ?? throw new KeyNotFoundException($"Work order {request.WorkOrderId} was not found.");

            if (!string.Equals(workOrder.LineId, machine.LineId, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Machine {machine.MachineId} is on {machine.LineId}, but work order {workOrder.WorkOrderId} is on {workOrder.LineId}.");
            }
        }

        var startedAt = request.StartedAt ?? DateTimeOffset.UtcNow.AddMinutes(-request.DurationMinutes);
        var endedAt = startedAt.AddMinutes(request.DurationMinutes);

        var downtimeEvent = new DowntimeEvent
        {
            MachineId = machine.MachineId,
            LineId = machine.LineId,
            WorkOrderId = workOrder?.WorkOrderId,
            ReasonCode = request.ReasonCode.Trim().ToUpperInvariant(),
            DurationMinutes = request.DurationMinutes,
            StartedAt = startedAt,
            EndedAt = endedAt,
            Notes = request.Notes,
            CreatedAt = DateTimeOffset.UtcNow
        };

        machine.Status = "DOWN";
        machine.CurrentWorkOrderId = workOrder?.WorkOrderId ?? machine.CurrentWorkOrderId;
        machine.LastUpdatedAt = DateTimeOffset.UtcNow;

        _db.DowntimeEvents.Add(downtimeEvent);
        await _db.SaveChangesAsync();
        _metrics.IncrementDowntimeEventsProcessed();

        return new DowntimeEventResult(
            "Downtime event created.",
            downtimeEvent.EventId,
            downtimeEvent.MachineId,
            downtimeEvent.LineId,
            downtimeEvent.WorkOrderId,
            downtimeEvent.ReasonCode,
            downtimeEvent.DurationMinutes,
            downtimeEvent.StartedAt,
            downtimeEvent.EndedAt
        );
    }

    public async Task<MaintenanceAlertResult> RecordMaintenanceAlertAsync(MaintenanceAlertMessage request)
    {
        if (string.IsNullOrWhiteSpace(request.AlertId))
        {
            throw new ArgumentException("AlertId is required.", nameof(request.AlertId));
        }

        if (string.IsNullOrWhiteSpace(request.MachineId))
        {
            throw new ArgumentException("MachineId is required.", nameof(request.MachineId));
        }

        if (string.IsNullOrWhiteSpace(request.AlertType))
        {
            throw new ArgumentException("AlertType is required.", nameof(request.AlertType));
        }

        if (string.IsNullOrWhiteSpace(request.Severity))
        {
            throw new ArgumentException("Severity is required.", nameof(request.Severity));
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new ArgumentException("Message is required.", nameof(request.Message));
        }

        var duplicateAlert = await _db.MaintenanceAlerts.AnyAsync(a => a.AlertId == request.AlertId);
        if (duplicateAlert)
        {
            return new MaintenanceAlertResult(
                "Maintenance alert already existed.",
                request.AlertId,
                request.MachineId,
                request.WorkOrderId,
                request.AlertType,
                request.Severity,
                request.CreatedAt
            );
        }

        var alert = new MaintenanceAlert
        {
            AlertId = request.AlertId,
            MachineId = request.MachineId.Trim(),
            WorkOrderId = string.IsNullOrWhiteSpace(request.WorkOrderId) ? null : request.WorkOrderId.Trim(),
            AlertType = request.AlertType.Trim(),
            Severity = request.Severity.Trim(),
            Message = request.Message.Trim(),
            SourceEventId = request.SourceEventId,
            TemperatureC = request.TemperatureC,
            VibrationMmS = request.VibrationMmS,
            CycleTimeSeconds = request.CycleTimeSeconds,
            ErrorCount = request.ErrorCount,
            ScrapRate = request.ScrapRate,
            CreatedAt = request.CreatedAt
        };

        _db.MaintenanceAlerts.Add(alert);
        await _db.SaveChangesAsync();
        _metrics.IncrementMaintenanceAlertsCreated();

        return new MaintenanceAlertResult(
            "Maintenance alert created.",
            alert.AlertId,
            alert.MachineId,
            alert.WorkOrderId,
            alert.AlertType,
            alert.Severity,
            alert.CreatedAt
        );
    }
}