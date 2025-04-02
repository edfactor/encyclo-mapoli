using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace YEMatch;

public class ReadyActivity(SshClient client, SftpClient sftpClient, bool chatty, string AName, string ksh, string args, string dataDirectory) : Activity
{
    public override string ActivityLetterNumber { get; set; } = AName;

    public override async Task<Outcome> execute()
    {
        Console.WriteLine($"\n{ActivityLetterNumber}");
        if (ksh.StartsWith('!'))
        {
            Console.WriteLine($"    No Operation for {ksh.Substring(1)}");
            return new Outcome(ActivityLetterNumber, ksh, "", OutcomeStatus.NoOperation, "", null, false);
        }

        Console.WriteLine($"$ EJR {ksh} {args}");

        var stopwatch = Stopwatch.StartNew();
        SshCommand? result = null;
        try
        {
            result = client.RunCommand(
                $". ~/setyematch;sed -e's|/production/|/dsmdev/data/PAYROLL/tmp-yematch/|g' jcl/{ksh}.ksh > jcl/YE-{ksh}.ksh;chmod +x jcl/YE-{ksh}.ksh;EJR YE-{ksh} {args}");
        }
        catch (Exception e)
        {
            return new Outcome(ActivityLetterNumber, ksh, $"{ksh} {args}", OutcomeStatus.Error, e.Message, null, false, result?.Result ?? "", result?.Error ?? "");
        }

        stopwatch.Stop();

        var took = stopwatch.Elapsed;

        var lines = result.Result.Trim().Split('\n');
        if (chatty)
        {
            foreach (var line in lines)
                // Printing this line stops the JRider console cold.  Probably a special character in the line?
            {
                if (!line.StartsWith("runb param ="))
                {
                    Console.WriteLine(line);
                }
            }
        }

        // Find log file in output
        var match = Regex.Match(result.Result.Trim(), @"LogFile:\s*(\S+)");
        if (match.Success)
        {
            string logFilePath = match.Groups[1].Value;
            string logFileName = Path.GetFileName(logFilePath);
            string localPath = Path.Combine(dataDirectory, logFileName);

            Console.WriteLine("Full Log File Path: " + logFilePath);
            Console.WriteLine("Log File Name: " + logFileName);

            // download the log file from READY
            await using (var fileStream = File.OpenWrite(localPath))
            {
                sftpClient.DownloadFile(logFilePath, fileStream);
            }
            Console.WriteLine($"Log file copied to: {localPath}");
        }
        else
        {
            Console.WriteLine("Log file path not found.");
        }

        if (result.Error.Trim().Length != 0)
        {
            Console.WriteLine($"---Error Output;\n{result.Error}");
        }

        if (result.ExitStatus != 0)
        {
            Console.WriteLine($"-- ERROR EXIT.  Standard Out;\n{lines}");
            return new Outcome(ActivityLetterNumber, ksh, $"{ksh} {args}", OutcomeStatus.Error, $"got bad exit status = {result.ExitStatus}", null, false,
                result.Result,
                result.Error);
        }

        return new Outcome(ActivityLetterNumber, ksh, $"{ksh} {args}", OutcomeStatus.Ok, lines[^1], took, false, result.Result, result.Error);
    }
}
