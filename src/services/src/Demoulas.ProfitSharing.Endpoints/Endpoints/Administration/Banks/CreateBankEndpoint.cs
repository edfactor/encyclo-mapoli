using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Validators.Administration;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class CreateBankEndpoint : ProfitSharingEndpoint<CreateBankRequest, Results<Created<BankDto>, BadRequest, ProblemHttpResult>>
{
    private readonly IBankService _bankService;

    public CreateBankEndpoint(IBankService bankService)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
    }

    public override void Configure()
    {
        Post("banks");
        Validator<CreateBankRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Creates a new bank.";
        });
        Group<AdministrationGroup>();
    }

    protected override async Task<Results<Created<BankDto>, BadRequest, ProblemHttpResult>> HandleRequestAsync(CreateBankRequest req, CancellationToken ct)
    {
        var result = await _bankService.CreateAsync(req, ct);
        return result.Match<Results<Created<BankDto>, BadRequest, ProblemHttpResult>>(
            success => TypedResults.Created($"/api/administration/banks/{success.Id}", success),
            error => error.Status == StatusCodes.Status400BadRequest
                ? TypedResults.BadRequest()
                : TypedResults.Problem(error.Detail));
    }
}
