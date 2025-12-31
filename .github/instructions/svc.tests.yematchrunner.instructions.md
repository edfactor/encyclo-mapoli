---
applyTo: "src/services/tests/yematchrunner/**/*.*"
paths: "src/services/tests/yematchrunner/**/*.*"
---

## Project Overview

YEMatch is an integration testing framework for comparing READY (legacy) and SMART (new) profit sharing systems during
year-end processing. It executes parallel operations against both systems and validates that they produce identical
results.

**Confluence Documentation:
** https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/272007173/READY+SMART+Year+End+Exact+Match+Testing+YE+Match

## Architecture

### Core Concepts

- **Runs**: End-to-end test scenarios (e.g., `GoldenYearEndRun`, `BaselineRun`, `TerminationsRun`) that orchestrate
  sequences of activities
- **Activities**: Individual test steps implementing the `IActivity` interface, categorized as:
  - `ReadyActivity`: SSH commands executed against READY system (via Renci.SshNet)
  - `SmartActivity`: API calls to SMART system (via NSwag-generated `ApiClient`)
  - Assert Activities: SQL-based validation queries comparing READY and SMART databases
  - Arrange Activities: Setup and teardown operations
- **Outcomes**: Activity execution results with status (`Ok`, `Error`, `NoOperation`, `ToBeDone`), timing, and output
  capture
- **Parallel Execution**: Activities can run in parallel using `ParallelActivity` wrapper; outcomes are merged

### Dependency Injection Architecture

YEMatch uses **.NET Dependency Injection** with the following lifetime scopes:

- **Singleton Factories**: `IReadySshClientFactory`, `ISmartApiClientFactory`, `IActivityFactory`

  - Maintain single SSH/HTTP connections throughout application lifetime
  - Create and register all available activities
  - Implement `IDisposable` for proper cleanup

- **Transient Runs**: All Run classes (BaselineRun, TinkerRun, etc.)

  - New instance created per execution
  - Dependencies injected via constructor

- **Configuration**: `IOptions<YeMatchOptions>` pattern
  - Strongly-typed configuration from `appsettings.json` and user secrets
  - Hierarchical structure: `YeMatch:ReadyHost:*`, `YeMatch:SmartApi:*`, etc.

**DI Container Setup** (Program.cs):

- Host.CreateDefaultBuilder with Serilog
- User secrets loaded from ProgramMarker type
- IHttpClientFactory for SMART API client lifecycle
- Factories registered as singletons
- All Run types registered as transient

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
│   ├── ReadyActivity.cs          # SSH-based READY command execution
│   ├── ReadySshClientFactory.cs  # DI-based SSH client and activity factory
│   └── IReadySshClientFactory.cs # Factory interface
├── SmartActivities/
│   ├── SmartActivity.cs          # SMART API call execution
│   ├── SmartApiClientFactory.cs  # DI-based API client and activity factory
│   ├── ISmartApiClientFactory.cs # Factory interface
│   ├── ApiClient.cs              # NSwag-generated API client (regenerated via gen.bat)
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
dotnet user-secrets set "YeMatch:ReadyHost:Username" "your_ready_username"
dotnet user-secrets set "YeMatch:ReadyHost:Password" "your_ready_password"

# Database connection strings
dotnet user-secrets set SmartConnectionString "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=your_host)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=your_service)(SERVER=DEDICATED)));User Id=smart_user;Password=smart_pass"

dotnet user-secrets set ReadyConnectionString "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=your_host)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=your_service)(SERVER=DEDICATED)));User Id=ready_user;Password=ready_pass"

# JWT Signing Key (REQUIRED - must match the SMART API server's signing key)
# This key is used to generate authentication tokens for the SMART API.
# The key must be at least 32 characters (256 bits) for HS256 algorithm.
#
# IMPORTANT: This key MUST match the signing key used by the SMART API server.
# If the server uses 'dotnet user-jwts', find the key by running this command
# in the SMART API project directory:
#   dotnet user-secrets list
# Look for "Authentication:Schemes:Bearer:SigningKeys" and copy the Value field.
#
# Example (use a secure random key of at least 32 characters):
dotnet user-secrets set "YeMatch:Jwt:SigningKey" "your-jwt-signing-key-must-be-at-least-32-bytes-long"

