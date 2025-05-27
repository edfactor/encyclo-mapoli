using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace YEMatch;

// The base class for activities which directly interact with the database
public abstract class BaseSqlActivity : IActivity
{
    protected readonly string ReadyConnString;
    protected readonly string SmartConnString;


    protected BaseSqlActivity()
    {
        IConfigurationRoot secretConfig = new ConfigurationBuilder().AddUserSecrets<ReadyActivity>().Build();
        ReadyConnString = secretConfig["ReadyConnectionString"]!;
        SmartConnString = secretConfig["SmartConnectionString"]!;
    }

    public virtual string Name()
    {
        return GetType().Name;
    }

    public abstract Task<Outcome> Execute();

    // Compares two sql statements by subtracting the resuls from each results.  This yields the differences.
    protected static string QueryDiff(string queryA, string queryB)
    {
        return $"select count(*) from ( {queryA} minus {queryB} union all {queryB} minus {queryA} )";
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
