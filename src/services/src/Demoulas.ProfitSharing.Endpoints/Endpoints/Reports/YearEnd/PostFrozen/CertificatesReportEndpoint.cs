using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public sealed class CertificatesReportEndpoint
    : ProfitSharingEndpoint<CerficatePrintRequest, Results<Ok<ReportResponseBase<CertificateReprintResponse>>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly ICertificateService _certificateService;
    private readonly ILogger<CertificatesReportEndpoint> _logger;

    public CertificatesReportEndpoint(ICertificateService certificateService, ILogger<CertificatesReportEndpoint> logger)
        : base(Navigation.Constants.PrintProfitCerts)
    {
        _certificateService = certificateService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("post-frozen/certificates");
        Summary(s =>
        {
            s.Summary = "Returns base data for the certificates to be printed";
            s.Description = "Returns a list of employees who are to receive certificates.  This is typically used to reprint lost certificates.";
            s.ExampleRequest = new CerficatePrintRequest
            {
                ProfitYear = 2025,
                BadgeNumbers = new int[] { 12345, 23456 }
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<CertificateReprintResponse>
                    {
                        ReportName = "Certificate Reprint",
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<CertificateReprintResponse>
                        {
                            Results = new List<CertificateReprintResponse> { CertificateReprintResponse.ResponseExample() }
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
    }

    public override Task<Results<Ok<ReportResponseBase<CertificateReprintResponse>>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(
        CerficatePrintRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _certificateService.GetMembersWithBalanceActivityByStore(req, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "certificates_report"),
                new("profit_year", req.ProfitYear.ToString()));

            if (result.IsSuccess)
            {
                var response = result.Value;
                var resultCount = response?.Response?.Results?.Count() ?? 0;
                EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                    new("record_type", "post-frozen-certificates"),
                    new("endpoint", nameof(CertificatesReportEndpoint)));

                var badgeCount = req.BadgeNumbers?.Length ?? 0;
                if (badgeCount > 0)
                {
                    EndpointTelemetry.RecordCountsProcessed.Record(badgeCount,
                        new("operation", "certificates_report"),
                        new("metric_type", "badge_numbers_requested"));
                }
            }

            return result.ToHttpResultWithValidation();
        });
    }
}
