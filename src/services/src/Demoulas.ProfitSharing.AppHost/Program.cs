using Demoulas.ProfitSharing.AppHost;
using Demoulas.ProfitSharing.AppHost.Helpers;
using Microsoft.Extensions.Configuration;
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

var database = builder.AddConnectionString("ProfitSharing", "ConnectionStrings:ProfitSharing");

Demoulas_ProfitSharing_Data_Cli cli = new Demoulas_ProfitSharing_Data_Cli();
var projectPath = new FileInfo(cli.ProjectPath).Directory?.FullName;

var cliRunner = builder.AddExecutable("Database-Cli",
        "dotnet",
        projectPath!,
        "run", "--no-build", "--launch-profile", "upgrade-db")
    .WithReference(database)
    .WithCommand(
        name: "upgrade-db",
        displayName: "Upgrade database",
        executeCommand: (c) => Task.FromResult(CommandHelper.RunConsoleApp(projectPath!, "upgrade-db", logger, "upgrade-db")),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "drop-recreate-db",
        displayName: "Drop and recreate database",
        executeCommand: (c) => Task.FromResult(CommandHelper.RunConsoleApp(projectPath!, "drop-recreate-db", logger, "drop-recreate-db")),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "import-from-ready",
        displayName: "Import from READY",
        executeCommand: (c) => Task.FromResult(CommandHelper.RunConsoleApp(projectPath!, "import-from-ready", logger, "import-from-ready")),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "import-from-navigation",
        displayName: "Import from navigation",
        executeCommand: (c) => Task.FromResult(CommandHelper.RunConsoleApp(projectPath!, "import-from-navigation", logger, "import-from-navigation")),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled });

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .Build();

var api = builder.AddProject<Demoulas_ProfitSharing_Api>("ProfitSharing-Api")
    .WithParentRelationship(database)
    .WithReference(database)
    .WithSwaggerUi()
    .WithRedoc()
    .WithScalar()
    .WaitForCompletion(cliRunner)
    .WithUrlForEndpoint("https", annotation =>
    {
        annotation.DisplayText = "Swagger UI";
    });

// Use AddViteApp for Vite applications as per the latest CommunityToolkit.Aspire guidance
var ui = builder.AddNpmApp("ProfitSharing-Ui", "../../../ui/", scriptName: "dev")
    .WithHttpEndpoint(port: uiPort, isProxied: false)
    .WithUrlForEndpoint("http", annotation =>
    {
        annotation.DisplayText = "Profit Sharing";
    })
    .WithOtlpExporter()
    .WithNpmPackageInstallation();

ui.WithReference(api)
    .WaitFor(api)
    .WithParentRelationship(api)
    .WithExternalHttpEndpoints()
    .WithOtlpExporter();

_ = builder.AddProject<Demoulas_ProfitSharing_EmployeeFull_Sync>(name: "ProfitSharing-EmployeeFull-Sync")
     .WaitFor(api)
     .WithParentRelationship(database)
    .WithExplicitStart();

_ = builder.AddProject<Demoulas_ProfitSharing_EmployeePayroll_Sync>(name: "ProfitSharing-EmployeePayroll-Sync")
     .WaitFor(api)
     .WithParentRelationship(database)
    .WithExplicitStart();

_ = builder.AddProject<Demoulas_ProfitSharing_EmployeeDelta_Sync>(name: "ProfitSharing-EmployeeDelta-Sync")
     .WaitFor(api)
     .WithParentRelationship(database)
    .WithExplicitStart();

await using DistributedApplication host = builder.Build();
await host.RunAsync();
