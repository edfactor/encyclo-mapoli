using System.Net.Http.Json;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Client.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using FluentValidation;

namespace Demoulas.ProfitSharing.Client;
public sealed class PayProfitClient : IPayProfitService
{
    private const string BaseApiPath = "api/payprofit";

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;
    private readonly PayProfitRequestDtoValidator _validator;

    public PayProfitClient(HttpClient? client)
    {
        ArgumentNullException.ThrowIfNull(client);
        
        _httpClient = client;
        _validator = new PayProfitRequestDtoValidator();
        _options = Constants.GetJsonSerializerOptions();
    }

    public async Task<PaginatedResponseDto<PayProfitResponseDto>?> GetAllProfits(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(req);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BaseApiPath}/all", req, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaginatedResponseDto<PayProfitResponseDto>>(_options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ISet<PayProfitResponseDto>?> AddProfit(IEnumerable<PayProfitRequestDto> profitRequest, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(profitRequest);

        IEnumerable<PayProfitRequestDto> payProfitRequestDtos = profitRequest.ToList();
        if (!payProfitRequestDtos.Any())
        {
            return new HashSet<PayProfitResponseDto>(0);
        }

        foreach (PayProfitRequestDto demo in payProfitRequestDtos)
        {
            await _validator.ValidateAndThrowAsync(demo, cancellationToken);
        }

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseApiPath, payProfitRequestDtos, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ISet<PayProfitResponseDto>>(_options, cancellationToken).ConfigureAwait(false) ?? new HashSet<PayProfitResponseDto>(0);
    }
}
