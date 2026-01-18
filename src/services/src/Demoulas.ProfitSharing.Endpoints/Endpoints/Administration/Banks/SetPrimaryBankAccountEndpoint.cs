using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class SetPrimaryBankAccountEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<bool>, NotFound, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly IAuditService _auditService;
    private readonly IAppUser _appUser;

    public SetPrimaryBankAccountEndpoint(
        IBankAccountService bankAccountService,
        IAuditService auditService,
        IAppUser appUser)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
        _auditService = auditService;
        _appUser = appUser;
    }

    public override void Configure()
    {
        Patch("bank-accounts/{id}/set-primary");
        Summary(s =>
        {
            s.Summary = "Sets a bank account as primary for its bank.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<bool>, NotFound, ProblemHttpResult>> HandleRequestAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");
        var result = await _bankAccountService.SetPrimaryAsync(id, _auditService, _appUser, ct);
        return result.ToHttpResult(Error.BankAccountNotFound);
    }
}
