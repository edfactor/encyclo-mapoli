using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace YEMatch.AssertActivities;

/// <summary>
/// Compares enrollment values between READY and SMART for profit year 2025.
/// This test validates the EnrollmentSummarizer change that aligns SMART with READY's
/// behavior of skipping enrollment updates for 0% vested employees.
///
/// Expected results BEFORE the fix:
///   | SMART | READY | Count |
///   |-------|-------|-------|
///   | 0     | 0     | 82    |
///   | 1     | 1     | 734   |
///   | 2     | 2     | 7,552 |
///   | 3     | 3     | 11    |
///   | 4     | 4     | 1,256 |
///   | 3     | 1     | 3     | ← Mismatch (SMART updated, READY skipped)
///   | 4     | 2     | 823   | ← Mismatch (SMART updated, READY skipped)
///
/// Expected results AFTER the fix:
///   All enrollments should match (0 mismatches).
/// </summary>
[SuppressMessage("Usage", "VSTHRD103:Call async methods when in an async method")]
public class TestEnrollmentComparison : BaseSqlActivity
{
    private const short ProfitYear = 2025;

    public override Task<Outcome> Execute()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Get enrollment data from both systems
        Dictionary<int, EnrollmentData> readyData = GetReadyEnrollment().GetAwaiter().GetResult();
        Dictionary<int, EnrollmentData> smartData = GetSmartEnrollment().GetAwaiter().GetResult();

        // Build comparison results
        var matchCounts = new Dictionary<string, int>();
        var mismatchCounts = new Dictionary<string, int>();
        var mismatchDetails = new List<(int BadgeNumber, byte SmartEnrollment, byte ReadyEnrollment)>();

        foreach (int badge in readyData.Keys)
        {
            if (!smartData.TryGetValue(badge, out EnrollmentData? smartRow))
            {
                continue; // Badge not in SMART - skip
            }

            EnrollmentData readyRow = readyData[badge];
            string key = $"{smartRow.EnrollmentId}->{readyRow.EnrollmentId}";

            if (smartRow.EnrollmentId == readyRow.EnrollmentId)
            {
                matchCounts[key] = matchCounts.GetValueOrDefault(key, 0) + 1;
            }
            else
            {
                mismatchCounts[key] = mismatchCounts.GetValueOrDefault(key, 0) + 1;
                mismatchDetails.Add((badge, smartRow.EnrollmentId, readyRow.EnrollmentId));
            }
        }

        stopwatch.Stop();

        // Build result message
        StringBuilder message = new();
        message.AppendLine($"Enrollment Comparison for Profit Year {ProfitYear}");
        message.AppendLine();
        message.AppendLine("Matches:");
        foreach (var kvp in matchCounts.OrderBy(k => k.Key))
        {
            message.AppendLine($"  {kvp.Key}: {kvp.Value:N0}");
        }

        int totalMatches = matchCounts.Values.Sum();
        int totalMismatches = mismatchCounts.Values.Sum();

        message.AppendLine();
        message.AppendLine("Mismatches:");
        if (mismatchCounts.Count == 0)
        {
            message.AppendLine("  None - all enrollments match!");
        }
        else
        {
            foreach (var kvp in mismatchCounts.OrderByDescending(k => k.Value))
            {
                message.AppendLine($"  SMART {kvp.Key.Split("->")[0]} vs READY {kvp.Key.Split("->")[1]}: {kvp.Value:N0}");
            }
        }

        message.AppendLine();
        message.AppendLine($"Summary: {totalMatches:N0} match, {totalMismatches:N0} mismatch ({(double)totalMatches / (totalMatches + totalMismatches):P1})");

        // Include sample mismatched badges if any
        if (mismatchDetails.Count > 0)
        {
            message.AppendLine();
            message.AppendLine("Sample mismatched badges (first 10):");
            foreach (var (badge, smart, ready) in mismatchDetails.Take(10))
            {
                message.AppendLine($"  Badge {badge}: SMART={smart}, READY={ready}");
            }
        }

        // Test passes if mismatches are below threshold (expecting 0 after fix)
        OutcomeStatus status = totalMismatches == 0 ? OutcomeStatus.Ok : OutcomeStatus.Error;

        return Task.FromResult(new Outcome(
            Name(),
            "enrollment",
            "",
            status,
            message.ToString(),
            stopwatch.Elapsed,
            false));
    }

    private async Task<Dictionary<int, EnrollmentData>> GetSmartEnrollment()
    {
        await using OracleConnection connection = new(SmartConnString);
        await connection.OpenAsync();

        string query = $"""
                        SELECT
                            d.badge_number,
                            CASE
                            WHEN VESTING_SCHEDULE_ID = 1 AND HAS_FORFEITED = 0 THEN 1
                            WHEN VESTING_SCHEDULE_ID = 2 AND HAS_FORFEITED = 0 THEN 2
                            WHEN VESTING_SCHEDULE_ID = 1 AND HAS_FORFEITED = 1 THEN 3
                            WHEN VESTING_SCHEDULE_ID = 2 AND HAS_FORFEITED = 1 THEN 4
                            ELSE 0
                        END AS enrollment_id
                        FROM pay_profit pp
                        JOIN demographic d ON pp.demographic_id = d.id
                        WHERE pp.profit_year = {ProfitYear}
                        """;

        await using OracleCommand command = new(query, connection);
        await using OracleDataReader reader = await command.ExecuteReaderAsync();

        Dictionary<int, EnrollmentData> data = new();
        while (await reader.ReadAsync())
        {
            int badge = reader.GetInt32(0);
            byte enrollment = reader.GetByte(1);
            data[badge] = new EnrollmentData { EnrollmentId = enrollment };
        }

        return data;
    }

    private async Task<Dictionary<int, EnrollmentData>> GetReadyEnrollment()
    {
        await using OracleConnection connection = new(ReadyConnString);
        await connection.OpenAsync();

        string query = "SELECT PAYPROF_BADGE, PY_PS_ENROLLED FROM payprofit";

        await using OracleCommand command = new(query, connection);
        await using OracleDataReader reader = await command.ExecuteReaderAsync();

        Dictionary<int, EnrollmentData> data = new();
        while (await reader.ReadAsync())
        {
            int badge = reader.GetInt32(0);
            byte enrollment = reader.GetByte(1);
            data[badge] = new EnrollmentData { EnrollmentId = enrollment };
        }

        return data;
    }

    private sealed record EnrollmentData
    {
        public byte EnrollmentId { get; init; }
    }
}
