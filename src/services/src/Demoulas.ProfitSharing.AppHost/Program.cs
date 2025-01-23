using System.Diagnostics;
using Aspire.Hosting;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(options: new DistributedApplicationOptions { AllowUnsecuredTransport = true });


int uiPort = 3100;

try
{
    // Get all processes with the name "node"
    Process[] nodeProcesses = Process.GetProcessesByName("node");

    if (nodeProcesses.Length != 0)
    {
        Console.WriteLine($"Found {nodeProcesses.Length} instance(s) of node.exe. Terminating...");

        // Loop through each node process and kill it
        foreach (Process nodeProcess in nodeProcesses)
        {
            Console.WriteLine($"Killing process ID: {nodeProcess.Id}");
            nodeProcess.Kill(); // Kills the process
            await nodeProcess.WaitForExitAsync(); // Ensures the process is completely terminated
            Console.WriteLine($"Process ID {nodeProcess.Id} terminated.");
        }

        Console.WriteLine("All instances of node.exe have been terminated.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

ExecuteCommandResult RunConsoleApp(string projectPath, string launchProfile)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = projectPath,
            Arguments = $"run --launch-profile {launchProfile}",
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
    
    Console.WriteLine(output);

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(error);
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
        string npmExecutable = OperatingSystem.IsWindows() ? @"C:\Program Files\nodejs\npm.cmd" : "/usr/local/bin/npm";


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

        Console.WriteLine(output);

        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"npm install error: {error}");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("npm install completed successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while running npm install: {ex.Message}");
    }
}


Demoulas_ProfitSharing_Data_Cli cli = new Demoulas_ProfitSharing_Data_Cli();
var projectPath = new FileInfo(cli.ProjectPath).Directory?.FullName;

var cliRunner = builder.AddExecutable("Database-Cli",
    "dotnet",
    projectPath!,
    "run", "--launch-profile", "upgrade-db")
    .WithCommand(
        name: "upgrade-db",
        displayName: "Upgrade database",
        executeCommand: (c) => Task.FromResult(RunConsoleApp(projectPath!, "upgrade-db")))
    .WithCommand(
        name: "drop-recreate-db",
        displayName: "Drop and recreate database",
        executeCommand: (c) => Task.FromResult(RunConsoleApp(projectPath!, "drop-recreate-db")))
    .WithCommand(
        name: "import-from-ready",
        displayName: "Import from READY",
        executeCommand: (c) => Task.FromResult(RunConsoleApp(projectPath!, "import-from-ready")));


var api = builder.AddProject<Demoulas_ProfitSharing_Api>("ProfitSharing-Api")
    .WithHttpHealthCheck("/health")
    .WithHttpsHealthCheck("/health")
    .WaitFor(cliRunner);

var ui = builder.AddNpmApp("ProfitSharing-Ui", "../../../ui/", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(port: uiPort, isProxied: false);
RunNpmInstall(ui.Resource.WorkingDirectory);

var fullSync = builder.AddProject<Demoulas_ProfitSharing_EmployeeFull_Sync>(name: "ProfitSharing-EmployeeFull-Sync")
    .WaitFor(api)
    .WaitFor(ui);

var payroll = builder.AddProject<Demoulas_ProfitSharing_EmployeePayroll_Sync>(name: "ProfitSharing-EmployeePayroll-Sync")
    .WaitFor(api)
    .WaitFor(ui)
    .WaitFor(fullSync);

builder.AddProject<Demoulas_ProfitSharing_EmployeeDelta_Sync>(name: "ProfitSharing-EmployeeDelta-Sync")
    .WaitFor(api)
    .WaitFor(ui)
    .WaitFor(fullSync)
    .WaitFor(payroll);


await using DistributedApplication host = builder.Build();
await host.RunAsync();
