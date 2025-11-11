using Oracle.ManagedDataAccess.Client;

namespace YEMatch.AssertActivities;

public class TestProfitDetailSelectedColumns : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        OracleConnection connection = new(SmartConnString);
        string readySchema = GetUserName(ReadyConnString);
        await connection.OpenAsync();

        string queryA = $"""
                         SELECT
                             pr_det_s_sec_number AS ssn,
                             TO_NUMBER(profit_code),
                             profit_cont,
                             profit_earn,
                             profit_fort,
                             TRIM(profit_cmnt),
                             TRIM(profit_zerocont)
                         FROM
                             {readySchema}.profit_detail
                         WHERE
                                 profit_year = 2024
                             AND profit_code IN ( 0, 8 )
                         """;

        string queryB = """
                        SELECT
                            ssn,
                            profit_code_id,
                            contribution,
                            earnings,
                            forfeiture,
                            TRIM(upper(CAST(remark AS CHAR(16 BYTE)))),
                            TRIM(to_char(zero_contribution_reason_id))
                        FROM
                            profit_detail
                        WHERE
                                profit_year = 2024
                            AND profit_year_iteration = 0
                            AND profit_code_id IN ( 0, 8 )
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
            return new Outcome(Name(), "test", "", OutcomeStatus.Error, "Too many bad rows", null, false);
        }

        return new Outcome(Name(), "test", "", OutcomeStatus.Ok, "", null, false);
    }
}
