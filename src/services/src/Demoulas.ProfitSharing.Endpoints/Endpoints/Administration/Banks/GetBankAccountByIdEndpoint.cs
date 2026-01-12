using Demoulas.ProfitSharing.Common.Contracts;
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

public sealed class GetBankAccountByIdEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<BankAccountDto>, NotFound, ProblemHttpResult>>
{
    private readonly IBankAccountService _bankAccountService;
    private readonly ILogger<GetBankAccountByIdEndpoint> _logger;

    public GetBankAccountByIdEndpoint(IBankAccountService bankAccountService, ILogger<GetBankAccountByIdEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankAccountService = bankAccountService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("bank-accounts/{id}");
        Summary(s =>
        {
            s.Summary = "Gets a bank account by ID.";
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<BankAccountDto>, NotFound, ProblemHttpResult>> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");

        return this.ExecuteWithTelemetry(HttpContext, _logger, new { Id = id }, async () =>
        {
            var result = await _bankAccountService.GetByIdAsync(id, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-account-get-by-id"),
                    new("endpoint", nameof(GetBankAccountByIdEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResult(Error.BankAccountNotFound);
        }, "AccountNumber");
    }
}
