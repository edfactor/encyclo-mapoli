using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Demoulas.ProfitSharing.UnitTests.Common.Common;

public class ClientBase
{
    private readonly HttpClient?[] _clients;

    public ClientBase(params HttpClient?[] clients)
    {
        _clients = clients;
    }

    public void CreateAndAssignTokenForClient(params string[] roles)
    {
#pragma warning disable S6781 // JWT secret keys should not be disclosed
        var securityKey = new SymmetricSecurityKey("abcdefghijklmnopqrstuvwxyz123456"u8.ToArray());
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
#pragma warning restore S6781 // JWT secret keys should not be disclosed

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Unit Test User"), new Claim(ClaimTypes.Email, "testuser@demoulasmarketbasket.com"), };

        foreach (var role in roles)
        {
            // Use "Testing" environment prefix to match the claims transformation in ImpersonationAndEnvironmentAwareClaimsTransformation
            // The GetEnvironment() method returns "Testing" when ASPNETCORE_ENVIRONMENT is "Testing"
            claims.Add(new Claim("groups", $"SMART-PS-Testing-{role}"));
        }

        var token = new JwtSecurityToken(
            issuer: "Unit Test Issuer",
            audience: "Unit Test Audience",
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(60),
            signingCredentials: credentials
        );

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(token);


        foreach (var client in _clients)
        {
            if (client != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }
}
