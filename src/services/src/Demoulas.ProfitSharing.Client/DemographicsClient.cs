using System.Net.Http.Json;
using System.Text.Json;
using Demoulas.ProfitSharing.Client.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using FluentValidation;

namespace Demoulas.ProfitSharing.Client;
public sealed class DemographicsClient : IDemographicsService
{
    private const string BaseApiPath = "api/demographics";

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;
    private readonly DemographicsRequestDtoValidator _validator;

    public DemographicsClient(HttpClient? client)
    {
        ArgumentNullException.ThrowIfNull(client);
        
        _httpClient = client;
        _validator = new DemographicsRequestDtoValidator();
        _options = Constants.GetJsonSerializerOptions();
    }
   
    public async Task<ISet<DemographicResponseDto>?> AddDemographics(IEnumerable<DemographicsRequest> demographics, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(demographics);

        IEnumerable<DemographicsRequest> demographicsRequestDtos = demographics.ToList();
        if (!demographicsRequestDtos.Any())
        {
            return new HashSet<DemographicResponseDto>(0);
        }

        foreach (DemographicsRequest demo in demographicsRequestDtos)
        {
            await _validator.ValidateAndThrowAsync(demo, cancellationToken);
        }

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseApiPath, demographicsRequestDtos, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ISet<DemographicResponseDto>>(_options, cancellationToken).ConfigureAwait(false) ?? new HashSet<DemographicResponseDto>(0);
    }
}
