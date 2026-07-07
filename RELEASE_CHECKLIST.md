# Release Checklist

Use this checklist before publishing a public release of the **Fabiq Smart Factory MES Integration Platform**.

---

## Release Target

| Field | Value |
|-------|-------|
| Version | `v1.0.0` |
| Release title | `v1.0.0 — Initial Public Release` |
| Release type | Portfolio MVP |
| Branch | `main` |

---

## 1. Repository Structure

Confirm the repository root contains:

- [ ] `README.md`
- [ ] `RUN.md`
- [ ] `CHANGELOG.md`
- [ ] `CONTRIBUTING.md`
- [ ] `SECURITY.md`
- [ ] `CODE_OF_CONDUCT.md`
- [ ] `LICENSE`
- [ ] `.github/`
- [ ] `docs/`
- [ ] `docker-compose.yml`

Confirm accidental or temporary files are removed:

- [ ] `README.md1` removed or backed up
- [ ] `RUN-new.md` merged into `RUN.md`
- [ ] temporary generated files removed
- [ ] local-only files are ignored

---

## 2. Documentation

Verify the following documentation files exist:

- [ ] `docs/architecture.md`
- [ ] `docs/runtime-flow.md`
- [ ] `docs/deployment.md`
- [ ] `docs/api-reference.md`
- [ ] `docs/kafka-topics.md`
- [ ] `docs/troubleshooting.md`
- [ ] `docs/technical-faq.md`
- [ ] `docs/design-decisions.md`
- [ ] `docs/demo-guide.md`
- [ ] `docs/portfolio.md`
- [ ] `docs/roadmap.md`
- [ ] `docs/openapi/openapi.json`
- [ ] `docs/releases/v1.0.0.md`

Verify documentation quality:

- [ ] all internal links work
- [ ] all diagram paths render on GitHub
- [ ] README links point to the correct files
- [ ] terminology is consistent
- [ ] no outdated endpoint names
- [ ] no duplicate temporary file references

---

## 3. GitHub Community Files

Verify:

- [ ] `.github/ISSUE_TEMPLATE/bug_report.md`
- [ ] `.github/ISSUE_TEMPLATE/feature_request.md`
- [ ] `.github/ISSUE_TEMPLATE/documentation.md`
- [ ] `.github/PULL_REQUEST_TEMPLATE.md`
- [ ] `.github/CODEOWNERS`
- [ ] `.github/workflows/ci.yml`

---

## 4. Local Build Verification

Run:

```bash
docker compose config
```

Run the full platform:

```bash
docker compose up -d --build
```

Verify:

- [ ] dashboard opens
- [ ] backend health endpoint responds
- [ ] backend readiness endpoint responds
- [ ] Swagger loads
- [ ] Kafka UI loads
- [ ] Prometheus loads
- [ ] Grafana loads
- [ ] simulator generates events
- [ ] anomaly worker produces alerts

Stop services:

```bash
docker compose down --remove-orphans
```

---

## 5. Git Status

Review changes:

```bash
git status
```

Stage files:

```bash
git add .
```

Commit:

```bash
git commit -m "Prepare v1.0.0 public release"
```

Push:

```bash
git push origin main
```

---

## 6. Create Release Tag

Create an annotated tag:

```bash
git tag -a v1.0.0 -m "Initial public release"
```

Push the tag:

```bash
git push origin v1.0.0
```

---

## 7. GitHub Release

Create a GitHub release from tag:

```text
v1.0.0
```

Release title:

```text
v1.0.0 — Initial Public Release
```

Use content from:

```text
docs/releases/v1.0.0.md
```

---

## 8. GitHub Repository Settings

Recommended description:

```text
Production-inspired Smart Factory MES Integration Platform built with ASP.NET Core, React, Kafka, PostgreSQL, Docker Compose, Prometheus, and Grafana.
```

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

Recommended website field:

```text
https://github.com/parthoece/fabiq-smart-factory
```

---

## 9. Final Smoke Test After Release

After the release is published:

- [ ] repository landing page renders correctly
- [ ] badges render correctly
- [ ] diagrams render correctly
- [ ] release notes display correctly
- [ ] issue templates appear in GitHub
- [ ] pull request template appears in GitHub
- [ ] CI workflow runs successfully
- [ ] repository topics are visible

---

## Release Complete

When all items are checked, the repository is ready to share publicly as:

**Fabiq Smart Factory MES Integration Platform — v1.0.0 Initial Public Release**
