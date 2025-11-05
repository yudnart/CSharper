#!/bin/bash
# Bash script to run tests with code coverage
# Usage: ./run-coverage.sh [framework]
# Example: ./run-coverage.sh net8.0

FRAMEWORK=${1:-net8.0}

echo -e "\033[0;32mRunning tests with code coverage for $FRAMEWORK...\033[0m"

# Clean previous coverage results
if [ -d "tests/CSharper.Tests/coverage" ]; then
    rm -rf tests/CSharper.Tests/coverage
    echo -e "\033[0;33mCleaned previous coverage results\033[0m"
fi

# Run tests with coverage
dotnet test tests/CSharper.Tests/CSharper.Tests.csproj \
    --framework $FRAMEWORK \
    --configuration Release \
    --collect:"XPlat Code Coverage" \
    --settings tests/CSharper.Tests/coverlet.runsettings \
    --results-directory tests/CSharper.Tests/coverage

# Display coverage results location
echo -e "\n\033[0;32mCoverage results saved to: tests/CSharper.Tests/coverage\033[0m"
echo -e "\033[0;36mYou can find the following formats:\033[0m"
echo -e "  \033[0;37m- Cobertura (coverage.cobertura.xml) - for CI/CD and Azure DevOps\033[0m"
echo -e "  \033[0;37m- OpenCover (coverage.opencover.xml) - for ReportGenerator\033[0m"
echo -e "  \033[0;37m- LCOV (coverage.info) - for SonarQube and other tools\033[0m"
echo -e "  \033[0;37m- JSON (coverage.json) - for programmatic access\033[0m"

echo -e "\n\033[0;33mTo generate an HTML report, install ReportGenerator:\033[0m"
echo -e "  \033[0;37mdotnet tool install -g dotnet-reportgenerator-globaltool\033[0m"
echo -e "\n\033[0;33mThen run:\033[0m"
echo -e "  \033[0;37mreportgenerator -reports:tests/CSharper.Tests/coverage/**/coverage.cobertura.xml -targetdir:tests/CSharper.Tests/coverage/report -reporttypes:Html\033[0m"

