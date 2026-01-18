using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class DistributionsAndForfeitureEndpoint : ProfitSharingEndpoint<DistributionsAndForfeituresRequest, Results<Ok<DistributionsAndForfeitureTotalsResponse>, NotFound, ProblemHttpResult>>
{
    private readonly ICleanupReportService _cleanupReportService;
    private readonly IProfitSharingAuditService _profitSharingAuditService;

    public DistributionsAndForfeitureEndpoint(ICleanupReportService cleanupReportService, IProfitSharingAuditService profitSharingAuditService)
        : base(Navigation.Constants.DistributionsAndForfeitures)
    {
        _cleanupReportService = cleanupReportService;
        _profitSharingAuditService = profitSharingAuditService;
    }

    public override void Configure()
    {
        Post("distributions-and-forfeitures");
        Summary(s =>
        {
            s.Summary = "Lists distributions and forfeitures for a date range";
            s.ExampleRequest = new DistributionsAndForfeituresRequest() { Skip = 0, Take = 100 };
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, DistributionsAndForfeitureTotalsResponse.ResponseExample() }
            };
        });
        Group<YearEndGroup>();
    }

    protected override async Task<Results<Ok<DistributionsAndForfeitureTotalsResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(DistributionsAndForfeituresRequest req, CancellationToken ct)
    {
        // Extract profit year from EndDate (or use current year if not provided)
        var profitYear = (req.EndDate?.Year ?? DateTime.Now.Year) + 1; //Report is run for the prior year.

        // Wrap service call with audit archiving
        var serviceResult = await _cleanupReportService.GetDistributionsAndForfeitureAsync(req, ct);

        // Check for errors before archiving
        if (serviceResult.IsError)
        {
            return serviceResult.ToHttpResult(Error.NoPayProfitsDataAvailable);
        }

        // Archive the successful result
        var result = await _profitSharingAuditService.ArchiveCompletedReportAsync<DistributionsAndForfeituresRequest, DistributionsAndForfeitureTotalsResponse>(
            ReportNames.DistributionAndForfeitures.ReportCode,
            (short)profitYear,
            req,
            async (archiveReq, _, cancellationToken) =>
            {
                var archiveServiceResult = await _cleanupReportService.GetDistributionsAndForfeitureAsync(archiveReq, cancellationToken);
                return archiveServiceResult.Value!;
            },
            ct);

        if (result.Response?.Results is not null)
        {
            return Result<DistributionsAndForfeitureTotalsResponse>.Success(result).ToHttpResult();
        }

        return Result<DistributionsAndForfeitureTotalsResponse>.Failure(Error.NoPayProfitsDataAvailable).ToHttpResult();
    }
}
