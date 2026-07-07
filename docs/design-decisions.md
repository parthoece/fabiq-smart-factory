# Architecture Decision Records (ADRs)

## Overview

This document captures the major architectural decisions made during the development of the Fabiq Smart Factory MES Integration Platform.

Rather than describing implementation details, it explains *why* specific technologies, architectural patterns, and engineering approaches were selected.

These decisions prioritize maintainability, scalability, and manufacturing domain realism while keeping the platform practical for local development and demonstration.

---

# ADR-001 — Event-Driven Architecture

## Status

Accepted

## Context

Manufacturing systems continuously generate machine status updates, production events, telemetry, quality information, and downtime notifications.

A synchronous request/response architecture would tightly couple every producer to every consumer.

## Decision

Adopt an event-driven architecture using Apache Kafka as the messaging backbone.

## Consequences

### Benefits

- Loose coupling
- Independent services
- Multiple consumers
- Easier scalability
- Better reflects industrial MES architectures

### Trade-offs

- Increased operational complexity
- Event ordering considerations
- Event schema management

---

# ADR-002 — Apache Kafka as the Event Backbone

## Status

Accepted

## Context

Machine events must be delivered to multiple services including:

- Backend API
- AI anomaly worker
- Future analytics services
- Reporting
- Observability

## Decision

Use Apache Kafka for asynchronous event streaming.

Primary topics include:

- machine.status
- production.counts
- downtime.events
- machine.telemetry
- maintenance.alerts

## Consequences

### Benefits

- Decoupled architecture
- Replayable events
- Durable messaging
- Natural manufacturing integration pattern

### Trade-offs

- Requires Kafka infrastructure
- Additional operational monitoring

---

# ADR-003 — PostgreSQL for Manufacturing Data

## Status

Accepted

## Context

Manufacturing history requires strong relational integrity.

Examples include:

- Work Orders
- Machines
- Production Events
- Downtime
- Traceability
- Maintenance Alerts

## Decision

Persist operational data in PostgreSQL.

## Consequences

### Benefits

- Strong relational model
- Excellent querying
- ACID transactions
- Mature ecosystem

### Trade-offs

- Less suitable for high-frequency time-series telemetry
- Horizontal scaling requires additional planning

---

# ADR-004 — Simulator Instead of PLC Hardware

## Status

Accepted

## Context

Real manufacturing equipment is unavailable during development.

The platform must remain reproducible on any developer machine.

## Decision

Develop a machine simulator capable of generating realistic manufacturing activity.

The simulator produces:

- Machine status
- Production counts
- Downtime
- Telemetry
- Work-order activity

## Consequences

### Benefits

- Repeatable demonstrations
- Automated testing
- Continuous event generation
- Hardware independence

### Trade-offs

- Simplified machine behavior
- Does not replace real PLC integration

---

# ADR-005 — AI Worker as an Independent Service

## Status

Accepted

## Context

Machine telemetry should be analyzed without coupling AI logic to the backend.

## Decision

Implement anomaly detection as an independent background worker.

Current implementation:

- Rule-based detection
- Threshold evaluation
- Maintenance alert generation

Future implementations may include:

- Statistical models
- ML.NET
- Python models
- Predictive maintenance

## Consequences

### Benefits

- Independent deployment
- Replaceable AI implementation
- Scalable processing

### Trade-offs

- Additional infrastructure
- Cross-service coordination

---

# ADR-006 — Docker Compose for Local Development

## Status

Accepted

## Context

The platform contains multiple independent services.

Developers should be able to run the entire environment consistently.

## Decision

Use Docker Compose as the local orchestration platform.

The stack includes:

- PostgreSQL
- Kafka
- Kafka UI
- MQTT
- Backend API
- AI Worker
- Simulator
- Frontend
- Prometheus
- Grafana

## Consequences

### Benefits

- One-command startup
- Reproducible environment
- Simplified onboarding

### Trade-offs

- Higher resource consumption
- Longer first-time builds

---

# ADR-007 — Health and Readiness Checks

## Status

Accepted

## Context

Containers may be running before applications are ready.

Dependent services should wait until required services become available.

## Decision

Expose health and readiness endpoints.

Examples:

- `/health`
- `/ready`

Docker Compose uses these endpoints to coordinate startup.

## Consequences

### Benefits

- Reliable startup
- Reduced race conditions
- Better diagnostics

---

# ADR-008 — Prometheus & Grafana

## Status

Accepted

## Context

Modern backend systems require operational visibility.

## Decision

Expose Prometheus-compatible metrics and provide Grafana dashboards.

Tracked metrics include:

- Event throughput
- Alert counts
- Service health
- Request metrics

## Consequences

### Benefits

- Operational visibility
- Easier troubleshooting
- Demonstrates production engineering practices

---

# ADR-009 — REST APIs for External Consumers

## Status

Accepted

## Context

While manufacturing events are asynchronous, dashboards and external clients require synchronous access.

## Decision

Expose REST APIs through ASP.NET Core.

Examples include:

- Machine Status
- OEE
- Downtime
- Traceability
- Work Orders
- Maintenance Alerts

## Consequences

REST complements Kafka rather than replacing it.

Kafka handles event transport.

REST serves operational queries.

---

# Future ADRs

Potential future architectural decisions include:

- Kubernetes deployment
- Helm packaging
- Schema Registry
- Authentication & RBAC
- OPC UA gateway
- Digital Twin integration
- Time-series database
- Cloud-native deployment

---

# Decision Summary

| ADR | Decision |
|------|----------|
| ADR-001 | Event-driven architecture |
| ADR-002 | Apache Kafka |
| ADR-003 | PostgreSQL |
| ADR-004 | Manufacturing simulator |
| ADR-005 | Independent AI worker |
| ADR-006 | Docker Compose |
| ADR-007 | Health & readiness |
| ADR-008 | Prometheus & Grafana |
| ADR-009 | REST APIs |

---

# Related Documentation

- architecture.md
- runtime-flow.md
- deployment.md
- api-reference.md
- kafka-topics.md
- technical-faq.md
- roadmap.md