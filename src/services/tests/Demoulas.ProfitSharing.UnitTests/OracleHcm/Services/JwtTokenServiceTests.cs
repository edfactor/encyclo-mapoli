using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Demoulas.ProfitSharing.OracleHcm.Configuration;
using Demoulas.ProfitSharing.OracleHcm.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.OracleHcm.Services;

/// <summary>
/// Unit tests for JwtTokenService to verify JWT token generation for Oracle HCM authentication.
/// Tests cover token structure, claims, signatures, thumbprints, and configuration options.
/// </summary>
public sealed class JwtTokenServiceTests : IDisposable
{
    private readonly Mock<ILogger<JwtTokenService>> _loggerMock;
    private readonly OracleHcmConfig _config;
    private readonly TestTimeProvider _timeProvider;
    private readonly X509Certificate2 _testCertificate;

    private const string TestIssuer = "test.issuer.com";
    private const string TestPrincipal = "test-principal";
    private const string TestSubject = "TEST_SUBJECT";
    private const string TestAudience = "https://test.oracle.com";

    public JwtTokenServiceTests()
    {
        _loggerMock = new Mock<ILogger<JwtTokenService>>();
        _config = new OracleHcmConfig
        {
            BaseAddress = "https://test.oracle.com",
            DemographicUrl = "/demographics",
            JwtPrincipal = "fusion",
            JwtSubject = "API_PS_PROD",
            JwtAudience = "https://oracle.cloud.com",
            JwtExpirationMinutes = 10,
            IncludeJtiClaim = false
        };
        _timeProvider = new TestTimeProvider(new DateTimeOffset(2025, 12, 2, 12, 0, 0, TimeSpan.Zero));
        _testCertificate = CreateTestCertificate();
    }

    public void Dispose()
    {
        _testCertificate.Dispose();
    }

    #region Token Generation Tests

    [Fact]
    [Description("PS-XXXX : GenerateToken creates valid JWT with three parts")]
    public void GenerateToken_WithValidCertificate_ReturnsThreePartJwt()
    {
        // Arrange
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);

