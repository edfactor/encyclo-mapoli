using System.Net.Http.Json;
using System.Text.Json;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
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

    public async Task<PaginatedResponseDto<DemographicsResponseDto>?> GetAllDemographics(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(req);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BaseApiPath}/all", req, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaginatedResponseDto<DemographicsResponseDto>>(_options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ISet<DemographicsResponseDto>?> AddDemographics(IEnumerable<DemographicsRequestDto> demographics, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(demographics);

        IEnumerable<DemographicsRequestDto> demographicsRequestDtos = demographics.ToList();
        if (!demographicsRequestDtos.Any())
        {
            return new HashSet<DemographicsResponseDto>(0);
        }

        foreach (DemographicsRequestDto demo in demographicsRequestDtos)
        {
            await _validator.ValidateAndThrowAsync(demo, cancellationToken);
        }

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseApiPath, demographicsRequestDtos, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ISet<DemographicsResponseDto>>(_options, cancellationToken).ConfigureAwait(false) ?? new HashSet<DemographicsResponseDto>(0);
    }
}
