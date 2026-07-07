# Deployment Guide

## Overview

This guide explains how to deploy and run the Fabiq Smart Factory MES
Integration Platform in a local development environment.

The platform is designed to run entirely with Docker Compose and
includes infrastructure, backend services, frontend, observability, and
a machine simulator.

------------------------------------------------------------------------

# Prerequisites

Install the following software before starting:

  Software         Version
  ---------------- -----------------------------------
  Docker Desktop   Latest
  Docker Compose   Included with Docker Desktop
  .NET SDK         8.0+
  Node.js          22+
  npm              Latest
  PowerShell       Optional (Windows helper scripts)

------------------------------------------------------------------------

# Clone the Repository

``` bash
git clone https://github.com/parthoece/fabiq-smart-factory.git
cd fabiq-smart-factory
```

------------------------------------------------------------------------

# Start the Platform

Build and start every service:

``` bash
docker compose up -d --build
```

Docker Compose coordinates startup using health checks and dependency
conditions.

------------------------------------------------------------------------

# Services

  Service           Default URL
  ----------------- -------------------------------
  React Dashboard   http://localhost:3000
  Backend API       http://localhost:5078
  Swagger           http://localhost:5078/swagger
  Health            http://localhost:5078/health
  Kafka UI          http://localhost:8081
  Prometheus        http://localhost:9090
  Grafana           http://localhost:3001

------------------------------------------------------------------------

# Startup Order

``` text
PostgreSQL
 ↓
Database Migrator
 ↓
Kafka
 ↓
Kafka Topic Initializer
 ↓
Backend API
 ↓
AI Worker
 ↓
Machine Simulator
 ↓
React Frontend
```

------------------------------------------------------------------------

# Verify the Deployment

Check container status:

``` bash
docker compose ps
```

Review logs:

``` bash
docker logs fabiq-backend --tail=100
docker logs fabiq-simulator --tail=100
```

Verify the health endpoint:

``` bash
curl http://localhost:5078/health
```

------------------------------------------------------------------------

# PowerShell Helper Scripts

For Windows users:

``` powershell
.\fabiq-on.ps1
.\fabiq-off.ps1
.\fabiq-reset.ps1
```

-   **fabiq-on.ps1** --- Starts the full platform
-   **fabiq-off.ps1** --- Stops the platform
-   **fabiq-reset.ps1** --- Removes containers and the PostgreSQL volume

------------------------------------------------------------------------

# Reset the Environment

Stop services:

``` bash
docker compose down --remove-orphans
```

Remove persistent data:

``` bash
docker compose down --remove-orphans -v
```

Rebuild everything:

``` bash
docker compose up -d --build
```

------------------------------------------------------------------------

# Environment Configuration

Configuration is provided through Docker Compose and environment
variables.

Typical settings include:

-   PostgreSQL connection string
-   Kafka bootstrap servers
-   MQTT broker endpoint
-   ASP.NET Core environment
-   Prometheus scrape configuration

------------------------------------------------------------------------

# Deployment Notes

-   Kafka topics are created automatically during startup.
-   The simulator waits for backend readiness before publishing events.
-   Health checks reduce startup race conditions.
-   Prometheus and Grafana are preconfigured for local monitoring.

------------------------------------------------------------------------

# Troubleshooting Checklist

-   Docker Desktop is running.
-   Required ports are available.
-   All containers report healthy.
-   Backend `/health` endpoint responds successfully.
-   Kafka UI displays the expected topics.
-   Grafana connects to the provisioned Prometheus datasource.

For detailed troubleshooting, see `troubleshooting.md`.

------------------------------------------------------------------------

# Related Documentation

-   `architecture.md`
-   `runtime-flow.md`
-   `api-reference.md`
-   `kafka-topics.md`
-   `technical-faq.md`
