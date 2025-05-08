using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace YEMatch;

#pragma warning disable S6781 // JWT secret keys should not be disclosed

public static class TestToken
{
    public static void CreateAndAssignTokenForClient(HttpClient client, params string[] roles)
    {
        // Use test certs.  
        SymmetricSecurityKey securityKey = new("abcdefghijklmnopqrstuvwxyz123456"u8.ToArray());
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
        List<Claim> claims = new() { new Claim(ClaimTypes.Name, "Unit Test User"), new Claim(ClaimTypes.Email, "testuser@demoulasmarketbasket.com") };

        foreach (string role in roles)
        {
            claims.Add(new Claim("groups", $"SMART-PS-QA-{role}"));
        }

        JwtSecurityToken token = new(
            "Unit Test Issuer",
            "Unit Test Audience",
            claims,
            expires: DateTime.UtcNow.AddSeconds(60),
            signingCredentials: credentials
        );

        JwtSecurityTokenHandler handler = new();
        string? accessToken = handler.WriteToken(token);

        client.DefaultRequestHeaders.Add("Impersonation", "Finance-Manager");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}
