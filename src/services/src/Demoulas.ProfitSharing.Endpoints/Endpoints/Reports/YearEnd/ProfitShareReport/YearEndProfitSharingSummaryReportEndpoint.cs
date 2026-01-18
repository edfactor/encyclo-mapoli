using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;

public sealed class YearEndProfitSharingSummaryReportEndpoint : ProfitSharingEndpoint<BadgeNumberRequest, Results<Ok<YearEndProfitSharingReportSummaryResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IProfitSharingSummaryReportService _cleanupReportService;
    private readonly IProfitSharingAuditService _profitSharingAuditService;

    public YearEndProfitSharingSummaryReportEndpoint(
        IProfitSharingSummaryReportService cleanupReportService,
        IProfitSharingAuditService profitSharingAuditService
    )
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _cleanupReportService = cleanupReportService;
        _profitSharingAuditService = profitSharingAuditService;
    }

    public override void Configure()
    {
        Post("yearend-profit-sharing-summary-report");
        Summary(s =>
        {
            s.Summary = "Yearend profit sharing summary report";
            s.Description = "Returns a breakdown of member counts/sum by various descriminators";
            s.ExampleRequest = new BadgeNumberRequest { ProfitYear = 2025, BadgeNumber = 723456, UseFrozenData = false };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    YearEndProfitSharingReportSummaryResponse.SampleResponse()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    protected override async Task<Results<Ok<YearEndProfitSharingReportSummaryResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(BadgeNumberRequest req, CancellationToken ct)
    {
        try
        {
            var additionalChecksums = new List<Func<YearEndProfitSharingReportSummaryResponse, (string, object)>>()
            {
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y => y.LineItemPrefix == "1");
                    return ("18_20WillVestMembers",lineItem?.NumberOfMembers ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y => y.LineItemPrefix == "1");
                    return ("18_20WillVestWages",lineItem?.TotalWages ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y => y.LineItemPrefix == "1");
                    return ("18_20WillVestBalance",lineItem?.TotalBalance ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y => y.LineItemPrefix == "2");
                    return ("Over21WillVestMembers",lineItem?.NumberOfMembers ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y => y.LineItemPrefix == "2");
                    return ("Over21WillVestWages",lineItem?.TotalWages ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y => y.LineItemPrefix == "2");
                    return ("Over21WillVestBalance",lineItem?.TotalBalance ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y => y.LineItemPrefix == "3");
                    return ("Under18Members",lineItem?.NumberOfMembers ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y => y.LineItemPrefix == "3");
                    return ("Under18Wages",lineItem?.TotalWages ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y => y.LineItemPrefix == "3");
                    return ("Under18Balance",lineItem?.TotalBalance ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="4");
                    return ("Ovr18WillNotVestPriorMembers", lineItem?.NumberOfMembers ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="4");
                    return ("Ovr18WillNotVestPriorWages", lineItem?.TotalWages ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="4");
                    return ("Ovr18WillNotVestPriorBalance", lineItem?.TotalBalance ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="5");
                    return ("Ovr18WillNotVestNoPriorMembers", lineItem?.NumberOfMembers ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="5");
                    return ("Ovr18WillNotVestNoPriorWages", lineItem?.TotalWages ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="5");
                    return ("Ovr18WillNotVestNoPriorBalance", lineItem?.TotalBalance ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="6");
                    return ("TermOvr18WouldVestMembers", lineItem?.NumberOfMembers ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="6");
                    return ("TermOvr18WouldVestWages", lineItem?.TotalWages ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="6");
                    return ("TermOvr18WouldVestBalance", lineItem?.TotalBalance ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="7");
                    return ("TermUndrWouldNotVestNoPriorMembers", lineItem?.NumberOfMembers ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="7");
                    return ("TermUndr18WouldNotVestNoPriorWages", lineItem?.TotalWages ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="7");
                    return ("TermUndr18WouldNotVestNoPriorBalance", lineItem?.TotalBalance ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="8");
                    return ("TermOvr18WouldNotVestNoPriorMembers", lineItem?.NumberOfMembers ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="8");
                    return ("TermOvr18WouldNotVestNoPriorWages", lineItem?.TotalWages ?? 0);
                },
                (x) =>
                {
                    var lineItem = x.LineItems.Find(y=>y.LineItemPrefix=="8");
                    return ("TermOvr18WouldNotVestNoPriorBalance", lineItem?.TotalBalance ?? 0);
                },
            };

            var reportSuffix = req.UseFrozenData ? "_FROZEN" : "";

            var data = await _profitSharingAuditService.ArchiveCompletedReportAsync(
                ReportNames.ProfitSharingSummary.ReportCode + reportSuffix,
                req.ProfitYear,
                req,
                (archiveReq, _, cancellationToken) => _cleanupReportService.GetYearEndProfitSharingSummaryReportAsync(req, ct),
                additionalChecksums,
                ct);

            // Always return Ok even if underlying collections are empty; absence of a single resource not applicable here.
            return Result<YearEndProfitSharingReportSummaryResponse>.Success(data).ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<YearEndProfitSharingReportSummaryResponse>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
