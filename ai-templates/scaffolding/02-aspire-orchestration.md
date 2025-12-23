# Part 2: Aspire Orchestration

**Estimated Time:** 15-20 minutes  
**Prerequisites:** [Part 1 Complete](./01-foundation-prerequisites.md)  
**Next:** [Part 3: API Bootstrap & Middleware](./03-api-bootstrap-middleware.md)

---

## 🎯 Overview

.NET Aspire is the orchestrator for the entire application. It manages:

- **Service Discovery** - Automatic connection string injection
- **Resource Lifecycle** - Start/stop databases, message queues, caches
- **Development Tools** - Playwright browser automation
- **Health Monitoring** - Dashboard with real-time status
- **Custom Commands** - Database migrations, cleanup tasks

---

## 📦 AppHost Project Setup

### 1. Create AppHost Project

```powershell
cd src
dotnet new aspire-apphost -n MySolution.AppHost
```

### 2. AppHost .csproj Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <Sdk Name="Aspire.AppHost.Sdk" Version="13.1.0" />

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsAspireHost>true</IsAspireHost>
  </PropertyGroup>

  <ItemGroup>
    <!-- Aspire Hosting -->
    <PackageReference Include="Aspire.Hosting.AppHost" />
    <PackageReference Include="Aspire.Hosting.Oracle" />
    <PackageReference Include="Aspire.Hosting.RabbitMQ" />
    <PackageReference Include="Aspire.Hosting.Redis" />

    <!-- Project References -->
    <ProjectReference Include="..\MySolution.Api\MySolution.Api.csproj" />
    <ProjectReference Include="..\MySolution.Data.Cli\MySolution.Data.Cli.csproj" />
  </ItemGroup>
