# Changelog

All notable changes to this project will be documented in this file.

The format is based on **Keep a Changelog** and follows **Semantic Versioning (SemVer)**.

---

## [Unreleased]

### Planned

#### Platform

- Authentication & Role-Based Access Control (RBAC)
- Centralized configuration management
- Kubernetes deployment support
- Cloud deployment profiles

#### Manufacturing

- Shift reporting
- MTBF / MTTR calculations
- Quality inspection workflows
- Expanded OEE analytics

#### Integration

- OPC UA connectivity
- Enhanced MQTT ingestion
- Kafka Schema Registry
- Event versioning

#### AI & Analytics

- Predictive maintenance models
- Statistical anomaly detection
- Historical trend analysis
- Digital Twin integration

---

## [1.0.0] - 2026-07-07

### Added

#### Smart Manufacturing Platform

- Initial release of the Fabiq Smart Factory MES Integration Platform
- Production-inspired Manufacturing Execution System (MES) architecture
- Simulated PCB/electronics production line
- Event-driven backend architecture

#### Backend

- ASP.NET Core REST API
- Entity Framework Core persistence
- Manufacturing domain services
- Health and readiness endpoints
- Swagger / OpenAPI documentation

#### Manufacturing

- Machine status tracking
- Work order management
- Production event processing
- Downtime tracking
- Overall Equipment Effectiveness (OEE)
- Part traceability

#### Messaging

- Apache Kafka integration
- Automatic Kafka topic initialization
- Manufacturing event streaming
- MQTT broker integration

#### AI

- Independent anomaly detection worker
- Telemetry processing
- Maintenance alert generation

#### Frontend

- React dashboard
- Live machine monitoring
- Production visualization
- OEE dashboard
- Traceability views

#### Infrastructure

- Docker Compose deployment
- Health-aware startup sequencing
- Database migration service
- PowerShell helper scripts

#### Observability

- Prometheus metrics
- Grafana dashboards
- Backend metrics endpoint

#### Documentation

- Comprehensive project documentation
- Architecture guide
- Runtime flow documentation
- Deployment guide
- API reference
- Kafka topic reference
- Technical FAQ
- Design decisions
- Troubleshooting guide
- Demo guide
- Portfolio guide
- Project roadmap

---

## Release Notes

Version **1.0.0** represents the first public portfolio release of the Fabiq Smart Factory MES Integration Platform.

The project demonstrates modern software engineering practices commonly used in Manufacturing Execution Systems (MES), Industrial IoT (IIoT), and event-driven backend platforms.

Major capabilities include:

- Event-driven architecture
- Apache Kafka messaging
- ASP.NET Core backend
- PostgreSQL persistence
- React dashboard
- Machine simulator
- AI anomaly detection
- Prometheus & Grafana observability
- Docker Compose orchestration
- Comprehensive technical documentation

---

## Versioning Strategy

Future releases will follow Semantic Versioning.

| Version | Meaning |
|----------|---------|
| MAJOR | Breaking architectural or API changes |
| MINOR | New features with backward compatibility |
| PATCH | Bug fixes and documentation improvements |

Examples:

- **1.0.0** – Initial public release
- **1.1.0** – Additional manufacturing features
- **1.2.0** – Platform enhancements
- **2.0.0** – Major architectural evolution

---

## References

- README.md
- ROADMAP.md
- LICENSE