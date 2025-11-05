# Code Coverage with Coverlet

This project uses [Coverlet](https://github.com/coverlet-coverage/coverlet) for code coverage analysis. Coverlet is a cross-platform code coverage framework for .NET, with support for line, branch, and method coverage.

## Quick Start

### Option 1: Using the Helper Scripts

#### Windows (PowerShell)

```powershell
# Run tests with coverage for net8.0
.\run-coverage.ps1

# Run tests with coverage for net48
.\run-coverage.ps1 net48
```

#### Linux/macOS (Bash)

```bash
# Make the script executable (first time only)
chmod +x run-coverage.sh

# Run tests with coverage for net8.0
./run-coverage.sh

# Run tests with coverage for net48
./run-coverage.sh net48
```

### Option 2: Using dotnet CLI

#### With MSBuild Integration

```bash
# Run tests with coverage (uses project configuration)
dotnet test tests/CSharper.Tests/CSharper.Tests.csproj /p:CollectCoverage=true

# Run tests with coverage for a specific framework
dotnet test tests/CSharper.Tests/CSharper.Tests.csproj --framework net8.0 /p:CollectCoverage=true
```

#### With Data Collector

```bash
# Run tests with coverage using the runsettings file
dotnet test tests/CSharper.Tests/CSharper.Tests.csproj \
    --collect:"XPlat Code Coverage" \
    --settings tests/CSharper.Tests/coverlet.runsettings \
    --results-directory tests/CSharper.Tests/coverage
```

### Option 3: Using Visual Studio

1. Open the solution in Visual Studio
2. Go to **Test** > **Analyze Code Coverage for All Tests**
3. Coverage results will be displayed in the Code Coverage Results window

## Coverage Configuration

### Project Configuration

The test project (`tests/CSharper.Tests/CSharper.Tests.csproj`) includes the following Coverlet configuration:

- **CollectCoverage**: `true` - Enables coverage collection
- **CoverletOutputFormat**: `json,cobertura,lcov,opencover` - Generates multiple report formats
- **CoverletOutput**: `./coverage/` - Output directory for coverage reports
- **ExcludeByFile**: Excludes Designer files, obj, and bin directories
- **ExcludeByAttribute**: Excludes Obsolete, GeneratedCode, and CompilerGenerated code
- **Exclude**: Excludes xunit and test assemblies from coverage
- **Include**: Includes only the CSharper assembly

### RunSettings Configuration

The `coverlet.runsettings` file provides additional configuration for the XPlat Code Coverage data collector:

```xml
<Include>[CSharper]*</Include>
<Exclude>[xunit.*]*,[*.Tests]*</Exclude>
<ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute</ExcludeByAttribute>
```

## Output Formats

Coverlet generates coverage reports in multiple formats:

### 1. Cobertura (coverage.cobertura.xml)

- **Use Case**: Azure DevOps, Jenkins, GitLab CI
- **Format**: XML
- **Best For**: CI/CD pipelines that support Cobertura format

### 2. OpenCover (coverage.opencover.xml)

- **Use Case**: ReportGenerator, Visual Studio
- **Format**: XML
- **Best For**: Generating HTML reports

### 3. LCOV (coverage.info)

- **Use Case**: SonarQube, Coveralls, Codecov
- **Format**: Text-based
- **Best For**: Integration with code quality platforms

### 4. JSON (coverage.json)

- **Use Case**: Programmatic access
- **Format**: JSON
- **Best For**: Custom tooling and analysis

## Generating HTML Reports

To generate a human-readable HTML report, use [ReportGenerator](https://github.com/danielpalme/ReportGenerator):

### Install ReportGenerator

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Generate HTML Report

```bash
# From Cobertura format
reportgenerator \
    -reports:tests/CSharper.Tests/coverage/**/coverage.cobertura.xml \
    -targetdir:tests/CSharper.Tests/coverage/report \
    -reporttypes:Html

# From OpenCover format
reportgenerator \
    -reports:tests/CSharper.Tests/coverage/**/coverage.opencover.xml \
    -targetdir:tests/CSharper.Tests/coverage/report \
    -reporttypes:Html
```

### View the Report

Open `tests/CSharper.Tests/coverage/report/index.html` in your browser.

## CI/CD Integration

### GitHub Actions

```yaml
- name: Run tests with coverage
  run: dotnet test tests/CSharper.Tests/CSharper.Tests.csproj --collect:"XPlat Code Coverage" --settings tests/CSharper.Tests/coverlet.runsettings

- name: Upload coverage to Codecov
  uses: codecov/codecov-action@v3
  with:
    files: tests/CSharper.Tests/coverage/**/coverage.cobertura.xml
```

### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: "Run tests with coverage"
  inputs:
    command: "test"
    projects: "tests/CSharper.Tests/CSharper.Tests.csproj"
    arguments: '--collect:"XPlat Code Coverage" --settings tests/CSharper.Tests/coverlet.runsettings'

- task: PublishCodeCoverageResults@1
  inputs:
    codeCoverageTool: "Cobertura"
    summaryFileLocation: "$(Agent.TempDirectory)/**/coverage.cobertura.xml"
```

## Coverage Thresholds

You can enforce minimum coverage thresholds by adding these properties to your project file:

```xml
<PropertyGroup>
  <Threshold>80</Threshold>
  <ThresholdType>line,branch,method</ThresholdType>
  <ThresholdStat>total</ThresholdStat>
</PropertyGroup>
```

This will fail the build if coverage falls below 80%.

## Troubleshooting

### Coverage files not generated

1. Ensure `coverlet.msbuild` and `coverlet.collector` packages are installed
2. Verify that `CollectCoverage` is set to `true` in the project file
3. Check that the output directory has write permissions

### Zero coverage reported

1. Verify that the `Include` filter matches your assembly name
2. Ensure test discovery is working: `dotnet test --list-tests`
3. Check that tests are actually running and passing

### Missing coverage for some files

1. Review the `ExcludeByFile` and `ExcludeByAttribute` filters
2. Ensure the files are part of the included assemblies
3. Check that the code is actually being executed by tests

## Additional Resources

- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/README.md)
- [ReportGenerator Documentation](https://github.com/danielpalme/ReportGenerator/wiki)
- [Code Coverage Best Practices](https://martinfowler.com/bliki/TestCoverage.html)
