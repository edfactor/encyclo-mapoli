using System.Net.Http.Json;
using Demoulas.ProfitSharing.Client.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Client;
public sealed  class PayClassificationClient : IPayClassificationService
{
    private readonly HttpClient _httpClient;

    public PayClassificationClient(IHttpClientFactory httpClientFactory)
    {
       _httpClient =  httpClientFactory.CreateClient(Constants.HttpClient);
    }
    public async Task<ISet<PayClassificationResponseDto>> GetAllPayClassifications(CancellationToken cancellationToken = default)
    {
      ISet<PayClassificationResponseDto>? response =  await _httpClient.GetFromJsonAsync<ISet<PayClassificationResponseDto>>("lookup/payclassification/all", cancellationToken);

      return response ?? new HashSet<PayClassificationResponseDto>(0);
    }
}
