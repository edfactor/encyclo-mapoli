using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YEMatch;

public static class ScriptRunner
{
    public static int Run(string scriptName)
    {
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var scriptExtension = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".bat" : "";
        var scriptPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $@"{homeDirectory}\bin\{scriptName}{scriptExtension}"
            : $@"{homeDirectory}/bin/{scriptName}{scriptExtension}";

        var shell = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/bash";
        var arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"/c \"{scriptPath}\"" : $"\"{scriptPath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = shell,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"Error: {error}");
        }

        return process.ExitCode;
    }
}
