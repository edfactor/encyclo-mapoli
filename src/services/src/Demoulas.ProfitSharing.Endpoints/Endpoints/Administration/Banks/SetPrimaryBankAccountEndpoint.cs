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

public sealed class SetPrimaryBankAccountEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<bool>, NotFound, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly IAuditService _auditService;
    private readonly IAppUser _appUser;
    private readonly ILogger<SetPrimaryBankAccountEndpoint> _logger;

    public SetPrimaryBankAccountEndpoint(
        IBankAccountService bankAccountService,
        IAuditService auditService,
        IAppUser appUser,
        ILogger<SetPrimaryBankAccountEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
        _auditService = auditService;
        _appUser = appUser;
        _logger = logger;
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

    public override Task<Results<Ok<bool>, NotFound, ProblemHttpResult>> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");

        return this.ExecuteWithTelemetry(HttpContext, _logger, new { Id = id }, async () =>
        {
            var result = await _bankAccountService.SetPrimaryAsync(id, _auditService, _appUser, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-account-set-primary"),
                    new("endpoint", nameof(SetPrimaryBankAccountEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "bank-account"),
                    new("endpoint", nameof(SetPrimaryBankAccountEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResult(Error.BankAccountNotFound);
        });
    }
}
