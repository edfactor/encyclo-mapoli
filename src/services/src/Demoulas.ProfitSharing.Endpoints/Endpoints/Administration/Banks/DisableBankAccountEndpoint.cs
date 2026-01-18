using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class DisableBankAccountEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly IAuditService _auditService;
    private readonly IAppUser _appUser;

    public DisableBankAccountEndpoint(
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
        Delete("bank-accounts/{id}");
        Summary(s =>
        {
            s.Summary = "Disables a bank account (soft delete). Cannot disable primary accounts.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");
        var result = await _bankAccountService.DisableAsync(id, _auditService, _appUser, ct);
        return result.ToHttpResultWithValidation(Error.BankAccountNotFound);
    }
}
