# Portfolio Guide

## Project Summary

**Fabiq Smart Factory MES Integration Platform** is a production-inspired smart manufacturing platform that demonstrates how modern Manufacturing Execution Systems (MES) ingest, process, store, and visualize manufacturing data using an event-driven architecture.

The platform simulates a PCB production line using a machine simulator, Apache Kafka, ASP.NET Core, PostgreSQL, React, Docker Compose, Prometheus, and Grafana. It also includes an AI anomaly worker that analyzes telemetry and generates maintenance alerts.

---

# Elevator Pitch (30 Seconds)

I built an event-driven smart manufacturing platform that simulates a PCB production line. Machine events are streamed through Apache Kafka, processed by ASP.NET Core services, stored in PostgreSQL, and visualized in a React dashboard. The platform also demonstrates telemetry-driven anomaly detection, health-aware orchestration, and observability with Prometheus and Grafana.

---

# Elevator Pitch (2 Minutes)

Fabiq demonstrates how data flows through a modern MES platform. A simulator generates machine status, production, downtime, and telemetry events that are transported through Kafka. Backend services validate and persist the events, calculate operational metrics such as OEE, expose REST APIs, and serve a live dashboard. A separate anomaly worker consumes telemetry, detects abnormal behavior using rule-based logic, and publishes maintenance alerts. The project emphasizes event-driven architecture, traceability, observability, and maintainable service boundaries.

---

# Skills Demonstrated

## Backend Engineering

- ASP.NET Core
- Entity Framework Core
- REST APIs
- Background workers
- Health checks

## Messaging

- Apache Kafka
- Event-driven architecture
- Asynchronous processing

## Data

- PostgreSQL
- Relational modeling
- Traceability

## Frontend

- React
- Vite
- Dashboard development

## Infrastructure

- Docker Compose
- Prometheus
- Grafana
- CI-ready repository

## Manufacturing Domain

- MES concepts
- OEE
- Downtime
- Work orders
- Machine telemetry
- Traceability
- IIoT integration patterns

---

# STAR Interview Story

**Situation**

Demonstrate a realistic manufacturing software platform without access to factory equipment.

**Task**

Design a system that reflects modern MES architecture and demonstrates backend engineering skills.

**Action**

Implemented a simulator, Kafka event streaming, ASP.NET Core APIs, PostgreSQL persistence, React dashboard, AI anomaly detection, health-aware orchestration, and observability.

**Result**

Produced a portable, reproducible demonstration platform that showcases smart manufacturing concepts and production-inspired software architecture.

---

# Suitable Roles

- Manufacturing Software Engineer
- MES Engineer
- Industrial IoT Engineer
- Backend Software Engineer
- Platform Engineer
- Automation Software Engineer

---

# Suggested Interview Topics

- Event-driven architecture
- Kafka messaging
- Manufacturing traceability
- OEE calculations
- Docker Compose orchestration
- AI anomaly detection
- Observability
- API design
- System scalability

---

# Related Documentation

- README.md
- architecture.md
- design-decisions.md
- demo-guide.md
- roadmap.md