# Optional JWT configuration (override defaults if needed)
# dotnet user-secrets set "YeMatch:Jwt:Issuer" "dotnet-user-jwts"
# dotnet user-secrets set "YeMatch:Jwt:Audience" "https://localhost:7141"
# dotnet user-secrets set "YeMatch:Jwt:ExpirationSeconds" "3600"
```

### Optional Configuration

Create `appsettings.json` to override defaults:

```json
{
  "YeMatch": {
    "BaseDataDirectory": "/custom/path/for/logs",
    "ReadyHost": {
      "Host": "appt07d",
      "ConnectionTimeoutSeconds": 30,
      "Chatty": true
    },
    "SmartApi": {
      "BaseUrl": "https://localhost:7141",
      "TimeoutHours": 2
    },
    "Jwt": {
      "Issuer": "dotnet-user-jwts",
      "Audience": "https://localhost:7141",
      "ExpirationSeconds": 3600
    },
    "YearEndDates": {
      "ProfitYear": 2025,
      "FirstSaturday": "250104",
      "LastSaturday": "251227",
      "CutOffSaturday": "260103",
      "MoreThanFiveYears": "181231"
    }
  }
}
```

**Note**: The JWT `SigningKey` should NEVER be in `appsettings.json`. It must be stored in user secrets only.

Default data directory: `/tmp/ye/<dd-MMM-HH-mm>`

## Running Tests

### Prerequisites

1. **Configure JWT signing key** in user secrets (see Configuration section above)
   - The key must match the SMART API server's signing key
   - If the server uses `dotnet user-jwts`, copy the signing key from the server's user secrets
2. Start the local SMART API with the JWT authentication enabled
3. Ensure READY environment is accessible via SSH with `setyematch` script configured

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
2. Add constructor with DI dependencies (IActivityFactory, IReadySshClientFactory, ISmartApiClientFactory, ILogger)
3. Override `Exec()` method and use `Specify()` to select activities by name
4. Register in `Program.cs` DI container as `AddTransient<YourNewRun>()`
5. Add to `GetRunner()` switch statement

Example:

```csharp
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;

namespace YEMatch.Runs;

public class MyNewRun : Runnable
{
    public MyNewRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<MyNewRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        // Use Specify() to select activities by their registered names
        await Run(Specify(
            "R0",    // READY activity to build database
            "S0",    // SMART activity to import from READY
            "TestPayProfitSelectedColumns"  // Assertion activity
        ));
    }
}
```

Then register in Program.cs:

```csharp
// In ConfigureServices:
services.AddTransient<MyNewRun>();

// In GetRunner switch:
"mynewrun" => services.GetRequiredService<MyNewRun>(),
```

### Adding a New Activity

Activities are created by factories and registered in the ActivityFactory's activities dictionary. Use the `Specify()`
method in Run classes to select activities by name.

**READY Activity:**
Add to `ReadySshClientFactory.CreateActivities()`:

```csharp
new ReadyActivity(ssh, sftp, _chatty, "A30", "MY-NEW-SCRIPT", $"ARG1={value}", dataDirectory)
```

**SMART Activity:**
Add to `SmartApiClientFactory.CreateActivities()`:

```csharp
new SmartActivity(async (client, name, command) =>
{
    var result = await client.MyNewEndpointAsync(command, null);
    return new Outcome(name, "My Operation Description", command,
        OutcomeStatus.Ok, "", null, true);
}, Client!, "A30", "optional-args")
```

**Assert/Test Activity:**
Add to `TestActivityFactory.CreateActivities()`:

```csharp
new MyCustomTestActivity()  // Implements IActivity
```

**Using Activities in Runs:**

```csharp
// In your Run's Exec() method:
await Run(Specify(
    "R30",  // References the READY activity with name "A30"
    "S30",  // References the SMART activity with name "A30"
    "MyCustomTestActivity"  // References by activity name
));
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

