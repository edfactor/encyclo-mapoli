using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace YEMatch.YEMatch;

public static class ScriptRunner
{
    public static int Run(bool chatty, string workingDir, string command, string args)
    {
        Console.WriteLine($"cd {workingDir}; {command} {args}");

        Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = workingDir,
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        // Event handlers for real-time output
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data) && chatty)
            {
                Console.WriteLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            Console.Error.WriteLine(e.Data);
        };

        process.Start();

        // Begin asynchronous reading
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();
        StringBuilder stdoutBuilder = new();
        StringBuilder stderrBuilder = new();

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                stdoutBuilder.AppendLine(e.Data);
                if (chatty)
                {
                    Console.WriteLine(e.Data);
                }
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                stderrBuilder.AppendLine(e.Data);
                Console.Error.WriteLine(e.Data);
            }
        };

        process.WaitForExit();

        string stdout = stdoutBuilder.ToString();
        string error = stderrBuilder.ToString();

        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"<ERROR-OUTPUT>: {error}</ERROR-OUTPUT>");
        }

        if (chatty)
        {
            Console.WriteLine($"<STANDARD-OUT>{stdout}</STANDARD-OUT>");
        }

        if (stdout.Contains("error", StringComparison.CurrentCultureIgnoreCase))
        {
            Console.WriteLine($"<STANDARD-OUT>{stdout}</STANDARD-OUT>");
            throw new EvaluateException($"Error running script - string 'error' found in output when running, command: {command} with args: {args}");
        }

        return process.ExitCode;
    }
}
