using System.Data.SqlTypes;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Demoulas.ProfitSharing.Client.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Client.Reports.YearEnd;
public sealed class YearEndClient : IYearEndService
{
    private const string BaseApiPath = "api/yearend";

    private readonly HttpClient _httpClient;
    private readonly HttpClient _httpDownloadClient;
    private readonly JsonSerializerOptions _options;

    public YearEndClient(IHttpClientFactory httpClientFactory) : 
        this(httpClientFactory.CreateClient(Constants.Http.ReportsHttpClient),
            httpClientFactory.CreateClient(Constants.Http.ReportsDownloadClient))
    {
        
    }
    public YearEndClient(HttpClient? jsonClient, HttpClient? fileDownloadClient)
    {
        ArgumentNullException.ThrowIfNull(jsonClient);
        ArgumentNullException.ThrowIfNull(fileDownloadClient);
        _httpClient = jsonClient;
        _httpDownloadClient = fileDownloadClient;
        _options = Constants.GetJsonSerializerOptions();
    }

    public async Task<IList<PayrollDuplicateSSNResponseDto>> GetDuplicateSSNs(CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"{BaseApiPath}/duplicatessns", ct);
        _ = response.EnsureSuccessStatusCode();

        var rslt = await response.Content.ReadFromJsonAsync<IList<PayrollDuplicateSSNResponseDto>>(_options, ct);
        return rslt ?? ([]);
    }

    #region Negative ETVA For SSNs On PayProfit

    public async Task<ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponse(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{BaseApiPath}/negative-evta-ssn", cancellationToken);
        _ = response.EnsureSuccessStatusCode();

        var rslt = await response.Content.ReadFromJsonAsync<ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>>(_options, cancellationToken);

        return rslt ?? new ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>
        {
            ReportName = Constants.ErrorMessages.ReportNotFound,
            ReportDate = SqlDateTime.MinValue.Value,
            Results = new HashSet<NegativeETVAForSSNsOnPayProfitResponse>(0)
        };
    }

    public Task<Stream> DownloadNegativeETVAForSSNsOnPayProfitResponse(CancellationToken cancellationToken = default)
    {
        return _httpDownloadClient.GetStreamAsync($"{BaseApiPath}/negative-evta-ssn", cancellationToken);
    }

    #endregion

    #region Mismatched Ssns Payprofit And Demographics On Same Badge
    public async Task<ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>> GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{BaseApiPath}/mismatched-ssns-payprofit-and-demo-on-same-badge", cancellationToken);
        _ = response.EnsureSuccessStatusCode();

        var rslt = await response.Content.ReadFromJsonAsync<ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>>(_options, cancellationToken);
        return rslt ?? new ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>
        {
            ReportName = Constants.ErrorMessages.ReportNotFound,
            ReportDate = SqlDateTime.MinValue.Value,
            Results = new HashSet<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>(0)
        };
    }

    public Task<ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>> GetPayrollDuplicateSsnsOnPayprofit(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Stream> DownloadMismatchedSsnsPayprofitAndDemographicsOnSameBadge(CancellationToken cancellationToken = default)
    {
        return _httpDownloadClient.GetStreamAsync($"{BaseApiPath}/mismatched-ssns-payprofit-and-demo-on-same-badge", cancellationToken);
    }

    #endregion
}
