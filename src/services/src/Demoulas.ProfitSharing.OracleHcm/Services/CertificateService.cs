using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Manages loading and caching of X509 certificates for Oracle HCM authentication.
/// Provides secure certificate loading from multiple sources (files, streams, byte arrays)
/// with password protection and intelligent caching for efficient reuse.
/// </summary>
/// <remarks>
/// This service caches loaded certificates to avoid repeated file I/O and parsing overhead.
/// Certificates are stored in-memory for the lifetime of the application.
/// For highly sensitive scenarios, consider implementing additional key protection mechanisms.
/// 
/// Loading sources:
/// - File paths: Cached by normalized absolute path
/// - Streams: Cached by SHA256 hash of content (requires sourceIdentifier)
/// - Byte arrays: Cached by SHA256 hash of content (requires sourceIdentifier)
/// 
/// Oracle HCM Certificate Authentication Reference:
/// https://www.oracle.com/webfolder/technetwork/tutorials/obe/fusionapps/HCM/JWT_API_Authentication_OBE/html/index.html
/// 
/// Certificate Lifecycle:
/// - Expired: Logged as CRITICAL - authentication will fail immediately
/// - Expiring within 30 days: Logged as ERROR - renewal required
/// - Expiring within 90 days: Logged as WARNING - plan renewal soon
/// - Not yet valid: Logged as CRITICAL - authentication will fail immediately
/// </remarks>
public sealed class OracleHcmCertificateService : IOracleHcmCertificateService, IDisposable
{
    private readonly ILogger<OracleHcmCertificateService> _logger;
    private readonly Dictionary<string, X509Certificate2> _certificateCache = new();
    private readonly ReaderWriterLockSlim _cacheLock = new();

