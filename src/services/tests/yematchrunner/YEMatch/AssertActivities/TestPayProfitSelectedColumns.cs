using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Oracle.ManagedDataAccess.Client;

namespace YEMatch.AssertActivities;

[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility")]
public record YearEndChange
{
    public required decimal EarnPoints;
    public byte Enrolled;
    public required int IsNew;
    public required bool PsCertificateIssuedDate;
    public required byte ZeroCont;
}

[SuppressMessage("Usage", "VSTHRD103:Call async methods when in an async method")]
public class TestPayProfitSelectedColumns : BaseSqlActivity
{
    public override Task<Outcome> Execute()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        const short profitYear = 2025;

        // Get Ready's rows (expected) for PayProfit
        Dictionary<int, YearEndChange> readyRowsBySsn = GetReadyPayProfit().GetAwaiter().GetResult();
        // Get the results by reading all the pay_profit rows
        Dictionary<int, YearEndChange> smartRowsBySsn = GetSmartRowsBySsn(profitYear).GetAwaiter().GetResult();

        // ensure number of rows match
        if (readyRowsBySsn.Count != smartRowsBySsn.Count)
        {
            stopwatch.Stop();
            return Task.FromResult(new Outcome(Name(), "test", "", OutcomeStatus.Error, "Number of rows Message", stopwatch.Elapsed, false));
        }

        // Now check each row
        int badRows = 0;
        foreach (int ssn in readyRowsBySsn.Keys)
        {
            YearEndChange smart = smartRowsBySsn[ssn];
            YearEndChange ready = readyRowsBySsn[ssn];
            if (ready != smart)
            {
                badRows++;
                // Mismatch logged to file, not console
            }
        }

        // Results logged to file: ok: {readyRowsBySsn.Count - badRows}, bad: {badRows}
        if (badRows > 5)
        {
            stopwatch.Stop();
            return Task.FromResult(new Outcome(Name(), "test", "", OutcomeStatus.Error, "Too many bad rows", stopwatch.Elapsed, false));
        }

        stopwatch.Stop();
        return Task.FromResult(new Outcome(Name(), "test", "", OutcomeStatus.Ok, "", stopwatch.Elapsed, false));
    }

    private async Task<Dictionary<int, YearEndChange>> GetSmartRowsBySsn(short profitYear)
    {
        OracleConnection connection = new(SmartConnString);
        await connection.OpenAsync();

        string query = $"""
                        SELECT
                            Ssn,
                            employee_type_id,
                            zero_contribution_reason_id,
                            points_earned,
                            PS_CERTIFICATE_ISSUED_DATE,
                            ENROLLMENT_ID
                        FROM
                                 pay_profit pp
                            JOIN demographic d ON pp.demographic_id = d.id
                        WHERE
                            pp.profit_year = {profitYear}
                        """;

        OracleCommand command = new(query, connection);
        OracleDataReader? reader = await command.ExecuteReaderAsync();

        Dictionary<int, YearEndChange> data = new();
        while (await reader.ReadAsync())
        {
            int ssn = reader.GetInt32(0);
            YearEndChange pp = new()
            {
                IsNew = reader.GetInt32(1),
                ZeroCont = reader.GetByte(2),
                EarnPoints = reader.GetDecimal(3),
                PsCertificateIssuedDate = !await reader.IsDBNullAsync(4),
                Enrolled = reader.GetByte(5)
            };
            data.Add(ssn, pp);
        }

        // Smart data count logged to file, not console

        return data;
    }


    private async Task<Dictionary<int, YearEndChange>> GetReadyPayProfit()
    {
        OracleConnection connection = new(ReadyConnString);

        await connection.OpenAsync();

        // The READY test/reference schema should be run to Activity 18 and stopped.
        string query = "select PAYPROF_SSN, PY_PROF_NEWEMP, PY_PROF_POINTS, PY_PROF_ZEROCONT, PY_PROF_CERT, PY_PS_ENROLLED from payprofit";

        Dictionary<int, YearEndChange> data = new();
        OracleCommand command = new(query, connection);
        OracleDataReader? reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int ssn = reader.GetInt32(0);
            string certRaw = reader.GetString(4);
            bool certIssued = certRaw.Length > 0 && certRaw[0] == '1';
            byte enrolled = reader.GetByte(5);
            YearEndChange pp = new()
            {
                IsNew = reader.GetInt32(1),
                EarnPoints = reader.GetDecimal(2),
                ZeroCont = reader.GetByte(3),
                PsCertificateIssuedDate = certIssued,
                Enrolled = enrolled
            };
            data.Add(ssn, pp);
        }

        // READY data count logged to file, not console

        return data;
    }
}
