using System.Data.SqlTypes;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Client.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Client.Reports.YearEnd;
public sealed class CleanupReportClient : ClientBase, ICleanupReportService
{
    private const string BaseApiPath = "api/yearend";

    private readonly HttpClient _httpClient;
    private readonly HttpClient _httpDownloadClient;
    private readonly JsonSerializerOptions _options;

    public CleanupReportClient(IHttpClientFactory httpClientFactory) :
        this(httpClientFactory.CreateClient(Constants.Http.ReportsHttpClient),
            httpClientFactory.CreateClient(Constants.Http.ReportsDownloadClient))
    {
        
    }
    public CleanupReportClient(HttpClient? jsonClient, HttpClient? fileDownloadClient) : base(jsonClient, fileDownloadClient)
    {
        ArgumentNullException.ThrowIfNull(jsonClient);
        ArgumentNullException.ThrowIfNull(fileDownloadClient);
        _httpClient = jsonClient;
        _httpDownloadClient = fileDownloadClient;
        _options = Constants.GetJsonSerializerOptions();
    }

    public Task<ReportResponseBase<PayrollDuplicateSsnResponseDto>> GetDuplicateSsnAsync(ProfitYearRequest req, CancellationToken ct)
    {
        return CallReportEndpoint<PayrollDuplicateSsnResponseDto, ProfitYearRequest>(req, "duplicate-ssns", ct);
    }

    public Task<Stream> DownloadDuplicateSsNs(short profitYear, CancellationToken cancellationToken)
    {
        return DownloadCsvReport(profitYear,"duplicate-ssns", cancellationToken);
    }

    public Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetDemographicBadgesNotInPayProfitAsync(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<DemographicBadgesNotInPayProfitResponse, PaginationRequestDto>(req, "demographic-badges-not-in-payprofit", cancellationToken);
    }

    public Task<Stream> DownloadDemographicBadgesNotInPayProfit(CancellationToken ct = default)
    {
        return DownloadCsvReport(0, "demographic-badges-not-in-payprofit", ct);
    }

    #region Negative ETVA For SSNs On PayProfit

    public Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetNegativeETVAForSSNsOnPayProfitResponseAsync(ProfitYearRequest req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<NegativeEtvaForSsNsOnPayProfitResponse, ProfitYearRequest>(req, "negative-evta-ssn", cancellationToken);
    }

    public Task<Stream> DownloadNegativeETVAForSSNsOnPayProfitResponse(short profitYear, CancellationToken cancellationToken = default)
    {
        return DownloadCsvReport(profitYear,"negative-evta-ssn", cancellationToken);
    }

    #endregion
    

    public Task<ReportResponseBase<NamesMissingCommaResponse>> GetNamesMissingCommaAsync(PaginationRequestDto req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<NamesMissingCommaResponse, PaginationRequestDto>(req, "names-missing-commas", cancellationToken);
    }

    public Task<Stream> DownloadNamesMissingComma(CancellationToken cancellationToken = default)
    {
        return DownloadCsvReport(0, "names-missing-commas", cancellationToken);
    }

    public Task<ReportResponseBase<DuplicateNamesAndBirthdaysResponse>> GetDuplicateNamesAndBirthdaysAsync(ProfitYearRequest req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<DuplicateNamesAndBirthdaysResponse, ProfitYearRequest>(req, "duplicate-names-and-birthdays", cancellationToken);
    }

    public Task<Stream> DownloadDuplicateNamesAndBirthdays(short profitYear, CancellationToken cancellationToken = default)
    {
        return DownloadCsvReport(profitYear, "duplicate-names-and-birthdays", cancellationToken);
    }

    public Task<ReportResponseBase<DistributionsAndForfeitureResponse>> GetDistributionsAndForfeitureAsync(DistributionsAndForfeituresRequest req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<DistributionsAndForfeitureResponse, DistributionsAndForfeituresRequest>(req, "distributions-and-forfeitures", cancellationToken);
    }

    public Task<ReportResponseBase<YearEndProfitSharingReportResponse>> GetYearEndProfitSharingReportAsync(YearEndProfitSharingReportRequest req, CancellationToken cancellationToken = default)
    {
        return CallReportEndpoint<YearEndProfitSharingReportResponse, YearEndProfitSharingReportRequest>(req, "yearend-profit-sharing-report", cancellationToken);
    }

    private async Task<ReportResponseBase<TResponseDto>> CallReportEndpoint<TResponseDto, TPaginatedRequest>(TPaginatedRequest req, string endpointRoute, CancellationToken cancellationToken) where TResponseDto : class where TPaginatedRequest : PaginationRequestDto
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
    private Task<Stream> DownloadCsvReport(short profitYear, string endpointRoute, CancellationToken cancellationToken)
    {
        UriBuilder uriBuilder = BuildPaginatedUrl(new ProfitYearRequest {ProfitYear = profitYear, Skip = 0, Take = int.MaxValue }, endpointRoute);
        return _httpDownloadClient.GetStreamAsync(uriBuilder.Uri, cancellationToken);
    }

    private UriBuilder BuildPaginatedUrl<TPaginatedRequest>(TPaginatedRequest req, string endpointRoute) where TPaginatedRequest : PaginationRequestDto
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

        if (req is ProfitYearRequest preq)
        {
            query[nameof(ProfitYearRequest.ProfitYear)] = preq.ProfitYear.ToString();
        }

        if (req is YearEndProfitSharingReportRequest yreq)
        {
            query[nameof(YearEndProfitSharingReportRequest.IsYearEnd)] = yreq.IsYearEnd.ToString();
        }

        if (req is DistributionsAndForfeituresRequest dafr)
        {
            if (dafr.StartMonth.HasValue)
            {
                query[nameof(DistributionsAndForfeituresRequest.StartMonth)] = dafr.StartMonth.ToString();
            }

            if (dafr.EndMonth.HasValue)
            {
                query[nameof(DistributionsAndForfeituresRequest.EndMonth)] = dafr.EndMonth.ToString();
            }
            if (dafr.IncludeOutgoingForfeitures.HasValue)
            {
                query[nameof(DistributionsAndForfeituresRequest.IncludeOutgoingForfeitures)] = dafr.IncludeOutgoingForfeitures.ToString();
            }
        }

        var uriBuilder = new UriBuilder($"{_httpClient.BaseAddress}{BaseApiPath}/{endpointRoute}/")
        {
            Query = query.ToString()
        };
        return uriBuilder;
    }
}
