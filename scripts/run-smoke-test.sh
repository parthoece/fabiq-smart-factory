#!/usr/bin/env sh
set -eu

curl -fsS http://localhost:5078/health
curl -fsS http://localhost:5078/ready
curl -fsS http://localhost:5078/api/platform/version
curl -fsS http://localhost:5078/metrics
