import { useEffect, useState } from "react";
import {
  getDowntimeSummary,
  getLineOee,
  getMachines,
  getRecentProductionEvents,
} from "./api/smartFactoryApi";

import type {
  DowntimeSummary,
  Machine,
  OeeResponse,
  ProductionEvent,
} from "./api/smartFactoryApi";

import { DowntimeChart } from "./components/DowntimeChart";
import { MachineStatusCards } from "./components/MachineStatusCards";
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
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  async function loadDashboard() {
    try {
      setError("");

      const [machinesData, oeeData, downtimeData, eventsData] =
        await Promise.all([
          getMachines(),
          getLineOee(LINE_ID),
          getDowntimeSummary(LINE_ID),
          getRecentProductionEvents(),
        ]);

      setMachines(machinesData);
      setOee(oeeData);
      setDowntime(downtimeData);
      setEvents(eventsData);
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
            <TraceabilitySearch />
          </div>
        </>
      )}
    </main>
  );
}

export default App;