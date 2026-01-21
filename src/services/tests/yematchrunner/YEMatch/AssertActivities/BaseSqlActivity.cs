using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using YEMatch.ReadyActivities;

namespace YEMatch.AssertActivities;

// The base class for activities which directly interact with the database
public abstract class BaseSqlActivity : BaseActivity
{
    protected readonly string ReadySchemaName;
    protected readonly string ReadyConnString;
    protected readonly string SmartConnString;

    protected BaseSqlActivity()
    {
        // Build configuration from appsettings.json and user secrets
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets<ReadyActivity>()
            .Build();

        // Get the schema name from configuration (defaults to "tbherrmann")
        ReadySchemaName = config["YeMatch:ReadySchemaName"] ?? "tbherrmann";

        // Build connection string keys using the schema name pattern:
        // YeMatch:{schemaName}:ReadyConnectionString and YeMatch:{schemaName}:SmartConnectionString
        string readyConnKey = $"YeMatch:{ReadySchemaName}:ReadyConnectionString";
        string smartConnKey = "SmartConnectionString";

        ReadyConnString = config[readyConnKey]
                          ?? throw new InvalidOperationException(
                              $"Connection string not found for key '{readyConnKey}'. " +
                              $"Please configure it in user secrets: dotnet user-secrets set \"{readyConnKey}\" \"your-connection-string\"");

        SmartConnString = config[smartConnKey]
                          ?? throw new InvalidOperationException(
                              $"Connection string not found for key '{smartConnKey}'. " +
                              $"Please configure it in user secrets: dotnet user-secrets set \"{smartConnKey}\" \"your-connection-string\"");
    }

    // Compares two sql statements by subtracting the resuls from each results.  This yields the differences.
    protected static string QueryDiffCount(string queryA, string queryB)
    {
        return $"select count(*) from ( ({queryA} minus {queryB}) union all ({queryB} minus {queryA}) )";
    }

    protected static string QueryDiffRows(string nameA, string nameB, string sqlA, string sqlB, string orderByColumn)
    {
        return $@"SELECT
    *
FROM
    (
    SELECT '{nameA}' AS src, kk.* FROM (
        {sqlA}
        MINUS
        {sqlB}
    ) kk
    UNION ALL
    SELECT '{nameB}' AS src, jj.* FROM (
        {sqlB}
        MINUS
        {sqlA}
    ) jj
) ORDER BY {orderByColumn}, src";
    }

    protected async Task<int> RdySql(string sql)
    {
        await using OracleConnection conn = new(ReadyConnString);
        await conn.OpenAsync();
        await using OracleCommand cmd = new(sql, conn);
        int rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected;
    }

    protected async Task<int> SmtSql(string sql)
    {
        await using OracleConnection conn = new(SmartConnString);
        await conn.OpenAsync();
        await using OracleCommand cmd = new(sql, conn);
        int rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected;
    }

    // given a connection string, return the schema or username - which are the same in Oracle
    public static string GetUserName(string connStr)
    {
        if (string.IsNullOrWhiteSpace(connStr))
        {
            throw new ArgumentException("Connection string is null or empty.", nameof(connStr));
        }

        string[] parts = connStr.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (trimmed.StartsWith("User Id=", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed.Substring("User Id=".Length).Trim();
            }
        }

        throw new FormatException("User Id not found in connection string.");
    }
}
