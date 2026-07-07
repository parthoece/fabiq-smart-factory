# Local Development Guide

This guide explains how to set up, run, verify, and reset the **Fabiq Smart Factory MES Integration Platform** in a local development environment.

For architecture and design details, see the documentation in the `docs/` directory.

---

# Prerequisites

| Software | Version |
|----------|---------|
| Docker Desktop | Latest |
| Docker Compose | Included |
| .NET SDK | 8.0+ |
| Entity Framework CLI | `dotnet-ef` |
| Node.js | 22+ |
| npm | Latest |

Install EF CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

# Clone

```bash
git clone https://github.com/parthoece/fabiq-smart-factory.git
cd fabiq-smart-factory
```

# Start

```bash
docker compose up -d --build
```

# Endpoints

- Dashboard: http://localhost:3000
- Backend: http://localhost:5078
- Swagger: http://localhost:5078/swagger
- Health: http://localhost:5078/health
- Ready: http://localhost:5078/ready
- Metrics: http://localhost:5078/metrics
- Kafka UI: http://localhost:8081
- Prometheus: http://localhost:9090
- Grafana: http://localhost:3001

# Verify

```bash
docker compose ps
curl http://localhost:5078/health
docker logs fabiq-backend --tail=100
docker logs fabiq-simulator --tail=100
```

# Local Development

Backend:

```bash
cd backend/Fabiq.SmartFactory.Api/Fabiq.SmartFactory.Api
dotnet restore
dotnet ef database update
dotnet run
```

Simulator:

```bash
cd machine-simulator/Fabiq.SmartFactory.Simulator
dotnet restore
dotnet run
```

Frontend:

```bash
cd frontend/fabiq-dashboard
npm install
npm run dev
```

# Common Commands

Start:

```bash
docker compose up -d --build
```

Stop:

```bash
docker compose down
```

Reset:

```bash
docker compose down --remove-orphans -v
```

# Windows Scripts

```powershell
.abiq-on.ps1
.abiq-off.ps1
.abiq-reset.ps1
```

# Build Checks

```bash
dotnet build
npm run build
npm run lint
```

# Documentation

- README.md
- docs/architecture.md
- docs/runtime-flow.md
- docs/deployment.md
- docs/api-reference.md
- docs/kafka-topics.md
- docs/troubleshooting.md
- docs/technical-faq.md
