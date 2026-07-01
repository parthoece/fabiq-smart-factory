import type { MaintenanceAlert } from "../api/smartFactoryApi";

type Props = {
  alerts: MaintenanceAlert[];
};

function getSeverityClass(severity: string) {
  switch (severity.toUpperCase()) {
    case "CRITICAL":
      return "severity-critical";
    case "WARNING":
      return "severity-warning";
    case "INFO":
      return "severity-info";
    default:
      return "severity-unknown";
  }
}

export function MaintenanceAlertsPanel({ alerts }: Props) {
  return (
    <section className="panel">
      <div className="panel-header">
        <h2>AI Maintenance Alerts</h2>
        <span>{alerts.length} active alerts</span>
      </div>

      {alerts.length === 0 ? (
        <p className="muted">No active anomaly alerts.</p>
      ) : (
        <div className="trace-table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Time</th>
                <th>Machine</th>
                <th>Severity</th>
                <th>Alert Type</th>
                <th>Message</th>
                <th>Work Order</th>
              </tr>
            </thead>
            <tbody>
              {alerts.map((alert) => (
                <tr key={alert.alertId}>
                  <td>{new Date(alert.createdAt).toLocaleString()}</td>
                  <td>{alert.machineId}</td>
                  <td>
                    <span className={`status-pill ${getSeverityClass(alert.severity)}`}>
                      {alert.severity}
                    </span>
                  </td>
                  <td>{alert.alertType}</td>
                  <td>{alert.message}</td>
                  <td>{alert.workOrderId ?? "-"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
