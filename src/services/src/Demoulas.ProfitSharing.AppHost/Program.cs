using Demoulas.ProfitSharing.AppHost;
using Demoulas.ProfitSharing.AppHost.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Projects;


var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("AppHost");
IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(options: new DistributedApplicationOptions { AllowUnsecuredTransport = true });
short uiPort = 3100;

// Kill all node processes using helper if port is in use
if (PortHelper.IsTcpPortInUse(uiPort))
{
    ProcessHelper.KillProcessesByName("node", logger);
}
var database = builder.AddConnectionString("ProfitSharing", "ConnectionStrings:ProfitSharing").ExcludeFromManifest();
var warehouse = builder.AddConnectionString("Warehouse", "ConnectionStrings:Warehouse").WithParentRelationship(database).ExcludeFromManifest();

Demoulas_ProfitSharing_Data_Cli cli = new Demoulas_ProfitSharing_Data_Cli();
var projectPath = new FileInfo(cli.ProjectPath).Directory?.FullName;

// Create resource manager for API lifecycle management during database operations
var resourceManager = new ResourceManager(builder, logger);

var cliRunner = builder.AddExecutable("Database-Cli",
        "dotnet",
        projectPath!,
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
                projectPath!,
                "upgrade-db",
                logger,
                "Upgrade Database",
                interactionService,
                stopApiCallback: () => resourceManager.StopApiAsync(),
                startApiCallback: () => resourceManager.StartApiAsync());
        },
        commandOptions: new CommandOptions { IconName = "ArrowUp", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "drop-recreate-db",
        displayName: "Drop and recreate database",
        executeCommand: async (c) =>
        {
            var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();

            // Show confirmation for destructive operation
            if (interactionService.IsAvailable)
            {
                var confirmation = await interactionService.PromptConfirmationAsync(
                    title: "⚠️ Confirm Database Drop",
                    message: "Are you sure you want to **drop and recreate** the database? All data will be lost!",
                    options: new MessageBoxInteractionOptions
                    {
                        Intent = MessageIntent.Warning,
                        PrimaryButtonText = "Yes, Drop Database",
                        SecondaryButtonText = "Cancel",
                        ShowSecondaryButton = true,
                        EnableMessageMarkdown = true
                    });

                if (!confirmation.Data)
                {
                    return CommandResults.Failure("User cancelled the operation.");
                }
            }

            return await CommandHelper.RunDatabaseOperationWithApiManagementAsync(
                projectPath!,
                "drop-recreate-db",
                logger,
                "Drop & Recreate Database",
                interactionService,
                stopApiCallback: () => resourceManager.StopApiAsync(),
                startApiCallback: () => resourceManager.StartApiAsync());
        },
        commandOptions: new CommandOptions { IconName = "DatabaseWarning", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "import-from-ready",
        displayName: "Import from READY",
        executeCommand: async (c) =>
        {
            var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
            return await CommandHelper.RunDatabaseOperationWithApiManagementAsync(
                projectPath!,
                "import-from-ready",
                logger,
                "Import from READY",
                interactionService,
                stopApiCallback: () => resourceManager.StopApiAsync(),
                startApiCallback: () => resourceManager.StartApiAsync());
        },
        commandOptions: new CommandOptions { IconName = "ArrowDownload", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "import-from-navigation",
        displayName: "Import from navigation",
        executeCommand: async (c) =>
        {
            var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
            return await CommandHelper.RunDatabaseOperationWithApiManagementAsync(
                projectPath!,
                "import-from-navigation",
                logger,
                "Import Navigation",
                interactionService,
                stopApiCallback: () => resourceManager.StopApiAsync(),
                startApiCallback: () => resourceManager.StartApiAsync());
        },
        commandOptions: new CommandOptions { IconName = "Navigation", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "import-uat-navigation",
        displayName: "Import UAT navigation",
        executeCommand: async (c) =>
        {
            var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();
            return await CommandHelper.RunDatabaseOperationWithApiManagementAsync(
                projectPath!,
                "import-uat-navigation",
                logger,
                "Import UAT Navigation",
                interactionService,
                stopApiCallback: () => resourceManager.StopApiAsync(),
                startApiCallback: () => resourceManager.StartApiAsync());
        },
        commandOptions: new CommandOptions { IconName = "Navigation", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "Nuclear-Option",
        displayName: "Full Nuclear Reset",
        executeCommand: async (c) =>
        {
            var interactionService = c.ServiceProvider.GetRequiredService<IInteractionService>();

            // Show initial confirmation dialog
            if (interactionService.IsAvailable)
            {
                var confirmation = await interactionService.PromptConfirmationAsync(
                    title: "⚠️ Nuclear Option - Confirm Destructive Action",
                    message: """
                    # WARNING: This will completely destroy and recreate the database!
                    
                    **This operation will:**
                    1. **Drop the entire database** (all data will be permanently lost)
                    2. **Recreate the database schema** from migrations
                    3. **Import data from READY system**
                    4. **Import navigation data**
                    
                    ⚠️ **This action cannot be undone!**
                    
                    Are you absolutely sure you want to proceed?
                    """,
                    options: new MessageBoxInteractionOptions
                    {
                        Intent = MessageIntent.Warning,
                        PrimaryButtonText = "Yes, Destroy and Recreate",
                        SecondaryButtonText = "Cancel",
                        ShowSecondaryButton = true,
                        EnableMessageMarkdown = true
                    });

                if (!confirmation.Data)
                {
                    await interactionService.PromptNotificationAsync(
                        title: "Operation Cancelled",
                        message: "Nuclear Option was cancelled by user.",
                        options: new NotificationInteractionOptions
                        {
                            Intent = MessageIntent.Information
                        });
                    return CommandResults.Failure("User cancelled the nuclear option.");
                }

                // Show starting notification
                _ = interactionService.PromptNotificationAsync(
                    title: "🚀 Starting Nuclear Option",
                    message: "Beginning full database reset. This may take several minutes...",
                    options: new NotificationInteractionOptions
                    {
                        Intent = MessageIntent.Information
                    });
            }

            // Step 1: Drop and recreate (with API management)
            var step1 = await CommandHelper.RunDatabaseOperationWithApiManagementAsync(projectPath!, "drop-recreate-db", logger, "Step 1/3: Drop & Recreate Database", interactionService, () => resourceManager.StopApiAsync(), () => resourceManager.StartApiAsync());
            if (!step1.Success)
            {
                return CommandResults.Failure($"Nuclear Option failed at step 1: {step1.ErrorMessage}");
            }

            // Step 2: Import from READY (with API management)
            var step2 = await CommandHelper.RunDatabaseOperationWithApiManagementAsync(projectPath!, "import-from-ready", logger, "Step 2/3: Import from READY", interactionService, () => resourceManager.StopApiAsync(), () => resourceManager.StartApiAsync());
            if (!step2.Success)
            {
                return CommandResults.Failure($"Nuclear Option failed at step 2: {step2.ErrorMessage}");
            }

            // Step 3: Import navigation (with API management)
            var step3 = await CommandHelper.RunDatabaseOperationWithApiManagementAsync(projectPath!, "import-from-navigation", logger, "Step 3/3: Import Navigation", interactionService, () => resourceManager.StopApiAsync(), () => resourceManager.StartApiAsync());
            if (!step3.Success)
            {
                return CommandResults.Failure($"Nuclear Option failed at step 3: {step3.ErrorMessage}");
            }

            // Show final success notification
            if (interactionService.IsAvailable)
            {
                _ = interactionService.PromptNotificationAsync(
                    title: "✅ Nuclear Option Complete!",
                    message: "Database has been successfully reset. All steps completed.",
                    options: new NotificationInteractionOptions
                    {
                        Intent = MessageIntent.Success,
                        LinkText = "View Logs",
                        LinkUrl = "/console/Database-Cli"
                    });
            }

            return CommandResults.Success();
        },
        commandOptions: new CommandOptions { IconName = "Fire", IconVariant = IconVariant.Filled });

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .Build();

var api = builder.AddProject<Demoulas_ProfitSharing_Api>("ProfitSharing-Api")
    .WithParentRelationship(database)
    .WithReference(database)
    .WithReference(warehouse)
    .WithSwaggerUi()
    .WithRedoc()
    .WithScalar()
    .WaitForCompletion(cliRunner)
    .WithUrlForEndpoint("https", annotation =>
    {
        annotation.DisplayText = "Swagger UI";
    });

// Register the API resource with resource manager for lifecycle management during DB operations
resourceManager.RegisterApiResource(api);

// Use AddViteApp for Vite applications as per the Aspire 13.1 JavaScript integration
// This automatically handles npm package installation and has Vite-specific defaults
// Configure the default "http" endpoint to use port 3100 (required for Okta callback)
var ui = builder.AddViteApp("ProfitSharing-Ui", "../../../ui/")
    .WithEndpoint("http", endpoint =>
    {
        endpoint.Port = uiPort;
        endpoint.TargetPort = uiPort;
        endpoint.IsProxied = false;
    })
    .WithUrlForEndpoint("http", annotation =>
    {
        annotation.DisplayText = "Profit Sharing";
    })
    .WithOtlpExporter();

ui.WithReference(api)
    .WaitFor(api)
    .WithParentRelationship(api)
    .WithExternalHttpEndpoints()
    .WithOtlpExporter();

_ = builder.AddProject<Demoulas_ProfitSharing_EmployeeFull_Sync>(name: "ProfitSharing-EmployeeFull-Sync")
     .WaitFor(api)
     .WithParentRelationship(database)
     .WithReference(database)
     .WithReference(warehouse)
    .WithExplicitStart();

_ = builder.AddProject<Demoulas_ProfitSharing_EmployeePayroll_Sync>(name: "ProfitSharing-EmployeePayroll-Sync")
     .WaitFor(api)
     .WithParentRelationship(database)
     .WithReference(database)
     .WithReference(warehouse)
    .WithExplicitStart();

_ = builder.AddProject<Demoulas_ProfitSharing_EmployeeDelta_Sync>(name: "ProfitSharing-EmployeeDelta-Sync")
     .WaitFor(api)
     .WithParentRelationship(database)
     .WithReference(database)
     .WithReference(warehouse)
    .WithExplicitStart();

// Playwright E2E test runner as an executable resource
// Find solution root by looking for the .slnx file, then navigate to ui folder
var appHostBinDir = AppContext.BaseDirectory;
var currentDir = new DirectoryInfo(appHostBinDir);

// Navigate up to find the services directory (where .slnx file exists)
while (currentDir != null && !File.Exists(Path.Combine(currentDir.FullName, "Demoulas.ProfitSharing.slnx")))
{
    currentDir = currentDir.Parent;
}

if (currentDir == null)
{
    throw new InvalidOperationException("Could not find services directory with .slnx file");
}

// currentDir is now at src/services/, go up one level to src/, then into ui/
var srcDir = currentDir.Parent;
if (srcDir == null)
{
    throw new InvalidOperationException("Could not navigate to src directory");
}

var uiFullPath = Path.Combine(srcDir.FullName, "ui");

if (!Directory.Exists(uiFullPath))
{
    throw new DirectoryNotFoundException($"UI directory not found at: {uiFullPath}");
}

logger.LogInformation("Playwright tests working directory: {WorkingDirectory}", uiFullPath);

var playwrightTests = builder.AddExecutable("Playwright-Tests",
    command: OperatingSystem.IsWindows() ? "npm.cmd" : "npm",
    workingDirectory: uiFullPath,
    "run", "e2e")
    .WithReference(api)
    .WaitFor(api)
    .WaitFor(ui)
    .WithParentRelationship(api)
    .WithCommand(
        name: "run-e2e",
        displayName: "Run E2E Tests",
        executeCommand: c => Task.FromResult(CommandHelper.RunNpmScript(uiFullPath, "e2e", logger, "playwright-e2e")),
        commandOptions: new CommandOptions { IconName = "Play", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "show-report",
        displayName: "Open E2E Report",
        executeCommand: c => Task.FromResult(CommandHelper.RunShellCommand(uiFullPath, "npx playwright show-report", logger, "playwright-report")),
        commandOptions: new CommandOptions { IconName = "Link", IconVariant = IconVariant.Filled })
    .WithExplicitStart();

// Continuous watch mode resource (does not auto-exit). Explicit start to avoid consuming resources by default.
var playwrightWatch = builder.AddExecutable("Playwright-Tests-Watch",
        command: OperatingSystem.IsWindows() ? "npm.cmd" : "npm",
        workingDirectory: uiFullPath,
        "run",
    "e2e:watch")
    .WithReference(api)
    .WaitFor(api)
    .WaitFor(ui)
    .WithParentRelationship(api)
    .WithExplicitStart()
    .WithCommand(
        name: "start-watch",
        displayName: "Start E2E Watch",
        executeCommand: c => Task.FromResult(CommandHelper.RunNpmScript(uiFullPath, "e2e:watch", logger, "playwright-watch")),
        commandOptions: new CommandOptions { IconName = "Play", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "test-ui",
        displayName: "Open Test UI",
        executeCommand: c => Task.FromResult(CommandHelper.RunShellCommand(uiFullPath, "npx playwright test --ui", logger, "playwright-ui")),
        commandOptions: new CommandOptions { IconName = "Browser", IconVariant = IconVariant.Filled });

// Playwright report server (serves existing latest report). Explicit start; depends on tests having generated a report.
int reportPort = 4321;
var playwrightReport = builder.AddExecutable("Playwright-Report",
        command: OperatingSystem.IsWindows() ? "npx.cmd" : "npx",
        workingDirectory: uiFullPath,
        // Host on all interfaces so Aspire can proxy/expose it
        "playwright", "show-report", "--host", "0.0.0.0", "--port", reportPort.ToString())
    .WaitFor(playwrightTests)
    .WithParentRelationship(playwrightTests)
    .WithHttpEndpoint(port: reportPort, isProxied: false, name: "report")
    .WithUrlForEndpoint("report", annotation =>
    {
        annotation.DisplayText = "Playwright Report";
    })
    .WithExplicitStart();

await using DistributedApplication host = builder.Build();
await host.RunAsync();
