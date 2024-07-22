using System.Data.SqlTypes;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
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

    public Task<ReportResponseBase<PayrollDuplicateSSNResponseDto>> GetDuplicateSSNs(CancellationToken ct)
    {
        return CallReportEndpoint<PayrollDuplicateSSNResponseDto>("duplicate-ssns", ct);
    }

    public Task<Stream> DownloadDuplicateSSNs(CancellationToken ct)
    {
        return _httpDownloadClient.GetStreamAsync($"{BaseApiPath}/duplicate-ssns", ct);
    }

    #region Negative ETVA For SSNs On PayProfit

    public Task<ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponse(CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<NegativeETVAForSSNsOnPayProfitResponse>("negative-evta-ssn", cancellationToken);
    }

    public Task<Stream> DownloadNegativeETVAForSSNsOnPayProfitResponse(CancellationToken cancellationToken = default)
    {
        return _httpDownloadClient.GetStreamAsync($"{BaseApiPath}/negative-evta-ssn", cancellationToken);
    }

    #endregion

    #region Mismatched Ssns Payprofit And Demographics On Same Badge
    public Task<ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>> GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>("mismatched-ssns-payprofit-and-demo-on-same-badge", cancellationToken);
    }

    public Task<Stream> DownloadMismatchedSsnsPayprofitAndDemographicsOnSameBadge(CancellationToken cancellationToken = default)
    {
        return _httpDownloadClient.GetStreamAsync($"{BaseApiPath}/mismatched-ssns-payprofit-and-demo-on-same-badge", cancellationToken);
    }

    public async Task<ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>> GetPayProfitBadgesNotInDemographics(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{BaseApiPath}/payprofit-badges-without-demographics", cancellationToken);
        _ = response.EnsureSuccessStatusCode();

        var rslt = await response.Content.ReadFromJsonAsync<ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>>(_options, cancellationToken);
        return rslt ?? new ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>
        {
            ReportName = Constants.ErrorMessages.ReportNotFound,
            ReportDate = SqlDateTime.MinValue.Value,
            Results = new HashSet<PayProfitBadgesNotInDemographicsResponse>(0)
        };
    }

    #endregion

    #region Get Payroll Duplicate Ssns On Payprofit
    public Task<ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>> GetPayrollDuplicateSsnsOnPayprofit(CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<PayrollDuplicateSsnsOnPayprofitResponseDto>("payroll-duplicate-ssns-on-payprofit", cancellationToken);
    }

    public Task<Stream> DownloadPayrollDuplicateSsnsOnPayprofit(CancellationToken cancellationToken = default)
    {
        return _httpDownloadClient.GetStreamAsync($"{BaseApiPath}/payroll-duplicate-ssns-on-payprofit", cancellationToken);
    }

    #endregion


    private async Task<ReportResponseBase<TResponseDto>> CallReportEndpoint<TResponseDto>(string endpointRoute, CancellationToken cancellationToken) where TResponseDto : class
    {
        var response = await _httpClient.GetAsync($"{BaseApiPath}/{endpointRoute}", cancellationToken);
        _ = response.EnsureSuccessStatusCode();

        var rslt = await response.Content.ReadFromJsonAsync<ReportResponseBase<TResponseDto>>(_options, cancellationToken);
        return rslt ?? new ReportResponseBase<TResponseDto>
        {
            ReportName = Constants.ErrorMessages.ReportNotFound,
            ReportDate = SqlDateTime.MinValue.Value,
            Results = new HashSet<TResponseDto>(0)
        };
    }
}
