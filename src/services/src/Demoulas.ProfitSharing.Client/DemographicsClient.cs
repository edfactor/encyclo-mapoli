using System.Net.Http.Json;
using System.Text.Json;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Client.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Client;
public sealed class DemographicsClient : IDemographicsService
{
    private const string BaseApiPath = "api/demographics";

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public DemographicsClient(HttpClient? client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _httpClient = client;
        _options = Constants.GetJsonSerializerOptions();
    }

    public async Task<PaginatedResponseDto<DemographicsResponseDto>?> GetAllDemographics(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseApiPath}/all", req, cancellationToken);

        response.EnsureSuccessStatusCode();
        try
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var obj = JsonSerializer.Deserialize<PaginatedResponseDto<DemographicsResponseDto>>(json, _options);
            return obj;
        }
        catch (JsonException jsonEx)
        {
            // Log or handle the JSON exception
            Console.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
            Console.WriteLine($"Path: {jsonEx.Path}");
            Console.WriteLine($"Line Number: {jsonEx.LineNumber}");
            Console.WriteLine($"Byte Position in Line: {jsonEx.BytePositionInLine}");
            throw;
        }
        catch (Exception ex)
        {
            // Log or handle the general exception
            Console.WriteLine($"General Error: {ex.Message}");
            throw;
        }

        //return await response.Content.ReadFromJsonAsync<PaginatedResponseDto<DemographicsResponseDto>>(options: _options, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<ISet<DemographicsResponseDto>> AddDemographics(IEnumerable<DemographicsRequestDto> demographics, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PutAsJsonAsync("lookup/payclassification/all", demographics, cancellationToken);

        response.EnsureSuccessStatusCode();

        return new HashSet<DemographicsResponseDto>(0);
    }
}
