#!/usr/bin/env sh
set -eu

curl -fsS -X POST http://localhost:5078/api/machines/seed
curl -fsS -X POST http://localhost:5078/api/workorders/seed
