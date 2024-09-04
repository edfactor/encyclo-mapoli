using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Demoulas.ProfitSharing.Client;
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
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(string.Concat(Enumerable.Repeat("UNIT TEST SECRET KEY",16))));
#pragma warning restore S6781 // JWT secret keys should not be disclosed
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Unit Test User"),
            new Claim(ClaimTypes.Email, "Unit@test.com"),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim("groups", $"SMART-PS-QA-{role}"));  
        }

        var token = new JwtSecurityToken(
            issuer: "Unit Test Issuer",
            audience: "Unit Test Audience",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
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
