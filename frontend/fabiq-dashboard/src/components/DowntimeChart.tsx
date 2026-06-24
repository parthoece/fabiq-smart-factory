import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import type { DowntimeSummary } from "../api/smartFactoryApi";

type Props = {
  data: DowntimeSummary[];
};

export function DowntimeChart({ data }: Props) {
  return (
    <section className="panel">
      <div className="panel-header">
        <h2>Downtime by Reason</h2>
        <span>{data.length} reasons</span>
      </div>

      {data.length === 0 ? (
        <p className="muted">No downtime data available.</p>
      ) : (
        <div className="chart-container">
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={data}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="reasonCode" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="totalDurationMinutes" name="Minutes" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      )}
    </section>
  );
}