# Project Plan

## Project Direction

This project started as a smart factory MES integration platform and is now being extended into an AI-ready smart manufacturing backend platform.

The completed MVP demonstrates the core manufacturing data pipeline:

```text
Machine Simulator → Kafka → ASP.NET Core Backend API → Postgres → React Dashboard
```

The next version focuses on AI anomaly detection, platform engineering, observability, reliability, and stronger interview/demo positioning for smart manufacturing, automation, MES, and AI backend roles.

---

## Phase 1 — Infrastructure

**Status: Complete**

* Docker Compose environment
* Postgres
* Kafka
* Kafka UI
* MQTT broker
* Database schema
* Seed machine data
* Seed work-order data

---

## Phase 2 — Event Simulation

**Status: Complete**

* Generate machine status events
* Generate production count events
* Generate AOI quality inspection events
* Generate downtime events
* Seed machines and work orders
* Continuously drive backend API through simulator-generated manufacturing activity

---

## Phase 3 — Backend API

**Status: Complete**

* Machine status endpoint
* Work order endpoint
* OEE endpoint
* Downtime endpoint
* Part traceability endpoint
* Production event endpoint
* Manufacturing KPI API slice for dashboard

---

## Phase 4 — Dashboard

**Status: Complete**

* Machine status cards
* OEE chart
* Downtime reason chart
* Quality / defect chart
* Part traceability search
* Live manufacturing KPI dashboard

---

## Phase 5 — Interview Polish v1

**Status: Mostly complete**

* Architecture diagram
* Runtime flow diagram
* Demo script
* README improvements
* Interview prep notes
* Local run documentation
* Swagger demo flow
* Kafka UI demo flow
* Screenshot / GIF preparation

---

# v2 Extension Plan

## Phase 6 — AI Anomaly Detection Extension

**Goal:** Add an AI backend capability on top of the existing event-driven manufacturing platform.

### Scope

* Add simulated machine telemetry events:

  * temperature
  * vibration
  * cycle time
  * error count
  * scrap spike indicator
* Publish telemetry events to Kafka topic:

```text
machine.telemetry
```

* Add an AI anomaly detection worker service.
* Consume telemetry events from Kafka.
* Detect abnormal machine behavior.
* Publish AI-generated alerts to Kafka topic:

```text
maintenance.alerts
```

* Persist anomaly alerts in Postgres.
* Expose backend API for maintenance alerts.
* Display anomaly alerts on the dashboard.

### Suggested implementation

* Add service folder:

```text
ai-anomaly-service/
```

* Use Python FastAPI worker, ML.NET worker, or a simple C# background worker.
* Start with rule-based anomaly detection first.
* Optionally upgrade to Isolation Forest, Z-score, moving average, or lightweight ML model later.

### Example anomaly logic

* Temperature exceeds expected range.
* Vibration increases above threshold.
* Cycle time is significantly slower than baseline.
* Scrap rate spikes during a work order.
* Repeated downtime events occur on the same machine.

### Deliverables

* `ai-anomaly-service`
* Kafka consumer for `machine.telemetry`
* Kafka producer for `maintenance.alerts`
* Backend alert persistence
* Alert API endpoint
* Dashboard alert panel
* README section explaining AI extension

---

## Phase 7 — Platform Engineering Improvements

**Goal:** Make the project look closer to a backend platform used by internal engineering, manufacturing, or automation teams.

### Scope

* Add backend health check endpoints.
* Add service readiness checks.
* Add environment-based configuration.
* Add structured logging.
* Add API error response format.
* Add validation for event payloads.
* Add basic retry behavior for event processing.
* Add local developer scripts.

### Suggested endpoints

```text
GET /health
GET /ready
GET /api/Platform/services
GET /api/Platform/version
```

### Suggested scripts

```text
scripts/start-demo.sh
scripts/stop-demo.sh
scripts/reset-db.sh
scripts/seed-demo-data.sh
scripts/run-smoke-test.sh
```

### Deliverables

