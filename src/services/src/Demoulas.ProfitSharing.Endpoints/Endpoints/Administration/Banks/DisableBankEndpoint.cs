using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class DisableBankEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<bool>, NotFound, ProblemHttpResult>>
{
    private readonly IBankService _bankService;

    public DisableBankEndpoint(IBankService bankService)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
    }

    public override void Configure()
    {
        Delete("banks/{id}");
        Summary(s =>
        {
            s.Summary = "Disables a bank (soft delete).";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<bool>, NotFound, ProblemHttpResult>> HandleRequestAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");
        var result = await _bankService.DisableAsync(id, ct);
        return result.ToHttpResult(Error.BankNotFound);
    }
}
