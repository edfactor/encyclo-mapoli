using Demoulas.ProfitSharing.Common.Contracts;
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

public sealed class DisableBankEndpoint : ProfitSharingEndpoint<EmptyRequest, Results<Ok<bool>, NotFound, ProblemHttpResult>>
{
    private readonly IBankService _bankService;
    private readonly ILogger<DisableBankEndpoint> _logger;

    public DisableBankEndpoint(IBankService bankService, ILogger<DisableBankEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("banks/{id}");
        Summary(s =>
        {
            s.Summary = "Disables a bank (soft delete).";
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<bool>, NotFound, ProblemHttpResult>> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");

        return this.ExecuteWithTelemetry(HttpContext, _logger, new { Id = id }, async () =>
        {
            var result = await _bankService.DisableAsync(id, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "bank-disable"),
                    new("endpoint", nameof(DisableBankEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "bank"),
                    new("endpoint", nameof(DisableBankEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResult(Error.BankNotFound);
        });
    }
}
