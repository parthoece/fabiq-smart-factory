# Technical FAQ

## Overview

This document explains the key engineering decisions behind the Fabiq Smart Factory MES Integration Platform. Rather than focusing on implementation details, it describes the architectural choices and the trade-offs made to create a production-inspired manufacturing platform.

---

# Why an Event-Driven Architecture?

Manufacturing systems generate a continuous stream of machine and production events. Using an event-driven architecture allows producers and consumers to operate independently while supporting multiple downstream services such as dashboards, analytics, reporting, and AI.

---

# Why Apache Kafka Instead of REST?

REST works well for synchronous operations such as seeding data or serving dashboards.

Kafka is used for asynchronous manufacturing events because it:

- Decouples producers and consumers
- Supports multiple subscribers
- Preserves event ordering within partitions
- Better reflects industrial integration patterns

---

# Why PostgreSQL?

Manufacturing systems require historical, relational data for:

- Work orders
- Production history
- Downtime
- OEE calculations
- Traceability
- Maintenance alerts

A relational database is well suited for these relationships and reporting queries.

---

# Why Build a Simulator?

Real production equipment is not always available for development.

The simulator provides:

- Repeatable demonstrations
- Continuous manufacturing events
- Testable scenarios
- Consistent portfolio demonstrations

It represents production equipment without requiring PLC hardware.

---

# Why Add an AI Anomaly Worker?

The anomaly worker demonstrates how telemetry can be processed independently from the core backend.

Current implementation:

- Rule-based detection
- Temperature thresholds
- Vibration thresholds
- Cycle-time monitoring
- Error count monitoring

Future implementations could incorporate statistical models or machine learning without changing the event producers.

---

# Why Docker Compose?

Docker Compose enables the complete platform to run consistently across development environments.

Benefits include:

- Reproducible setup
- Service isolation
- Health-aware startup
- One-command deployment
- Easier demonstrations

---

# Why Prometheus and Grafana?

Observability is an important aspect of modern backend systems.

Prometheus collects metrics exposed by the backend while Grafana visualizes operational information such as event throughput and service health.

---

# Why Health Checks?

Container startup does not guarantee application readiness.

Health and readiness endpoints allow dependent services to wait until required components are available, reducing startup race conditions.

---

# How Could This Platform Evolve?

Potential production enhancements include:

- OPC UA integration
- PLC connectivity
- Authentication and RBAC
- Schema Registry for Kafka
- Kubernetes deployment
- Centralized logging
- Cloud deployment
- Predictive maintenance models
- Digital Twin integration

---

# Is This a Production MES?

No.

Fabiq is intentionally positioned as a production-inspired platform that demonstrates architecture, integration patterns, and engineering practices commonly found in Manufacturing Execution Systems and Industrial IoT solutions.

The focus is on demonstrating system design rather than replicating every capability of a commercial MES.

---

# Related Documentation

- README.md
- architecture.md
- runtime-flow.md
- deployment.md
- api-reference.md
- kafka-topics.md
- troubleshooting.md
