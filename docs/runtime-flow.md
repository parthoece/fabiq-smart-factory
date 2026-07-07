# Runtime Flow

## Overview

This document describes how the Fabiq Smart Factory MES Integration
Platform starts, coordinates services, and processes manufacturing
events during runtime.

The platform uses Docker Compose with health checks, one-shot
initialization services, and readiness-aware dependencies to provide a
predictable startup sequence.

------------------------------------------------------------------------

## Startup Sequence

``` text
PostgreSQL
    │
    ▼
Health Check (pg_isready)
    │
    ▼
Database Migrator
    │
    ▼
Apache Kafka
    │
    ▼
Kafka Topic Initializer
    │
    ▼
Backend API
   ├──────────────┐
   ▼              ▼
AI Worker     React Frontend
   │
   ▼
Machine Simulator
```

### Startup Stages

1.  PostgreSQL starts and reports healthy.
2.  The database migrator applies Entity Framework migrations.
3.  Kafka starts and becomes available.
4.  The Kafka initializer creates required topics.
5.  The backend API starts after the database and Kafka are ready.
6.  The AI anomaly worker begins consuming telemetry.
7.  The simulator waits for backend health before seeding data.
8.  The frontend starts once the backend is healthy.

------------------------------------------------------------------------

## Runtime Data Flow

``` text
Machine Simulator
      │
      ▼
machine.status
production.counts
downtime.events
machine.telemetry
      │
      ▼
Apache Kafka
      │
      ▼
Backend API
      │
      ▼
PostgreSQL
      │
      ▼
React Dashboard
```

The backend consumes manufacturing events, validates them, persists
operational history, and exposes REST APIs used by the dashboard.

------------------------------------------------------------------------

## AI Alert Pipeline

``` text
Machine Telemetry
        │
        ▼
Apache Kafka
        │
        ▼
AI Anomaly Worker
        │
maintenance.alerts
        │
        ▼
Apache Kafka
        │
        ▼
Backend API
        │
        ▼
PostgreSQL
        │
        ▼
Dashboard Alert Panel
```

Rule-based anomaly detection evaluates telemetry such as temperature,
vibration, cycle time, error count, and scrap-rate spikes before
publishing maintenance alerts.

------------------------------------------------------------------------

## Observability Flow

``` text
Backend Metrics
      │
      ▼
Prometheus
      │
      ▼
Grafana
```

The backend exposes health, readiness, and metrics endpoints that
support local monitoring and Docker Compose orchestration.

------------------------------------------------------------------------

## Health and Readiness

The platform uses health-aware startup instead of relying only on
container state.

### Health Endpoints

-   `/health` --- service liveness
-   `/ready` --- dependency readiness
-   `/metrics` --- Prometheus metrics

These endpoints allow dependent services to wait until required
components are actually ready.

------------------------------------------------------------------------

## Kafka Topic Initialization

Topics are created automatically during startup to simplify local
development.

Core topics:

-   `machine.status`
-   `production.counts`
-   `downtime.events`
-   `machine.telemetry`
-   `maintenance.alerts`

------------------------------------------------------------------------

## Failure Recovery

The runtime design supports common local failure scenarios:

-   Simulator retries backend connectivity.
-   Backend waits for infrastructure dependencies.
-   Topic initialization runs once and exits successfully.
-   Health checks reduce startup race conditions.
-   Containers can be restarted independently.

------------------------------------------------------------------------

## Runtime Responsibilities

  Service           Responsibility
  ----------------- ----------------------------------
  PostgreSQL        Persistent manufacturing history
  Kafka             Event transport
  Backend API       Processing, persistence, APIs
  AI Worker         Telemetry analysis
  Simulator         Manufacturing event generation
  React Dashboard   Operational visualization
  Prometheus        Metrics collection
  Grafana           Metrics visualization

------------------------------------------------------------------------

## Related Documentation

-   `architecture.md`
-   `deployment.md`
-   `kafka-topics.md`
-   `api-reference.md`
-   `troubleshooting.md`
