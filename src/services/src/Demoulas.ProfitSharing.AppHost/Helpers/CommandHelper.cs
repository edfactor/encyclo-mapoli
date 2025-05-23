using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.AppHost.Helpers;

public static class CommandHelper
{
    public static ExecuteCommandResult RunConsoleApp(string projectPath, string launchProfile, ILogger logger, string? operationName = null)
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
        ExecuteCommandResult result;
        if (!string.IsNullOrWhiteSpace(error))
        {
            logger.LogError(error);
            result = new ExecuteCommandResult { Success = false, ErrorMessage = error };
        }
        else
        {
            result = CommandResults.Success();
        }

        // Feedback logging if operationName is provided
        if (!string.IsNullOrWhiteSpace(operationName))
        {
            if (result.Success)
            {
                logger.LogInformation("[{Operation}] completed successfully.", operationName);
            }
            else
            {
                logger.LogError("[{Operation}] failed: {ErrorMessage}", operationName, result.ErrorMessage);
            }
        }
        return result;
    }
}
