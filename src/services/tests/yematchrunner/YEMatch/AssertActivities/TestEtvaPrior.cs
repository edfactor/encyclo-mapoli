using Oracle.ManagedDataAccess.Client;

namespace YEMatch.AssertActivities;

public class TestEtvaPrior : BaseSqlActivity
{
    public override async Task<Outcome> Execute()
    {
        OracleConnection connection = new(SmartConnString);
        await connection.OpenAsync();

        string queryA = $"""
                        SELECT d.id AS demographic_id, pp.Etva
                        FROM profit_detail pd
                        JOIN demographic d ON pd.ssn = d.ssn
                        JOIN pay_profit pp ON d.id = pp.demographic_id
                        WHERE pd.profit_code_id = 8
                          AND pd.profit_year = {TestConstants.OpenProfitYear - 1}
                          AND pd.comment_type_id = 23
                          AND pp.profit_year = {TestConstants.OpenProfitYear}
                        """;

        string queryB = $"""
                        SELECT d.id AS demographic_id, pp.Etva
                        FROM profit_detail pd
                        JOIN demographic d ON pd.ssn = d.ssn
                        JOIN pay_profit pp ON d.id = pp.demographic_id
                        WHERE pd.profit_code_id = 8
                          AND pd.profit_year = {TestConstants.OpenProfitYear - 1}
                          AND pd.comment_type_id = 23
                          AND pp.profit_year = {TestConstants.OpenProfitYear}
                        """;

        OracleCommand command = new(QueryDiffCount(queryA, queryB), connection);
        OracleDataReader? countReader = await command.ExecuteReaderAsync();

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
