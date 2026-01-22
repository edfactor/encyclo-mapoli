using System.Data.Common;
using Demoulas.ProfitSharing.Services.Services.Distributions;
using Demoulas.ProfitSharing.Services.Services.YearEnd;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Oracle.ManagedDataAccess.Client;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

/// <summary>
/// Record for comparing year-end changes between READY and SMART systems.
/// </summary>
public record YearEndChange
{
    public required int IsNew { get; init; }
    public required byte ZeroCont { get; init; }
    public required decimal EarnPoints { get; init; }
    public DateOnly? PsCertificateIssuedDate { get; init; }
}

/*
 * This integration test requires that READY's test/reference schema should be run to "Activity 18", for this test to pass.
 *      YEMatch Specify(["R0", "R18"])
 */

public class YearEndServiceTests : PristineBaseTest
{

    public YearEndServiceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task TestPay426DataUpdates()
    {
        // ------- Arrange
        const short profitYear = 2025;
        CancellationToken ct = CancellationToken.None;

        // Get SMART results within transaction (will rollback to avoid modifying database)
        Dictionary<int, YearEndChange> smartRowsBySsn = await DbFactory.UseWritableContext(async ctx =>
        {
            OracleConnection c = (ctx.Database.GetDbConnection() as OracleConnection)!;
            await c.OpenAsync(ct);

            // ------- Act
            DbTransaction transaction = await c.BeginTransactionAsync(ct);
            await YearEndService.RunFinalYearEndUpdatesAsync(profitYear, false, ct);

            // Read results BEFORE rollback (so changes are visible but not committed)
            // IMPORTANT: Use the same connection/transaction to see uncommitted changes
            var smartResults = await GetSmartRowsBySsn(profitYear, c);

            // Rollback transaction (don't modify database - allows multiple test runs)
            await transaction.RollbackAsync(ct);

            return smartResults;
        }, ct);

        //  ----- Assert
        // Get Ready's rows (expected) for PayProfit
        Dictionary<int, YearEndChange> readyRowsBySsn = await GetReadyPayProfit();

        // ensure number of rows match
        readyRowsBySsn.Count.ShouldBe(smartRowsBySsn.Count);

        // Now check each row
        int badRows = smartRowsBySsn.Count(kvp =>
        {
            var readyValue = readyRowsBySsn.GetValueOrDefault(kvp.Key)!;
            var smartValue = kvp.Value;
            bool mismatch = smartValue != readyValue;
            if (mismatch)
            {
                TestOutputHelper.WriteLine($"Ssn {kvp.Key} r:{readyValue} s:{smartValue}");
            }

            return mismatch;
        });

        TestOutputHelper.WriteLine($"ok: {readyRowsBySsn.Count - badRows}, bad: {badRows}");
        badRows.ShouldBeLessThan(2);
    }


    private async Task<Dictionary<int, YearEndChange>> GetSmartRowsBySsn(short profitYear, OracleConnection connection)
    {
        string query = $"""
                        SELECT
                            Ssn,
                            employee_type_id,
                            zero_contribution_reason_id,
                            points_earned,
                            PS_CERTIFICATE_ISSUED_DATE
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
                PsCertificateIssuedDate = await reader.IsDBNullAsync(4) ? null : reader.GetDateTime(4).ToDateOnly()
            };
            data.Add(ssn, pp);
        }

        TestOutputHelper.WriteLine($"SMART data count {data.Count}");

        return data;
    }

    private async Task<Dictionary<int, YearEndChange>> GetReadyPayProfit()
    {
        OracleConnection connection = new(DbFactory.ConnectionString);

        await connection.OpenAsync();

        // The READY test/reference schema should be run to Activity 18 and stopped.
        string query = "select PAYPROF_SSN, PY_PROF_NEWEMP, PY_PROF_POINTS, PY_PROF_ZEROCONT, PY_PROF_CERT from tbherrmann.payprofit";

        Dictionary<int, YearEndChange> data = new();
        OracleCommand command = new(query, connection);
        OracleDataReader? reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int ssn = reader.GetInt32(0);
            string certRaw = reader.GetString(4);
            bool certIssued = certRaw.Length > 0 && certRaw[0] == '1';
            YearEndChange pp = new()
            {
                IsNew = reader.GetInt32(1),
                EarnPoints = reader.GetDecimal(2),
                ZeroCont = reader.GetByte(3),
                PsCertificateIssuedDate = certIssued ? DateOnly.FromDateTime(DateTime.Now) : null
            };
            data.Add(ssn, pp);
        }

        TestOutputHelper.WriteLine($"READY data count {data.Count}");

        return data;
    }
}
