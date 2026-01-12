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

public sealed class GetBankByIdEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<BankDto>, NotFound, ProblemHttpResult>>
{
    private readonly IBankService _bankService;
    private readonly ILogger<GetBankByIdEndpoint> _logger;

    public GetBankByIdEndpoint(IBankService bankService, ILogger<GetBankByIdEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("banks/{id}");
        Summary(s =>
        {
            s.Summary = "Gets a bank by ID.";
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<BankDto>, NotFound, ProblemHttpResult>> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");

        return this.ExecuteWithTelemetry(HttpContext, _logger, new { Id = id }, async () =>
        {
            var result = await _bankService.GetByIdAsync(id, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-get-by-id"),
                    new("endpoint", nameof(GetBankByIdEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResult(Error.BankNotFound);
        });
    }
}
