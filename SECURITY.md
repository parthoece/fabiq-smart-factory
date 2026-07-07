# Security Policy

## Supported Versions

The project is currently maintained as a portfolio and learning platform.

| Version | Supported |
|---------|-----------|
| 1.x | Yes |
| < 1.0 | No |

---

## Reporting a Vulnerability

If you discover a potential security vulnerability, please **do not** create a public GitHub issue.

Instead:

1. Contact the project maintainer privately.
2. Provide a clear description of the issue.
3. Include reproduction steps, affected components, and any relevant logs.
4. Allow reasonable time to investigate before public disclosure.

---

## Scope

Security reports may include issues related to:

- Authentication and authorization
- API security
- Docker configuration
- Dependency vulnerabilities
- Secrets management
- Kafka or MQTT configuration
- Database access
- Cross-site scripting (XSS)
- Injection vulnerabilities

---

## Response Process

Reported vulnerabilities will be:

1. Acknowledged as soon as practical.
2. Investigated and reproduced.
3. Assessed for impact and severity.
4. Fixed and documented.
5. Included in a future release or patch.

---

## Security Best Practices

Production deployments should consider:

- HTTPS/TLS
- OAuth2 / OpenID Connect
- Role-Based Access Control (RBAC)
- Secret management
- Kafka ACLs
- Least-privilege database access
- Network segmentation
- Image and dependency scanning
- Regular dependency updates
- Centralized logging and monitoring

---

## Third-Party Dependencies

Keep dependencies current by:

- Updating NuGet packages
- Updating npm packages
- Rebuilding Docker images regularly
- Reviewing dependency advisories

---

## Disclosure Policy

Security fixes will be documented in the project changelog after a fix has been released.

---

## Contact

For responsible disclosure, contact the repository maintainer through the contact information provided on the repository profile.

Thank you for helping improve the security of the project.
