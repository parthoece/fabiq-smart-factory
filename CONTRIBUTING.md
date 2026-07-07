# Contributing

Thank you for your interest in contributing to **Fabiq Smart Factory MES Integration Platform**.

Fabiq is a production-inspired smart manufacturing project focused on MES, IIoT, event-driven architecture, Kafka, PostgreSQL, React, Docker Compose, and observability.

---

## Ways to Contribute

You can contribute by:

- Reporting bugs
- Improving documentation
- Suggesting new manufacturing features
- Improving local deployment scripts
- Adding tests
- Improving dashboard UX
- Enhancing observability
- Proposing architecture improvements

---

## Development Setup

See:

- [README.md](README.md)
- [RUN.md](RUN.md)
- [Deployment Guide](docs/deployment.md)

Basic startup:

```bash
docker compose up -d --build
```

---

## Branch Naming

Use clear branch names:

```text
feature/add-shift-reporting
fix/backend-health-check
docs/update-kafka-topics
chore/refactor-compose
```

---

## Commit Style

Use concise commit messages:

```text
Add Kafka topic initializer
Fix simulator startup retry
Update API documentation
Improve dashboard layout
```

---

## Pull Request Checklist

Before opening a pull request:

- [ ] Code builds locally
- [ ] Docker Compose configuration is valid
- [ ] Backend builds successfully
- [ ] Frontend builds successfully
- [ ] Documentation is updated if behavior changed
- [ ] Screenshots are updated if UI changed
- [ ] New Kafka topics or APIs are documented

---

## Code Style

General expectations:

- Keep services loosely coupled
- Prefer clear domain naming
- Keep manufacturing concepts explicit
- Avoid hardcoded environment-specific values
- Keep documentation synchronized with implementation

---

## Documentation

Documentation lives in `docs/`.

Update relevant files when changing:

- Architecture
- Runtime behavior
- API routes
- Kafka topics
- Deployment process
- Troubleshooting steps

---

## Issue Reporting

When reporting an issue, include:

- Operating system
- Docker version
- Steps to reproduce
- Expected behavior
- Actual behavior
- Relevant logs
- Screenshots if applicable

---

## Security

Do not open public issues for security vulnerabilities.

See [SECURITY.md](SECURITY.md).

---

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
