using Demoulas.ProfitSharing.Common.Contracts.Response.Administration;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.Administration;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.Banks;

public sealed class GetAllBanksEndpoint : ProfitSharingResultResponseEndpoint<IReadOnlyList<BankDto>>
{
    private readonly IBankService _bankService;
    private readonly ILogger<GetAllBanksEndpoint> _logger;

    public GetAllBanksEndpoint(IBankService bankService, ILogger<GetAllBanksEndpoint> logger)
        : base(Navigation.Constants.ManageBanks)
    {
        _bankService = bankService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("banks");
        Summary(s =>
        {
            s.Summary = "Gets all banks, optionally including disabled banks.";
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<IReadOnlyList<BankDto>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, new { }, async () =>
        {
            var result = await _bankService.GetAllAsync(includeDisabled: true, ct);

            try
            {
                var count = result.Value?.Count ?? 0;

                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "banks-list"),
                    new("endpoint", nameof(GetAllBanksEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(count,
                    new("record_type", "banks"),
                    new("endpoint", nameof(GetAllBanksEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResult();
        });
    }
}
