using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.TotalSvc;

// Load reference PROFITSHARE data for comparison
public static class ReadyPayProfitLoader
{
    public static async Task<Dictionary<int, PayProfitData>> GetReadyPayProfitByBadge(string connectionString)
    {
        OracleConnection connection = new(connectionString);
        await connection.OpenAsync();

        string query = @"
            select p.payprof_badge, p.PAYPROF_SSN, p.PY_PS_YEARS, p.PY_PS_AMT, p.PY_PS_VAMT, p.PY_PS_ENROLLED, d.PY_FREQ, d.PY_TERM_DT, d.PY_TERM
            from PROFITSHARE.PAYPROFIT p
            join PROFITSHARE.DEMOGRAPHICS d on p.payprof_badge = d.DEM_BADGE";

        Dictionary<int, PayProfitData> data = new();
        OracleCommand command = new(query, connection);

        OracleDataReader? reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int badge = reader.GetInt32(0);

            // Parse termination date from YYYYMMDD format
            DateOnly? termDate = null;
            if (!await reader.IsDBNullAsync(7))
            {
                int termDateInt = reader.GetInt32(7);
                if (termDateInt > 0)
                {
                    int year = termDateInt / 10000;
                    int month = (termDateInt / 100) % 100;
                    int day = termDateInt % 100;
                    termDate = new DateOnly(year, month, day);
                }
            }

            // Get termination code
            char? termCode = null;
            if (!await reader.IsDBNullAsync(8))
            {
                string termCodeStr = reader.GetString(8);
                if (!string.IsNullOrWhiteSpace(termCodeStr))
                {
                    termCode = termCodeStr[0];
                }
            }

            PayProfitData pp = new()
            {
                BadgeNumber = reader.GetInt32(0),
                Ssn = reader.GetInt32(1),
                Years = reader.GetInt32(2),
                Amount = reader.GetDecimal(3),
                VestedAmount = reader.GetDecimal(4),
                Enrollment = reader.GetInt32(5),
                Frequency = int.Parse(reader.GetString(6)),
                TerminationDate = termDate,
                TerminationCodeId = termCode
            };
            data.Add(badge, pp);
        }

        return data;
    }

    public static async Task<Dictionary<int, decimal>> GetReadyEtvaByBadge(string connectionString, List<int> badges)
    {
        if (badges == null || badges.Count == 0)
        {
            return new Dictionary<int, decimal>();
        }

        OracleConnection connection = new(connectionString);
        await connection.OpenAsync();

        string badgeList = string.Join(",", badges);
        string query = $@"
            select payprof_badge, PY_PS_ETVA
            from PROFITSHARE.PAYPROFIT
            where payprof_badge IN ({badgeList})";

        Dictionary<int, decimal> etvaByBadge = new();
        OracleCommand command = new(query, connection);
        OracleDataReader? reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int badge = reader.GetInt32(0);
            decimal etva = reader.GetDecimal(1);
            etvaByBadge.Add(badge, etva);
        }

        await connection.CloseAsync();
        return etvaByBadge;
    }
}
