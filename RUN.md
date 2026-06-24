# Run Guide

## Quick Run

This is the fastest path for a local demo.

### Required software

- Docker Desktop with Docker Compose
- .NET 8 SDK
- Entity Framework Core CLI: `dotnet-ef`
- Node.js 22 or newer
- npm

If you do not already have the EF Core CLI, install it with:

```bash
dotnet tool install --global dotnet-ef
```

### Windows note

The repo paths and commands in this guide are written for Windows PowerShell, but the Docker and .NET commands are the same on macOS and Linux with only path syntax adjusted.

### Start everything

```bash
docker compose up -d --build
```

### Open the app

- Frontend dashboard: http://localhost:3000
- Backend Swagger: http://localhost:5078/swagger
- Kafka UI: http://localhost:8081

### Stop everything

```bash
docker compose down
```

## Full Rebuild

Use this if you want to rebuild the project from scratch on a clean machine.

### 1. Install prerequisites

- Install Docker Desktop
- Install .NET 8 SDK
- Install Node.js 22+
- Install npm (bundled with Node)

### 2. Clone the repository

```bash
git clone <repo-url>
cd fabiq-smart-factory
```

### 3. Create local config files

Copy [.env.example](.env.example) to `.env` if you want to override local values.

### 4. Build and run infrastructure

```bash
docker compose up -d --build postgres kafka kafka-ui mqtt backend simulator frontend
```

### 5. Verify services

- `postgres` on port `5432`
- `kafka` on port `9092`
- `mqtt` on port `1883`
- backend on `http://localhost:5078`, 'http://localhost:5078/swagger'

- frontend on `http://localhost:3000`

### 6. Run the backend locally instead of Docker, if preferred

```bash
cd backend/Fabiq.SmartFactory.Api/Fabiq.SmartFactory.Api
dotnet restore
dotnet ef database update
dotnet run
```

### 7. Run the simulator locally instead of Docker, if preferred

```bash
cd machine-simulator/Fabiq.SmartFactory.Simulator
dotnet restore
dotnet run
```

### 8. Run the frontend locally instead of Docker, if preferred

```bash
cd frontend/fabiq-dashboard
npm install
npm run dev
```

### 9. Optional checks

- Backend build: `dotnet build`
- Frontend build: `npm run build`
- Frontend lint: `npm run lint`

### 10. Repo architecture

- `backend/Fabiq.SmartFactory.Api/Fabiq.SmartFactory.Api` - ASP.NET Core API, EF Core models, Kafka ingestion, and business logic
- `frontend/fabiq-dashboard` - Vite React dashboard for live manufacturing KPIs
- `machine-simulator/Fabiq.SmartFactory.Simulator` - Kafka-driven event generator that seeds and simulates the line
- `database/migrations` - Postgres bootstrap SQL for local infrastructure
- `docs` - API contract, project plan, architecture, and supporting docs
- `infra` - Prometheus and Grafana scaffolding for later observability work
- `mqtt` - Mosquitto broker configuration
- `scripts` - helper scripts for automation or future demo setup

## Notes

- The simulator seeds machines and work orders automatically on startup.
- Kafka topics used by the demo are `machine.status`, `production.counts`, and `downtime.events`.
- The backend listens on port `5078` and the frontend points to that API URL.