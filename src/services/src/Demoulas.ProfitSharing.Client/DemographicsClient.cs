using System.Net.Http.Json;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Client;
public sealed class DemographicsClient : IDemographicsService
{
    private const string BaseApiPath = "api/demographics/";

    private readonly HttpClient _httpClient;

    public DemographicsClient(HttpClient client)
    {
        _httpClient = client;
    }

    public async Task<PaginatedResponseDto<DemographicsResponseDto>?> GetAllDemographics(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseApiPath}/all", req, cancellationToken);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaginatedResponseDto<DemographicsResponseDto>>(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<ISet<DemographicsResponseDto>> AddDemographics(IEnumerable<DemographicsRequestDto> demographics, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PutAsJsonAsync("lookup/payclassification/all", demographics, cancellationToken);

        response.EnsureSuccessStatusCode();

        return new HashSet<DemographicsResponseDto>(0);
    }
}
