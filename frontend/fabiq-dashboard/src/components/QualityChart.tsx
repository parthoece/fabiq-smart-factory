import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import type { ProductionEvent } from "../api/smartFactoryApi";

type Props = {
  events: ProductionEvent[];
};

export function QualityChart({ events }: Props) {
  const defectMap = new Map<string, number>();

  for (const event of events) {
    if (event.eventType === "SCRAP_PART") {
      const defect = event.defectType ?? "UNKNOWN";
      defectMap.set(defect, (defectMap.get(defect) ?? 0) + event.quantity);
    }
  }

  const data = Array.from(defectMap.entries()).map(([defectType, quantity]) => ({
    defectType,
    quantity,
  }));

  return (
    <section className="panel">
      <div className="panel-header">
        <h2>Quality / Defects</h2>
        <span>{data.length} defect types</span>
      </div>

      {data.length === 0 ? (
        <p className="muted">No scrap events available.</p>
      ) : (
        <div className="chart-container">
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={data}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="defectType" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="quantity" name="Scrap Quantity" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      )}
    </section>
  );
}