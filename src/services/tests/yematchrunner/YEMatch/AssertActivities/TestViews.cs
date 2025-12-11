using System.Diagnostics.CodeAnalysis;
using Oracle.ManagedDataAccess.Client;

namespace YEMatch.AssertActivities;

[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
[SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed")]
public class TestViews : BaseSqlActivity
{
    private const string ViewDirectory = "/Users/robertherrmann/prj/smart-profit-sharing/src/database/SQL calculations view/";
    private OracleConnection? _readyConnection;
    private OracleConnection? _smartConnection;


    public override async Task<Outcome> Execute()
    {
        _readyConnection = new OracleConnection(ReadyConnString);
        await _readyConnection.OpenAsync();
        _smartConnection = new OracleConnection(SmartConnString);
        await _smartConnection.OpenAsync();

        await LoadViews();

        await CompareViews("transaction_totals");
        await CompareViews("vested_percent");
        await CompareViews("pscalcview2");


        return new Outcome("TestViews", "test", "", OutcomeStatus.Ok, "", null, false);
    }

    private async Task CompareViews(string viewName)
    {
        string smartSchema = GetUserName(SmartConnString);

        string queryA = $"""
                         SELECT * from {viewName} 
                         """;

        string queryB = $"""
                         SELECT * from {smartSchema}.{viewName} 
                         """;

        OracleCommand command = new(QueryDiffCount(queryA, queryB), _readyConnection);
        int count = Convert.ToInt32(await command.ExecuteScalarAsync());
        Console.WriteLine($"Difference count on {viewName} is {count}");

        if (count > -1)
        {
            string sql = QueryDiffRows("ready", "smart", queryA, queryB, "ssn");
            Console.WriteLine($"This query can dig into why.\n{sql}");
        }
    }

    private async Task LoadViews()
    {
        await LoadViews(false, "transaction_totals");
        await LoadViews(false, "vested_percent");
        await LoadViews(false, "pscalcview2");

        await LoadViews(true, "transaction_totals");
        await LoadViews(true, "vested_percent");
        await LoadViews(true, "pscalcview2");
    }

    private async Task LoadViews(bool smart, string viewName)
    {
        string viewFileName = $"{(smart ? "SMART" : "READY")} {viewName} view.sql";
        Console.WriteLine($"Loading view {viewFileName} ");
        string viewText = await File.ReadAllTextAsync($"{ViewDirectory}{viewFileName}");

        OracleCommand command = new(viewText, smart ? _smartConnection : _readyConnection);
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
    }
}
