using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;

public sealed class YearEndSetEnrollmentEndpoint : ProfitSharingRequestEndpoint<ProfitYearRequest>
{
    private readonly IYearEndService _yearEndService;

    public YearEndSetEnrollmentEndpoint(IYearEndService yearEndService)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        Post("update-enrollment");
        Summary(s => { s.Summary = "Updates the enrollment id of all members for the year"; });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        await _yearEndService.UpdateEnrollmentId(req.ProfitYear, ct);
        await Send.NoContentAsync(ct);
    }
}
