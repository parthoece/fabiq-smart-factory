$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $Root

Write-Host ""
Write-Host "Starting Fabiq Smart Factory..." -ForegroundColor Cyan
Write-Host "Project root: $Root"
Write-Host ""

docker compose down --remove-orphans
docker compose up -d --build

Write-Host ""
Write-Host "Current container status:" -ForegroundColor Cyan
docker compose ps -a

Write-Host ""
Write-Host "Useful URLs:" -ForegroundColor Green
Write-Host "Frontend:    http://localhost:3000"
Write-Host "Backend:     http://localhost:5078/swagger"
Write-Host "Health:      http://localhost:5078/health"
Write-Host "Kafka UI:    http://localhost:8081"
Write-Host "Prometheus:  http://localhost:9090"
Write-Host "Grafana:     http://localhost:3001"
Write-Host ""

Write-Host "Fabiq stack started." -ForegroundColor Green
Read-Host "Press Enter to close"