using System.Net.Http.Json;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Client;
public class DemographicsClient : IDemographicsService
{
    private readonly HttpClient _httpClient;

    public DemographicsClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(Constants.HttpClient);
    }

    public Task<PaginatedResponseDto<DemographicsResponseDto>> GetAllDemographics(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ISet<DemographicsResponseDto>> AddDemographics(IEnumerable<DemographicsRequestDto> demographics, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PutAsJsonAsync("lookup/payclassification/all", demographics, cancellationToken);

        response.EnsureSuccessStatusCode();

        return new HashSet<DemographicsResponseDto>(0);
    }
}
