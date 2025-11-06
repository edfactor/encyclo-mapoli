using System.Security.Cryptography.X509Certificates;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Manages loading and caching of X509 certificates for Oracle HCM authentication.
/// Provides secure certificate loading with support for password-protected PFX files.
/// </summary>
/// <remarks>
/// This service caches loaded certificates to avoid repeated file I/O and parsing overhead.
/// Certificates are stored in-memory for the lifetime of the application.
/// For highly sensitive scenarios, consider implementing additional key protection mechanisms.
/// </remarks>
public sealed class OracleHcmCertificateService : IOracleHcmCertificateService
{
    private readonly ILogger<OracleHcmCertificateService> _logger;
    private readonly Dictionary<string, X509Certificate2> _certificateCache = new();
    private readonly ReaderWriterLockSlim _cacheLock = new();

    public OracleHcmCertificateService(ILogger<OracleHcmCertificateService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<X509Certificate2> GetCertificateAsync(string pfxFilePath, string? password, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pfxFilePath, nameof(pfxFilePath));

        // Normalize the path (convert to absolute if relative)
        string absolutePath = Path.IsPathRooted(pfxFilePath)
            ? pfxFilePath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pfxFilePath);

        _cacheLock.EnterReadLock();
        try
        {
            if (_certificateCache.TryGetValue(absolutePath, out var cachedCert))
            {
                _logger.LogDebug("Retrieved certificate from cache: {CertificatePath}", MaskPath(absolutePath));
                return cachedCert;
            }
        }
        finally
        {
            _cacheLock.ExitReadLock();
        }

        // Certificate not in cache, load it
        X509Certificate2 certificate = await LoadCertificateAsync(absolutePath, password, ct);

        _cacheLock.EnterWriteLock();
        try
        {
            // Double-check pattern: another thread may have loaded it while we were loading
            if (_certificateCache.TryGetValue(absolutePath, out var existingCert))
            {
                _logger.LogDebug("Another thread loaded the certificate while we were loading. Using existing.");
                certificate.Dispose();
                return existingCert;
            }

            _certificateCache[absolutePath] = certificate;
            _logger.LogInformation("Certificate loaded and cached: {CertificatePath}", MaskPath(absolutePath));
            return certificate;
        }
        finally
        {
            _cacheLock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public bool ValidateCertificateFile(string pfxFilePath)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(pfxFilePath, nameof(pfxFilePath));

            string absolutePath = Path.IsPathRooted(pfxFilePath)
                ? pfxFilePath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pfxFilePath);

            if (!File.Exists(absolutePath))
            {
                _logger.LogWarning("Certificate file does not exist: {CertificatePath}", MaskPath(absolutePath));
                return false;
            }

            // Check if file is readable
            using (var fs = File.OpenRead(absolutePath))
            {
                var buffer = new byte[1];
                _ = fs.Read(buffer, 0, 1);
            }

            _logger.LogInformation("Certificate file validated: {CertificatePath}", MaskPath(absolutePath));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating certificate file: {CertificatePath}", MaskPath(pfxFilePath));
            return false;
        }
    }

    /// <summary>
    /// Loads a certificate from the specified PFX file using the modern X509CertificateLoader API.
    /// </summary>
    /// <param name="absolutePath">The absolute file path to the PFX file.</param>
    /// <param name="password">The certificate password (optional).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The loaded X509Certificate2.</returns>
    /// <exception cref="FileNotFoundException">If the file is not found.</exception>
    /// <exception cref="InvalidOperationException">If the certificate cannot be loaded.</exception>
    private async Task<X509Certificate2> LoadCertificateAsync(string absolutePath, string? password, CancellationToken ct)
    {
        return await Task.Run(() =>
        {
            try
            {
                if (!File.Exists(absolutePath))
                {
                    var ex = new FileNotFoundException($"Certificate file not found: {absolutePath}", absolutePath);
                    _logger.LogCritical(ex, "Certificate file not found at {CertificatePath}. Oracle HCM authentication cannot proceed.", MaskPath(absolutePath));
                    throw ex;
                }

                // Load certificate using modern API (X509CertificateLoader)
                X509Certificate2 certificate = X509CertificateLoader.LoadPkcs12FromFile(
                    absolutePath,
                    password,
                    X509KeyStorageFlags.EphemeralKeySet);

                _logger.LogInformation(
                    "Certificate loaded successfully. Subject: {Subject}, Thumbprint: {Thumbprint}, Valid From: {NotBefore}, Valid To: {NotAfter}",
                    certificate.Subject,
                    certificate.Thumbprint,
                    certificate.NotBefore,
                    certificate.NotAfter);

                // Validate certificate is not expired
                if (certificate.NotAfter < DateTime.UtcNow)
                {
                    _logger.LogWarning("Certificate is expired. Valid To: {NotAfter}", certificate.NotAfter);
                }

                if (certificate.NotBefore > DateTime.UtcNow)
                {
                    _logger.LogWarning("Certificate is not yet valid. Valid From: {NotBefore}", certificate.NotBefore);
                }

                return certificate;
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogCritical(ex, "Certificate file not found at {CertificatePath}. Oracle HCM authentication will fail. Ensure the certificate file exists at the configured path.", MaskPath(absolutePath));
                throw;
            }
            catch (Exception ex)
            {
                var invalidOpEx = new InvalidOperationException(
                    $"Failed to load certificate from {MaskPath(absolutePath)}. This may indicate the file is corrupted, the password is incorrect, or the file is not a valid PFX certificate.",
                    ex);
                _logger.LogCritical(invalidOpEx, "Certificate load failure at {CertificatePath}. Oracle HCM authentication will fail. Verify the file format, password, and file permissions.", MaskPath(absolutePath));
                throw invalidOpEx;
            }
        }, ct);
    }

    /// <summary>
    /// Masks the file path for logging purposes (shows only filename).
    /// </summary>
    private static string MaskPath(string path) => Path.GetFileName(path);

    /// <summary>
    /// Disposes all cached certificates.
    /// Should be called during application shutdown.
    /// </summary>
    public void Dispose()
    {
        _cacheLock.EnterWriteLock();
        try
        {
            foreach (var cert in _certificateCache.Values)
            {
                cert?.Dispose();
            }
            _certificateCache.Clear();
        }
        finally
        {
            _cacheLock.ExitWriteLock();
        }

        _cacheLock?.Dispose();
    }
}
