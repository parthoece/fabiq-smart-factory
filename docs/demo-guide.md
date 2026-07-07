# Demo Guide

## Purpose

This guide provides a structured walkthrough for demonstrating the Fabiq Smart Factory MES Integration Platform during interviews, technical reviews, or portfolio presentations.

The objective is to explain the architecture, demonstrate the runtime behavior, and highlight the engineering decisions behind the platform.

---

# Before the Demo

Ensure the platform is running:

```bash
docker compose up -d --build
```

Verify:

- Dashboard is accessible
- Swagger UI loads
- Kafka UI is available
- Prometheus and Grafana are running
- All containers report healthy

---

# Demo Flow

## 1. Introduce the Project (1–2 min)

Explain that Fabiq is a production-inspired MES platform that simulates a PCB manufacturing line using an event-driven architecture.

Highlight:
- ASP.NET Core backend
- Apache Kafka
- PostgreSQL
- React dashboard
- AI anomaly worker
- Docker Compose
- Prometheus & Grafana

---

## 2. Show the Architecture (2 min)

Open the architecture diagram and explain:

Simulator → Kafka → Backend → PostgreSQL → Dashboard

Then describe the telemetry pipeline:

Telemetry → Kafka → AI Worker → Maintenance Alerts → Backend → Dashboard

---

## 3. Start the Platform (1 min)

Show:

```bash
docker compose ps
```

Explain the health-aware startup sequence and topic initialization.

---

## 4. Dashboard Walkthrough (3–4 min)

Demonstrate:

- Machine status
- Active work orders
- Production counts
- Downtime
- OEE
- Traceability
- Maintenance alerts

Explain how each view is backed by persisted manufacturing events.

---

## 5. Kafka Event Flow (2 min)

Open Kafka UI and show:

- machine.status
- production.counts
- downtime.events
- machine.telemetry
- maintenance.alerts

Explain how producers and consumers remain loosely coupled.

---

## 6. Swagger & APIs (2 min)

Open Swagger and highlight:

- Machines
- Work Orders
- Production Events
- Downtime
- OEE
- Traceability
- Health

Explain that the dashboard consumes these REST APIs while manufacturing events flow through Kafka.

---

## 7. Observability (2 min)

Open Prometheus and Grafana.

Discuss:

- Metrics
- Health
- Event throughput
- Operational visibility

---

## 8. Engineering Decisions (2 min)

Summarize why the project uses:

- Event-driven architecture
- Kafka
- PostgreSQL
- Docker Compose
- Independent AI worker
- Health checks
- Observability

---

# Key Takeaways

The platform demonstrates:

- Event-driven backend engineering
- Smart manufacturing concepts
- Manufacturing traceability
- OEE reporting
- AI-assisted anomaly detection
- Containerized deployment
- Production-inspired architecture

---

# Related Documentation

- README.md
- architecture.md
- runtime-flow.md
- api-reference.md
- technical-faq.md
- design-decisions.md
