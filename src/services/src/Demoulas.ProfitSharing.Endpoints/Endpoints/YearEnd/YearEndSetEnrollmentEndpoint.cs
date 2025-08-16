using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;

public sealed class YearEndSetEnrollmentEndpoint: Endpoint<ProfitYearRequest>
{
    private readonly IYearEndService _yearEndService;

    public YearEndSetEnrollmentEndpoint(IYearEndService yearEndService)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        Post("update-enrollment");
        Summary(s =>
        {
            s.Summary = "Updates the enrollment id of all members for the year";
        });
        Policies(Security.Policy.CanRunYearEndProcesses);
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        await _yearEndService.UpdateEnrollmentId(req.ProfitYear, ct);
        await Send.NoContentAsync(ct);
    }
}
