using System.Security.Cryptography.X509Certificates;

namespace Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;

/// <summary>
/// Defines the interface for certificate management and loading for Oracle HCM authentication.
/// Handles loading PFX certificates with password protection and caching for efficient reuse.
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
    /// Validates that a certificate file exists and is accessible.
    /// Useful for health checks and startup validation.
    /// </summary>
    /// <param name="pfxFilePath">The file path to validate.</param>
    /// <returns>True if the file exists and is accessible, false otherwise.</returns>
    bool ValidateCertificateFile(string pfxFilePath);
}
