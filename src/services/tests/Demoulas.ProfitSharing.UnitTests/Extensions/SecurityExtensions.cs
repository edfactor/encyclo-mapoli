using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Demoulas.ProfitSharing.UnitTests.Extensions;

public static class SecurityExtensions
{
    public static void CreateAndAssignTokenForClient(this HttpClient client, params string[] roles)
    {
#pragma warning disable S6781 // JWT secret keys should not be disclosed
        var securityKey = new SymmetricSecurityKey("abcdefghijklmnopqrstuvwxyz123456"u8.ToArray());
#pragma warning restore S6781 // JWT secret keys should not be disclosed
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Unit Test User"), new Claim(ClaimTypes.Email, "Unit@test.com"), };

        foreach (var role in roles)
        {
            claims.Add(new Claim("groups", $"SMART-PS-QA-{role}"));
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


        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}
