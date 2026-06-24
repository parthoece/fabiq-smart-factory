import { type FormEvent, useState } from "react";
import { getPartTraceability } from "../api/smartFactoryApi";
import type { PartTraceability } from "../api/smartFactoryApi";

export function TraceabilitySearch() {
  const [partId, setPartId] = useState("");
  const [result, setResult] = useState<PartTraceability | null>(null);
  const [error, setError] = useState("");

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();

    setError("");
    setResult(null);

    if (!partId.trim()) {
      setError("Enter a part ID.");
      return;
    }

    try {
      const data = await getPartTraceability(partId.trim());
      setResult(data);
    } catch {
      setError("No traceability history found for this part.");
    }
  }

  return (
    <section className="panel">
      <div className="panel-header">
        <h2>Part Traceability</h2>
        <span>Search by Part ID</span>
      </div>

      <form className="trace-form" onSubmit={handleSubmit}>
        <input
          value={partId}
          onChange={(event) => setPartId(event.target.value)}
          placeholder="Example: PCB-123456"
        />
        <button type="submit">Search</button>
      </form>

      {error && <p className="error-text">{error}</p>}

      {result && (
        <div className="trace-result">
          <div className="trace-summary">
            <strong>{result.partId}</strong>
            <span className="status-pill status-running">
              {result.finalStatus}
            </span>
          </div>

          <p className="muted">
            Current machine: {result.currentMachineId} / {result.currentLineId}
          </p>

          <div className="trace-table-wrapper">
            <table>
              <thead>
                <tr>
                  <th>Time</th>
                  <th>Machine</th>
                  <th>Work Order</th>
                  <th>Event</th>
                  <th>Defect</th>
                </tr>
              </thead>
              <tbody>
                {result.route.map((step) => (
                  <tr key={step.eventId}>
                    <td>{new Date(step.createdAt).toLocaleString()}</td>
                    <td>{step.machineId}</td>
                    <td>{step.workOrderId}</td>
                    <td>{step.eventType}</td>
                    <td>{step.defectType ?? "-"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </section>
  );
}