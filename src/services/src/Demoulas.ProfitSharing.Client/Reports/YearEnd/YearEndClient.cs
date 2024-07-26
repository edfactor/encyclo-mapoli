using System;
using System.Data.SqlTypes;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Web;
using Demoulas.Common.Contracts.Request;
using Demoulas.Common.Contracts.Response;
using Demoulas.ProfitSharing.Client.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using FastEndpoints;

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

    public Task<ReportResponseBase<PayrollDuplicateSSNResponseDto>> GetDuplicateSSNs(PaginationRequestDto req, CancellationToken ct)
    {
        return CallReportEndpoint<PayrollDuplicateSSNResponseDto>(req, "duplicate-ssns", ct);
    }

    public Task<Stream> DownloadDuplicateSsNs(CancellationToken cancellationToken)
    {
        return DownloadCsvReport("duplicate-ssns", cancellationToken);
    }

    public Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfit(PaginationRequestDto req, CancellationToken ct = default)
    {
        return CallReportEndpoint<DemographicBadgesNotInPayProfitResponse>(req, "demographic-badges-not-in-payprofit", ct);
    }

    #region Negative ETVA For SSNs On PayProfit

    public Task<ReportResponseBase<NegativeETVAForSSNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponse(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<NegativeETVAForSSNsOnPayProfitResponse>(req, "negative-evta-ssn", cancellationToken);
    }

    public Task<Stream> DownloadNegativeETVAForSSNsOnPayProfitResponse(CancellationToken cancellationToken = default)
    {
        return DownloadCsvReport("negative-evta-ssn", cancellationToken);
    }

    #endregion

    #region Mismatched Ssns Payprofit And Demographics On Same Badge
    public Task<ReportResponseBase<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>> GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<MismatchedSsnsPayprofitAndDemographicsOnSameBadgeResponseDto>(req, "mismatched-ssns-payprofit-and-demo-on-same-badge", cancellationToken);
    }

    public Task<Stream> DownloadMismatchedSsnsPayprofitAndDemographicsOnSameBadge(CancellationToken cancellationToken = default)
    {
        return DownloadCsvReport("mismatched-ssns-payprofit-and-demo-on-same-badge", cancellationToken);
    }

    public Task<ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>> GetPayProfitBadgesNotInDemographics(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<PayProfitBadgesNotInDemographicsResponse>(req, "payprofit-badges-without-demographics", cancellationToken);
    }

    #endregion

    #region Get Payroll Duplicate Ssns On Payprofit
    public Task<ReportResponseBase<PayrollDuplicateSsnsOnPayprofitResponseDto>> GetPayrollDuplicateSsnsOnPayprofit(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<PayrollDuplicateSsnsOnPayprofitResponseDto>(req, "payroll-duplicate-ssns-on-payprofit", cancellationToken);
    }

    public Task<Stream> DownloadPayrollDuplicateSsnsOnPayprofit(CancellationToken cancellationToken = default)
    {
        return DownloadCsvReport("payroll-duplicate-ssns-on-payprofit", cancellationToken);
    }

    #endregion

    private Task<Stream> DownloadCsvReport(string endpointRoute, CancellationToken cancellationToken)
    {
        UriBuilder uriBuilder = BuildPaginatedUrl(new PaginationRequestDto { Skip = 0, Take = int.MaxValue }, endpointRoute);
        return _httpDownloadClient.GetStreamAsync(uriBuilder.Uri, cancellationToken);
    }

    private async Task<ReportResponseBase<TResponseDto>> CallReportEndpoint<TResponseDto>(PaginationRequestDto req, string endpointRoute, CancellationToken cancellationToken) where TResponseDto : class
    {
        UriBuilder uriBuilder = BuildPaginatedUrl(req, endpointRoute);
        var response = await _httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
        _ = response.EnsureSuccessStatusCode();

        var rslt = await response.Content.ReadFromJsonAsync<ReportResponseBase<TResponseDto>>(_options, cancellationToken);
        return rslt ?? new ReportResponseBase<TResponseDto>
        {
            ReportName = Constants.ErrorMessages.ReportNotFound,
            ReportDate = SqlDateTime.MinValue.Value,
            Response = new PaginatedResponseDto<TResponseDto>()
        };
    }

    private UriBuilder BuildPaginatedUrl(PaginationRequestDto req, string endpointRoute)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (req.Skip.HasValue)
        {
            query["skip"] = req.Skip.Value.ToString();
        }

        if (req.Take.HasValue)
        {
            query["take"] = req.Take.Value.ToString();
        }

        var uriBuilder = new UriBuilder($"{_httpClient.BaseAddress}{BaseApiPath}/{endpointRoute}/")
        {
            Query = query.ToString()
        };
        return uriBuilder;
    }
}
