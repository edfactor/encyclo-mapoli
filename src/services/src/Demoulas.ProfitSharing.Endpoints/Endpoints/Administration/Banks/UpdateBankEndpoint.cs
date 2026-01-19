using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Validators.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class UpdateBankEndpoint : ProfitSharingEndpoint<UpdateBankRequest, Results<Ok<BankDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IBankService _bankService;

    public UpdateBankEndpoint(IBankService bankService)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
    }

    public override void Configure()
    {
        Put("banks");
        Validator<UpdateBankRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Updates an existing bank.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Ok<BankDto>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(UpdateBankRequest req, CancellationToken ct)
    {
        var result = await _bankService.UpdateAsync(req, ct);
        return result.ToHttpResultWithValidation(Error.BankNotFound);
    }
}
