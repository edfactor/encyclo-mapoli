using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

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
        Put("/executive-hours");
        Summary(s =>
        {
            s.Summary = "Update executive hours and dollars";
            s.Description = "Updates executive hours and dollars allocations for a profit year. Requires FINANCEMANAGER role.";

            s.ExampleRequest = SetExecutiveHoursAndDollarsRequest.RequestExample();

            s.Responses[204] = "Success, No Content";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });

        Group<YearEndGroup>();
    }

    protected override Task HandleRequestAsync(SetExecutiveHoursAndDollarsRequest req, CancellationToken ct)
    {
        return _executiveHoursAndDollarsService.SetExecutiveHoursAndDollarsAsync(req.ProfitYear, req.ExecutiveHoursAndDollars, ct);
    }
}
