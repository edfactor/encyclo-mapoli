using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ExecutiveHoursAndDollars;

public class SetExecutiveHoursAndDollarsEndpoint : ProfitSharingRequestEndpoint<SetExecutiveHoursAndDollarsRequest>
{
    private readonly IExecutiveHoursAndDollarsService _executiveHoursAndDollarsService;

    public SetExecutiveHoursAndDollarsEndpoint(IExecutiveHoursAndDollarsService executiveHoursAndDollarsService) : base(Navigation.Constants.ManageExecutiveHours)
    {
        _executiveHoursAndDollarsService = executiveHoursAndDollarsService;
    }

    public override void Configure()
    {
        Put("executive-hours-and-dollars");
        Summary(s =>
        {
            s.Summary = "Executive Hours and Dollars Endpoint";
            s.Description =
                "This endpoint allows the executive hours and dollars to be set.";

            s.ExampleRequest = SetExecutiveHoursAndDollarsRequest.RequestExample();

            s.Responses[204] = "Success, No Content";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });

        Group<YearEndGroup>();
    }

    public override Task HandleAsync(SetExecutiveHoursAndDollarsRequest req, CancellationToken ct)
    {
        return _executiveHoursAndDollarsService.SetExecutiveHoursAndDollarsAsync(req.ProfitYear, req.ExecutiveHoursAndDollars, ct);
    }
}
