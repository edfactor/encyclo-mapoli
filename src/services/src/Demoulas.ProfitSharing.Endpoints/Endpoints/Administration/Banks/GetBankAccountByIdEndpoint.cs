using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class GetBankAccountByIdEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<BankAccountDto>, NotFound, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;

    public GetBankAccountByIdEndpoint(IBankAccountService bankAccountService)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
    }

    public override void Configure()
    {
        Get("bank-accounts/{id}");
        Summary(s =>
        {
            s.Summary = "Gets a bank account by ID.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<BankAccountDto>, NotFound, ProblemHttpResult>> HandleRequestAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");
        var result = await _bankAccountService.GetByIdAsync(id, ct);
        return result.ToHttpResult(Error.BankAccountNotFound);
    }
}
