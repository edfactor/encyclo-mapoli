using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public sealed class CertificatesReportEndpoint
    : ProfitSharingEndpoint<CerficatePrintRequest, Results<Ok<ReportResponseBase<CertificateReprintResponse>>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly ICertificateService _certificateService;

    public CertificatesReportEndpoint(ICertificateService certificateService)
        : base(Navigation.Constants.PrintProfitCerts)
    {
        _certificateService = certificateService;
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

    protected override async Task<Results<Ok<ReportResponseBase<CertificateReprintResponse>>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        CerficatePrintRequest req,
        CancellationToken ct)
    {
        var result = await _certificateService.GetMembersWithBalanceActivityByStore(req, ct);
        return result.ToHttpResultWithValidation();
    }
}
