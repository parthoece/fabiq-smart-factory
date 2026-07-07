# API Reference

## Overview

The Fabiq Smart Factory MES Integration Platform exposes REST APIs that provide access to manufacturing data, production history, operational KPIs, and platform services.

The API is organized by manufacturing domains rather than database entities, making it easier to integrate dashboards, simulators, and future MES components.

---

# API Design Principles

- RESTful resource-oriented endpoints
- JSON request and response payloads
- Domain-driven endpoint grouping
- Stateless request processing
- OpenAPI/Swagger documentation
- Health and observability endpoints

---

# Base URL

Local development:

```text
http://localhost:5078
```

Swagger UI:

```text
http://localhost:5078/swagger
```

OpenAPI specification:

```text
docs/openapi/openapi.json
```

---

# Endpoint Groups

## Machine APIs

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Machines/status` | Current machine status |
| POST | `/api/Machines/seed` | Seed demo machines |

---

## Work Order APIs

| Method | Endpoint |
|--------|----------|
| GET | `/api/WorkOrders` |
| GET | `/api/WorkOrders/active` |
| GET | `/api/WorkOrders/{workOrderId}` |
| POST | `/api/WorkOrders/seed` |

---

## Production Event APIs

| Method | Endpoint |
|--------|----------|
| GET | `/api/ProductionEvents/recent` |
| POST | `/api/ProductionEvents` |
| POST | `/api/ProductionEvents/demo-good` |
| POST | `/api/ProductionEvents/demo-scrap` |

---

## Downtime APIs

| Method | Endpoint |
|--------|----------|
| GET | `/api/Downtime/recent` |
| GET | `/api/Downtime/summary` |
| POST | `/api/Downtime` |
| POST | `/api/Downtime/demo` |

---

## OEE APIs

| Method | Endpoint |
|--------|----------|
| GET | `/api/Oee/line/{lineId}` |
| GET | `/api/Oee/work-order/{workOrderId}` |

---

## Traceability APIs

| Method | Endpoint |
|--------|----------|
| GET | `/api/Traceability/part/{partId}` |
| GET | `/api/Traceability/work-order/{workOrderId}` |

---

## Maintenance Alert APIs

The backend persists alerts produced by the AI anomaly worker and exposes them for dashboard consumption.

---

## Platform APIs

| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/health` | Liveness |
| GET | `/ready` | Readiness |
| GET | `/metrics` | Prometheus metrics |

---

# Example Request

```http
GET /api/Machines/status
Accept: application/json
```

# Example Response

```json
[
  {
    "machineId": "SMT-01",
    "status": "Running",
    "workOrderId": "WO-1001"
  }
]
```

---

# Error Handling

Typical HTTP status codes:

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Resource created |
| 400 | Invalid request |
| 404 | Resource not found |
| 500 | Internal server error |

---

# Authentication

The current portfolio version is intended for local development and does not require authentication.

A production deployment should add:

- OAuth2 / OpenID Connect
- JWT bearer authentication
- Role-Based Access Control (RBAC)

---

# Versioning

Current version: **v1**

Future versions should use explicit API versioning to support backward compatibility.

---

# Related Documentation

- architecture.md
- runtime-flow.md
- deployment.md
- kafka-topics.md
- technical-faq.md
