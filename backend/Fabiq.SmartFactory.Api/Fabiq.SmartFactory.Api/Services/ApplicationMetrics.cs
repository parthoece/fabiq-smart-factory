namespace Fabiq.SmartFactory.Api.Services;

public sealed class ApplicationMetrics
{
    private long _machineStatusEventsProcessed;
    private long _productionEventsProcessed;
    private long _downtimeEventsProcessed;
    private long _maintenanceAlertsCreated;

    public long MachineStatusEventsProcessed => Interlocked.Read(ref _machineStatusEventsProcessed);

    public long ProductionEventsProcessed => Interlocked.Read(ref _productionEventsProcessed);

    public long DowntimeEventsProcessed => Interlocked.Read(ref _downtimeEventsProcessed);

    public long MaintenanceAlertsCreated => Interlocked.Read(ref _maintenanceAlertsCreated);

    public void IncrementMachineStatusEventsProcessed() => Interlocked.Increment(ref _machineStatusEventsProcessed);

    public void IncrementProductionEventsProcessed() => Interlocked.Increment(ref _productionEventsProcessed);

    public void IncrementDowntimeEventsProcessed() => Interlocked.Increment(ref _downtimeEventsProcessed);

    public void IncrementMaintenanceAlertsCreated() => Interlocked.Increment(ref _maintenanceAlertsCreated);
}