    public OracleHcmCertificateService(ILogger<OracleHcmCertificateService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public X509Certificate2 GetCertificate(string pfxFilePath, string? password)
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
        X509Certificate2 certificate = LoadCertificateFromFile(absolutePath, password);

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
    public X509Certificate2 GetCertificateFromStream(Stream pfxStream, string? password, string? sourceIdentifier = null)
    {
        ArgumentNullException.ThrowIfNull(pfxStream);

        // If no sourceIdentifier, bypass caching
        string cacheKey = sourceIdentifier ?? string.Empty;

        if (!string.IsNullOrEmpty(cacheKey))
        {
            _cacheLock.EnterReadLock();
            try
            {
                if (_certificateCache.TryGetValue(cacheKey, out var cachedCert))
                {
                    _logger.LogDebug("Retrieved certificate from cache: {SourceIdentifier}", cacheKey);
                    return cachedCert;
                }
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        // Certificate not in cache (or no caching requested), load it
        X509Certificate2 certificate = LoadCertificateFromStream(pfxStream, password, sourceIdentifier);

        // Only cache if sourceIdentifier is provided
        if (!string.IsNullOrEmpty(cacheKey))
        {
            _cacheLock.EnterWriteLock();
            try
            {
                // Double-check pattern
                if (_certificateCache.TryGetValue(cacheKey, out var existingCert))
                {
                    _logger.LogDebug("Another thread loaded the certificate while we were loading. Using existing.");
                    certificate.Dispose();
                    return existingCert;
                }

                _certificateCache[cacheKey] = certificate;
                _logger.LogInformation("Certificate loaded and cached from stream: {SourceIdentifier}", cacheKey);
                return certificate;
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        return certificate;
    }

    /// <inheritdoc />
    public X509Certificate2 GetCertificateFromBytes(byte[] pfxData, string? password, string? sourceIdentifier = null)
    {
        ArgumentNullException.ThrowIfNull(pfxData);
        if (pfxData.Length == 0)
        {
            throw new ArgumentException("PFX data cannot be empty.", nameof(pfxData));
        }

        // If no sourceIdentifier, bypass caching
        string cacheKey = sourceIdentifier ?? string.Empty;

        if (!string.IsNullOrEmpty(cacheKey))
        {
            _cacheLock.EnterReadLock();
            try
            {
                if (_certificateCache.TryGetValue(cacheKey, out var cachedCert))
                {
                    _logger.LogDebug("Retrieved certificate from cache: {SourceIdentifier}", cacheKey);
                    return cachedCert;
                }
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        // Certificate not in cache (or no caching requested), load it
        X509Certificate2 certificate = LoadCertificateFromBytes(pfxData, password, sourceIdentifier);

        // Only cache if sourceIdentifier is provided
        if (!string.IsNullOrEmpty(cacheKey))
        {
            _cacheLock.EnterWriteLock();
            try
            {
                // Double-check pattern
                if (_certificateCache.TryGetValue(cacheKey, out var existingCert))
                {
                    _logger.LogDebug("Another thread loaded the certificate while we were loading. Using existing.");
                    certificate.Dispose();
                    return existingCert;
                }

                _certificateCache[cacheKey] = certificate;
                _logger.LogInformation("Certificate loaded and cached from bytes: {SourceIdentifier}", cacheKey);
                return certificate;
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        return certificate;
    }

    /// <inheritdoc />
    public bool ClearCacheEntry(string sourceIdentifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceIdentifier, nameof(sourceIdentifier));

        _cacheLock.EnterWriteLock();
        try
        {
            if (_certificateCache.TryGetValue(sourceIdentifier, out var cert))
            {
                cert?.Dispose();
                _certificateCache.Remove(sourceIdentifier);
                _logger.LogInformation("Cache entry cleared for: {SourceIdentifier}", sourceIdentifier);
                return true;
            }

            return false;
        }
        finally
        {
            _cacheLock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public void ClearAllCache()
    {
        _cacheLock.EnterWriteLock();
        try
        {
            foreach (var cert in _certificateCache.Values)
            {
                cert?.Dispose();
            }
            _certificateCache.Clear();
            _logger.LogInformation("All certificate cache entries cleared.");
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
    /// <returns>The loaded X509Certificate2.</returns>
    /// <exception cref="FileNotFoundException">If the file is not found.</exception>
    /// <exception cref="InvalidOperationException">If the certificate cannot be loaded.</exception>
    private X509Certificate2 LoadCertificateFromFile(string absolutePath, string? password)
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

            ValidateAndLogCertificate(certificate);
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
    }

    /// <summary>
    /// Loads a certificate from a stream containing PFX data.
    /// </summary>
    /// <param name="pfxStream">Stream containing PFX data.</param>
    /// <param name="password">The certificate password (optional).</param>
    /// <param name="sourceIdentifier">Optional identifier for logging (e.g., "database:cert-1").</param>
    /// <returns>The loaded X509Certificate2.</returns>
    /// <exception cref="InvalidOperationException">If the certificate cannot be loaded.</exception>
    private X509Certificate2 LoadCertificateFromStream(Stream pfxStream, string? password, string? sourceIdentifier)
    {
        try
        {
            // Read all bytes from stream (required for X509CertificateLoader)
            using var memoryStream = new MemoryStream();
            pfxStream.CopyTo(memoryStream);
            byte[] pfxData = memoryStream.ToArray();

            if (pfxData.Length == 0)
            {
                throw new InvalidOperationException("Stream contains no data.");
            }

            // Load certificate using modern API
            X509Certificate2 certificate = X509CertificateLoader.LoadPkcs12(
                pfxData,
                password,
                X509KeyStorageFlags.EphemeralKeySet);

            ValidateAndLogCertificate(certificate, sourceIdentifier);
            return certificate;
        }
        catch (Exception ex)
        {
            var invalidOpEx = new InvalidOperationException(
                $"Failed to load certificate from stream ({sourceIdentifier ?? "no identifier"}). This may indicate the data is corrupted, the password is incorrect, or the data is not a valid PFX certificate.",
                ex);
            _logger.LogCritical(invalidOpEx, "Certificate load failure from stream {SourceIdentifier}. Oracle HCM authentication will fail. Verify the data format and password.", sourceIdentifier ?? "unknown");
            throw invalidOpEx;
        }
    }

    /// <summary>
    /// Loads a certificate from a byte array containing PFX data.
    /// </summary>
    /// <param name="pfxData">Byte array containing PFX data.</param>
    /// <param name="password">The certificate password (optional).</param>
    /// <param name="sourceIdentifier">Optional identifier for logging (e.g., "env:CERT_VAR").</param>
    /// <returns>The loaded X509Certificate2.</returns>
    /// <exception cref="InvalidOperationException">If the certificate cannot be loaded.</exception>
    private X509Certificate2 LoadCertificateFromBytes(byte[] pfxData, string? password, string? sourceIdentifier)
    {
        try
        {
            // Load certificate using modern API
            X509Certificate2 certificate = X509CertificateLoader.LoadPkcs12(
                pfxData,
                password,
                X509KeyStorageFlags.EphemeralKeySet);

            ValidateAndLogCertificate(certificate, sourceIdentifier);
            return certificate;
        }
        catch (Exception ex)
        {
            var invalidOpEx = new InvalidOperationException(
                $"Failed to load certificate from byte array ({sourceIdentifier ?? "no identifier"}). This may indicate the data is corrupted, the password is incorrect, or the data is not a valid PFX certificate.",
                ex);
            _logger.LogCritical(invalidOpEx, "Certificate load failure from bytes {SourceIdentifier}. Oracle HCM authentication will fail. Verify the data format and password.", sourceIdentifier ?? "unknown");
            throw invalidOpEx;
        }
    }

    /// <summary>
    /// Validates certificate expiration status and logs appropriate messages.
    /// </summary>
    /// <param name="certificate">The certificate to validate.</param>
    /// <param name="sourceIdentifier">Optional source identifier for logging.</param>
    private void ValidateAndLogCertificate(X509Certificate2 certificate, string? sourceIdentifier = null)
    {
        string sourceInfo = !string.IsNullOrEmpty(sourceIdentifier) ? $" from {sourceIdentifier}" : string.Empty;

        _logger.LogInformation(
            "Certificate loaded successfully{SourceInfo}. Subject: {Subject}, Thumbprint: {Thumbprint}, Valid From: {NotBefore}, Valid To: {NotAfter}",
            sourceInfo,
            certificate.Subject,
            certificate.Thumbprint,
            certificate.NotBefore,
            certificate.NotAfter);

        // Validate certificate expiration status
        if (certificate.NotAfter < DateTime.UtcNow)
        {
            _logger.LogCritical("Certificate is EXPIRED{SourceInfo}. Valid To: {NotAfter}. Oracle HCM authentication will fail immediately.", sourceInfo, certificate.NotAfter);
        }
        else
        {
            var daysUntilExpiration = (certificate.NotAfter - DateTime.UtcNow).TotalDays;
            if (daysUntilExpiration <= 30)
            {
                _logger.LogError("Certificate will expire in {DaysUntilExpiration} days ({ExpirationDate}){SourceInfo}. Immediate renewal required.", Math.Floor(daysUntilExpiration), certificate.NotAfter, sourceInfo);
            }
            else if (daysUntilExpiration <= 90)
            {
                _logger.LogWarning("Certificate will expire in {DaysUntilExpiration} days ({ExpirationDate}){SourceInfo}. Plan renewal soon.", Math.Floor(daysUntilExpiration), certificate.NotAfter, sourceInfo);
            }
        }

        if (certificate.NotBefore > DateTime.UtcNow)
        {
            _logger.LogCritical("Certificate is NOT YET VALID{SourceInfo}. Valid From: {NotBefore}. Oracle HCM authentication will fail.", sourceInfo, certificate.NotBefore);
        }
    }

    /// <summary>
    /// Masks the file path for logging purposes (shows only filename).
    /// </summary>
    private static string MaskPath(string path) => Path.GetFileName(path);

    /// <summary>
    /// Disposes all cached certificates and releases the cache lock.
    /// Should be called during application shutdown or when the service is no longer needed.
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
