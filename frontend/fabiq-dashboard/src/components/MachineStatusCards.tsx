import type { Machine } from "../api/smartFactoryApi";

type Props = {
  machines: Machine[];
};

function getStatusClass(status: string) {
  switch (status.toUpperCase()) {
    case "RUNNING":
      return "status-running";
    case "DOWN":
      return "status-down";
    case "IDLE":
      return "status-idle";
    default:
      return "status-unknown";
  }
}

export function MachineStatusCards({ machines }: Props) {
  return (
    <section className="panel">
      <div className="panel-header">
        <h2>Machine Status</h2>
        <span>{machines.length} machines</span>
      </div>

      <div className="machine-grid">
        {machines.map((machine) => (
          <div className="machine-card" key={machine.machineId}>
            <div className="machine-card-header">
              <h3>{machine.machineId}</h3>
              <span className={`status-pill ${getStatusClass(machine.status)}`}>
                {machine.status}
              </span>
            </div>

            <p className="muted">Line: {machine.lineId}</p>
            <p>Work Order: {machine.currentWorkOrderId ?? "None"}</p>

            <div className="metric-row">
              <div>
                <span className="metric-label">Good</span>
                <strong>{machine.goodCount}</strong>
              </div>
              <div>
                <span className="metric-label">Scrap</span>
                <strong>{machine.scrapCount}</strong>
              </div>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}