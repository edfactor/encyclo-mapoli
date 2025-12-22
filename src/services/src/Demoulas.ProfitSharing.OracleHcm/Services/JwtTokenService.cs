using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Generates JWT tokens signed with an X509 certificate private key for Oracle HCM REST API authentication.
/// </summary>
/// <remarks>
/// JWT tokens are required by Oracle HCM in addition to mutual TLS (mTLS).
/// This service creates RS256/RS384/RS512-signed tokens with the required claims and certificate thumbprints.
/// Configuration is loaded from OracleHcmConfig for principal, algorithm, and expiration.
/// </remarks>
public sealed partial class JwtTokenService : IJwtTokenService
{
    private readonly OracleHcmConfig _config;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly TimeProvider _timeProvider;

    private const string TokenType = "JWT";

    /// <summary>
    /// Cached JSON serializer options for consistent serialization.
    /// </summary>
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
    /// </summary>
    /// <param name="config">The Oracle HCM configuration.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="timeProvider">The time provider for testability. If null, uses <see cref="TimeProvider.System"/>.</param>
    public JwtTokenService(OracleHcmConfig config, ILogger<JwtTokenService> logger, TimeProvider? timeProvider = null)
    {
        _config = config;
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <inheritdoc />
    public string GenerateToken(X509Certificate2 certificate, string issuer, string principal, string subject, string audience, int expirationMinutes = 10)
    {
        ArgumentNullException.ThrowIfNull(certificate);
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(principal);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(audience);

        if (!certificate.HasPrivateKey)
        {
            throw new InvalidOperationException("Certificate does not contain a private key. Cannot sign JWT.");
        }

        // Get the private key for signing
        using var rsa = certificate.GetRSAPrivateKey();
        if (rsa is null)
        {
            throw new InvalidOperationException("Failed to extract RSA private key from certificate.");
        }

        // Extract the signing algorithm from the certificate
        (string signingAlgorithm, HashAlgorithmName hashAlgorithm) = ExtractSigningAlgorithmFromCertificate(certificate);

        // Calculate thumbprints for JWT header
        string x5t = CalculateSha1Thumbprint(certificate);
        string x5tS256 = CalculateSha256Thumbprint(certificate);
        string kid = certificate.Thumbprint; // Use hex thumbprint as Key ID

        // Create header with both x5t (SHA-1) and x5t#S256 (SHA-256) thumbprints
        var header = new Dictionary<string, object>
        {
            ["alg"] = signingAlgorithm,
            ["typ"] = TokenType,
            ["x5t"] = x5t,
            ["x5t#S256"] = x5tS256,
            ["kid"] = kid
        };

        // Create payload with Oracle HCM required claims
        long nowUnix = _timeProvider.GetUtcNow().ToUnixTimeSeconds();
        long expUnix = nowUnix + (expirationMinutes * 60);

        var payload = new Dictionary<string, object>
        {
            ["iss"] = issuer,
            ["sub"] = subject,
            ["aud"] = audience,
            ["iat"] = nowUnix,
            ["exp"] = expUnix,
            ["prn"] = principal
        };

        // Add jti (JWT ID) claim if configured for replay attack prevention
        if (_config.IncludeJtiClaim)
        {
            payload["jti"] = Guid.NewGuid().ToString("N");
        }

        // Encode header and payload as base64url
        string headerJson = JsonSerializer.Serialize(header, s_jsonSerializerOptions);
        string payloadJson = JsonSerializer.Serialize(payload, s_jsonSerializerOptions);

        string headerEncoded = Base64UrlEncode(headerJson);
        string payloadEncoded = Base64UrlEncode(payloadJson);

        // Log JWT components for debugging
        _logger.LogDebug("JWT Token Generation - Header: {HeaderJson}", headerJson);
        _logger.LogDebug("JWT Token Generation - Payload: {PayloadJson}", payloadJson);
        _logger.LogDebug("JWT Token Generation - Algorithm: {Algorithm}, Issuer: {Issuer}, Principal: {Principal}, Expiration (Unix): {Expiration}",
            signingAlgorithm, issuer, principal, expUnix);
        _logger.LogInformation("JWT Token Generated - Certificate Subject: {Subject}, Thumbprint: {Thumbprint}, KeyId: {KeyId}",
            certificate.Subject, certificate.Thumbprint, kid);

        // Create signature
        string signatureInput = $"{headerEncoded}.{payloadEncoded}";
        byte[] signatureInputBytes = Encoding.UTF8.GetBytes(signatureInput);

        byte[] signatureBytes = rsa.SignData(signatureInputBytes, hashAlgorithm, RSASignaturePadding.Pkcs1);
        string signatureEncoded = Base64UrlEncode(signatureBytes);

        // Combine to create JWT
        string jwtToken = $"{signatureInput}.{signatureEncoded}";
        _logger.LogDebug("JWT Token (first 100 chars): {JwtToken}...", jwtToken[..Math.Min(100, jwtToken.Length)]);

        return jwtToken;
    }

    /// <inheritdoc />
    public string GenerateTokenWithDefaults(X509Certificate2 certificate, int expirationMinutes = 10)
    {
        // Extract issuer from certificate subject
        string issuer = ExtractIssuerFromCertificate(certificate);

        // Use configured principal, subject, audience, and expiration (or provided override)
        int tokenExpiration = expirationMinutes == 10 ? _config.JwtExpirationMinutes : expirationMinutes;
        return GenerateToken(certificate, issuer, _config.JwtPrincipal, _config.JwtSubject, _config.JwtAudience, tokenExpiration);
    }

    /// <summary>
    /// Extracts and maps the signing algorithm from the certificate to JWT algorithm format.
    /// The certificate's signature algorithm is converted to the corresponding JWT algorithm identifier
    /// and the matching <see cref="HashAlgorithmName"/> for signing.
    /// </summary>
    /// <returns>A tuple containing the JWT algorithm identifier (e.g., "RS256") and the corresponding <see cref="HashAlgorithmName"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the certificate uses an unsupported algorithm.</exception>
    private (string JwtAlgorithm, HashAlgorithmName HashAlgorithm) ExtractSigningAlgorithmFromCertificate(X509Certificate2 certificate)
    {
        string? certAlgorithm = certificate.SignatureAlgorithm.FriendlyName;

        if (string.IsNullOrEmpty(certAlgorithm))
        {
            throw new InvalidOperationException(
                "Unable to determine certificate signature algorithm. The certificate's algorithm information is missing or invalid.");
        }

        _logger.LogDebug("Certificate signature algorithm detected: {CertificateAlgorithm}", certAlgorithm);

        // Map certificate algorithm to JWT algorithm and HashAlgorithmName
        (string jwtAlgorithm, HashAlgorithmName hashAlgorithm) = certAlgorithm switch
        {
            "sha1RSA" => ("RS256", HashAlgorithmName.SHA256),  // Fallback to SHA256 for signing, but log warning
            "sha256RSA" => ("RS256", HashAlgorithmName.SHA256),
            "sha384RSA" => ("RS384", HashAlgorithmName.SHA384),
            "sha512RSA" => ("RS512", HashAlgorithmName.SHA512),
            _ => throw new InvalidOperationException(
                $"Certificate uses unsupported signing algorithm '{certAlgorithm}'. " +
                $"Supported algorithms are: sha256RSA, sha384RSA, sha512RSA")
        };

        if (certAlgorithm == "sha1RSA")
        {
            _logger.LogWarning("Certificate uses SHA-1 (sha1RSA). While this still works, consider upgrading to SHA-256 or higher for better security.");
        }

        _logger.LogInformation("Using JWT algorithm: {JwtAlgorithm} (from certificate algorithm: {CertificateAlgorithm})",
            jwtAlgorithm, certAlgorithm);

        return (jwtAlgorithm, hashAlgorithm);
    }

    /// <summary>
    /// Calculates the SHA-1 thumbprint of the certificate in base64url format (x5t).
    /// This is required by Oracle HCM in the JWT header.
    /// </summary>
    /// <remarks>
    /// Oracle HCM specifically requires SHA-1 thumbprint for the x5t claim.
    /// While SHA-1 is generally considered weak for signing, it's acceptable here
    /// as a fingerprint/identifier rather than for cryptographic signing.
    /// </remarks>
#pragma warning disable CA5350 // Do not use weak cryptographic algorithms - Oracle HCM requirement for x5t claim
    private static string CalculateSha1Thumbprint(X509Certificate2 certificate)
    {
        byte[] thumbprintBytes = SHA1.HashData(certificate.RawData);
        return Base64UrlEncode(thumbprintBytes);
    }
#pragma warning restore CA5350

    /// <summary>
    /// Calculates the SHA-256 thumbprint of the certificate in base64url format (x5t#S256).
    /// This is a more secure alternative to SHA-1 x5t and is included for modern JWT consumers.
    /// </summary>
    private static string CalculateSha256Thumbprint(X509Certificate2 certificate)
    {
        byte[] thumbprintBytes = SHA256.HashData(certificate.RawData);
        return Base64UrlEncode(thumbprintBytes);
    }

    /// <summary>
    /// Encodes a string to base64url format (no padding, url-safe).
    /// Required for JWT encoding per RFC 7515.
    /// </summary>
    private static string Base64UrlEncode(string input)
    {
        return Base64UrlEncoder.Encode(input);
    }

    /// <summary>
    /// Encodes a byte array to base64url format (no padding, url-safe).
    /// Required for JWT encoding per RFC 7515.
    /// </summary>
    private static string Base64UrlEncode(byte[] input)
    {
        return Base64UrlEncoder.Encode(input);
    }

    /// <summary>
    /// Extracts the issuer identifier from the certificate issuer field (not subject).
    /// The issuer is the Certificate Authority that signed this certificate.
    /// Attempts to extract CN (Common Name) first, then L (Locality), then uses the full issuer as fallback.
    /// </summary>
    private string ExtractIssuerFromCertificate(X509Certificate2 certificate)
    {
        string issuer = certificate.Issuer;

        // Try to extract CN (Common Name) - e.g., "CN = Demoulas Cloud Issuing CA 2"
        Match cnMatch = CnRegex().Match(issuer);
        if (cnMatch.Success)
        {
            string extracted = cnMatch.Groups[1].Value.Trim();
            _logger.LogDebug("Extracted CN from certificate issuer: {ExtractedCn}", extracted);
            return extracted;
        }

        // Fallback to L (Locality) - e.g., "L = Demoulas Supermarkets Inc."
        Match localityMatch = LocalityRegex().Match(issuer);
        if (localityMatch.Success)
        {
            string extracted = localityMatch.Groups[1].Value.Trim();
            _logger.LogDebug("Extracted L (Locality) from certificate issuer: {ExtractedLocality}", extracted);
            return extracted;
        }

        // Last resort: use the full issuer
        _logger.LogWarning("Could not extract CN or L from certificate issuer. Using full issuer: {Issuer}", issuer);
        return issuer;
    }

    /// <summary>
    /// Regex pattern to extract CN (Common Name) from X.500 distinguished name.
    /// </summary>
    [GeneratedRegex(@"CN\s*=\s*([^,]+)", RegexOptions.IgnoreCase)]
    private static partial Regex CnRegex();

    /// <summary>
    /// Regex pattern to extract L (Locality) from X.500 distinguished name.
    /// </summary>
    [GeneratedRegex(@"L\s*=\s*([^,]+)", RegexOptions.IgnoreCase)]
    private static partial Regex LocalityRegex();
}
