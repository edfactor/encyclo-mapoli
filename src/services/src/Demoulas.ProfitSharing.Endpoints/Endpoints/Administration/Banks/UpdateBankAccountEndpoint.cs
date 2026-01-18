using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Validators.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class UpdateBankAccountEndpoint : ProfitSharingEndpoint<UpdateBankAccountRequest, Results<Ok<BankAccountDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly IProfitSharingAuditService _profitSharingAuditService;
    private readonly IAppUser _appUser;

    public UpdateBankAccountEndpoint(
        IBankAccountService bankAccountService,
        IProfitSharingAuditService profitSharingAuditService,
        IAppUser appUser)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
        _profitSharingAuditService = profitSharingAuditService;
        _appUser = appUser;
    }

    public override void Configure()
    {
        Put("bank-accounts");
        Validator<UpdateBankAccountRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Updates an existing bank account.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<BankAccountDto>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(UpdateBankAccountRequest req, CancellationToken ct)
    {
        var result = await _bankAccountService.UpdateAsync(req, _profitSharingAuditService, _appUser, ct);
        return result.ToHttpResultWithValidation(Error.BankAccountNotFound);
    }
}
