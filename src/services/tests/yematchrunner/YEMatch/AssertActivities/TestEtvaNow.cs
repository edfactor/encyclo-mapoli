using Oracle.ManagedDataAccess.Client;

namespace YEMatch.AssertActivities;

public class TestEtvaNow : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        OracleConnection connection = new(ReadyConnString);
        string smartSchema = GetUserName(SmartConnString);
        await connection.OpenAsync();

        string queryA = """
                        SELECT
                        payprof_badge,
                        py_ps_etva
                        FROM
                        payprofit
                        """;

        string queryB = $"""
                         SELECT
                         badge_number,
                         etva
                         FROM
                              {smartSchema}.pay_profit pp
                         JOIN {smartSchema}.demographic d ON d.id = pp.demographic_id
                         WHERE
                         pp.profit_year = 2025
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
