using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class GetBankByIdEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<BankDto>, NotFound, ProblemHttpResult>>
{
    private readonly IBankService _bankService;

    public GetBankByIdEndpoint(IBankService bankService)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
    }

    public override void Configure()
    {
        Get("banks/{id}");
        Summary(s =>
        {
            s.Summary = "Gets a bank by ID.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<BankDto>, NotFound, ProblemHttpResult>> HandleRequestAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");
        var result = await _bankService.GetByIdAsync(id, ct);
        return result.ToHttpResult(Error.BankNotFound);
    }
}
