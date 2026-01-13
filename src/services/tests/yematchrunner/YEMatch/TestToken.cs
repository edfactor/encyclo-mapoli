using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace YEMatch;

#pragma warning disable S6781 // JWT secret keys should not be disclosed

[SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed")]
public static class TestToken
{
    /// <summary>
    ///     Creates a JWT token and assigns it to the HttpClient authorization header.
    ///     Uses configuration from JwtOptions to match the SMART API server's JWT validation.
    /// </summary>
    /// <param name="client">HttpClient to assign the token to</param>
    /// <param name="jwtOptions">JWT configuration options (signing key, issuer, audience)</param>
    /// <param name="roles">Roles to include in the token (will be prefixed with "SMART-PS-QA-")</param>
    /// <exception cref="InvalidOperationException">Thrown when SigningKey is not configured</exception>
    public static void CreateAndAssignTokenForClient(HttpClient client, JwtOptions jwtOptions, params string[] roles)
    {
        if (string.IsNullOrEmpty(jwtOptions.SigningKey))
        {
            throw new InvalidOperationException(
                "JWT SigningKey is not configured. Please set YeMatch:Jwt:SigningKey in user secrets. " +
                "This should match the signing key used by the SMART API server. " +
                "If using 'dotnet user-jwts', you can find the key in the server's user secrets under 'Authentication:Schemes:Bearer:SigningKeys'.");
        }

        // Convert the signing key from Base64 string to byte array
        // The key should be at least 32 bytes (256 bits) for HS256
        // The signing key from dotnet user-jwts is Base64-encoded
        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(jwtOptions.SigningKey);
        }
        catch (FormatException)
        {
            // Fallback to UTF-8 if not Base64 (for backward compatibility)
            keyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
        }

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                $"JWT SigningKey must be at least 32 bytes (256 bits) for HS256 algorithm. Current length: {keyBytes.Length} bytes. " +
                "Please use a longer key.");
        }

        SymmetricSecurityKey securityKey = new(keyBytes);
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, "YEMatch Test User"),
            new Claim(ClaimTypes.Email, "yematch-test@demoulasmarketbasket.com"),
            new Claim(JwtRegisteredClaimNames.Sub, "yematch-test-user")
        };

        foreach (string role in roles)
        {
            claims.Add(new Claim("groups", $"SMART-PS-QA-{role}"));
        }

        JwtSecurityToken token = new(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddSeconds(jwtOptions.ExpirationSeconds),
            signingCredentials: credentials
        );

        JwtSecurityTokenHandler handler = new();
        string accessToken = handler.WriteToken(token);

        client.DefaultRequestHeaders.Add("Impersonation", "Finance-Manager");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

}
