# ============================================================
# Ofichinna - Executar testes e gerar relatório de cobertura
# ============================================================

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "   OFICHINA - CODE COVERAGE" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# ------------------------------------------------------------
# 1. Remover relatórios/pastas anteriores
# ------------------------------------------------------------

Write-Host "[1/5] Removendo arquivos de cobertura anteriores..." -ForegroundColor Yellow

$foldersToRemove = @(
    ".\coverage-report",
    ".\coverage-report-badges",
    ".\tests\Ofichina.ArchitectureTests\TestResults",
    ".\tests\Ofichina.IntegrationTests\TestResults",
    ".\tests\Ofichina.UnitTests\TestResults"
)

foreach ($folder in $foldersToRemove) {
    if (Test-Path $folder) {
        Write-Host "  Removendo: $folder" -ForegroundColor DarkGray
        Remove-Item $folder -Recurse -Force
    }
    else {
        Write-Host "  Nao existe: $folder" -ForegroundColor DarkGray
    }
}

Write-Host "Limpeza concluida." -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------
# 2. Executar testes com Code Coverage
# ------------------------------------------------------------

Write-Host "[2/5] Executando testes com Code Coverage..." -ForegroundColor Yellow
Write-Host ""

dotnet test --collect:"XPlat Code Coverage"

if ($LASTEXITCODE -ne 0) {
    throw "Os testes falharam. O processo foi interrompido."
}

Write-Host ""
Write-Host "Testes concluidos com sucesso." -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------
# 3. Gerar relatório HTML
# ------------------------------------------------------------

Write-Host "[3/5] Gerando relatorio HTML de cobertura..." -ForegroundColor Yellow
Write-Host ""

reportgenerator `
    -reports:"**/TestResults/**/coverage.cobertura.xml" `
    -targetdir:"coverage-report" `
    "-reporttypes:Html"

if ($LASTEXITCODE -ne 0) {
    throw "Falha ao gerar o relatorio HTML."
}

Write-Host ""
Write-Host "Relatorio HTML gerado com sucesso." -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------
# 4. Gerar badges
# ------------------------------------------------------------

Write-Host "[4/5] Gerando badges de cobertura..." -ForegroundColor Yellow
Write-Host ""

reportgenerator `
    -reports:"**/TestResults/**/coverage.cobertura.xml" `
    -targetdir:"coverage-report-badges" `
    "-reporttypes:Badges"

if ($LASTEXITCODE -ne 0) {
    throw "Falha ao gerar os badges."
}

Write-Host ""
Write-Host "Badges gerados com sucesso." -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------
# 5. Copiar badges para docs/image/coverage
# ------------------------------------------------------------

Write-Host "[5/5] Copiando badges para docs/image/coverage..." -ForegroundColor Yellow

$badgesSource = ".\coverage-report-badges"

$badgesDestination = "M:\Documentos\FIAP - Tech Challenge\Ofichinna\docs\image\coverage"

$badges = @(
    "badge_branchcoverage.svg",
    "badge_combined.svg",
    "badge_linecoverage.svg"
)

# Verifica se a pasta de destino existe
if (-not (Test-Path $badgesDestination)) {
    Write-Host "  Criando pasta de destino..." -ForegroundColor DarkGray
    New-Item -ItemType Directory -Path $badgesDestination -Force | Out-Null
}

foreach ($badge in $badges) {

    $sourceFile = Join-Path $badgesSource $badge
    $destinationFile = Join-Path $badgesDestination $badge

    if (-not (Test-Path $sourceFile)) {
        throw "Badge não encontrado: $sourceFile"
    }

    Write-Host "  Copiando: $badge" -ForegroundColor DarkGray

    Copy-Item `
        -Path $sourceFile `
        -Destination $destinationFile `
        -Force
}

Write-Host "Badges copiados com sucesso." -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------
# Remover pasta temporária dos badges
# ------------------------------------------------------------

Write-Host "Removendo pasta temporária coverage-report-badges..." -ForegroundColor Yellow

if (Test-Path $badgesSource) {
    Remove-Item $badgesSource -Recurse -Force
}

Write-Host "Pasta coverage-report-badges removida." -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------
# Abrir relatório
# ------------------------------------------------------------

Write-Host "Abrindo relatorio de cobertura..." -ForegroundColor Cyan

Start-Process ".\coverage-report\index.html"

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "   PROCESSO CONCLUIDO COM SUCESSO!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""