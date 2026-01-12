using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Validators.Administration;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class CreateBankAccountEndpoint : ProfitSharingEndpoint<CreateBankAccountRequest, Results<Created<BankAccountDto>, BadRequest, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly IAuditService _auditService;
    private readonly IAppUser _appUser;
    private readonly ILogger<CreateBankAccountEndpoint> _logger;

    public CreateBankAccountEndpoint(
        IBankAccountService bankAccountService,
        IAuditService auditService,
        IAppUser appUser,
        ILogger<CreateBankAccountEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
        _auditService = auditService;
        _appUser = appUser;
        _logger = logger;
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

    public override Task<Results<Created<BankAccountDto>, BadRequest, ProblemHttpResult>> ExecuteAsync(CreateBankAccountRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _bankAccountService.CreateAsync(req, _auditService, _appUser, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-account-create"),
                    new("endpoint", nameof(CreateBankAccountEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "bank-account"),
                    new("endpoint", nameof(CreateBankAccountEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.Match<Results<Created<BankAccountDto>, BadRequest, ProblemHttpResult>>(
                success => TypedResults.Created($"/api/administration/banks/{success.BankId}/accounts/{success.Id}", success),
                error => error.Status == StatusCodes.Status400BadRequest
                    ? TypedResults.BadRequest()
                    : TypedResults.Problem(error.Detail)
            );
        }, "AccountNumber");
    }
}