* Health check endpoints
* Platform/version endpoint
* Developer automation scripts
* Improved local setup flow
* Updated `RUN.md`

---

## Phase 8 — Observability and Reliability

**Goal:** Show production-thinking beyond feature development.

### Scope

* Add Prometheus-compatible metrics or basic application metrics.
* Add Grafana dashboard if time allows.
* Track event ingestion counts.
* Track API request latency.
* Track anomaly count by machine.
* Track Kafka consumer processing status.
* Add backend logs for event processing.
* Add failure-handling documentation.

### Suggested metrics

```text
manufacturing_events_processed_total
machine_status_events_processed_total
production_count_events_processed_total
downtime_events_processed_total
telemetry_events_processed_total
maintenance_alerts_created_total
api_request_duration_ms
```

### Deliverables

* Metrics endpoint or metrics logs
* Optional Grafana dashboard
* Observability documentation
* Screenshot of metrics/dashboard
* README section: “Observability”

---

## Phase 9 — CI/CD and Quality Checks

**Goal:** Demonstrate engineering discipline and maintainability.

### Scope

* Add GitHub Actions workflow.
* Build backend.
* Build frontend.
* Run backend tests if available.
* Run frontend lint/build check.
* Validate Docker Compose configuration.
* Add smoke test workflow if feasible.

### Suggested workflow checks

```text
dotnet build
dotnet test
npm install
npm run build
docker compose config
```

### Deliverables

* `.github/workflows/ci.yml`
* CI badge in README
* Basic automated build validation
* Documentation of quality checks

---

## Phase 10 — Demo Polish v2

**Goal:** Prepare the project for resume, GitHub, and interview presentation.

### Scope

* Update README title and positioning.
* Add AI extension architecture diagram.
* Add updated runtime flow diagram.
* Add screenshots for:

  * dashboard
  * machine status
  * OEE
  * downtime
  * traceability
  * Kafka UI
  * Swagger
  * AI anomaly alerts
* Add a short GIF or screen recording.
* Add interview talking points for the AI extension.
* Add “Why this is relevant to smart manufacturing roles” section.

### Suggested README title

```text
Fabiq — AI-Ready Smart Factory MES Integration Platform
```

### Suggested one-line description

```text
A Dockerized smart manufacturing backend platform that simulates a PCB production line, streams machine and production events through Kafka, stores traceable manufacturing history in Postgres, visualizes live MES/OEE KPIs in React, and extends the pipeline with AI anomaly detection for machine telemetry.
```

### Deliverables

* Updated README
* Updated architecture diagram
* Updated demo script
* Updated screenshots/GIF
* Updated interview prep notes
* Resume-ready project summary

---

# Current Docs

* `RUN.md` — quick run and full rebuild instructions
* `docs/project-architecture.md` — repo layout and runtime flow
* `docs/interview-prep.md` — demo script and interview talking points
* `docs/screenshots/` — dashboard and system screenshots
* `docs/diagrams/` — architecture and runtime diagrams

---

# Updated Current Status

* Phase 1 complete
* Phase 2 complete with runnable simulator
* Phase 3 complete for current dashboard/API slice
* Phase 4 complete for current dashboard/API slice
* Phase 5 mostly complete for portfolio review
* Phase 6 planned as the main AI backend extension
* Phase 7 planned for platform-engineering polish
* Phase 8 planned for observability and reliability
* Phase 9 planned for CI/CD and quality checks
* Phase 10 planned for final demo and resume polish

---

# Priority Order

For job-search impact, complete the next work in this order:

1. AI anomaly detection extension
2. Dashboard alert panel
3. Health checks and platform endpoints
4. CI pipeline
5. Observability metrics
6. Updated README, screenshots, and interview demo script

---

# Final Portfolio Positioning

This project should be presented as one flagship project:

```text
Fabiq — AI-Ready Smart Factory MES Integration Platform
```

It demonstrates backend engineering, event-driven architecture, smart manufacturing domain knowledge, MES concepts, factory KPI tracking, traceability, and AI backend extensibility.
