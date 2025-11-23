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
                         pp.profit_year = {TestConstants.OpenProfitYear}
                         """;

        // First get the count
        OracleCommand countCommand = new(QueryDiffCount(queryA, queryB), connection);
        OracleDataReader? countReader = await countCommand.ExecuteReaderAsync();
        await countReader.ReadAsync();
        long differences = countReader.GetInt64(0);
        await countReader.CloseAsync();

        if (differences != 0)
        {
            return new Outcome(Name(), "test", "", OutcomeStatus.Error, "", null, false);
        }

        return new Outcome(Name(), Name(), "", OutcomeStatus.Ok, "", null, false);
    }
}
