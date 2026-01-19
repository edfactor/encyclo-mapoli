using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class GetAllBanksEndpoint : ProfitSharingResultResponseEndpoint<IReadOnlyList<BankDto>>
{
    private readonly IBankService _bankService;

    public GetAllBanksEndpoint(IBankService bankService)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
    }

    public override void Configure()
    {
        Get("banks");
        Summary(s =>
        {
            s.Summary = "Gets all banks, optionally including disabled banks.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<IReadOnlyList<BankDto>>, NotFound, ProblemHttpResult>> HandleRequestAsync(CancellationToken ct)
    {
        var result = await _bankService.GetAllAsync(includeDisabled: true, ct);
        return result.ToHttpResult();
    }
}
