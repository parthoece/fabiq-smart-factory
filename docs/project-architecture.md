# Project Architecture

## High-Level Flow

```mermaid
flowchart LR
    A[Simulator] -->|machine.status| K[Kafka]
    A -->|production.counts| K
    A -->|downtime.events| K
    K --> B[Backend API]
    B --> P[(Postgres)]
    B --> F[Frontend Dashboard]
    F --> B
```

## Repository Layout

- `backend/Fabiq.SmartFactory.Api/Fabiq.SmartFactory.Api` - API host, controllers, data access, EF Core models, Kafka consumer, and domain services
- `frontend/fabiq-dashboard` - React + Vite dashboard
- `machine-simulator/Fabiq.SmartFactory.Simulator` - local event generator and Kafka producer
- `database/migrations` - database bootstrap SQL used by Docker Compose
- `docs` - architecture, OpenAPI, and the project plan
- `infra` - future observability assets
- `mqtt` - Mosquitto configuration
- `scripts` - utility scripts and future demo helpers

## Runtime Components

- Postgres stores machines, work orders, production events, and downtime events.
- Kafka carries simulated manufacturing events between producer and consumer.
- The backend validates and persists incoming events, then serves dashboard APIs.
- The frontend polls the backend for live dashboard data.
- The simulator seeds initial data and drives the production line continuously.

## Startup Order

1. Start Docker services: Postgres, Kafka, Kafka UI, MQTT.
2. Start the backend API.
3. Start the simulator.
4. Start the frontend.

The Docker Compose file now handles this full path for the local demo.