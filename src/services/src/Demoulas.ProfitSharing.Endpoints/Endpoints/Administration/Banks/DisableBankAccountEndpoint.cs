using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class DisableBankAccountEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly IAuditService _auditService;
    private readonly IAppUser _appUser;
    private readonly ILogger<DisableBankAccountEndpoint> _logger;

    public DisableBankAccountEndpoint(
        IBankAccountService bankAccountService,
        IAuditService auditService,
        IAppUser appUser,
        ILogger<DisableBankAccountEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
        _auditService = auditService;
        _appUser = appUser;
        _logger = logger;
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

    public override Task<Results<Ok<bool>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");

        return this.ExecuteWithTelemetry(HttpContext, _logger, new { Id = id }, async () =>
        {
            var result = await _bankAccountService.DisableAsync(id, _auditService, _appUser, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-account-disable"),
                    new("endpoint", nameof(DisableBankAccountEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "bank-account"),
                    new("endpoint", nameof(DisableBankAccountEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResultWithValidation(Error.BankAccountNotFound);
        });
    }
}
