using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Client.Common;
using System.Net.Http.Json;
using Demoulas.Common.Contracts.Response;
using System.Threading;

namespace Demoulas.ProfitSharing.Client;
public sealed class YearEndClient : IYearEndService
{
    private const string BaseApiPath = "api/yearend";

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;
    public YearEndClient(HttpClient client)
    {
        _httpClient = client;
        _options = Constants.GetJsonSerializerOptions();
    }

    public async Task<IList<PayrollDuplicateSSNResponseDto>> GetDuplicateSSNs(CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{BaseApiPath}/duplicatessns", ct);
        _ = response.EnsureSuccessStatusCode();

        var rslt = await response.Content.ReadFromJsonAsync<IList<PayrollDuplicateSSNResponseDto>>(_options, ct);
        return rslt ?? ([]);
    }
}
