using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

public class AdhocBeneficiariesReportEndpoint : EndpointWithCsvTotalsBase<AdhocBeneficiariesReportRequest,
    AdhocBeneficiariesReportResponse,
    BeneficiaryReportDto,
    AdhocBeneficiariesReportEndpoint.AdhocBeneficiariesReportResponseMap>
{
    private readonly IAdhocBeneficiariesReport _reportService;
    private readonly ILogger<AdhocBeneficiariesReportEndpoint> _logger;

    public AdhocBeneficiariesReportEndpoint(IAdhocBeneficiariesReport reportService, ILogger<AdhocBeneficiariesReportEndpoint> logger)
        : base(Navigation.Constants.QPAY066AdHocReports)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("adhoc-beneficiaries-report");
        Summary(s =>
        {
            s.Summary = "Adhoc Beneficiaries Report";
            s.Description = "Returns a report of employee and non-employee beneficiaries, with optional detail lines.";
            s.ExampleRequest = new AdhocBeneficiariesReportRequest(true);
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new AdhocBeneficiariesReportResponse
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.MinValue,
                        EndDate = DateOnly.MaxValue,
                        TotalEndingBalance = 10000,
                        Response = new PaginatedResponseDto<BeneficiaryReportDto>
                        {
                            Results = new List<BeneficiaryReportDto>()
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "AdhocBeneficiariesReport";

    public override async Task<AdhocBeneficiariesReportResponse> GetResponse(AdhocBeneficiariesReportRequest req, CancellationToken ct)
    {
        var result = await _reportService.GetAdhocBeneficiariesReportAsync(req, ct);

        // Record business metrics for adhoc beneficiaries report
        Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new KeyValuePair<string, object?>("operation", "adhoc-beneficiaries-report"),
            new KeyValuePair<string, object?>("endpoint.category", "reports"));

        // Record result count
        var resultCount = result?.Response?.Results?.Count() ?? 0;
        Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
            new KeyValuePair<string, object?>("operation", "adhoc-beneficiaries-report"),
            new KeyValuePair<string, object?>("endpoint.category", "reports"));

        _logger.LogInformation("Adhoc beneficiaries report generated, returned {Count} beneficiaries, total balance: {TotalBalance} (correlation: {CorrelationId})",
            resultCount, result?.TotalEndingBalance, HttpContext.TraceIdentifier);

        return result ?? new AdhocBeneficiariesReportResponse
        {
            ReportName = ReportFileName,
            ReportDate = DateTimeOffset.Now,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today),
            Response = new PaginatedResponseDto<BeneficiaryReportDto> { Results = [] }
        };
    }



    public sealed class AdhocBeneficiariesReportResponseMap : ClassMap<BeneficiaryReportDto>
    {
        public AdhocBeneficiariesReportResponseMap()
        {
            Map(m => m.BeneficiaryId).Name("Beneficiary Id");
            Map(m => m.FullName).Name("Full Name");
            Map(m => m.Ssn).Name("SSN");
            Map(m => m.Relationship).Name("Relationship");
            Map(m => m.Balance).Name("Balance");
            Map(m => m.BadgeNumber).Name("Badge Number");
        }
    }
}
