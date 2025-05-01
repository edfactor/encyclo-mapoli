using System.Diagnostics;
using Demoulas.ProfitSharing.AppHost;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Projects;
using Microsoft.Extensions.Logging;
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("AppHost");


IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(options: new DistributedApplicationOptions { AllowUnsecuredTransport = true });


int uiPort = 3100;

try
{
    // Get all processes with the name "node"
    Process[] nodeProcesses = Process.GetProcessesByName("node");

    if (nodeProcesses.Length != 0)
    {
        logger.LogInformation("Found {NodeProcesses} instance(s) of node.exe. Terminating...", nodeProcesses.Length);

        // Loop through each node process and kill it
        foreach (Process nodeProcess in nodeProcesses)
        {
            logger.LogInformation("Killing process ID: {NodeProcess}", nodeProcess.Id);
            nodeProcess.Kill(); // Kills the process
            logger.LogInformation("Process ID {NodeProcess} terminated.", nodeProcess.Id);
        }

        logger.LogInformation("All instances of node.exe have been terminated.");
    }
}
catch (Exception ex)
{
    logger.LogInformation(ex, "An error occurred: {Message}", ex.Message);
}

ExecuteCommandResult RunConsoleApp(string projectPath, string launchProfile)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = projectPath,
            Arguments = $"run --no-build --launch-profile {launchProfile}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    process.Start();

    // Read the output (optional)
    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();

    process.WaitForExit();
    
    logger.LogInformation(output);

    Console.ForegroundColor = ConsoleColor.Red;
    logger.LogInformation(error);
    Console.ForegroundColor = ConsoleColor.DarkGray;

    if (string.IsNullOrWhiteSpace(error))
    {
        return CommandResults.Success();
    }

    return new ExecuteCommandResult { Success = false, ErrorMessage = error };
}

void RunNpmInstall(string projectPath)
{
    try
    {
        // Determine the correct npm executable based on the operating system
        string npmExecutable = OperatingSystem.IsWindows() ? @"C:\Program Files\nodejs\npm.cmd" : "npm";


        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = npmExecutable,
                WorkingDirectory = projectPath,
                Arguments = "install",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        // Read the output (optional)
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        logger.LogInformation(output);

        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            logger.LogInformation("npm install error: {Error}", error);
            Console.ResetColor();
        }
        else
        {
            logger.LogInformation("npm install completed successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogInformation(ex, "An error occurred while running npm install: {Message}", ex.Message);
    }
}


Demoulas_ProfitSharing_Data_Cli cli = new Demoulas_ProfitSharing_Data_Cli();
var projectPath = new FileInfo(cli.ProjectPath).Directory?.FullName;

var cliRunner = builder.AddExecutable("Database-Cli",
        "dotnet",
        projectPath!,
        "run", "--no-build", "--launch-profile", "upgrade-db")
    .WithCommand(
        name: "upgrade-db",
        displayName: "Upgrade database",
        executeCommand: (c) => Task.FromResult(RunConsoleApp(projectPath!, "upgrade-db")),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "drop-recreate-db",
        displayName: "Drop and recreate database",
        executeCommand: (c) => Task.FromResult(RunConsoleApp(projectPath!, "drop-recreate-db")),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
    .WithCommand(
        name: "import-from-ready",
        displayName: "Import from READY",
        executeCommand: (c) => Task.FromResult(RunConsoleApp(projectPath!, "import-from-ready")),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled })
.WithCommand(
        name: "import-from-navigation",
        displayName: "Import from navigation",
        executeCommand: (c) => Task.FromResult(RunConsoleApp(projectPath!, "import-from-navigation")),
        commandOptions: new CommandOptions { IconName = "Database", IconVariant = IconVariant.Filled });


var api = builder.AddProject<Demoulas_ProfitSharing_Api>("ProfitSharing-Api")
    .WithHttpsHealthCheck("/health")
    .WithSwaggerUi()
    .WithRedoc()
    .WithScalar()
    .WaitForCompletion(cliRunner);

var ui = builder.AddNpmApp("ProfitSharing-Ui", "../../../ui/", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithParentRelationship(api)
    .WithHttpEndpoint(port: uiPort, isProxied: false)
    .WithOtlpExporter();
RunNpmInstall(ui.Resource.WorkingDirectory);

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
