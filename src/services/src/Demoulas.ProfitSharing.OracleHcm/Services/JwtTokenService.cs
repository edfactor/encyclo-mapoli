using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Services.Interfaces;
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
    private const string TokenType = "JWT";

    public JwtTokenService(OracleHcmConfig config)
    {
        _config = config;
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

        // Calculate x5t (SHA-1 thumbprint in base64) - Oracle HCM requirement
        string x5t = CalculateThumbprint(certificate);

        // Create header
        var header = new
        {
            alg = _config.JwtSigningAlgorithm,
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

        // Create signature
        string signatureInput = $"{headerEncoded}.{payloadEncoded}";
        byte[] signatureInputBytes = Encoding.UTF8.GetBytes(signatureInput);

        byte[] signatureBytes = rsa.SignData(signatureInputBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        string signatureEncoded = Base64UrlEncode(signatureBytes);

        // Combine to create JWT
        return $"{signatureInput}.{signatureEncoded}";
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
    /// Extracts the issuer identifier from the certificate subject.
    /// Attempts to extract CN (Common Name) first, then L (Locality), then uses the full subject as fallback.
    /// </summary>
    private static string ExtractIssuerFromCertificate(X509Certificate2 certificate)
    {
        string subject = certificate.Subject;

        // Try to extract CN (Common Name) - e.g., "CN = Demoulas Cloud Issuing CA 2"
        var cnMatch = System.Text.RegularExpressions.Regex.Match(subject, @"CN\s*=\s*([^,]+)");
        if (cnMatch.Success)
        {
            return cnMatch.Groups[1].Value.Trim();
        }

        // Fallback to L (Locality) - e.g., "L = Demoulas Supermarkets Inc."
        var lMatch = System.Text.RegularExpressions.Regex.Match(subject, @"L\s*=\s*([^,]+)");
        if (lMatch.Success)
        {
            return lMatch.Groups[1].Value.Trim();
        }

        // Last resort: use the full subject
        return subject;
    }
}
