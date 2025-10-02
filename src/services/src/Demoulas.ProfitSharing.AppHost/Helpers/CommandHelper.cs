using System.Diagnostics;
using Aspire.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.AppHost.Helpers;

public static class CommandHelper
{
    public static ExecuteCommandResult RunConsoleApp(string projectPath, string launchProfile, ILogger logger, string? operationName = null, IInteractionService? interactionService = null)
    {
        // Show starting notification if interaction service is available
        if (interactionService?.IsAvailable == true && !string.IsNullOrWhiteSpace(operationName))
        {
            _ = interactionService.PromptNotificationAsync(
                title: $"Starting: {operationName}",
                message: $"Beginning database operation: {operationName}",
                options: new NotificationInteractionOptions
                {
                    Intent = MessageIntent.Information
                });
        }

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

        // Log output for debugging
        if (!string.IsNullOrWhiteSpace(output))
        {
            logger.LogInformation(output);
        }

        // Determine success based on exit code (most reliable indicator)
        ExecuteCommandResult result;
        if (process.ExitCode != 0)
        {
            // Operation failed - collect error details
            var errorMessage = !string.IsNullOrWhiteSpace(error) 
                ? error 
                : $"Process exited with code {process.ExitCode}. Check console logs for details.";
            
            logger.LogError("Process failed with exit code {ExitCode}: {ErrorMessage}", process.ExitCode, errorMessage);
            result = new ExecuteCommandResult { Success = false, ErrorMessage = errorMessage };
        }
        else if (!string.IsNullOrWhiteSpace(error))
        {
            // Exit code 0 but stderr has content - log as warning but treat as success
            // (some tools write non-error info to stderr)
            logger.LogWarning("Process succeeded but wrote to stderr: {StdErr}", error);
            result = CommandResults.Success();
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

                // Show success notification if interaction service is available
                if (interactionService?.IsAvailable == true)
                {
                    _ = interactionService.PromptNotificationAsync(
                        title: $"Completed: {operationName}",
                        message: $"Database operation completed successfully: {operationName}",
                        options: new NotificationInteractionOptions
                        {
                            Intent = MessageIntent.Success
                        });
                }
            }
            else
            {
                logger.LogError("[{Operation}] failed: {ErrorMessage}", operationName, result.ErrorMessage);

                // Show error notification if interaction service is available
                if (interactionService?.IsAvailable == true)
                {
                    _ = interactionService.PromptNotificationAsync(
                        title: $"❌ Failed: {operationName}",
                        message: $"**Database operation failed**: {operationName}\n\n**Error Details:**\n```\n{result.ErrorMessage}\n```\n\nCheck console logs for more information.",
                        options: new NotificationInteractionOptions
                        {
                            Intent = MessageIntent.Error
                        });
                }
            }
        }
        return result;
    }

    public static ExecuteCommandResult RunNpmScript(string workingDirectory, string script, ILogger logger, string? operationName = null)
    {
        // On Windows, launching "npm" directly from a service/hosted process can fail if PATH/env not propagated.
        // Use a platform-aware invocation. Prefer npm, fallback to npx, and finally report error.
        string fileName;
        string arguments;
        if (OperatingSystem.IsWindows())
        {
            // Use cmd /c to leverage PATH resolution (Volta shim etc.).
            fileName = "cmd.exe";
            arguments = $"/c npm run {script}";
        }
        else
        {
            fileName = "bash";
            arguments = $"-lc 'npm run {script}'";
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = workingDirectory,
                Arguments = arguments,
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
        ExecuteCommandResult result;
        if (!string.IsNullOrWhiteSpace(error) && process.ExitCode != 0)
        {
            logger.LogError(error);
            result = new ExecuteCommandResult { Success = false, ErrorMessage = error };
        }
        else
        {
            result = CommandResults.Success();
        }

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

    public static ExecuteCommandResult RunShellCommand(string workingDirectory, string command, ILogger logger, string? operationName = null)
    {
        string fileName;
        string arguments;
        if (OperatingSystem.IsWindows())
        {
            fileName = "cmd.exe";
            arguments = $"/c {command}";
        }
        else
        {
            fileName = "bash";
            arguments = $"-lc '{command}'";
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = workingDirectory,
                Arguments = arguments,
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

        if (!string.IsNullOrWhiteSpace(output))
        {
            logger.LogInformation(output);
        }

        ExecuteCommandResult result;
        if (process.ExitCode != 0)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                logger.LogError(error);
            }
            result = new ExecuteCommandResult { Success = false, ErrorMessage = error };
        }
        else
        {
            result = CommandResults.Success();
        }

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
