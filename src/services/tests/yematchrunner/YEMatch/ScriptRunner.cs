using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YEMatch.YEMatch;

public static class ScriptRunner
{
    public static int Run(bool chatty, string scriptName)
    {
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string scriptExtension = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".bat" : "";
        string scriptPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $@"{homeDirectory}\bin\{scriptName}{scriptExtension}"
            : $@"{homeDirectory}/bin/{scriptName}{scriptExtension}";

        string shell = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/bash";
        string arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"/c \"{scriptPath}\"" : $"\"{scriptPath}\"";

        Process process = new()
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
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"<ERROR-OUTPUT>: {error}</ERROR-OUTPUT>");
        }

        string stdout = process.StandardOutput.ReadToEnd();
        if (chatty)
        {
            Console.WriteLine($"<STANDARD-OUT>{stdout}</STANDARD-OUT>");
        }

        if (stdout.Contains("error", StringComparison.CurrentCultureIgnoreCase))
        {
            Console.WriteLine($"<STANDARD-OUT>{stdout}</STANDARD-OUT>");
            throw new EvaluateException($"Error running script - string 'error' found in output when running : {scriptName}");
        }

        return process.ExitCode;
    }
}
