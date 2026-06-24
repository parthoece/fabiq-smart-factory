CREATE TABLE IF NOT EXISTS machines (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    line_id TEXT NOT NULL,
    station_type TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS work_orders (
    id TEXT PRIMARY KEY,
    product_code TEXT NOT NULL,
    target_quantity INTEGER NOT NULL,
    status TEXT NOT NULL,
    started_at TIMESTAMPTZ,
    completed_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS machine_events (
    id BIGSERIAL PRIMARY KEY,
    machine_id TEXT NOT NULL REFERENCES machines(id),
    work_order_id TEXT REFERENCES work_orders(id),
    event_type TEXT NOT NULL,
    status TEXT,
    payload JSONB NOT NULL,
    event_time TIMESTAMPTZ NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS quality_inspections (
    id BIGSERIAL PRIMARY KEY,
    part_id TEXT NOT NULL,
    machine_id TEXT NOT NULL REFERENCES machines(id),
    work_order_id TEXT REFERENCES work_orders(id),
    result TEXT NOT NULL,
    defect_type TEXT,
    confidence NUMERIC(5,4),
    inspected_at TIMESTAMPTZ NOT NULL
);

CREATE TABLE IF NOT EXISTS downtime_events (
    id BIGSERIAL PRIMARY KEY,
    machine_id TEXT NOT NULL REFERENCES machines(id),
    reason TEXT NOT NULL,
    started_at TIMESTAMPTZ NOT NULL,
    ended_at TIMESTAMPTZ,
    duration_seconds INTEGER
);

CREATE TABLE IF NOT EXISTS part_traceability (
    id BIGSERIAL PRIMARY KEY,
    part_id TEXT NOT NULL,
    work_order_id TEXT NOT NULL REFERENCES work_orders(id),
    machine_id TEXT NOT NULL REFERENCES machines(id),
    station_result TEXT NOT NULL,
    event_time TIMESTAMPTZ NOT NULL
);