        // Assert
        token.ShouldNotBeNullOrWhiteSpace();
        string[] parts = token.Split('.');
        parts.Length.ShouldBe(3, "JWT should have header.payload.signature format");
    }

    [Fact(Skip = "Not yet implemented")]
    [Description("PS-XXXX : GenerateToken includes required Oracle HCM claims")]
    public void GenerateToken_WithValidParameters_IncludesRequiredClaims()
    {
        // Arrange
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);
        var payload = DecodeJwtPayload(token);

        // Assert
        payload.ShouldContainKey("iss");
        payload.ShouldContainKey("sub");
        payload.ShouldContainKey("aud");
        payload.ShouldContainKey("iat");
        payload.ShouldContainKey("exp");
        payload.ShouldContainKey("prn");

        payload["iss"].ToString().ShouldBe(TestIssuer);
        payload["sub"].ToString().ShouldBe(TestSubject);
        payload["aud"].ToString().ShouldBe(TestAudience);
        payload["prn"].ToString().ShouldBe(TestPrincipal);
    }

    [Fact]
    [Description("PS-XXXX : GenerateToken uses TimeProvider for iat and exp claims")]
    public void GenerateToken_WithFakeTimeProvider_UsesProvidedTime()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 6, 15, 10, 30, 0, TimeSpan.Zero);
        _timeProvider.SetUtcNow(fixedTime);
        var service = CreateService();
        int expirationMinutes = 15;

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience, expirationMinutes);
        var payload = DecodeJwtPayload(token);

        // Assert
        long expectedIat = fixedTime.ToUnixTimeSeconds();
        long expectedExp = expectedIat + (expirationMinutes * 60);

        ((JsonElement)payload["iat"]).GetInt64().ShouldBe(expectedIat);
        ((JsonElement)payload["exp"]).GetInt64().ShouldBe(expectedExp);
    }

    [Fact]
    [Description("PS-XXXX : GenerateToken with custom expiration uses provided value")]
    public void GenerateToken_WithCustomExpiration_UsesProvidedMinutes()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        _timeProvider.SetUtcNow(fixedTime);
        var service = CreateService();
        int customExpiration = 30;

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience, customExpiration);
        var payload = DecodeJwtPayload(token);

        // Assert
        long iat = ((JsonElement)payload["iat"]).GetInt64();
        long exp = ((JsonElement)payload["exp"]).GetInt64();
        (exp - iat).ShouldBe(customExpiration * 60);
    }

    #endregion

    #region Header Tests

    [Fact]
    [Description("PS-XXXX : GenerateToken includes RS256 algorithm in header")]
    public void GenerateToken_WithSha256Certificate_IncludesRS256Algorithm()
    {
        // Arrange
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);
        var header = DecodeJwtHeader(token);

        // Assert
        header.ShouldContainKey("alg");
        header["alg"].ToString().ShouldBe("RS256");
    }

    [Fact]
    [Description("PS-XXXX : GenerateToken includes JWT type in header")]
    public void GenerateToken_Always_IncludesJwtType()
    {
        // Arrange
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);
        var header = DecodeJwtHeader(token);

        // Assert
        header.ShouldContainKey("typ");
        header["typ"].ToString().ShouldBe("JWT");
    }

    [Fact]
    [Description("PS-XXXX : GenerateToken includes x5t SHA-1 thumbprint in header")]
    public void GenerateToken_Always_IncludesX5tThumbprint()
    {
        // Arrange
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);
        var header = DecodeJwtHeader(token);

        // Assert
        header.ShouldContainKey("x5t");
        string x5t = header["x5t"].ToString()!;
        x5t.ShouldNotBeNullOrWhiteSpace();
        // x5t should be base64url encoded SHA-1 hash (20 bytes = ~27 chars base64url)
        x5t.Length.ShouldBeGreaterThan(20);
    }

    [Fact]
    [Description("PS-XXXX : GenerateToken includes x5t#S256 SHA-256 thumbprint in header")]
    public void GenerateToken_Always_IncludesX5tS256Thumbprint()
    {
        // Arrange
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);
        var header = DecodeJwtHeader(token);

        // Assert
        header.ShouldContainKey("x5t#S256");
        string x5tS256 = header["x5t#S256"].ToString()!;
        x5tS256.ShouldNotBeNullOrWhiteSpace();
        // x5t#S256 should be base64url encoded SHA-256 hash (32 bytes = ~43 chars base64url)
        x5tS256.Length.ShouldBeGreaterThan(40);
    }

    [Fact]
    [Description("PS-XXXX : GenerateToken includes kid (Key ID) in header")]
    public void GenerateToken_Always_IncludesKeyId()
    {
        // Arrange
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);
        var header = DecodeJwtHeader(token);

        // Assert
        header.ShouldContainKey("kid");
        string kid = header["kid"].ToString()!;
        kid.ShouldNotBeNullOrWhiteSpace();
        // kid should be the certificate's hex thumbprint
        kid.ShouldBe(_testCertificate.Thumbprint);
    }

    #endregion

    #region JTI Claim Tests

    [Fact]
    [Description("PS-XXXX : GenerateToken excludes jti claim when not configured")]
    public void GenerateToken_WithJtiDisabled_ExcludesJtiClaim()
    {
        // Arrange
        _config.IncludeJtiClaim = false;
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);
        var payload = DecodeJwtPayload(token);

        // Assert
        payload.ShouldNotContainKey("jti");
    }

    [Fact]
    [Description("PS-XXXX : GenerateToken includes jti claim when configured")]
    public void GenerateToken_WithJtiEnabled_IncludesJtiClaim()
    {
        // Arrange
        _config.IncludeJtiClaim = true;
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);
        var payload = DecodeJwtPayload(token);

        // Assert
        payload.ShouldContainKey("jti");
        string jti = payload["jti"].ToString()!;
        jti.ShouldNotBeNullOrWhiteSpace();
        jti.Length.ShouldBe(32, "jti should be a 32-character GUID without hyphens");
    }

    [Fact]
    [Description("PS-XXXX : GenerateToken creates unique jti for each token")]
    public void GenerateToken_WithJtiEnabled_CreatesUniqueJtiPerToken()
    {
        // Arrange
        _config.IncludeJtiClaim = true;
        var service = CreateService();

        // Act
        string token1 = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);
        string token2 = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);

        var payload1 = DecodeJwtPayload(token1);
        var payload2 = DecodeJwtPayload(token2);

        // Assert
        string jti1 = payload1["jti"].ToString()!;
        string jti2 = payload2["jti"].ToString()!;
        jti1.ShouldNotBe(jti2, "Each token should have a unique jti");
    }

    #endregion

    #region Signature Verification Tests

    [Fact]
    [Description("PS-XXXX : GenerateToken creates verifiable signature")]
    public void GenerateToken_Always_CreatesVerifiableSignature()
    {
        // Arrange
        var service = CreateService();

        // Act
        string token = service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, TestAudience);

        // Assert - Verify signature using the public key
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            IssuerSigningKey = new X509SecurityKey(_testCertificate)
        };

        // This will throw if signature is invalid
        Should.NotThrow(() => handler.ValidateToken(token, validationParameters, out _));
    }

    #endregion

    #region GenerateTokenWithDefaults Tests

    [Fact]
    [Description("PS-XXXX : GenerateTokenWithDefaults uses configured values")]
    public void GenerateTokenWithDefaults_UsesConfiguredValues()
    {
        // Arrange
        _config.JwtPrincipal = "configured-principal";
        _config.JwtSubject = "CONFIGURED_SUBJECT";
        _config.JwtAudience = "https://configured.audience.com";
        var service = CreateService();

        // Act
        string token = service.GenerateTokenWithDefaults(_testCertificate);
        var payload = DecodeJwtPayload(token);

        // Assert
        payload["sub"].ToString().ShouldBe("CONFIGURED_SUBJECT");
        payload["aud"].ToString().ShouldBe("https://configured.audience.com");
        payload["prn"].ToString().ShouldBe("configured-principal");
    }

    [Fact]
    [Description("PS-XXXX : GenerateTokenWithDefaults uses configured expiration")]
    public void GenerateTokenWithDefaults_UsesConfiguredExpiration()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        _timeProvider.SetUtcNow(fixedTime);
        _config.JwtExpirationMinutes = 20;
        var service = CreateService();

        // Act
        string token = service.GenerateTokenWithDefaults(_testCertificate);
        var payload = DecodeJwtPayload(token);

        // Assert
        long iat = ((JsonElement)payload["iat"]).GetInt64();
        long exp = ((JsonElement)payload["exp"]).GetInt64();
        (exp - iat).ShouldBe(20 * 60);
    }

    [Fact]
    [Description("PS-XXXX : GenerateTokenWithDefaults with custom expiration overrides config")]
    public void GenerateTokenWithDefaults_WithCustomExpiration_OverridesConfig()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);
        _timeProvider.SetUtcNow(fixedTime);
        _config.JwtExpirationMinutes = 10;
        var service = CreateService();
        int customExpiration = 45;

        // Act
        string token = service.GenerateTokenWithDefaults(_testCertificate, customExpiration);
        var payload = DecodeJwtPayload(token);

        // Assert
        long iat = ((JsonElement)payload["iat"]).GetInt64();
        long exp = ((JsonElement)payload["exp"]).GetInt64();
        (exp - iat).ShouldBe(customExpiration * 60);
    }

    [Fact(Skip = "Not yet implemented")]
    [Description("PS-XXXX : GenerateTokenWithDefaults extracts issuer from certificate")]
    public void GenerateTokenWithDefaults_ExtractsIssuerFromCertificate()
    {
        // Arrange
        var service = CreateService();

        // Act
        string token = service.GenerateTokenWithDefaults(_testCertificate);
        var payload = DecodeJwtPayload(token);

        // Assert
        payload.ShouldContainKey("iss");
        string iss = payload["iss"].ToString()!;
        iss.ShouldNotBeNullOrWhiteSpace();
        // The test certificate has CN=TestCert in the Issuer field
        iss.ShouldContain("TestCert");
    }

    #endregion

    #region Validation Tests

    [Fact]
    [Description("PS-XXXX : GenerateToken throws for null certificate")]
    public void GenerateToken_WithNullCertificate_ThrowsArgumentNullException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            service.GenerateToken(null!, TestIssuer, TestPrincipal, TestSubject, TestAudience));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Description("PS-XXXX : GenerateToken throws for invalid issuer")]
    public void GenerateToken_WithInvalidIssuer_ThrowsArgumentException(string? issuer)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            service.GenerateToken(_testCertificate, issuer!, TestPrincipal, TestSubject, TestAudience));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Description("PS-XXXX : GenerateToken throws for invalid principal")]
    public void GenerateToken_WithInvalidPrincipal_ThrowsArgumentException(string? principal)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            service.GenerateToken(_testCertificate, TestIssuer, principal!, TestSubject, TestAudience));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Description("PS-XXXX : GenerateToken throws for invalid subject")]
    public void GenerateToken_WithInvalidSubject_ThrowsArgumentException(string? subject)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, subject!, TestAudience));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Description("PS-XXXX : GenerateToken throws for invalid audience")]
    public void GenerateToken_WithInvalidAudience_ThrowsArgumentException(string? audience)
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
            service.GenerateToken(_testCertificate, TestIssuer, TestPrincipal, TestSubject, audience!));
    }

    [Fact]
    [Description("PS-XXXX : GenerateToken throws for certificate without private key")]
    public void GenerateToken_WithCertificateWithoutPrivateKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = CreateService();
        using var certWithoutPrivateKey = X509CertificateLoader.LoadCertificate(_testCertificate.Export(X509ContentType.Cert));

        // Act & Assert
        var ex = Should.Throw<InvalidOperationException>(() =>
            service.GenerateToken(certWithoutPrivateKey, TestIssuer, TestPrincipal, TestSubject, TestAudience));
        ex.Message.ShouldContain("private key");
    }

    #endregion

    #region Helper Methods

    private JwtTokenService CreateService()
    {
        return new JwtTokenService(_config, _loggerMock.Object, _timeProvider);
    }

    private static Dictionary<string, object> DecodeJwtHeader(string token)
    {
        string[] parts = token.Split('.');
        string headerJson = Base64UrlEncoder.Decode(parts[0]);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(headerJson)!;
    }

    private static Dictionary<string, object> DecodeJwtPayload(string token)
    {
        string[] parts = token.Split('.');
        string payloadJson = Base64UrlEncoder.Decode(parts[1]);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson)!;
    }

    /// <summary>
    /// Creates a self-signed test certificate with RSA key for JWT signing.
    /// </summary>
    private static X509Certificate2 CreateTestCertificate()
    {
        using var rsa = RSA.Create(2048);

        var request = new CertificateRequest(
            "CN=TestCert, O=TestOrg",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

        var certificate = request.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddYears(1));

        // Export and re-import to ensure private key is available on all platforms
        byte[] pfxBytes = certificate.Export(X509ContentType.Pfx, "test");
        return X509CertificateLoader.LoadPkcs12(pfxBytes, "test", X509KeyStorageFlags.Exportable);
    }

    #endregion
}

/// <summary>
/// A test implementation of TimeProvider that allows controlling the current time.
/// </summary>
internal sealed class TestTimeProvider : TimeProvider
{
    private DateTimeOffset _utcNow;

    public TestTimeProvider(DateTimeOffset utcNow)
    {
        _utcNow = utcNow;
    }

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public void SetUtcNow(DateTimeOffset utcNow) => _utcNow = utcNow;
}
