import type { OeeResponse } from "../api/smartFactoryApi";
type Props = {
  oee: OeeResponse | null;
};

function percent(value?: number) {
  if (value === undefined || Number.isNaN(value)) return "0.0%";
  return `${value.toFixed(1)}%`;
}

export function OeePanel({ oee }: Props) {
  return (
    <section className="panel">
      <div className="panel-header">
        <h2>Line OEE</h2>
        <span>LINE-A</span>
      </div>

      {!oee ? (
        <p className="muted">No OEE data available.</p>
      ) : (
        <>
          <div className="oee-main">
            <span>OEE</span>
            <strong>{percent(oee.oee)}</strong>
          </div>

          <div className="oee-grid">
            <div>
              <span className="metric-label">Availability</span>
              <strong>{percent(oee.availability)}</strong>
            </div>
            <div>
              <span className="metric-label">Performance</span>
              <strong>{percent(oee.performance)}</strong>
            </div>
            <div>
              <span className="metric-label">Quality</span>
              <strong>{percent(oee.quality)}</strong>
            </div>
          </div>

          <div className="metric-row">
            <div>
              <span className="metric-label">Good</span>
              <strong>{oee.goodCount}</strong>
            </div>
            <div>
              <span className="metric-label">Scrap</span>
              <strong>{oee.scrapCount}</strong>
            </div>
            <div>
              <span className="metric-label">Total</span>
              <strong>{oee.totalCount}</strong>
            </div>
          </div>
        </>
      )}
    </section>
  );
}