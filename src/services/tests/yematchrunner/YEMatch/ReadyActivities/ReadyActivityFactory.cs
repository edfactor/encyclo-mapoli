using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using YEMatch.YEMatch.Activities;

namespace YEMatch.YEMatch.ReadyActivities;

public static class ReadyActivityFactory
{
    public static SshClient? sshClient { get; set; }
    public static SftpClient? SftpClient { get; set; }

    public static List<IActivity> CreateActivities(string dataDirectory)
    {
        IConfigurationRoot secretConfig = new ConfigurationBuilder().AddUserSecrets<ReadyActivity>().Build();
        string? username = secretConfig["YEMatchHost:Username"];
        string? password = secretConfig["YEMatchHost:Password"];
        string host = "appt07d"; // was "tduapp01"
        bool chatty = true; // set to true to enable debugging chatter

        if (username == null || password == null)
        {
            throw new InvalidOperationException("Username and password are required");
        }

        sshClient = new(host, username, password);
        sshClient.Connect();

        SftpClient = new SftpClient(host, username, password);
        SftpClient.Connect();

        // Appropriate for December 2025
        var firstSaturday = "250104"; // 240106
        var lastSaturday = "251227"; // 241228
        var cutOffSaturday = "260103";
        var YEAR = "2025"; // {YEAR}

        List<IActivity> activities =
        [
            // A0 creates new datbase by importing from PROFITSHARE schema
            new ReadyActivity(sshClient, SftpClient, chatty, "A0", "PROFSHARE-BUILD-READY", "", dataDirectory),

            // Clean Up
            new ReadyActivity(sshClient, SftpClient, chatty, "A1", "PROFSHARE-SSN-CLEANUP-RPTS", "", dataDirectory),
            // 241228 <-- Saturday of "last week" for 2024.   Week # 52 some year, some years Week 53
            // 251227 <-- Saturday 
            new ReadyActivity(sshClient, SftpClient, chatty, "A2", "TERM-REHIRE", $"BDATE={firstSaturday} EDATE={lastSaturday}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A3", "PROF-TERM", $"SDATE={firstSaturday} EDATE={lastSaturday} YDATE={YEAR}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A4", "QRY-PSLOAN", $"YR={YEAR}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A5", "PROF-DOLLAR-EXEC-EXTRACT", $"YEAR={YEAR}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A6", "PAYPROFIT-CLEAR-EXEC", "", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A7", "!Ready-Screen-008-09", "", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A8", "PROF-SHARE", $"sw[3]=1 CDATE={cutOffSaturday} SUMREP=Y YEAREND=N", dataDirectory),

            // Frozen
            new ReadyActivity(sshClient, SftpClient, chatty, "A9", "!YE-Oracle-Payroll-Processing", "", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A10", "!Load-Oracle-PAYPROFIT(weekly job)", "", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A11", "PROF-DOLLAR-EXTRACT", "", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A12", "PROF-LOAD-YREND-DEMO-PROFSHARE", $"YEAR={YEAR}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A13A", "PAYPROFIT-SHIFT", "MODE=U TYPE=P", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A13B", "PAYPROFIT-SHIFT", "MODE=U TYPE=W", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A14", "ZERO-PY-PD-PAYPROFIT", "", dataDirectory),

            // Fiscal Close
            new ReadyActivity(sshClient, SftpClient, chatty, "A15", "PROF-DOLLAR-EXTRACT", "", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A16", "!READY-Screen-008-09", "", dataDirectory),
            // sw[2]=1 <--- last years exec hours dollars   sw[3]=1 <--- do not update
            new ReadyActivity(sshClient, SftpClient, chatty, "A17", "PROF-SHARE", $"sw[2]=1 sw[3]=1 CDATE={lastSaturday} YEAREND=Y SUMREP=Y", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A18", "PROF-SHARE", $"sw[2]=1 CDATE={lastSaturday} YEAREND=Y", dataDirectory),
            
            new ReadyActivity(sshClient, SftpClient, chatty, "A19", "GET-ELIGIBLE-EMPS", $" CDATE={firstSaturday}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A20", "PROF-FORT", $"YEAR={YEAR}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A21", "PROF-UPD1", $"YEAR={YEAR}.0", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A22", "PROF-EDIT", $"YEAR={YEAR}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A23", "PROF-DBUPD", $"YEAR={YEAR}.0", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A24", "PROF-UPD2", $"sw[6]=1 sw[8]=1 YEAR={YEAR}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A24B", "PROF-UPD2", $"YEAR={YEAR}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A25", "PROFSHARE-RPT", $"CDATE=20{lastSaturday}", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A26", "PROFGROSS", $"YDATE={YEAR} GROSS=50000", dataDirectory),

            // Post-Frozen
            new ReadyActivity(sshClient, SftpClient, chatty, "A27", "PROF-BREAK", "FILEONLY=N FRSTDATE=20231231 LASTDATE=20{lastSaturday} YDATE={YEAR} LBLOPT=NO", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A28", "PROF-CERT01", "YDATE={YEAR} LASTDATE=20{lastSaturday} FRSTDATE=20231231", dataDirectory),
            new ReadyActivity(sshClient, SftpClient, chatty, "A29", "SAVE-PROF-PAYMSTR", " YEAR={YEAR}", dataDirectory)
        ];
        return activities;
    }
}