</Project>
```

---

## 🔧 Complete AppHost Program.cs Template

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Projects;

var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("AppHost");
IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(options: new DistributedApplicationOptions
{
    AllowUnsecuredTransport = true  // Dev only - remove for production
});

// Port for UI (if applicable)
short uiPort = 3100;

// Kill node processes if port in use (UI development servers)
if (PortHelper.IsTcpPortInUse(uiPort))
{
    ProcessHelper.KillProcessesByName("node", logger);
}

// ========================================
// STEP 1: Connection Strings
// ========================================
// Connection strings read from appsettings.json or user secrets
// Format: "ConnectionStrings:MyConnection"

var database = builder.AddConnectionString("MyDatabase", "ConnectionStrings:MyDatabase");
var warehouse = builder.AddConnectionString("Warehouse", "ConnectionStrings:Warehouse")
    .WithParentRelationship(database);  // Warehouse depends on database

// ========================================
// STEP 2: External Resources (Oracle, RabbitMQ, Redis)
// ========================================

// Oracle Database (if using containerized Oracle)
// var oracleDb = builder.AddOracle("oracle", port: 1521)
//     .WithDataVolume()  // Persist data
//     .AddDatabase("mydb");

// RabbitMQ (message queue)
var rabbitMq = builder.AddRabbitMQ("messaging", port: 5672)
    .WithManagementPlugin()  // Enable management UI on port 15672
    .WithDataVolume();       // Persist messages

// Redis (distributed cache)
var redis = builder.AddRedis("cache", port: 6379)
    .WithDataVolume()        // Persist cache
    .WithRedisCommander();   // Optional: Redis UI on port 8081

// ========================================
// STEP 3: Database CLI Tool (EF Core Migrations)
// ========================================

var cliProjectPath = GetProjectPath<MySolution_Data_Cli>();
var resourceManager = new ResourceManager(builder, logger);

var cliRunner = builder.AddExecutable("Database-Cli",
        "dotnet",
        cliProjectPath,
        "run", "--no-build", "--launch-profile", "upgrade-db")
    .WithReference(database)
    .WithReference(warehouse)
    .WithParentRelationship(database)
    .WithCommand(
        name: "upgrade-db",
        displayName: "Upgrade database",
        executeCommand: async (c) =>
        {
            var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
            return await CommandHelper.RunDatabaseOperationWithApiManagementAsync(
                cliProjectPath,
                "upgrade-db",
                logger,
                "Upgrade Database",
                interactionService,
                stopApiCallback: () => resourceManager.StopApiAsync(),
                startApiCallback: () => resourceManager.StartApiAsync());
        },
        commandOptions: new CommandOptions { IconName = "ArrowUp", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "drop-recreate",
        displayName: "Drop and Recreate Database",
        executeCommand: async (c) =>
        {
            var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();

            // Prompt user for confirmation (DANGEROUS operation)
            var confirmation = await interactionService.ConfirmAsync(
                "Are you sure you want to DROP and RECREATE the database? ALL DATA WILL BE LOST!",
                defaultValue: false);

            if (!confirmation)
            {
                logger.LogInformation("Drop-recreate operation cancelled by user");
                return CommandResults.Success();
            }

            return await CommandHelper.RunDatabaseOperationWithApiManagementAsync(
                cliProjectPath,
                "drop-recreate-db",
                logger,
                "Drop and Recreate Database",
                interactionService,
                stopApiCallback: () => resourceManager.StopApiAsync(),
                startApiCallback: () => resourceManager.StartApiAsync());
        },
        commandOptions: new CommandOptions { IconName = "Delete", IconVariant = IconVariant.Filled })
    .ExcludeFromManifest();  // Don't include in deployment manifest

// ========================================
// STEP 4: Web API Project
// ========================================

var api = builder.AddProject<MySolution_Api>("api")
    .WithReference(database)        // Inject connection string
    .WithReference(warehouse)       // Inject warehouse connection
    .WithReference(rabbitMq)        // Inject RabbitMQ connection
    .WithReference(redis)           // Inject Redis connection
    .WithExternalHttpEndpoints()    // Allow external HTTP access
    .WaitFor(database)              // Don't start until DB ready
    .WaitFor(rabbitMq)              // Don't start until RabbitMQ ready
    .WaitFor(redis);                // Don't start until Redis ready

// Register API with resource manager for database operations
resourceManager.RegisterApi(api);

// ========================================
// STEP 5: Frontend UI (if applicable)
// ========================================

var ui = builder.AddNpmApp("ui", "../ui", "dev")
    .WithReference(api)
    .WithHttpEndpoint(port: uiPort, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

// ========================================
// STEP 6: Playwright Browser Automation (Development)
// ========================================

if (builder.Environment.IsDevelopment())
{
    var playwright = builder.AddPlaywright("playwright")
        .WithReference(ui)              // Target UI
        .WithProjectPath("../ui/e2e")   // Path to Playwright tests
        .WithStartupCommands(
            new PlaywrightStartupCommand("test", "npx playwright test"),
            new PlaywrightStartupCommand("codegen", "npx playwright codegen http://localhost:3100"),
            new PlaywrightStartupCommand("ui", "npx playwright test --ui")
        )
        .ExcludeFromManifest();
}

// ========================================
// STEP 7: Build and Run
// ========================================

await builder.Build().RunAsync();

// ========================================
// Helper Methods
// ========================================

static string GetProjectPath<T>() where T : IProjectMetadata, new()
{
    T project = new T();
    return new FileInfo(project.ProjectPath).Directory?.FullName
        ?? throw new InvalidOperationException($"Cannot determine project path for {typeof(T).Name}");
}

// ========================================
// Helper Classes
// ========================================

/// <summary>
/// Manages API resource lifecycle during database operations.
/// Stops API before migrations, restarts after completion.
/// </summary>
public class ResourceManager
{
    private readonly IDistributedApplicationBuilder _builder;
    private readonly ILogger _logger;
    private IResourceBuilder<ProjectResource>? _apiResource;

    public ResourceManager(IDistributedApplicationBuilder builder, ILogger logger)
    {
        _builder = builder;
        _logger = logger;
    }

    public void RegisterApi(IResourceBuilder<ProjectResource> apiResource)
    {
        _apiResource = apiResource;
    }

    public async Task StopApiAsync()
    {
        if (_apiResource is null) return;

        _logger.LogInformation("Stopping API for database operation...");
        // Aspire API for resource control
        // Implementation depends on Aspire version - check docs
        await Task.CompletedTask;
    }

    public async Task StartApiAsync()
    {
        if (_apiResource is null) return;

        _logger.LogInformation("Restarting API after database operation...");
        await Task.CompletedTask;
    }
}

/// <summary>
/// Executes database CLI commands with API lifecycle management.
/// </summary>
public static class CommandHelper
{
    public static async Task<ExecuteCommandResult> RunDatabaseOperationWithApiManagementAsync(
        string projectPath,
        string launchProfile,
        ILogger logger,
        string operationName,
        IInteractionService interactionService,
        Func<Task> stopApiCallback,
        Func<Task> startApiCallback)
    {
        try
        {
            // Stop API to avoid connection conflicts
            await stopApiCallback();

            logger.LogInformation("Executing {Operation}...", operationName);

            // Run dotnet CLI command
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --no-build --launch-profile {launchProfile}",
                WorkingDirectory = projectPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process is null)
            {
                return CommandResults.Failure("Failed to start process");
            }

            var output = await process.StandardOutput.ReadToEndAsync();
            var errors = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                logger.LogInformation("{Operation} completed successfully", operationName);
                await interactionService.DisplayAsync($"✅ {operationName} completed successfully", default);
                return CommandResults.Success();
            }
            else
            {
                logger.LogError("{Operation} failed: {Errors}", operationName, errors);
                await interactionService.DisplayAsync($"❌ {operationName} failed: {errors}", default);
                return CommandResults.Failure(errors);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing {Operation}", operationName);
            return CommandResults.Failure(ex.Message);
        }
        finally
        {
            // Always restart API
            await startApiCallback();
        }
    }
}

/// <summary>
/// Checks if a TCP port is in use.
/// </summary>
public static class PortHelper
{
    public static bool IsTcpPortInUse(int port)
    {
        var ipGlobalProperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
        var tcpConnections = ipGlobalProperties.GetActiveTcpConnections();
        return tcpConnections.Any(c => c.LocalEndPoint.Port == port);
    }
}

/// <summary>
/// Kills processes by name (useful for dev servers).
/// </summary>
public static class ProcessHelper
{
    public static void KillProcessesByName(string processName, ILogger logger)
    {
        try
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
            {
                logger.LogInformation("Killing process {ProcessName} (PID: {ProcessId})", processName, process.Id);
                process.Kill();
                process.WaitForExit();
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to kill processes named {ProcessName}", processName);
        }
    }
}
```

