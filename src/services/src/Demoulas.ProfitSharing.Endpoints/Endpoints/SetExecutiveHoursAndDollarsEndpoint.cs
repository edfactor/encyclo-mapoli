using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Services;

public class SetExecutiveHoursAndDollarsEndpoint : Endpoint<SetExecutiveHoursAndDollarsRequest>
{
    private readonly ISetExecutiveHoursAndDollarsService SetExecutiveHoursAndDollarsService;

    public SetExecutiveHoursAndDollarsEndpoint(ISetExecutiveHoursAndDollarsService setExecutiveHoursAndDollarsService)
    {
        SetExecutiveHoursAndDollarsService = setExecutiveHoursAndDollarsService;
    }

    public override void Configure()
    {
        Verbs(Http.PUT);
        Routes("/executive-hours-and-dollars");
        AllowAnonymous();  // FIX ME
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(SetExecutiveHoursAndDollarsRequest req, CancellationToken ct)
    {
        try
        {
            await SetExecutiveHoursAndDollarsService.SetExecutiveHoursAndDollars(req.ProfitYear, req.ExecutiveHoursAndDollars);
            await SendNoContentAsync(ct);
        }
        catch (InvalidOperationException ex)
        {
            await SendAsync(new { message = ex.Message }, 400, ct);
        }
    }
}
