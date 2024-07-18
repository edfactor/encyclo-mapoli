using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
public class GetDuplicateSSNsEndpoint : EndpointWithoutRequest<IList<PayrollDuplicateSSNResponseDto>>
{
    private readonly IYearEndService _yearEndService;

    public GetDuplicateSSNsEndpoint(IYearEndService yearEndService)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("duplicatessns");
        Summary(s =>
        {
            s.Summary = "Get SSNs that are duplicated in the demographics area";
        });
        Group<YearEndGroup>();
    }

    public override Task<IList<PayrollDuplicateSSNResponseDto>> ExecuteAsync(CancellationToken ct)
    {
        return _yearEndService.GetDuplicateSSNs(ct);
    }
}
