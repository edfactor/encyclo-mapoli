using System.Diagnostics.CodeAnalysis;
using Renci.SshNet;
using YEMatch.ReadyActivities;

namespace YEMatch;

// Tries to parse the outcome.json and get all the remote files.    Used for creating the Golden Master
[SuppressMessage("Minor Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions")]
public static class GetGold
{
    /* delete all the golden files.   Ensures we dont mix a prior run with a new run. */

    public static void Purge()
    {
        string goldenDir = $"{ReadyActivity.OptionalLocalResourceBase}golden";
        foreach (string file in Directory.GetFiles(goldenDir))
        {
            if (file.EndsWith("README.md"))
            {
                continue;
            }

            File.Delete(file);
        }
    }

    public static void Fetch(SftpClient sftpClient, string readyName, string smartName)
    {
        string readyFile = "/dsmdev/data/PAYROLL/SYS/PVTSYSOUT/" + readyName;
        string goldenDir = $"{ReadyActivity.OptionalLocalResourceBase}golden";
        string smartFile = $"{goldenDir}/{smartName}";
        File.Delete(smartFile);
        using (FileStream fileStream = File.OpenWrite(smartFile))
        {
            sftpClient.DownloadFile(readyFile, fileStream);
        }
    }
}
