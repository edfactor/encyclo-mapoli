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
        var securityKey = new SymmetricSecurityKey("abcdefghijklmnopqrstuvwxyz123456"u8.ToArray());
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim> { new(ClaimTypes.Name, "Unit Test User"), new(ClaimTypes.Email, "testuser@demoulasmarketbasket.com") };

        foreach (var role in roles)
        {
            claims.Add(new Claim("groups", $"SMART-PS-QA-{role}"));
        }

        var token = new JwtSecurityToken(
            "Unit Test Issuer",
            "Unit Test Audience",
            claims,
            expires: DateTime.UtcNow.AddSeconds(60),
            signingCredentials: credentials
        );

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(token);

        client.DefaultRequestHeaders.Add("Impersonation", "Finance-Manager");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}
