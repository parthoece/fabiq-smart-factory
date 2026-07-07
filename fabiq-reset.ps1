$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $Root

Write-Host ""
Write-Host "WARNING: This will stop containers and delete Postgres data." -ForegroundColor Red
$confirm = Read-Host "Type RESET to continue"

if ($confirm -ne "RESET") {
    Write-Host "Reset cancelled." -ForegroundColor Yellow
    Read-Host "Press Enter to close"
    exit 0
}

docker compose down --remove-orphans -v

Write-Host ""
Write-Host "Fabiq stack reset complete. Database volume deleted." -ForegroundColor Green
Read-Host "Press Enter to close"