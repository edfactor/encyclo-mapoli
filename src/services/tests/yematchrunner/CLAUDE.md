# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

YEMatch is an integration testing framework for comparing READY (legacy) and SMART (new) profit sharing systems during year-end processing. It executes parallel operations against both systems and validates that they produce identical results.

**Confluence Documentation:** https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/272007173/READY+SMART+Year+End+Exact+Match+Testing+YE+Match

## Architecture

### Core Concepts

- **Runs**: End-to-end test scenarios (e.g., `GoldenYearEndRun`, `BaselineRun`, `TerminationsRun`) that orchestrate sequences of activities
- **Activities**: Individual test steps implementing the `IActivity` interface, categorized as:
  - `ReadyActivity`: SSH commands executed against READY system (via Renci.SshNet)
  - `SmartActivity`: API calls to SMART system (via NSwag-generated `ApiClient`)
  - Assert Activities: SQL-based validation queries comparing READY and SMART databases
  - Arrange Activities: Setup and teardown operations
- **Outcomes**: Activity execution results with status (`Ok`, `Error`, `NoOperation`, `ToBeDone`), timing, and output capture
- **Parallel Execution**: Activities can run in parallel using `ParallelActivity` wrapper; outcomes are merged

### Directory Structure

```
YEMatch/
├── Program.cs                    # Entry point with run selection via --run argument
├── Config.cs                     # Data directory creation (/tmp/ye/<timestamp>)
├── Activities/
│   ├── IActivity.cs             # Core activity interface
│   ├── ActivityFactory.cs       # Activity registration and creation
│   └── ParallelActivity.cs      # Wrapper for concurrent execution
├── Runs/
│   ├── Runnable.cs              # Base class for all test runs
│   ├── BaselineRun.cs           # Standard baseline comparison
│   ├── GoldenYearEndRun.cs      # Year-end processing validation
│   ├── AutoMatchRun.cs          # Automated matching scenarios
│   └── [Other run types]
├── ReadyActivities/
│   ├── ReadyActivity.cs         # SSH-based READY command execution
│   └── ReadyActivityFactory.cs  # READY activity creation
├── SmartActivities/
│   ├── SmartActivity.cs         # SMART API call execution
│   ├── SmartActivityFactory.cs  # SMART activity creation with Okta auth
│   ├── ApiClient.cs             # NSwag-generated API client (regenerated via gen.bat)
│   └── [Specific test implementations]
├── AssertActivities/
│   ├── BaseActivity.cs          # Base SQL assertion logic
│   ├── BaseSqlActivity.cs       # Database query execution
│   └── [Comparison validators]
├── ArrangeActivites/            # Setup activities
├── SmartIntegrationTests/       # Integration test implementations
└── Resources/                   # Test data files (e.g., MTPR.OUTFL)
```

## Configuration

### User Secrets (Required)

Run from the project directory (`src/services/tests/yematchrunner`):

```bash
# READY SSH credentials
dotnet user-secrets set "YEMatchHost:Username" "your_ready_username"
dotnet user-secrets set "YEMatchHost:Password" "your_ready_password"

# Database connection strings
dotnet user-secrets set SmartConnectionString "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=your_host)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=your_service)(SERVER=DEDICATED)));User Id=smart_user;Password=smart_pass"

dotnet user-secrets set ReadyConnectionString "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=your_host)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=your_service)(SERVER=DEDICATED)));User Id=ready_user;Password=ready_pass"
```

### Optional Configuration

Create `appsettings.json` to override default log directory:

```json
{
  "BaseDataDirectory": "/custom/path/for/logs"
}
```

Default: `/tmp/ye/<dd-MMM-HH-mm>`

## Running Tests

### Prerequisites

1. Start the local SMART API with test credentials using the "API YE Match (Test Certs)" launch configuration
2. Ensure READY environment is accessible via SSH with `setyematch` script configured

### Execute a Run

```bash
# From project directory
dotnet run -- --run <run_type>

# Available run types (see Program.cs GetRunner method):
dotnet run -- --run baseline           # Standard baseline comparison
dotnet run -- --run goldenyearend      # Year-end processing
dotnet run -- --run goldenexpress      # Express processing
dotnet run -- --run goldendecemberexpress
dotnet run -- --run masterinquiry      # Master inquiry validation
dotnet run -- --run tinker             # Development/debugging run
dotnet run -- --run terminations       # Termination processing
dotnet run -- --run seven              # Specific test scenario
dotnet run -- --run view               # View-only operations
dotnet run -- --run automatch          # Automated matching
```

### Output

- Execution logs are written to the data directory (displayed at startup as `file:///...`)
- READY command output files are captured in this directory
- Console output shows activity names, commands, timing, and outcome status

## Development Workflow

### Building and Testing

```bash
# Build
dotnet build YEMatch.csproj

# Run with specific scenario
dotnet run -- --run tinker

# Debug in IDE
# Set launch configuration with command-line args: --run tinker
```

### Adding a New Run

1. Create a class in `YEMatch/Runs/` extending `Runnable`
2. Override `Exec()` method and build activity list
3. Call `await Run(activitiesToRun)`
4. Register in `Program.cs` `GetRunner` switch statement

