$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $Root

Write-Host ""
Write-Host "Stopping Fabiq Smart Factory..." -ForegroundColor Yellow
Write-Host "Project root: $Root"
Write-Host ""

docker compose down --remove-orphans

Write-Host ""
Write-Host "Fabiq stack stopped." -ForegroundColor Green
Write-Host "Postgres data volume was NOT deleted." -ForegroundColor Cyan
Write-Host ""

Read-Host "Press Enter to close"