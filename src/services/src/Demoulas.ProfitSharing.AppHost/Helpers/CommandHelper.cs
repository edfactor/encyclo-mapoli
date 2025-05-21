using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.AppHost.Helpers;

public static class CommandHelper
{
    public static ExecuteCommandResult RunConsoleApp(string projectPath, string launchProfile, ILogger logger)
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
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        logger.LogError(output);
        if (!string.IsNullOrWhiteSpace(error))
        {
            logger.LogError(error);
            return new ExecuteCommandResult { Success = false, ErrorMessage = error };
        }
        return CommandResults.Success();
    }
}
