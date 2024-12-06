using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ExecutiveHoursAndDollars;

public class SetExecutiveHoursAndDollarsEndpoint : Endpoint<SetExecutiveHoursAndDollarsRequest>
{
    private readonly IExecutiveHoursAndDollarsService ExecutiveHoursAndDollarsService;

    public SetExecutiveHoursAndDollarsEndpoint(IExecutiveHoursAndDollarsService executiveHoursAndDollarsService)
    {
        ExecutiveHoursAndDollarsService = executiveHoursAndDollarsService;
    }

    public override void Configure()
    {
        Put("/executive-hours-and-dollars");
        Group<YearEndGroup>();
        Summary(s =>
        {
            s.Summary = "Executive Hours and Dollars Endpoint";
            s.Description =
                "This endpoint allows the executive hours and dollars to be set.";

            s.ExampleRequest = SetExecutiveHoursAndDollarsRequest.Example();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    204, "Success, No Content"
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
    }

    public override async Task<object?> ExecuteAsync(SetExecutiveHoursAndDollarsRequest req, CancellationToken ct)
    {
        // Effect the change
        await ExecutiveHoursAndDollarsService.SetExecutiveHoursAndDollarsAsync(req.ProfitYear, req.ExecutiveHoursAndDollars, ct);
        // Tell the Client everything good.
        // The Client gets a http status 204 (HttpStatusCode.NoContent), which means everything was peachy but there is no data in the response, which is fine for a PUT
        await SendNoContentAsync(ct);
        return null;
    }

}
