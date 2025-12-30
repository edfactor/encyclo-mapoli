namespace Demoulas.ProfitSharing.OracleHcm.Configuration;

/// <summary>
/// Represents the configuration settings for the Oracle HCM integration.
/// Supports two authentication methods:
/// 1. Basic Authentication: Using Username and Password properties
/// 2. Certificate-Based Authentication: Using PFX certificate with optional password
///
/// Provides properties for enabling sync operations, API endpoints, credentials, and sync parameters.
/// </summary>
/// <remarks>
/// For certificate-based authentication (recommended for production):
/// - Set PfxFilePath to the path of the certificate file
/// - Set PfxPassword to the certificate password (optional if not password-protected)
/// - Leave Username and Password empty (or they will be ignored)
///
/// For basic authentication (development only):
/// - Set Username and Password
/// - Leave PfxFilePath empty
///
/// Authentication method is determined automatically based on configuration presence.
/// </remarks>
public sealed record OracleHcmConfig
{
    public bool EnableSync { get; set; } = true;
    public required string BaseAddress { get; set; }
    public required string DemographicUrl { get; set; }
    public string? PayrollUrl { get; set; }

    // Basic Authentication (Legacy - for development only)
    public bool UseBasicAuth { get; set; } = true;
    public string? Username { get; set; }
    public string? Password { get; set; }

    // Certificate-Based Authentication (Recommended for production)
    /// <summary>
    /// Gets or sets the file path to the PFX (PKCS#12) certificate file.
    /// When set, certificate-based authentication will be used instead of basic auth.
    /// Supports both absolute paths and environment-relative paths.
    /// </summary>
    public string? PfxFilePath { get; set; }

    /// <summary>
    /// Gets or sets the password for the PFX certificate file.
    /// Only required if the certificate is password-protected.
    /// Must be stored securely (e.g., .NET User Secrets or Azure Key Vault in production).
    /// </summary>
    public string? PfxPassword { get; set; }

    public string RestFrameworkVersion { get; set; } = "9";
    public ushort Limit { get; set; } = byte.MaxValue;
    public short IntervalInHours { get; set; } = byte.MaxValue;
    public byte PayrollIntervalInHours { get; set; } = 150;

    /// <summary>
    /// Gets or sets the interval in minutes for synchronizing delta changes in employee data.
    /// This property is used to configure the frequency of delta synchronization operations
    /// within the Oracle HCM integration.
    /// </summary>
    public byte DeltaIntervalInMinutes { get; set; } = 15;

    /// <summary>
    /// Determines whether certificate-based authentication should be used.
    /// Returns true if PfxFilePath is set and not empty, false otherwise.
    /// </summary>
    public bool UseCertificateAuthentication => !string.IsNullOrWhiteSpace(PfxFilePath);

    // JWT Token Configuration
    /// <summary>
    /// Gets or sets the principal (username) claim for JWT tokens.
    /// IMPORTANT: This must be an actual Oracle HCM user account that has the "Fusion Applications Integration" privilege.
    /// This value is NOT found in the certificate - you must ask your Oracle HCM administrator for the correct username.
    /// The example "fusion" is a common default, but your environment may use a different user.
    /// Default: "fusion" (VERIFY THIS WITH YOUR ORACLE HCM ADMIN BEFORE USING IN PRODUCTION)
    /// </summary>
    public string JwtPrincipal { get; set; } = "API_PS";

    /// <summary>
    /// Gets or sets the JWT token expiration time in minutes.
    /// Default: 10 minutes
    /// </summary>
    public int JwtExpirationMinutes { get; set; } = 10;

    /// <summary>
    /// Gets or sets the subject (sub) claim for JWT tokens.
    /// This identifies the principal about which the JWT makes a statement.
    /// Example: "API_PS_PROD"
    /// </summary>
    public string JwtSubject { get; set; } = "API_PS";

    /// <summary>
    /// Gets or sets the audience (aud) claim for JWT tokens.
    /// This identifies the recipients that the JWT is intended for.
    /// Example: "https://eqma-dev3.fa.ocs.oraclecloud.com"
    /// </summary>
    public string JwtAudience { get; set; } = "https://eqma-dev3.fa.ocs.oraclecloud.com/";

    /// <summary>
    /// Gets or sets a value indicating whether to include the JWT ID (jti) claim in JWT tokens.
    /// The jti claim provides a unique identifier for each token instance, useful for token tracking and preventing replay attacks.
    /// Default: false
    /// </summary>
    public bool IncludeJtiClaim { get; set; } = false;
}
