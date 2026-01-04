using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public sealed class CertificatesReportEndpoint : EndpointWithCsvBase<CerficatePrintRequest, CertificateReprintResponse, CertificatesReportEndpoint.CertificateReprintResponseMap>
{
    private readonly ICertificateService _certificateService;
    private readonly ILogger<CertificatesReportEndpoint> _logger;

    public CertificatesReportEndpoint(ICertificateService certificateService, ILogger<CertificatesReportEndpoint> logger) : base(Navigation.Constants.PrintProfitCerts)
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
                        ReportName = ReportFileName,
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
        base.Configure();
    }

    public override string ReportFileName => "Certificate Reprint";

    public override async Task<ReportResponseBase<CertificateReprintResponse>> GetResponse(CerficatePrintRequest req, CancellationToken ct)
    {
        (ReportResponseBase<CertificateReprintResponse>? response, ProblemDetails? problem) = await TryGetResponseAsync(req, ct);
        if (problem is null)
        {
            return response!;
        }

        // Fallback for direct calls (HandleAsync will send ProblemDetails when TryGetResponseAsync returns one)
        return new ReportResponseBase<CertificateReprintResponse>
        {
            ReportName = ReportFileName,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today),
            Response = new() { Results = [] }
        };
    }

    protected override async Task<(ReportResponseBase<CertificateReprintResponse>? Response, ProblemDetails? Problem)> TryGetResponseAsync(
        CerficatePrintRequest req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var serviceResult = await _certificateService.GetMembersWithBalanceActivityByStore(req, ct);

            if (!serviceResult.IsSuccess)
            {
                if (serviceResult.Error?.ValidationErrors?.Count > 0)
                {
                    string errorMessages = string.Join("; ",
                        serviceResult.Error.ValidationErrors.SelectMany(kvp => kvp.Value));

                    _logger.LogWarning("Certificate report generation failed validation for profit year {ProfitYear}: {ErrorMessage} (correlation: {CorrelationId})",
                        req.ProfitYear, errorMessages, HttpContext.TraceIdentifier);

                    var pd = new ValidationProblemDetails(serviceResult.Error.ValidationErrors)
                    {
                        Title = "Certificate report validation failed",
                        Detail = errorMessages,
                        Status = StatusCodes.Status400BadRequest,
                        Instance = HttpContext.Request?.Path.Value,
                    };
                    pd.Extensions["correlationId"] = HttpContext.TraceIdentifier;
                    return (null, pd);
                }

                _logger.LogError("Certificate report generation failed for profit year {ProfitYear}: {ErrorMessage} (correlation: {CorrelationId})",
                    req.ProfitYear, serviceResult.Error?.Description ?? "Unknown error", HttpContext.TraceIdentifier);

                var problem = new ProblemDetails
                {
                    Title = "Certificate report generation failed",
                    Detail = "Failed to generate certificate report.",
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = HttpContext.Request?.Path.Value,
                };
                problem.Extensions["correlationId"] = HttpContext.TraceIdentifier;
                return (null, problem);
            }

            var result = serviceResult.Value!;

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "certificates_report"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "post-frozen-certificates"),
                new("endpoint", "CertificatesReportEndpoint"));

            var badgeCount = req.BadgeNumbers?.Length ?? 0;
            if (badgeCount > 0)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(badgeCount,
                    new("operation", "certificates_report"),
                    new("metric_type", "badge_numbers_requested"));
            }

            _logger.LogInformation("Year-end post-frozen certificates report generated, returned {Count} certificate records for {BadgeCount} badge numbers (correlation: {CorrelationId})",
                resultCount, badgeCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return (result, null);
            }

            var emptyResult = new ReportResponseBase<CertificateReprintResponse>
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] }
            };

            this.RecordResponseMetrics(HttpContext, _logger, emptyResult);
            return (emptyResult, null);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);

            var problem = new ProblemDetails
            {
                Title = "Unexpected error generating certificate report",
                Detail = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Instance = HttpContext.Request?.Path.Value,
            };
            problem.Extensions["correlationId"] = HttpContext.TraceIdentifier;
            return (null, problem);
        }
    }

    public sealed class CertificateReprintResponseMap : CsvHelper.Configuration.ClassMap<CertificateReprintResponse>
    {
        public CertificateReprintResponseMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("Badge Number");
            Map(m => m.FullName).Index(1).Name("Full Name");
            Map(m => m.Street1).Index(2).Name("Address");
            Map(m => m.City).Index(3).Name("City");
            Map(m => m.State).Index(4).Name("State");
            Map(m => m.PostalCode).Index(5).Name("Zip");
            Map(m => m.PayClassificationName).Index(6).Name("Pay Classification");
            Map(m => m.BeginningBalance).Index(7).Name("Beginning Balance").TypeConverterOption.Format("C2");
            Map(m => m.Earnings).Index(8).Name("Earnings").TypeConverterOption.Format("C2");
            Map(m => m.Contributions).Index(9).Name("Contributions").TypeConverterOption.Format("C2");
            Map(m => m.Forfeitures).Index(10).Name("Forfeitures").TypeConverterOption.Format("C2");
            Map(m => m.Distributions).Index(11).Name("Distributions").TypeConverterOption.Format("C2");
            Map(m => m.EndingBalance).Index(12).Name("Ending Balance").TypeConverterOption.Format("C2");
            Map(m => m.VestedAmount).Index(13).Name("Vested Amount").TypeConverterOption.Format("C2");
            Map(m => m.VestedPercent).Index(14).Name("Vested %").TypeConverterOption.Format("P0");
            Map(m => m.DateOfBirth).Index(15).Name("Date of Birth").TypeConverterOption.Format("MM/dd/yyyy");
            Map(m => m.HireDate).Index(16).Name("Hire Date").TypeConverterOption.Format("MM/dd/yyyy");
            Map(m => m.TerminationDate).Index(17).Name("Termination Date").TypeConverterOption.Format("MM/dd/yyyy");
            Map(m => m.EnrollmentId).Index(18).Name("Enrollment ID");
            Map(m => m.ProfitShareHours).Index(19).Name("Profit Share Hours").TypeConverterOption.Format("N2");
            Map(m => m.CertificateSort).Index(20).Name("Certificate Sort");
            Map(m => m.AnnuitySingleRate).Index(21).Name("Annuity Single Rate").TypeConverterOption.Format("C2");
            Map(m => m.AnnuityJointRate).Index(22).Name("Annuity Joint Rate").TypeConverterOption.Format("C2");
            Map(m => m.MonthlyPaymentSingle).Index(23).Name("Monthly Payment Single").TypeConverterOption.Format("C2");
            Map(m => m.MonthlyPaymentJoint).Index(24).Name("Monthly Payment Joint").TypeConverterOption.Format("C2");
        }
    }
}