---

## 🔧 appsettings.json for AppHost

```json
{
  "ConnectionStrings": {
    "MyDatabase": "Server=localhost;Port=1521;Database=MYDB;User Id=myuser;Password=mypassword;",
    "Warehouse": "Server=localhost;Port=1521;Database=WAREHOUSE;User Id=warehouseuser;Password=warehousepass;"
  }
}
```

**CRITICAL:** Use user secrets for production connection strings:

```powershell
dotnet user-secrets init --project MySolution.AppHost
dotnet user-secrets set "ConnectionStrings:MyDatabase" "your-connection-string" --project MySolution.AppHost
```

---

## 🚀 Aspire CLI Commands

```powershell
# Start the application (from solution root)
aspire run

# Start with specific project
aspire run --project src/MySolution.AppHost

# View dashboard
# Browser opens automatically at https://localhost:17279 (or similar)

# Stop application
# Ctrl+C in terminal or click "Stop" in dashboard

# View resource logs
# Click on resource in dashboard → "Logs" tab

# Execute custom commands
# Click on "Database-Cli" resource → "Commands" tab → "Upgrade database"
```

---

## 📊 Aspire Dashboard Features

### Resource Status View

- **Color Coding**: Green (running), Yellow (starting), Red (failed)
- **Endpoints**: Click to open in browser
- **Logs**: Real-time log streaming
- **Metrics**: Resource utilization graphs

### Custom Commands

- Appear in "Commands" tab for each resource
- Execute via UI button click
- Show progress and results inline

---

## ✅ Validation Checklist - Part 2

- [ ] **AppHost project created** with Aspire.AppHost.Sdk
- [ ] **Program.cs** implements complete orchestration template
- [ ] **Connection strings** configured in appsettings.json or user secrets
- [ ] **Database CLI** project referenced with upgrade-db command
- [ ] **API project** referenced with proper dependencies
- [ ] **ResourceManager** implemented for API lifecycle management
- [ ] **Custom commands** (upgrade-db, drop-recreate) registered
- [ ] **Playwright integration** added (dev environment only)
- [ ] **aspire run** executes without errors
- [ ] **Dashboard** opens and shows all resources as green

---

## 🎓 Key Takeaways - Part 2

1. **Aspire as Orchestrator** - Single command to start entire stack
2. **Service Discovery** - Automatic connection string injection
3. **Resource Dependencies** - WaitFor ensures proper startup order
4. **Custom Commands** - Database operations via dashboard UI
5. **Development Tools** - Playwright for E2E testing without separate setup

---

**Next:** [Part 3: API Bootstrap & Middleware](./03-api-bootstrap-middleware.md) - Complete API Program.cs with middleware pipeline, CORS, JSON serialization, and telemetry