Example:
```csharp
public class MyNewRun : Runnable
{
    public override async Task Exec()
    {
        List<IActivity> activities = [
            Factory.ReadyActivity("A01", "SCRIPT_NAME", "args"),
            Factory.SmartActivity("A02", SmartActivityFactory.SomeOperation),
            Factory.AssertActivity("A03", "ValidationQuery")
        ];
        await Run(activities);
    }
}
```

### Adding a New Activity

**READY Activity:**
```csharp
// In ReadyActivityFactory
public static ReadyActivity NewOperation(string args) =>
    new ReadyActivity(client, sftpClient, chatty, "A##", "KSH_SCRIPT", args, dataDirectory);
```

**SMART Activity:**
```csharp
// In SmartActivityFactory
public static SmartActivity NewOperation(string args) =>
    new SmartActivity(async (client, name, command) =>
    {
        var result = await client.SomeEndpointAsync(command, null);
        return new Outcome(name, "Operation Name", command, OutcomeStatus.Ok, "", null, true);
    }, Client!, "A##", args);
```

**Assert Activity:**
```csharp
// In TestActivityFactory
public static BaseSqlActivity ValidateData(string args) =>
    new BaseSqlActivity(dataDirectory, "A##", "SELECT ... FROM ... WHERE ...");
```

### Regenerating API Client

When SMART API changes:

```bash
# Ensure SMART API is running at https://localhost:7141
gen.bat  # Windows
# Or manually:
nswag openapi2csclient /classname:ApiClient /namespace:YEMatch /input:https://localhost:7141/swagger/Release%201.0/swagger.json /output:YEMatch\SmartActivities\ApiClient.cs
```

## Key Patterns

### Activity Naming Convention

- Activities are numbered sequentially (A01, A02, ...)
- READY activities display as R01, R02 (first letter replaced)
- SMART activities display as S01, S02
- Assertion activities display as T01, T02 (Test)

### Parallel Execution

```csharp
// Execute two activities concurrently
List<IActivity> activities = [
    new ParallelActivity(
        Factory.ReadyActivity("A01", "SCRIPT1", ""),
        Factory.SmartActivity("A01", SmartActivityFactory.Operation1)
    )
];
```

### READY SSH Command Pattern

ReadyActivity translates production paths to development paths:
```bash
. ~/setyematch;
sed -e's|/production/|/dsmdev/data/PAYROLL/tmp-yematch/|g' jcl/{ksh}.ksh > jcl/YE-{ksh}.ksh;
chmod +x jcl/YE-{ksh}.ksh;
EJR YE-{ksh} {args}
```

### Error Handling

- Activities return `Outcome` with `OutcomeStatus.Error` on failure
- `Runnable.CompletedWithoutError` tracks overall run success
- Exceptions are caught and wrapped in error outcomes
- Runs continue after errors unless explicitly stopped

## Common Tasks

### Debug a Specific Activity

Use `TinkerRun.cs` as a scratch pad:
```csharp
public override async Task Exec()
{
    List<IActivity> activities = [
        Factory.ReadyActivity("A01", "MY_SCRIPT", "args_to_test")
    ];
    await Run(activities);
}
```

### Compare Database Results

1. Add SQL comparison in `AssertActivities/`
2. Use `BaseSqlActivity` with dual database queries
3. `Shouldly` assertions validate equivalence

### Capture READY Output Files

ReadyActivity automatically downloads files matching patterns from `/dsmdev/data/PAYROLL/tmp-yematch/` to local data directory.

## Technology Stack

- **.NET 9** with C# 12
- **Renci.SshNet** for READY SSH/SFTP access
- **NSwag** for SMART API client generation
- **Okta.AspNetCore** for SMART API authentication
- **Oracle.ManagedDataAccess.Core** for database validation
- **Shouldly** for assertions
- **Microsoft.Extensions.Configuration** for secrets management

## Important Notes

- All database credentials must be in user secrets (never commit)
- READY system requires `setyematch` script configured on target host
- Log directories can grow large during year-end runs; clean `/tmp/ye` periodically
- API client is generated code; do not hand-edit
- Parallel activities merge outcomes; the longest duration is reported
- READY activities include hardcoded path: `/Users/robertherrmann/prj/smart-profit-sharing/src/services/tests/Demoulas.ProfitSharing.IntegrationTests/Resources/` for optional local resource base (may need adjustment per developer)

## Troubleshooting

**"No SMART authentication"**: Ensure local SMART API is running with "API YE Match (Test Certs)" configuration

**SSH connection failures**: Verify READY credentials in user secrets and network access to READY host

**Oracle connection errors**: Confirm connection strings in user secrets match target environments

**API client generation fails**: Ensure SMART API `/swagger/Release%201.0/swagger.json` endpoint is accessible

## Related Projects

This testing framework is part of the larger Demoulas Profit Sharing monorepo:
- **SMART application**: `/Users/robertherrmann/prj/smart-profit-sharing/` (main .NET 9 API)
- **Parent repository**: `/Users/robertherrmann/prj/yerunner/` (contains broader test infrastructure)
