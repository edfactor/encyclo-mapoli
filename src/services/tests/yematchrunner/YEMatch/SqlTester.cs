using Microsoft.Extensions.Configuration;

namespace YEMatch;

// The base class for tests which validate by going directly to sql and doing database comparisons
public abstract class SqlTester : IActivity
{
    protected readonly string ReadyConnString;
    protected readonly string SmartConnString;

    protected SqlTester()
    {
        IConfigurationRoot secretConfig = new ConfigurationBuilder().AddUserSecrets<ReadyActivity>().Build();
        ReadyConnString = secretConfig["ReadyConnectionString"]!;
        SmartConnString = secretConfig["SmartConnectionString"]!;
    }


    public abstract string Name();
    public abstract Task<Outcome> Execute();

    protected static string QueryDiff(string queryA, string queryB)
    {
        return $"select count(*) from ( {queryA} minus {queryB} union all {queryB} minus {queryA} )";
    }

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
