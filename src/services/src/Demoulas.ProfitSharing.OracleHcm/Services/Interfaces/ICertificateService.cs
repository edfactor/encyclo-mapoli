using System.Security.Cryptography.X509Certificates;

namespace Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;

/// <summary>
/// Defines the interface for certificate management and loading for Oracle HCM authentication.
/// Handles loading PFX certificates from multiple sources (file paths, streams, or byte arrays)
/// with password protection and caching for efficient reuse.
/// </summary>
public interface IOracleHcmCertificateService
{
    /// <summary>
    /// Loads and returns an X509Certificate2 from a PFX file.
    /// The certificate is cached after the first load to avoid repeated file I/O.
    /// </summary>
    /// <param name="pfxFilePath">The file path to the PFX certificate file.</param>
    /// <param name="password">The password for the PFX file (can be null or empty if not password-protected).</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>The loaded X509Certificate2 instance.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the PFX file is not found at the specified path.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the certificate cannot be loaded (e.g., invalid format or wrong password).</exception>
    Task<X509Certificate2> GetCertificateAsync(string pfxFilePath, string? password, CancellationToken ct = default);

    /// <summary>
    /// Loads and returns an X509Certificate2 from a FileStream containing PFX data.
    /// Useful for loading certificates from alternative sources (databases, network streams, etc.).
    /// Caching uses the stream's source identifier if available.
    /// </summary>
    /// <param name="pfxStream">A FileStream or Stream containing the PFX certificate data.</param>
    /// <param name="password">The password for the PFX file (can be null or empty if not password-protected).</param>
    /// <param name="sourceIdentifier">Optional identifier for caching purposes (e.g., "database:cert-1", "vault:prod-cert"). If null, caching is bypassed.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>The loaded X509Certificate2 instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the certificate cannot be loaded (e.g., invalid format or wrong password).</exception>
    /// <exception cref="ArgumentNullException">Thrown if the stream is null.</exception>
    Task<X509Certificate2> GetCertificateFromStreamAsync(Stream pfxStream, string? password, string? sourceIdentifier = null, CancellationToken ct = default);

    /// <summary>
    /// Loads and returns an X509Certificate2 from a byte array containing PFX data.
    /// Useful for loading certificates from configuration, environment variables, secrets managers, etc.
    /// Caching uses the provided source identifier.
    /// </summary>
    /// <param name="pfxData">A byte array containing the PFX certificate data.</param>
    /// <param name="password">The password for the PFX file (can be null or empty if not password-protected).</param>
    /// <param name="sourceIdentifier">Optional identifier for caching purposes (e.g., "env:CERT_VAR", "vault:secret-name"). If null, caching is bypassed.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>The loaded X509Certificate2 instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the certificate cannot be loaded (e.g., invalid format or wrong password).</exception>
    /// <exception cref="ArgumentNullException">Thrown if the byte array is null or empty.</exception>
    Task<X509Certificate2> GetCertificateFromBytesAsync(byte[] pfxData, string? password, string? sourceIdentifier = null, CancellationToken ct = default);

    /// <summary>
    /// Validates that a certificate file exists and is accessible.
    /// Useful for health checks and startup validation.
    /// </summary>
    /// <param name="pfxFilePath">The file path to validate.</param>
    /// <returns>True if the file exists and is accessible, false otherwise.</returns>
    bool ValidateCertificateFile(string pfxFilePath);

    /// <summary>
    /// Clears the certificate cache for a specific source.
    /// Useful when certificates are rotated or updated.
    /// </summary>
    /// <param name="sourceIdentifier">The identifier used when loading the certificate. For file paths, this is the normalized absolute path.</param>
    /// <returns>True if a cached certificate was removed, false if no entry found.</returns>
    bool ClearCacheEntry(string sourceIdentifier);

    /// <summary>
    /// Clears all cached certificates.
    /// Should be called during certificate rotation or application shutdown.
    /// </summary>
    void ClearAllCache();
}
