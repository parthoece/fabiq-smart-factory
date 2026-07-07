# Troubleshooting Guide

## Overview

This guide provides common troubleshooting steps for the Fabiq Smart Factory MES Integration Platform during local development and demonstrations.

---

# General Health Check

Verify all services are running:

```bash
docker compose ps
```

Expected services include:

- PostgreSQL
- Kafka
- Kafka UI
- Backend API
- AI Anomaly Worker
- Machine Simulator
- React Frontend
- Prometheus
- Grafana

---

# Backend Issues

## Backend does not start

Check logs:

```bash
docker logs fabiq-backend --tail=200
```

Verify health endpoint:

```bash
curl http://localhost:5078/health
```

Common causes:

- Database unavailable
- Kafka unavailable
- Pending migrations
- Port already in use

---

# Database Issues

Check PostgreSQL container:

```bash
docker logs fabiq-postgres --tail=100
```

Verify container health:

```bash
docker compose ps
```

If required, perform a clean reset:

```bash
docker compose down --remove-orphans -v
docker compose up -d --build
```

---

# Kafka Issues

Check Kafka logs:

```bash
docker logs fabiq-kafka --tail=200
```

Verify topics:

```bash
docker exec -it fabiq-kafka /opt/kafka/bin/kafka-topics.sh --bootstrap-server localhost:9092 --list
```

Expected topics:

- machine.status
- production.counts
- downtime.events
- machine.telemetry
- maintenance.alerts

---

# Simulator Issues

Check simulator logs:

```bash
docker logs fabiq-simulator --tail=200
```

Ensure:

- Backend health endpoint is available
- Kafka is reachable
- Seed data completed successfully

---

# Frontend Issues

If the dashboard is unavailable:

- Verify backend is healthy.
- Confirm the frontend container is running.
- Check browser developer tools for failed API requests.

Logs:

```bash
docker logs fabiq-frontend --tail=100
```

---

# AI Worker Issues

Verify the worker is running:

```bash
docker logs fabiq-ai-anomaly-worker --tail=200
```

Confirm telemetry is being published and maintenance alerts are produced.

---

# Prometheus & Grafana

Prometheus:

http://localhost:9090

Grafana:

http://localhost:3001

Verify:

- Prometheus target is UP.
- Grafana datasource is connected.
- Backend metrics endpoint is reachable.

---

# Docker Compose

Restart services:

```bash
docker compose restart
```

Rebuild:

```bash
docker compose up -d --build
```

Reset everything:

```bash
docker compose down --remove-orphans -v
```

---

# Log Collection

Useful commands:

```bash
docker logs fabiq-backend --tail=200
docker logs fabiq-simulator --tail=200
docker logs fabiq-kafka --tail=200
docker logs fabiq-postgres --tail=200
```

---

# Verification Checklist

- All containers are running
- Backend health endpoint returns Healthy
- Kafka topics exist
- Simulator publishes events
- Dashboard displays live data
- AI alerts appear when telemetry exceeds thresholds
- Prometheus collects metrics
- Grafana dashboards load

---

# Related Documentation

- architecture.md
- runtime-flow.md
- deployment.md
- api-reference.md
- kafka-topics.md
- technical-faq.md
