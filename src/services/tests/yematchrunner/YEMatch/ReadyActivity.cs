using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace YEMatch;

[SuppressMessage("Major Code Smell", "S6966:Awaitable method should be used")]
public class ReadyActivity(SshClient client, SftpClient sftpClient, bool chatty, string AName, string ksh, string args, string dataDirectory) : Activity
{
    public override string ActivityLetterNumber { get; set; } = AName;
    private const string OptionalLocalResourceBase = "/Users/robertherrmann/prj/smart-profit-sharing/src/services/tests/Demoulas.ProfitSharing.IntegrationTests/Resources/";

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

            Console.WriteLine($"Log file copied to: file:///{localPath}");

            var matchTermReport = Regex.Match(result.Result.Trim(), @" JOB: YE-PROF-TERM \((\d+)\) COMPLETED");
            if (matchTermReport.Success)
            {
                // Go grab prof term report.
                string unixProcessId = matchTermReport.Groups[1].Value;
                string qpay066Remote = "/dsmdev/data/PAYROLL/SYS/PVTSYSOUT/QPAY066-" + unixProcessId;
                string qpay066Local = Path.Combine(dataDirectory, "READY-QPAY066.txt");
                await using (var fileStream = File.OpenWrite(qpay066Local))
                {
                    sftpClient.DownloadFile(qpay066Remote, fileStream);
                }

                Console.WriteLine($"copied {qpay066Remote} to $qpay066Local");
                string testingFile2 = OptionalLocalResourceBase+"terminatedEmployeeAndBeneficiaryReport-correct.txt";
                if (HasDirectory(testingFile2))
                {
                    File.Copy(qpay066Local, testingFile2, overwrite: true);
                    Console.WriteLine($"NOTE::: Updated {testingFile2}");
                }
            }

            var matchTermReport2 = Regex.Match(result.Result.Trim(), @" JOB: YE-PROF-EDIT \((\d+)\) COMPLETED");
            if (matchTermReport2.Success)
            {
                // Go grab prof term report.
                string unixProcessId = matchTermReport2.Groups[1].Value;
                string remote = "/dsmdev/data/PAYROLL/SYS/PVTSYSOUT/PAY447-" + unixProcessId;
                string local = Path.Combine(dataDirectory, "READY-PAY447.txt");
                await using (var fileStream = File.OpenWrite(local))
                {
                    sftpClient.DownloadFile(remote, fileStream);
                }

                Console.WriteLine($"copied {remote} to {local}");

                string testingFile3 = OptionalLocalResourceBase + "pay447.txt";
                if (HasDirectory(testingFile3))
                {
                    File.Copy(local, testingFile3, overwrite: true);
                    Console.WriteLine($"NOTE::: Updated {testingFile3}");
                }
            }

            // #######  LP WAS CALLED WITH ARGS :-d plaser5 /dsmdev/data/PAYROLL/SYS/PVTSYSOUT/PAY444A-25103 
            var matches = Regex.Matches(result.Result, @"#######  LP WAS CALLED WITH ARGS :-d \w+ (/.+?-\d+)");
            foreach (Match lpMatch in matches)
            {
                string reportName = lpMatch.Groups[1].Value;
                string filenameLocal = Path.GetFileNameWithoutExtension(reportName).Split('-')[0];
                string qpay066Local = Path.Combine(dataDirectory, $"{filenameLocal}.txt");
                if (!sftpClient.Exists(reportName))
                {
                    Console.WriteLine($"Odd, but remote file does not exist: {reportName}");
                }
                else
                {
                    Console.WriteLine($"copying {reportName} to file:///{qpay066Local}");
                    await using (var fileStream = File.OpenWrite(qpay066Local))
                    {
                        sftpClient.DownloadFile(reportName, fileStream);
                    }
                }

                string testingFile = OptionalLocalResourceBase + "psupdate-pay444-r2.txt";
                if (HasDirectory(testingFile) && filenameLocal == "PAY444L" && File.Exists(testingFile))
                {
                    File.Copy(qpay066Local, testingFile, overwrite: true);
                    Console.WriteLine($"NOTE::: Updated {testingFile}");
                }
            }
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

    private static bool HasDirectory(string testingFile2)
    {
        string directory = Path.GetDirectoryName(testingFile2)!;
        return Directory.Exists(directory);
    }
}
