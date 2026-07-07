# GitHub Repository Setup Guide

Use this guide after pushing the repository-quality changes to GitHub.

---

## Repository Description

Set the repository description to:

```text
Production-inspired Smart Factory MES Integration Platform built with ASP.NET Core, React, Kafka, PostgreSQL, Docker Compose, Prometheus, and Grafana.
```

---

## Repository Topics

Recommended topics:

```text
aspnetcore
react
docker
kafka
postgresql
manufacturing
mes
iiot
smart-factory
industrial-iot
event-driven
oee
traceability
mqtt
prometheus
grafana
```

---

## Website Field

Recommended value:

```text
https://github.com/parthoece/fabiq-smart-factory
```

If a live demo is deployed later, replace this with the deployed dashboard or documentation URL.

---

## About Section Checklist

Enable or confirm:

- [ ] Description is set
- [ ] Topics are added
- [ ] Website field is set
- [ ] Releases are enabled
- [ ] Packages are enabled only if needed
- [ ] Issues are enabled
- [ ] Discussions are optional
- [ ] Wiki is optional and can remain disabled because documentation lives in `docs/`

---

## Branch Protection

Recommended branch protection for `main`:

- [ ] Require a pull request before merging
- [ ] Require status checks to pass before merging
- [ ] Require the `CI` workflow to pass
- [ ] Require branches to be up to date before merging
- [ ] Require conversation resolution before merging
- [ ] Do not allow force pushes
- [ ] Do not allow deletions

For a solo portfolio project, branch protection can be lighter:

- Require status checks
- Block force pushes
- Block branch deletion

---

## GitHub Actions

Workflow file should be located at:

```text
.github/workflows/ci.yml
```

Confirm the Actions tab shows:

```text
CI
```

Expected jobs:

- Backend build
- Frontend build
- Compose and documentation checks

---

## Community Files

Confirm GitHub detects these files:

```text
CONTRIBUTING.md
SECURITY.md
CODE_OF_CONDUCT.md
.github/ISSUE_TEMPLATE/bug_report.md
.github/ISSUE_TEMPLATE/feature_request.md
.github/ISSUE_TEMPLATE/documentation.md
.github/PULL_REQUEST_TEMPLATE.md
.github/CODEOWNERS
```

Check this from the repository homepage or the Community Standards page.

---

## Social Preview

Recommended social preview image:

```text
docs/assets/social-preview.png
```

Suggested content:

```text
Fabiq Smart Factory MES Integration Platform
ASP.NET Core • React • Kafka • PostgreSQL • Docker • Prometheus • Grafana
```

Recommended size:

```text
1280 x 640
```

---

## First Release

Create an annotated tag locally:

```bash
git tag -a v1.0.0 -m "Initial public release"
git push origin v1.0.0
```

Create a GitHub release from:

```text
v1.0.0
```

Release title:

```text
v1.0.0 — Initial Public Release
```

Use release notes from:

```text
docs/releases/v1.0.0.md
```

---

## Recommended Release Labels

Create these labels if you want a cleaner issue tracker:

```text
bug
documentation
enhancement
good first issue
help wanted
infra
backend
frontend
kafka
observability
manufacturing
security
```

---

## Repository Sharing Text

Short description for LinkedIn or portfolio:

```text
I built Fabiq, a production-inspired Smart Factory MES Integration Platform using ASP.NET Core, React, Apache Kafka, PostgreSQL, Docker Compose, Prometheus, and Grafana. The platform simulates a PCB production line, streams manufacturing events through Kafka, calculates OEE, stores traceability data, and demonstrates AI-driven anomaly detection with maintenance alerts.
```

Short technical summary:

```text
Fabiq demonstrates event-driven MES architecture, manufacturing traceability, Kafka-based integration, AI anomaly detection, observability, and full-stack smart factory dashboarding in a reproducible Docker Compose environment.
```

---

## Final GitHub Smoke Test

After pushing:

- [ ] README renders correctly
- [ ] badges render correctly
- [ ] diagrams render correctly
- [ ] docs links work
- [ ] issue templates appear
- [ ] pull request template appears
- [ ] CI workflow runs successfully
- [ ] release notes are visible
- [ ] repository topics are visible
- [ ] no accidental temporary files are present