Activities prefixed with "P" run READY and SMART operations in parallel. The `ActivityFactory` automatically creates
parallel wrappers for matching activity numbers.

```csharp
// In your Run's Exec() method:
await Run(Specify(
    "P0"   // Runs R0 (READY) and S0 (SMART) concurrently
));

// The ParallelActivity merges outcomes from both activities
// Displays timing for the longer of the two operations
```

Parallel activities are created automatically in `ActivityFactory`:

- "P0" wraps "R0" + "S0"
- "P1" wraps "R1" + "S1"
- etc.

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
    // Use Specify() to select activities by name
    await Run(Specify(
        "R17",  // Run specific READY activity
        "S12",  // Run specific SMART activity
        "TestPayProfitSelectedColumns"  // Run specific test
    ));
}
```

To see all available activity names, check:

- `ReadySshClientFactory.CreateActivities()` for READY activities (R0-R29)
- `SmartApiClientFactory.CreateActivities()` for SMART activities (S0-S29)
- `TestActivityFactory.CreateActivities()` for test/assertion activities

### Compare Database Results

1. Add SQL comparison in `AssertActivities/`
2. Use `BaseSqlActivity` with dual database queries
3. `Shouldly` assertions validate equivalence

### Capture READY Output Files

ReadyActivity automatically downloads files matching patterns from `/dsmdev/data/PAYROLL/tmp-yematch/` to local data
directory.

## Technology Stack

- **.NET 9** with C# 12
- **Microsoft.Extensions.DependencyInjection** - Dependency injection container
- **Microsoft.Extensions.Hosting** - Application host builder
- **Microsoft.Extensions.Configuration** - Configuration and user secrets management
- **IHttpClientFactory** - Proper HttpClient lifecycle management
- **Serilog** - Structured logging
- **Renci.SshNet** - READY SSH/SFTP access
- **NSwag** - SMART API client generation
- **Okta.AspNetCore** - SMART API authentication (test tokens)
- **Oracle.ManagedDataAccess.Core** - Database validation queries
- **Shouldly** - Assertion library

## Important Notes

- **JWT signing key MUST match the SMART API server**: This is the most common authentication issue. Copy the exact key from the server's user secrets
- All database credentials and secrets must be in user secrets (never commit)
- READY system requires `setyematch` script configured on target host
- Log directories can grow large during year-end runs; clean `/tmp/ye` periodically
- API client is generated code; do not hand-edit
- Parallel activities merge outcomes; the longest duration is reported
- READY activities include hardcoded path:
  `/Users/robertherrmann/prj/smart-profit-sharing/src/services/tests/Demoulas.ProfitSharing.IntegrationTests/Resources/`
  for optional local resource base (may need adjustment per developer)

## Troubleshooting

**JWT authentication errors (401 Unauthorized)**:

- Verify the JWT signing key in YEMatch user secrets matches the SMART API server's signing key
- Check the server's user secrets with: `dotnet user-secrets list` (in the SMART API project directory)
- Look for `Authentication:Schemes:Bearer:SigningKeys` and ensure the `Value` field matches `YeMatch:Jwt:SigningKey`
- Ensure the signing key is at least 32 characters long
- Verify the issuer and audience match the server's JWT configuration

**"No SMART authentication" or legacy authentication errors**:

- The old hardcoded test credentials no longer work
- You MUST configure the JWT signing key in user secrets (see Configuration section)

**SSH connection failures**: Verify READY credentials in user secrets and network access to READY host

**Oracle connection errors**: Confirm connection strings in user secrets match target environments

**API client generation fails**: Ensure SMART API `/swagger/Release%201.0/swagger.json` endpoint is accessible

## Related Projects

This testing framework is part of the larger Demoulas Profit Sharing monorepo:

- **SMART application**: `/Users/robertherrmann/prj/smart-profit-sharing/` (main .NET 9 API)
- **Parent repository**: `/Users/robertherrmann/prj/yerunner/` (contains broader test infrastructure)
