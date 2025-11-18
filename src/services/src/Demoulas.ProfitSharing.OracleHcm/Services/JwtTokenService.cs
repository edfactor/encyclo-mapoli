using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
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
/// This service creates RS256-signed tokens with the required claims and certificate thumbprint.
/// Configuration is loaded from OracleHcmConfig for principal, algorithm, and expiration.
/// </remarks>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly OracleHcmConfig _config;
    private readonly ILogger<JwtTokenService> _logger;
    private const string TokenType = "JWT";

    public JwtTokenService(OracleHcmConfig config, ILogger<JwtTokenService> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <inheritdoc />
    public string GenerateToken(X509Certificate2 certificate, string issuer, string principal, int expirationMinutes = 10)
    {
        ArgumentNullException.ThrowIfNull(certificate);
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(principal);

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
        string signingAlgorithm = ExtractSigningAlgorithmFromCertificate(certificate);

        // Calculate x5t (SHA-1 thumbprint in base64) - Oracle HCM requirement
        string x5t = CalculateThumbprint(certificate);

        // Create header
        var header = new
        {
            alg = signingAlgorithm,
            typ = TokenType,
            x5t = x5t
        };

        // Create payload with Oracle HCM required claims
        long nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long expUnix = nowUnix + (expirationMinutes * 60);

        var payload = new
        {
            iss = issuer,
            prn = principal,
            iat = nowUnix,
            exp = expUnix
        };

        // Encode header and payload as base64url
        string headerJson = JsonSerializer.Serialize(header, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        string payloadJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        string headerEncoded = Base64UrlEncode(headerJson);
        string payloadEncoded = Base64UrlEncode(payloadJson);

        // Log JWT components for debugging
        _logger.LogDebug("JWT Token Generation - Header: {HeaderJson}", headerJson);
        _logger.LogDebug("JWT Token Generation - Payload: {PayloadJson}", payloadJson);
        _logger.LogDebug("JWT Token Generation - Algorithm: {Algorithm}, Issuer: {Issuer}, Principal: {Principal}, Expiration (Unix): {Expiration}",
            signingAlgorithm, issuer, principal, expUnix);
        _logger.LogInformation("JWT Token Generated - Certificate Subject: {Subject}, Thumbprint: {Thumbprint}",
            certificate.Subject, certificate.Thumbprint);

        // Create signature
        string signatureInput = $"{headerEncoded}.{payloadEncoded}";
        byte[] signatureInputBytes = Encoding.UTF8.GetBytes(signatureInput);

        byte[] signatureBytes = rsa.SignData(signatureInputBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
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

        // Use configured principal and expiration (or provided override)
        int tokenExpiration = expirationMinutes == 10 ? _config.JwtExpirationMinutes : expirationMinutes;
        return GenerateToken(certificate, issuer, _config.JwtPrincipal, tokenExpiration);
    }

    /// <summary>
    /// Extracts and maps the signing algorithm from the certificate to JWT algorithm format.
    /// The certificate's signature algorithm is converted to the corresponding JWT algorithm identifier.
    /// </summary>
    /// <returns>JWT algorithm identifier (e.g., "RS256", "RS384", "RS512")</returns>
    /// <throws>InvalidOperationException if the certificate uses an unsupported algorithm</throws>
    private string ExtractSigningAlgorithmFromCertificate(X509Certificate2 certificate)
    {
        string? certAlgorithm = certificate.SignatureAlgorithm.FriendlyName;

        if (string.IsNullOrEmpty(certAlgorithm))
        {
            throw new InvalidOperationException(
                "Unable to determine certificate signature algorithm. The certificate's algorithm information is missing or invalid.");
        }

        _logger.LogDebug("Certificate signature algorithm detected: {CertificateAlgorithm}", certAlgorithm);

        // Map certificate algorithm to JWT algorithm
        string jwtAlgorithm = certAlgorithm switch
        {
            "sha1RSA" => "RS256",  // Fallback, but log warning since SHA-1 is weak
            "sha256RSA" => "RS256",
            "sha384RSA" => "RS384",
            "sha512RSA" => "RS512",
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

        return jwtAlgorithm;
    }

    /// <summary>
    /// Calculates the SHA-1 thumbprint of the certificate in base64 format (x5t).
    /// This is required by Oracle HCM in the JWT header.
    /// </summary>
    /// <remarks>
    /// Oracle HCM specifically requires SHA-1 thumbprint for the x5t claim.
    /// While SHA-1 is generally considered weak for signing, it's acceptable here
    /// as a fingerprint/identifier rather than for cryptographic signing.
    /// The actual JWT signature uses RS256 (RSA-SHA256).
    /// </remarks>
#pragma warning disable CA5350 // Do not use weak cryptographic algorithms - Oracle HCM requirement
    private static string CalculateThumbprint(X509Certificate2 certificate)
    {
        using var sha1 = SHA1.Create();
        byte[] thumbprintBytes = sha1.ComputeHash(certificate.RawData);
        return Base64UrlEncode(thumbprintBytes);
    }
#pragma warning restore CA5350

    /// <summary>
    /// Encodes a string or byte array to base64url format (no padding, url-safe).
    /// Required for JWT encoding per RFC 7515.
    /// </summary>
    private static string Base64UrlEncode(string input)
    {
        return Base64UrlEncode(Encoding.UTF8.GetBytes(input));
    }

    /// <summary>
    /// Encodes a byte array to base64url format (no padding, url-safe).
    /// Required for JWT encoding per RFC 7515.
    /// </summary>
    private static string Base64UrlEncode(byte[] input)
    {
        string base64 = Convert.ToBase64String(input);
        // Convert to base64url: replace +/= with -_
        return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
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
        var cnMatch = System.Text.RegularExpressions.Regex.Match(issuer, @"CN\s*=\s*([^,]+)");
        if (cnMatch.Success)
        {
            string extracted = cnMatch.Groups[1].Value.Trim();
            _logger.LogDebug("Extracted CN from certificate issuer: {ExtractedCn}", extracted);
            return extracted;
        }

        // Fallback to L (Locality) - e.g., "L = Demoulas Supermarkets Inc."
        var lMatch = System.Text.RegularExpressions.Regex.Match(issuer, @"L\s*=\s*([^,]+)");
        if (lMatch.Success)
        {
            string extracted = lMatch.Groups[1].Value.Trim();
            _logger.LogDebug("Extracted L (Locality) from certificate issuer: {ExtractedLocality}", extracted);
            return extracted;
        }

        // Last resort: use the full issuer
        _logger.LogWarning("Could not extract CN or L from certificate issuer. Using full issuer: {Issuer}", issuer);
        return issuer;
    }
}
