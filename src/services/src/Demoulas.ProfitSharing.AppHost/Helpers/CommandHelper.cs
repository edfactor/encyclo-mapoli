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

    public static void RunNpmInstall(string projectPath, ILogger logger)
    {
        try
        {
            string npmExecutable = OperatingSystem.IsWindows() ? @"C:\\Program Files\\nodejs\\npm.cmd" : "npm";
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
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            logger.LogInformation(output);
            if (!string.IsNullOrWhiteSpace(error))
            {
                logger.LogInformation("npm install error: {Error}", error);
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
}
