using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;
public class YearEndProcessFinalRunEndpoint : Endpoint<YearRequest>
{
    private readonly IYearEndService _yearEndService;

    public YearEndProcessFinalRunEndpoint(IYearEndService yearEndService)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        Post("final");
        Summary(s =>
        {
            s.Summary = "Updates data in prior to final run of the Profit Sharing report";
        });
        Policies(Security.Policy.CanRunYearEndProcesses);
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(YearRequest req, CancellationToken ct)
    {
        await _yearEndService.RunFinalYearEndUpdates(req.ProfitYear, ct);
        await SendNoContentAsync(ct);
    }
}
