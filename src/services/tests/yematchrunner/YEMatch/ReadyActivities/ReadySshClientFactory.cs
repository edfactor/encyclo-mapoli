using Microsoft.Extensions.Options;
using Renci.SshNet;
using YEMatch.Activities;

namespace YEMatch.ReadyActivities;

public sealed class ReadySshClientFactory : IReadySshClientFactory
{
    private readonly string _host;
    private readonly YeMatchOptions _options;
    private readonly string _password;
    private readonly string _username;
    private bool _disposed;
    private SftpClient? _sftpClient;
    private SshClient? _sshClient;

    public ReadySshClientFactory(IOptions<YeMatchOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
        _host = _options.ReadyHost.Host;

        _username = _options.ReadyHost.Username
                    ?? throw new InvalidOperationException("READY username is required. Set YeMatch:ReadyHost:Username in user secrets.");

        _password = _options.ReadyHost.Password
                    ?? throw new InvalidOperationException("READY password is required. Set YeMatch:ReadyHost:Password in user secrets.");
    }

    public string SchemaName => _options.ReadySchemaName;

    public SshClient GetSshClient()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_sshClient is null)
        {
            _sshClient = new SshClient(_host, _username, _password) { ConnectionInfo = { Timeout = TimeSpan.FromSeconds(_options.ReadyHost.ConnectionTimeoutSeconds) } };
            _sshClient.Connect();
        }

        return _sshClient;
    }

    public SftpClient GetSftpClient()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_sftpClient is null)
        {
            _sftpClient = new SftpClient(_host, _username, _password) { ConnectionInfo = { Timeout = TimeSpan.FromSeconds(_options.ReadyHost.ConnectionTimeoutSeconds) } };
            _sftpClient.Connect();
        }

        return _sftpClient;
    }

    public List<IActivity> CreateActivities(string dataDirectory)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        SshClient ssh = GetSshClient();
        SftpClient sftp = GetSftpClient();

        YearEndDatesOptions dates = _options.YearEndDates;
        string schema = _options.ReadySchemaName;

        List<IActivity> activities =
        [
            // A0 creates new database by importing from PROFITSHARE schema
            new ReadyActivity(ssh, sftp, "A0", "PROFSHARE-BUILD-READY", "", dataDirectory, schema),

            // Clean Up
            new ReadyActivity(ssh, sftp, "A1", "PROFSHARE-SSN-CLEANUP-RPTS", "", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A2", "TERM-REHIRE", $"BDATE={dates.FirstSaturday} EDATE={dates.LastSaturday}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A3", "PROF-TERM", $"SDATE={dates.PriorProfitYearCalendarStart} EDATE={dates.PriorProfitYearCalendarEnd} YDATE={dates.ProfitYear}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A4", "QRY-PSLOAN", $"YR={dates.ProfitYear}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A5", "PROF-DOLLAR-EXEC-EXTRACT", $"YEAR={dates.ProfitYear}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A6", "PAYPROFIT-CLEAR-EXEC", "", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A7", "!Ready-Screen-008-09", "", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A8", "PROF-SHARE", $"sw[3]=1 CDATE={dates.CutOffSaturday} SUMREP=Y YEAREND=N", dataDirectory, schema),

            // Frozen
            new ReadyActivity(ssh, sftp, "A9", "!YE-Oracle-Payroll-Processing", "", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A10", "!Load-Oracle-PAYPROFIT(weekly job)", "", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A11", "PROF-DOLLAR-EXTRACT", "", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A12", "PROF-LOAD-YREND-DEMO-PROFSHARE", $"YEAR={dates.ProfitYear}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A13A", "PAYPROFIT-SHIFT", "MODE=U TYPE=P", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A13B", "PAYPROFIT-SHIFT", "MODE=U TYPE=W", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A14", "ZERO-PY-PD-PAYPROFIT", "", dataDirectory, schema),

            // Fiscal Close
            new ReadyActivity(ssh, sftp, "A15", "PROF-DOLLAR-EXTRACT", "", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A16", "!READY-Screen-008-09", "", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A17", "PROF-SHARE", $"sw[2]=1 sw[3]=1 CDATE={dates.LastSaturday} YEAREND=Y SUMREP=Y", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A18", "PROF-SHARE", $"sw[2]=1 CDATE={dates.LastSaturday} YEAREND=Y", dataDirectory, schema),

            new ReadyActivity(ssh, sftp, "A19", "GET-ELIGIBLE-EMPS", $" CDATE={dates.FirstSaturday}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A20", "PROF-FORT", $"YEAR={dates.ProfitYear}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A21", "PROF-UPD1", $"YEAR={dates.ProfitYear}.0", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A22", "PROF-EDIT", $"YEAR={dates.ProfitYear}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A23", "PROF-DBUPD", $"YEAR={dates.ProfitYear}.0", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A24", "PROF-UPD2", $"sw[6]=1 sw[8]=1 YEAR={dates.ProfitYear}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A24B", "PROF-UPD2", $"YEAR={dates.ProfitYear}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A25", "PROFSHARE-RPT", $"CDATE=20{dates.LastSaturday}", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A26", "PROFGROSS", $"YDATE={dates.ProfitYear} GROSS=50000", dataDirectory, schema),

            // Post-Frozen
            new ReadyActivity(ssh, sftp, "A27", "PROF-BREAK", $"FILEONLY=N FRSTDATE=20231231 LASTDATE=20{dates.LastSaturday} YDATE={dates.ProfitYear} LBLOPT=NO",
                dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A28", "PROF-CERT01", $"YDATE={dates.ProfitYear} LASTDATE=20{dates.LastSaturday} FRSTDATE=20231231", dataDirectory, schema),
            new ReadyActivity(ssh, sftp, "A29", "SAVE-PROF-PAYMSTR", $" YEAR={dates.ProfitYear}", dataDirectory, schema)
        ];

        return activities;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _sshClient?.Disconnect();
        _sshClient?.Dispose();

        _sftpClient?.Disconnect();
        _sftpClient?.Dispose();

        _disposed = true;
    }
}
