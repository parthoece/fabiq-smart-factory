# Project Plan

## Phase 1 — Infrastructure

- Docker Compose: Postgres, Kafka, Kafka UI, MQTT
- Database schema
- Seed machine/work-order data

## Phase 2 — Event simulation

- Generate machine status events
- Generate production count events
- Generate AOI quality inspection events
- Generate downtime events
- Simulator seeds machines and work orders
- Simulator continuously drives the backend API

## Phase 3 — Backend API

- Machine status endpoint
- Work order endpoint
- OEE endpoint
- Downtime endpoint
- Part traceability endpoint

## Phase 4 — Dashboard

- Machine status cards
- OEE chart
- Downtime reason chart
- Quality/defect chart
- Part traceability search

## Phase 5 — Interview polish

- Architecture diagram
- Demo script
- Screenshots/GIF
- README improvements
- Interview prep notes

## Current docs

- `RUN.md` - quick run and full rebuild instructions
- `docs/project-architecture.md` - repo layout and runtime flow
- `docs/interview-prep.md` - demo script and interview talking points

## Current status

- Phase 1 complete
- Phase 2 in place with a runnable simulator
- Phase 3 complete for the current dashboard/API slice
- Phase 4 complete for the current dashboard/API slice
