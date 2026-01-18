using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Validators.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class CreateBankAccountEndpoint : ProfitSharingEndpoint<CreateBankAccountRequest, Results<Created<BankAccountDto>, BadRequest, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly IProfitSharingAuditService _profitSharingAuditService;
    private readonly IAppUser _appUser;

    public CreateBankAccountEndpoint(
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
        Post("bank-accounts");
        Validator<CreateBankAccountRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Creates a new bank account.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Created<BankAccountDto>, BadRequest, ProblemHttpResult>> HandleRequestAsync(CreateBankAccountRequest req, CancellationToken ct)
    {
        var result = await _bankAccountService.CreateAsync(req, _profitSharingAuditService, _appUser, ct);
        return result.Match<Results<Created<BankAccountDto>, BadRequest, ProblemHttpResult>>(
            success => TypedResults.Created($"/api/administration/banks/{success.BankId}/accounts/{success.Id}", success),
            error => error.Status == StatusCodes.Status400BadRequest
                ? TypedResults.BadRequest()
                : TypedResults.Problem(error.Detail));
    }
}
