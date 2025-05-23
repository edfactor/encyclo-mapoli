using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.AppHost.Helpers;

public static class ProcessHelper
{
    public static void KillProcessesByName(string processName, ILogger logger)
    {
        try
        {
            var nodeProcesses = Process.GetProcessesByName(processName);
            if (nodeProcesses.Length != 0)
            {
                logger.LogInformation("Found {Count} instance(s) of {ProcessName}. Terminating...", nodeProcesses.Length, processName);
                foreach (var process in nodeProcesses)
                {
                    logger.LogInformation("Killing process ID: {Id}", process.Id);
                    process.Kill();
                    logger.LogInformation("Process ID {Id} terminated.", process.Id);
                }
                logger.LogInformation("All instances of {ProcessName} have been terminated.", processName);
            }
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "An error occurred while killing processes: {Message}", ex.Message);
        }
    }
}
