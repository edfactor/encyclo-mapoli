namespace YEMatch;

/// <summary>
///     Configuration options for YEMatch testing framework
/// </summary>
public class YeMatchOptions
{
    public const string SectionName = "YeMatch";

    /// <summary>
    ///     Base directory for test data and logs. Defaults to /tmp/ye
    /// </summary>
    public string BaseDataDirectory { get; set; } = "/tmp/ye";

    /// <summary>
    ///     READY SSH host configuration
    /// </summary>
    public ReadyHostOptions ReadyHost { get; set; } = new();

    /// <summary>
    ///     SMART API configuration
    /// </summary>
    public SmartApiOptions SmartApi { get; set; } = new();

    /// <summary>
    ///     Year-end dates and parameters
    /// </summary>
    public YearEndDatesOptions YearEndDates { get; set; } = new();

    /// <summary>
    ///     Path configurations
    /// </summary>
    public PathOptions Paths { get; set; } = new();

    /// <summary>
    ///     JWT authentication configuration
    /// </summary>
    public JwtOptions Jwt { get; set; } = new();
}

/// <summary>
///     READY SSH host connection settings
/// </summary>
public class ReadyHostOptions
{
    /// <summary>
    ///     SSH hostname
    /// </summary>
    public string Host { get; set; } = "appt07d";

    /// <summary>
    ///     SSH username (stored in user secrets)
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     SSH password (stored in user secrets)
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    ///     Connection timeout in seconds
    /// </summary>
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    ///     Enable verbose SSH output
    /// </summary>
    public bool Chatty { get; set; } = false;
}

/// <summary>
///     SMART API connection settings
/// </summary>
public class SmartApiOptions
{
    /// <summary>
    ///     Base URL for SMART API
    /// </summary>
    public string BaseUrl { get; set; } = "https://localhost:7141";

    /// <summary>
    ///     HTTP client timeout in hours
    /// </summary>
    public int TimeoutHours { get; set; } = 2;
}

/// <summary>
///     Year-end processing dates and parameters
/// </summary>
public class YearEndDatesOptions
{
    /// <summary>
    ///     Profit sharing year
    /// </summary>
    public short ProfitYear { get; set; } = 2025;

    /// <summary>
    ///     First Saturday date (YYMMDD format)
    /// </summary>
    public string FirstSaturday { get; set; } = "250104";

    /// <summary>
    ///     Last Saturday date (YYMMDD format)
    /// </summary>
    public string LastSaturday { get; set; } = "251227";

    /// <summary>
    ///     Cut-off Saturday date (YYMMDD format)
    /// </summary>
    public string CutOffSaturday { get; set; } = "260103";

    /// <summary>
    ///     More than five years ago date (YYMMDD format)
    /// </summary>
    public string MoreThanFiveYears { get; set; } = "181231";
}

/// <summary>
///     File and directory path configurations
/// </summary>
public class PathOptions
{
    /// <summary>
    ///     Path to SMART project root (for accessing Data.Cli)
    /// </summary>
    public string? SmartProjectRoot { get; set; }

    /// <summary>
    ///     Path to integration test resources
    /// </summary>
    public string? IntegrationTestResourcesPath { get; set; }

    /// <summary>
    ///     Gets the full path to the Data.Cli tool
    /// </summary>
    public string GetDataCliPath()
    {
        if (string.IsNullOrEmpty(SmartProjectRoot))
        {
            throw new InvalidOperationException("SmartProjectRoot not configured in appsettings.json");
        }

        return Path.Combine(SmartProjectRoot, "src/services/src/Demoulas.ProfitSharing.Data.Cli");
    }

    /// <summary>
    ///     Gets the full path to integration test resources
    /// </summary>
    public string GetIntegrationTestResourcesPath()
    {
        if (string.IsNullOrEmpty(SmartProjectRoot) || string.IsNullOrEmpty(IntegrationTestResourcesPath))
        {
            throw new InvalidOperationException("SmartProjectRoot and IntegrationTestResourcesPath must be configured");
        }

        return Path.Combine(SmartProjectRoot, IntegrationTestResourcesPath);
    }
}

/// <summary>
///     JWT authentication configuration for SMART API
/// </summary>
public class JwtOptions
{
    /// <summary>
    ///     JWT signing key (stored in user secrets).
    ///     This should match the key used by the SMART API server (typically managed via 'dotnet user-jwts').
    ///     Minimum length: 32 bytes (256 bits) for HS256 algorithm.
    /// </summary>
    public string? SigningKey { get; set; }

    /// <summary>
    ///     JWT issuer. Defaults to "dotnet-user-jwts" to match the standard tool.
    /// </summary>
    public string Issuer { get; set; } = "dotnet-user-jwts";

    /// <summary>
    ///     JWT audience. Should match the application identifier.
    /// </summary>
    public string Audience { get; set; } = "https://localhost:7141";

    /// <summary>
    ///     Token expiration time in seconds. Defaults to 3600 (1 hour).
    /// </summary>
    public int ExpirationSeconds { get; set; } = 3600;
}
