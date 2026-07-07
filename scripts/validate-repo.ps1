# Fabiq Repository Validation Script
# Recommended location: scripts/validate-repo.ps1
# Run from the repository root:
#   .\scripts\validate-repo.ps1

$ErrorActionPreference = "Continue"

Write-Host ""
Write-Host "Fabiq Repository Validation" -ForegroundColor Cyan
Write-Host "===========================" -ForegroundColor Cyan
Write-Host ""

$failures = 0

function Test-RequiredPath {
    param (
        [string]$Path,
        [string]$Type = "Any"
    )

    $exists = $false

    if ($Type -eq "File") {
        $exists = Test-Path $Path -PathType Leaf
    }
    elseif ($Type -eq "Directory") {
        $exists = Test-Path $Path -PathType Container
    }
    else {
        $exists = Test-Path $Path
    }

    if ($exists) {
        Write-Host "OK   $Path" -ForegroundColor Green
    }
    else {
        Write-Host "MISS $Path" -ForegroundColor Red
        $script:failures++
    }
}

function Test-OptionalCommand {
    param (
        [string]$CommandName,
        [string]$DisplayName
    )

    if (Get-Command $CommandName -ErrorAction SilentlyContinue) {
        Write-Host "OK   $DisplayName is available" -ForegroundColor Green
    }
    else {
        Write-Host "WARN $DisplayName is not available in PATH" -ForegroundColor Yellow
    }
}

# Confirm repository root
Write-Host "Checking repository root..." -ForegroundColor Cyan

Test-RequiredPath "README.md" "File"
Test-RequiredPath "docker-compose.yml" "File"

Write-Host ""

# Root files
Write-Host "Checking root-level repository files..." -ForegroundColor Cyan

$rootFiles = @(
    ".editorconfig",
    ".gitattributes",
    ".gitignore",
    ".markdownlint.json",
    "README.md",
    "RUN.md",
    "CHANGELOG.md",
    "CONTRIBUTING.md",
    "SECURITY.md",
    "CODE_OF_CONDUCT.md",
    "RELEASE_CHECKLIST.md",
    "LICENSE",
    "docker-compose.yml",
    ".env.example"
)

foreach ($file in $rootFiles) {
    Test-RequiredPath $file "File"
}

Write-Host ""

# Main directories
Write-Host "Checking main project directories..." -ForegroundColor Cyan

$directories = @(
    ".github",
    "backend",
    "frontend",
    "machine-simulator",
    "ai-anomaly-service",
    "database",
    "docs",
    "infra",
    "mqtt",
    "scripts"
)

foreach ($dir in $directories) {
    Test-RequiredPath $dir "Directory"
}

Write-Host ""

# GitHub files
Write-Host "Checking GitHub community and workflow files..." -ForegroundColor Cyan

$githubFiles = @(
    ".github\CODEOWNERS",
    ".github\PULL_REQUEST_TEMPLATE.md",
    ".github\ISSUE_TEMPLATE\bug_report.md",
    ".github\ISSUE_TEMPLATE\feature_request.md",
    ".github\ISSUE_TEMPLATE\documentation.md",
    ".github\workflows\ci.yml"
)

foreach ($file in $githubFiles) {
    Test-RequiredPath $file "File"
}

Write-Host ""

# Documentation
Write-Host "Checking documentation files..." -ForegroundColor Cyan

$docsFiles = @(
    "docs\README.md",
    "docs\architecture.md",
    "docs\runtime-flow.md",
    "docs\deployment.md",
    "docs\api-reference.md",
    "docs\kafka-topics.md",
    "docs\troubleshooting.md",
    "docs\technical-faq.md",
    "docs\design-decisions.md",
    "docs\demo-guide.md",
    "docs\portfolio.md",
    "docs\roadmap.md",
    "docs\GITHUB_REPOSITORY_SETUP.md",
    "docs\openapi\openapi.json",
    "docs\releases\v1.0.0.md"
)

foreach ($file in $docsFiles) {
    Test-RequiredPath $file "File"
}

Write-Host ""

# Diagrams
Write-Host "Checking diagram files..." -ForegroundColor Cyan

$diagramFiles = @(
    "docs\diagrams\system-overview.svg",
    "docs\diagrams\runtime-data-flow.svg",
    "docs\diagrams\mes-feature-map.svg"
)

foreach ($file in $diagramFiles) {
    Test-RequiredPath $file "File"
}

Write-Host ""

# Temporary / accidental files
Write-Host "Checking for common accidental files..." -ForegroundColor Cyan

$temporaryFiles = @(
    "README.md1",
    "RUN-new.md",
    "ci-updated.yml",
    "github"
)

foreach ($file in $temporaryFiles) {
    if (Test-Path $file) {
        Write-Host "WARN Found temporary or incorrectly placed item: $file" -ForegroundColor Yellow
    }
    else {
        Write-Host "OK   $file not present" -ForegroundColor Green
    }
}

Write-Host ""

# Tool availability
Write-Host "Checking useful local tools..." -ForegroundColor Cyan

Test-OptionalCommand "git" "Git"
Test-OptionalCommand "docker" "Docker"
Test-OptionalCommand "dotnet" ".NET SDK"
Test-OptionalCommand "node" "Node.js"
Test-OptionalCommand "npm" "npm"

Write-Host ""

# Docker Compose validation
Write-Host "Validating Docker Compose configuration..." -ForegroundColor Cyan

if (Get-Command docker -ErrorAction SilentlyContinue) {
    docker compose config *> $null

    if ($LASTEXITCODE -eq 0) {
        Write-Host "OK   docker compose config passed" -ForegroundColor Green
    }
    else {
        Write-Host "MISS docker compose config failed" -ForegroundColor Red
        $failures++
    }
}
else {
    Write-Host "WARN Docker not available; skipped docker compose config" -ForegroundColor Yellow
}

Write-Host ""

# Git status
Write-Host "Git status summary..." -ForegroundColor Cyan

if (Get-Command git -ErrorAction SilentlyContinue) {
    git status --short
}
else {
    Write-Host "WARN Git not available; skipped git status" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Validation complete." -ForegroundColor Cyan

if ($failures -gt 0) {
    Write-Host "$failures required checks failed." -ForegroundColor Red
    exit 1
}
else {
    Write-Host "All required checks passed." -ForegroundColor Green
    exit 0
}
