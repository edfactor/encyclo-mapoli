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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class UpdateBankAccountEndpoint : ProfitSharingEndpoint<UpdateBankAccountRequest, Results<Ok<BankAccountDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly IAuditService _auditService;
    private readonly IAppUser _appUser;
    private readonly ILogger<UpdateBankAccountEndpoint> _logger;

    public UpdateBankAccountEndpoint(
        IBankAccountService bankAccountService,
        IAuditService auditService,
        IAppUser appUser,
        ILogger<UpdateBankAccountEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
        _auditService = auditService;
        _appUser = appUser;
        _logger = logger;
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

    public override Task<Results<Ok<BankAccountDto>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(UpdateBankAccountRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _bankAccountService.UpdateAsync(req, _auditService, _appUser, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-account-update"),
                    new("endpoint", nameof(UpdateBankAccountEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "bank-account"),
                    new("endpoint", nameof(UpdateBankAccountEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResultWithValidation(Error.BankAccountNotFound);
        }, "AccountNumber");
    }
}
