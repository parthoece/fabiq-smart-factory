import { useEffect, useState } from "react";
import {
  getActiveMaintenanceAlerts,
  getDowntimeSummary,
  getLineOee,
  getMachines,
  getRecentProductionEvents,
} from "./api/smartFactoryApi";

import type {
  DowntimeSummary,
  Machine,
  MaintenanceAlert,
  OeeResponse,
  ProductionEvent,
} from "./api/smartFactoryApi";

import { DowntimeChart } from "./components/DowntimeChart";
import { MachineStatusCards } from "./components/MachineStatusCards";
import { MaintenanceAlertsPanel } from "./components/MaintenanceAlertsPanel";
import { OeePanel } from "./components/OeePanel";
import { QualityChart } from "./components/QualityChart";
import { TraceabilitySearch } from "./components/TraceabilitySearch";
import "./index.css";

const LINE_ID = "LINE-A";

function App() {
  const [machines, setMachines] = useState<Machine[]>([]);
  const [oee, setOee] = useState<OeeResponse | null>(null);
  const [downtime, setDowntime] = useState<DowntimeSummary[]>([]);
  const [events, setEvents] = useState<ProductionEvent[]>([]);
  const [alerts, setAlerts] = useState<MaintenanceAlert[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const recentPartIds = Array.from(
    new Set(events.map((event) => event.partId).filter((partId): partId is string => Boolean(partId)))
  ).slice(0, 8);
  const activeAlertMachine = alerts[0]?.machineId ?? "None";

  async function loadDashboard() {
    try {
      setError("");

      const [machinesData, oeeData, downtimeData, eventsData, alertsData] =
        await Promise.all([
          getMachines(),
          getLineOee(LINE_ID),
          getDowntimeSummary(LINE_ID),
          getRecentProductionEvents(),
          getActiveMaintenanceAlerts(),
        ]);

      setMachines(machinesData);
      setOee(oeeData);
      setDowntime(downtimeData);
      setEvents(eventsData);
      setAlerts(alertsData);
    } catch {
      setError("Unable to load dashboard data. Check that the backend API is running.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadDashboard();

    const intervalId = window.setInterval(loadDashboard, 10000);

    return () => window.clearInterval(intervalId);
  }, []);

  return (
    <main className="app">
      <header className="hero">
        <div>
          <p className="eyebrow">Fabiq Smart Factory</p>
          <h1>MES Integration Dashboard</h1>
          <p>
            Live view of machine status, OEE, downtime, quality, and part
            traceability for Line A.
          </p>
        </div>

        <button onClick={loadDashboard}>Refresh</button>
      </header>

      <section className="status-strip">
        <div className="status-card">
          <span className="metric-label">AI anomaly alerts</span>
          <strong>{alerts.length}</strong>
          <p className="muted">Active alerts from the anomaly worker</p>
        </div>
        <div className="status-card">
          <span className="metric-label">Most recent alert machine</span>
          <strong>{activeAlertMachine}</strong>
          <p className="muted">Latest machine flagged by telemetry</p>
        </div>
        <div className="status-card">
          <span className="metric-label">Traceable parts</span>
          <strong>{recentPartIds.length}</strong>
          <p className="muted">Click a recent part ID to search traceability</p>
        </div>
      </section>

      {loading && <p className="muted">Loading dashboard...</p>}
      {error && <p className="error-text">{error}</p>}

      {!loading && (
        <>
          <MachineStatusCards machines={machines} />

          <div className="dashboard-grid">
            <OeePanel oee={oee} />
            <DowntimeChart data={downtime} />
          </div>

          <div className="dashboard-grid">
            <QualityChart events={events} />
            <TraceabilitySearch recentPartIds={recentPartIds} />
          </div>

          <MaintenanceAlertsPanel alerts={alerts} />
        </>
      )}
    </main>
  );
}

export default App;