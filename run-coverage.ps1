# PowerShell script to run tests with code coverage
# Usage: .\run-coverage.ps1 [framework]
# Example: .\run-coverage.ps1 net8.0

param(
    [string]$framework = "net8.0"
)

Write-Host "Running tests with code coverage for $framework..." -ForegroundColor Green

# Clean previous coverage results
if (Test-Path "tests/CSharper.Tests/coverage") {
    Remove-Item -Path "tests/CSharper.Tests/coverage" -Recurse -Force
    Write-Host "Cleaned previous coverage results" -ForegroundColor Yellow
}

# Run tests with coverage
dotnet test tests/CSharper.Tests/CSharper.Tests.csproj `
    --framework $framework `
    --configuration Release `
    --collect:"XPlat Code Coverage" `
    --settings tests/CSharper.Tests/coverlet.runsettings `
    --results-directory tests/CSharper.Tests/coverage

# Display coverage results location
Write-Host "`nCoverage results saved to: tests/CSharper.Tests/coverage" -ForegroundColor Green
Write-Host "You can find the following formats:" -ForegroundColor Cyan
Write-Host "  - Cobertura (coverage.cobertura.xml) - for CI/CD and Azure DevOps" -ForegroundColor White
Write-Host "  - OpenCover (coverage.opencover.xml) - for ReportGenerator" -ForegroundColor White
Write-Host "  - LCOV (coverage.info) - for SonarQube and other tools" -ForegroundColor White
Write-Host "  - JSON (coverage.json) - for programmatic access" -ForegroundColor White

Write-Host "`nTo generate an HTML report, install ReportGenerator:" -ForegroundColor Yellow
Write-Host "  dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor White
Write-Host "`nThen run:" -ForegroundColor Yellow
Write-Host "  reportgenerator -reports:tests/CSharper.Tests/coverage/**/coverage.cobertura.xml -targetdir:tests/CSharper.Tests/coverage/report -reporttypes:Html" -ForegroundColor White

