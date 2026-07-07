# Architecture

## Purpose

Fabiq Smart Factory MES Integration Platform demonstrates a
production-inspired Manufacturing Execution System (MES) architecture
using an event-driven design. The platform simulates a PCB/electronics
production line and shows how manufacturing events move from shop-floor
equipment to backend services, persistent storage, analytics, and
operational dashboards.

The architecture emphasizes loose coupling, scalability, observability,
and traceability while remaining lightweight enough to run locally with
Docker Compose.

------------------------------------------------------------------------

## Architecture Principles

-   **Event-Driven by Design** -- Manufacturing events are exchanged
    asynchronously through Apache Kafka.
-   **Loose Coupling** -- Producers and consumers communicate through
    topics instead of direct dependencies.
-   **Container-First Deployment** -- Every service runs independently
    and is orchestrated through Docker Compose.
-   **Observability by Default** -- Health, readiness, metrics,
    Prometheus, and Grafana are part of the platform.
-   **Traceability First** -- Manufacturing history is persisted for
    investigation and reporting.
-   **Incremental Intelligence** -- AI anomaly detection extends the
    event pipeline without changing existing services.

------------------------------------------------------------------------

## System Overview

![System Overview](diagrams/system-overview.svg)

Core runtime flow:

``` text
Machine Simulator
        │
        ▼
Apache Kafka
        │
 ┌──────┴────────┐
 ▼               ▼
Backend API   AI Anomaly Worker
 │               │
 ▼               │
 PostgreSQL ◄────┘
 │
 ▼
React Dashboard

Prometheus ─► Backend Metrics
Grafana ◄──── Prometheus
```

------------------------------------------------------------------------

## Service Responsibilities

### Machine Simulator

Simulates a PCB production line by generating:

-   Machine status
-   Production counts
-   Downtime events
-   Quality events
-   Machine telemetry

### Apache Kafka

Acts as the manufacturing event backbone.

Primary topics include:

-   `machine.status`
-   `machine.telemetry`
-   `production.counts`
-   `downtime.events`
-   `maintenance.alerts`

### Backend API

Responsible for:

-   Event validation
-   Domain logic
-   REST APIs
-   OEE calculations
-   Traceability
-   Manufacturing history
-   Metrics endpoints

### PostgreSQL

Stores:

-   Machines
-   Work Orders
-   Production Events
-   Downtime Events
-   Maintenance Alerts
-   Traceability history

### AI Anomaly Worker

Consumes telemetry events, evaluates rule-based conditions, and
publishes maintenance alerts back into Kafka.

### React Dashboard

Provides live operational visibility including:

-   Machine status
-   OEE
-   Downtime
-   Production
-   Traceability
-   Maintenance alerts

### Prometheus & Grafana

Prometheus collects backend metrics while Grafana visualizes operational
health and event-processing metrics.

------------------------------------------------------------------------

## Event Flow

1.  Simulator publishes manufacturing events.
2.  Kafka distributes events.
3.  Backend persists operational history.
4.  AI worker consumes telemetry.
5.  AI worker publishes maintenance alerts.
6.  Backend stores alerts.
7.  Dashboard displays operational KPIs.

------------------------------------------------------------------------

## Design Decisions

The architecture intentionally separates responsibilities so that new
consumers, analytics services, or integrations can subscribe to
manufacturing events without changing existing producers.

This mirrors common integration patterns used in MES and Industrial IoT
platforms.

------------------------------------------------------------------------

## Security Considerations

Future production deployments should include:

-   Authentication & Authorization
-   Role-Based Access Control (RBAC)
-   TLS
-   Secrets management
-   Kafka ACLs
-   Database least-privilege access

------------------------------------------------------------------------

## Scalability

The platform is designed so individual services can scale independently.

Potential future improvements include:

-   Kubernetes deployment
-   Schema Registry
-   OPC UA integration
-   PLC connectivity
-   Time-series storage
-   Predictive maintenance models

------------------------------------------------------------------------

## Related Documentation

-   `runtime-flow.md`
-   `deployment.md`
-   `api-reference.md`
-   `kafka-topics.md`
-   `technical-faq.md`
