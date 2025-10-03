using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.TotalSvc;

// Load reference PROFITSHARE data for comparison
public static class ReadyPayProfitLoader
{
    public static async Task<Dictionary<int, PayProfitData>> GetReadyPayProfitByBadge(string connectionString)
    {
        OracleConnection connection = new(connectionString);
        await connection.OpenAsync();

        string query = "select payprof_badge, PAYPROF_SSN, PY_PS_YEARS, PY_PS_AMT, PY_PS_VAMT, PY_PS_ETVA from PROFITSHARE.PAYPROFIT";
        // string query = "select payprof_badge, PAYPROF_SSN, PY_PS_YEARS, PY_PS_AMT, PY_PS_VAMT, PY_PS_ETVA from tbherrmann.PAYPROFIT"

        Dictionary<int, PayProfitData> data = new();
        OracleCommand command = new(query, connection);

        OracleDataReader? reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int badge = reader.GetInt32(0);
            PayProfitData pp = new()
            {
                BadgeNumber = reader.GetInt32(0),
                Ssn = reader.GetInt32(1),
                Years = reader.GetInt32(2),
                Amount = reader.GetDecimal(3),
                VestedAmount = reader.GetDecimal(4),
                Etva = reader.GetDecimal(5)
            };
            data.Add(badge, pp);
        }

        return data;
    }
}
