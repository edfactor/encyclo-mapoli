using Demoulas.ProfitSharing.AppHost;
using Demoulas.ProfitSharing.AppHost.Helpers;
using Projects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;


var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("AppHost");
IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(options: new DistributedApplicationOptions { AllowUnsecuredTransport = true });


// Kill all node processes using helper
ProcessHelper.KillProcessesByName("node", logger);

Demoulas_ProfitSharing_Data_Cli cli = new Demoulas_ProfitSharing_Data_Cli();
var projectPath = new FileInfo(cli.ProjectPath).Directory?.FullName;

var cliRunner = builder.AddExecutable("Database-Cli",
        "dotnet",
        projectPath!,
        "run", "--no-build", "--launch-profile", "upgrade-db")
    .WithCommand(
        name: "upgrade-db",
        displayName: "Upgrade database",
        executeCommand: (c) => Task.FromResult(CommandHelper.RunConsoleApp(projectPath!, "upgrade-db", logger)),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "drop-recreate-db",
        displayName: "Drop and recreate database",
        executeCommand: (c) => Task.FromResult(CommandHelper.RunConsoleApp(projectPath!, "drop-recreate-db", logger)),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "import-from-ready",
        displayName: "Import from READY",
        executeCommand: (c) => Task.FromResult(CommandHelper.RunConsoleApp(projectPath!, "import-from-ready", logger)),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "import-from-navigation",
        displayName: "Import from navigation",
        executeCommand: (c) => Task.FromResult(CommandHelper.RunConsoleApp(projectPath!, "import-from-navigation", logger)),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled });

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>()
    .Build();

var database = builder.AddConnectionString("Oracle", "ProfitSharing");

var api = builder.AddProject<Demoulas_ProfitSharing_Api>("ProfitSharing-Api")
    //.WithHealthCheck("/health")
    //.WithReference(database)
    .WithSwaggerUi()
    .WithRedoc()
    .WithScalar()
    .WaitForCompletion(cliRunner)
    .WithUrlForEndpoint("https", annotation =>
    {
        annotation.DisplayText = "Swagger";
    });

// Use AddViteApp for Vite applications as per the latest CommunityToolkit.Aspire guidance
var ui = builder.AddViteApp("ProfitSharing-Ui", "../../../ui/")
    .WithEndpoint("http", annotation =>
    {
        annotation.IsProxied = false;
        annotation.TargetPort = 3100;
        annotation.Port = 3100;
    })
    .WithUrlForEndpoint("http", annotation =>
    {
        annotation.DisplayText = "Profit Sharing";
    })
    .WithNpmPackageInstallation();

ui.WithReference(api)
    .WaitFor(api)
    .WithParentRelationship(api)
    .WithOtlpExporter();

_ = builder.AddProject<Demoulas_ProfitSharing_EmployeeFull_Sync>(name: "ProfitSharing-EmployeeFull-Sync")
     .WaitFor(api)
     .WithParentRelationship(api)
    .WithExplicitStart();

_ = builder.AddProject<Demoulas_ProfitSharing_EmployeePayroll_Sync>(name: "ProfitSharing-EmployeePayroll-Sync")
     .WaitFor(api)
     .WithParentRelationship(api)
    .WithExplicitStart();

_ = builder.AddProject<Demoulas_ProfitSharing_EmployeeDelta_Sync>(name: "ProfitSharing-EmployeeDelta-Sync")
     .WaitFor(api)
     .WithParentRelationship(api)
    .WithExplicitStart();

await using DistributedApplication host = builder.Build();
await host.RunAsync();
