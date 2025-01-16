using System.Diagnostics;
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

var api = builder.AddProject<Demoulas_ProfitSharing_Api>("ProfitSharing-Api")
    .WithHttpHealthCheck("/health")
    .WithHttpsHealthCheck("/health");

var ui = builder.AddNpmApp("ProfitSharing-Ui", "../../../ui/", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(port: uiPort, isProxied: false);

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
