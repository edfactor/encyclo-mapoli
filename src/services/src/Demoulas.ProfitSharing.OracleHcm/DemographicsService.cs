using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Configuration;

namespace Demoulas.ProfitSharing.OracleHcm;
public sealed class DemographicsService
{
    private readonly HttpClient _httpClient;

    public DemographicsService(HttpClient httpClient)
    {
        _httpClient = httpClient;        
    }

    public async Task<string> GetAllEmployees(OracleHcmConfig oracleHcmConfig, CancellationToken cancellationToken = default)
    {

        var bytes = Encoding.UTF8.GetBytes($"{oracleHcmConfig.Username}:{oracleHcmConfig.Password}");
        string base64String = Convert.ToBase64String(bytes);

        var query = new Dictionary<string, string>
        {
            { "limit", "500" },
            { "offset", "0" },
            { "totalResults", "false" },
        };

        var uriBuilder = new UriBuilder(oracleHcmConfig.Url);
        string queryString = await new FormUrlEncodedContent(query).ReadAsStringAsync(cancellationToken);
        uriBuilder.Query = queryString;


        var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
        request.Headers.Add("REST-Framework-Version", oracleHcmConfig.RestFrameworkVersion);
        request.Headers.Add("Authorization", $"Basic {base64String}");
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
