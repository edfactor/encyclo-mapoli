using System.Diagnostics;

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


var api = builder.AddProject<Projects.Demoulas_ProfitSharing_Api>("demoulas-profitsharing-api")
    .WithHttpHealthCheck("/health")
    .WithHttpsHealthCheck("/health")
    .AsHttp2Service();

builder.AddNpmApp("demoulas-profitsharing-ui", "../../../ui/", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(port: uiPort, isProxied:false)
    .WithExternalHttpEndpoints();


await using DistributedApplication host = builder.Build();
await host.RunAsync();
