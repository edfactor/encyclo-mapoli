using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class GetBankAccountsEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<IReadOnlyList<BankAccountDto>>, NotFound, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly ILogger<GetBankAccountsEndpoint> _logger;

    public GetBankAccountsEndpoint(IBankAccountService bankAccountService, ILogger<GetBankAccountsEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("banks/{bankId}/accounts");
        Summary(s =>
        {
            s.Summary = "Gets all accounts for a specific bank.";
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<IReadOnlyList<BankAccountDto>>, NotFound, ProblemHttpResult>> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var bankId = Route<int>("bankId");

        return this.ExecuteWithTelemetry(HttpContext, _logger, new { BankId = bankId }, async () =>
        {
            var result = await _bankAccountService.GetByBankIdAsync(bankId, includeDisabled: true, ct);

            try
            {
                var count = result.Value?.Count ?? 0;

                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-accounts-list"),
                    new("endpoint", nameof(GetBankAccountsEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(count,
                    new("record_type", "bank-accounts"),
                    new("endpoint", nameof(GetBankAccountsEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResult();
        }, "AccountNumber"); // Mark account number as sensitive
    }
}
