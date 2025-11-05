using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Renci.SshNet;
using YEMatch.YEMatch.Activities;

#pragma warning disable CS0162 // Unreachable code detected

namespace YEMatch.YEMatch.ReadyActivities;

[SuppressMessage("Major Code Smell", "S6966:Awaitable method should be used")]
public class ReadyActivity(SshClient client, SftpClient sftpClient, bool chatty, string AName, string ksh, string _args, string dataDirectory) : IActivity
{
    public const string OptionalLocalResourceBase = "/Users/robertherrmann/prj/smart-profit-sharing/src/services/tests/Demoulas.ProfitSharing.IntegrationTests/Resources/";
    private const bool UpdateIntegrationTestResources = true;
    public string Args = _args;

    public string Name()
    {
        return AName.Substring(0, 1).Replace("A", "R") + AName.Substring(1);
    }

    public async Task<Outcome> Execute()
    {
        Console.WriteLine($"\n{Name()}");

        if (ksh.StartsWith('!'))
        {
            Console.WriteLine($"    No Operation for {ksh.Substring(1)}");
            return new Outcome(Name(), ksh, "", OutcomeStatus.NoOperation, "", null, false);
        }

        Console.WriteLine($"    $ EJR {ksh} {Args}");
        Console.WriteLine($"    $ EJR YE-{ksh} {Args}");

        Stopwatch stopwatch = Stopwatch.StartNew();
        SshCommand? result = null;
        // Translates the production paths to safe development paths
       string rawCommand =
            $". ~/setyematch;sed -e's|/production/|/dsmdev/data/PAYROLL/tmp-yematch/|g' jcl/{ksh}.ksh > jcl/YE-{ksh}.ksh;chmod +x jcl/YE-{ksh}.ksh;EJR YE-{ksh} {Args}";
        try
        {
            result = client.RunCommand(rawCommand);
        }
        catch (Exception e)
        {
            return new Outcome(Name(), ksh, $"{ksh} {Args}", OutcomeStatus.Error, e.Message, null, false, result?.Result ?? "", result?.Error ?? "");
        }

        stopwatch.Stop();

        TimeSpan took = stopwatch.Elapsed;

        string[] lines = result.Result.Trim().Split('\n');
        if (chatty)
        {
            foreach (string line in lines)
            // Printing this line stops the JRider console cold.  Probably a special character in the line?
            {
                if (!line.StartsWith("runb param ="))
                {
                    Console.WriteLine(line);
                }
            }
        }

        // Find log file in output
        Match match = Regex.Match(result.Result.Trim(), @"LogFile:\s*(\S+)");
        if (match.Success)
        {
            string logFilePath = match.Groups[1].Value;
            string logFileName = Path.GetFileName(logFilePath);
            string localPath = Path.Combine(dataDirectory, logFileName);

            IfChatty("Full Log File Path: " + logFilePath);
            IfChatty("Log File Name: " + logFileName);

            // download the log file from READY
            await using (FileStream fileStream = File.OpenWrite(localPath))
            {
                sftpClient.DownloadFile(logFilePath, fileStream);
            }

            IfChatty($"Log file copied to: file:///{localPath}");

            // #######  LP WAS CALLED WITH ARGS :-d plaser5 /dsmdev/data/PAYROLL/SYS/PVTSYSOUT/PAY444A-25103 
            MatchCollection matches = Regex.Matches(result.Result, @"#######  LP WAS CALLED WITH ARGS :-d \w+ (/.+?-\d{4,})");
            foreach (Match lpMatch in matches)
            {
                string reportName = lpMatch.Groups[1].Value;
                string filenameLocal = Path.GetFileNameWithoutExtension(reportName).Split('-')[0];
                string qpay066Local = Path.Combine(dataDirectory, $"{filenameLocal}.txt");
                if (!sftpClient.Exists(reportName))
                {
                    IfChatty($"Odd, but remote file does not exist: {reportName}");
                }
                else
                {
                    IfChatty($"copying {reportName} to file:///{qpay066Local}");
                    await using FileStream fileStream = File.OpenWrite(qpay066Local);
                    sftpClient.DownloadFile(reportName, fileStream);
                }

                string testingFile = OptionalLocalResourceBase + "psupdate-pay444-r2.txt";
                if (HasDirectory(testingFile) && filenameLocal == "PAY444L" && File.Exists(testingFile) && UpdateIntegrationTestResources)
                {
                    File.Copy(qpay066Local, testingFile, true);
                    IfChatty($"NOTE::: Updated {testingFile}");
                }
            }
        }
        else
        {
            IfChatty("Log file path not found.");
        }

        if (result.Error.Trim().Length != 0)
        {
            Console.WriteLine($"---Error Output;\n{result.Error}");
        }

        if (result.ExitStatus != 0)
        {
            Console.WriteLine($"-- ERROR EXIT.  Standard Out;\n{lines}");
            return new Outcome(Name(), ksh, $"{ksh} {Args}", OutcomeStatus.Error, $"got bad exit status = {result.ExitStatus}", null, false,
                result.Result,
                result.Error);
        }

        return new Outcome(Name(), ksh, $"{ksh} {Args}", OutcomeStatus.Ok, lines[^1], took, false, result.Result, result.Error);
    }

    private void IfChatty(string msg)
    {
        if (chatty)
        {
            Console.WriteLine(msg);
        }
    }

    private static bool HasDirectory(string testingFile2)
    {
        string directory = Path.GetDirectoryName(testingFile2)!;
        return Directory.Exists(directory);
    }
}
