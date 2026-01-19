using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class GetBankAccountsEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<IReadOnlyList<BankAccountDto>>, NotFound, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;

    public GetBankAccountsEndpoint(IBankAccountService bankAccountService)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
    }

    public override void Configure()
    {
        Get("banks/{bankId}/accounts");
        Summary(s =>
        {
            s.Summary = "Gets all accounts for a specific bank.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<IReadOnlyList<BankAccountDto>>, NotFound, ProblemHttpResult>> HandleRequestAsync(EmptyRequest req, CancellationToken ct)
    {
        var bankId = Route<int>("bankId");
        var result = await _bankAccountService.GetByBankIdAsync(bankId, includeDisabled: true, ct);
        return result.ToHttpResult();
    }
}
