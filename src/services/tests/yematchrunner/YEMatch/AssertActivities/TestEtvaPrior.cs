using Oracle.ManagedDataAccess.Client;

namespace YEMatch.YEMatch.AssertActivities;

public class TestEtvaPrior : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        OracleConnection connection = new(SmartConnString);
        await connection.OpenAsync();

        string queryA = """
                        SELECT d.id AS demographic_id, pp.Etva
                        FROM profit_detail pd
                        JOIN demographic d ON pd.ssn = d.ssn
                        JOIN pay_profit pp ON d.id = pp.demographic_id
                        WHERE pd.profit_code_id = 8
                          AND pd.profit_year = 2024
                          AND pd.comment_type_id = 23
                          AND pp.profit_year = 2025
                        """;

        string queryB = """
                        SELECT d.id AS demographic_id, pp.Etva
                        FROM profit_detail pd
                        JOIN demographic d ON pd.ssn = d.ssn
                        JOIN pay_profit pp ON d.id = pp.demographic_id
                        WHERE pd.profit_code_id = 8
                          AND pd.profit_year = 2024
                          AND pd.comment_type_id = 23
                          AND pp.profit_year = 2025
                        """;

        OracleCommand command = new(QueryDiffCount(queryA, queryB), connection);
        OracleDataReader? reader = await command.ExecuteReaderAsync();

        int data = 0;
        while (await reader.ReadAsync())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                object value = await reader.IsDBNullAsync(i) ? "NULL" : reader.GetValue(i);
                Console.Write($"{columnName} = {value}; ");
            }

            data++;
            Console.WriteLine();
        }

        Console.WriteLine($"Compare data count {data}");

        if (data > 2)
        {
            return new Outcome(Name(), Name(), "", OutcomeStatus.Error, "Too many bad rows", null, false);
        }

        return new Outcome(Name(), Name(), "", OutcomeStatus.Ok, "", null, false);
    }
}
