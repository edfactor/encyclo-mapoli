using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public sealed class GetStateTaxRatesEndpoint : ProfitSharingResultResponseEndpoint<IReadOnlyList<StateTaxRateDto>>
{
    private readonly IStateTaxRatesService _stateTaxRatesService;
    private readonly ILogger<GetStateTaxRatesEndpoint> _logger;

    public GetStateTaxRatesEndpoint(IStateTaxRatesService stateTaxRatesService, ILogger<GetStateTaxRatesEndpoint> logger)
        : base(Navigation.Constants.ManageStateTaxRates)
    {
        _stateTaxRatesService = stateTaxRatesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("state-tax-rates");
        Summary(s =>
        {
            s.Summary = "Gets all state tax rates.";
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<IReadOnlyList<StateTaxRateDto>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, new { }, async () =>
        {
            var result = await _stateTaxRatesService.GetStateTaxRatesAsync(ct);

            try
            {
                var count = result.Value?.Count ?? 0;

                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "state-tax-rates-list"),
                    new("endpoint", nameof(GetStateTaxRatesEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(count,
                    new("record_type", "state-tax-rates"),
                    new("endpoint", nameof(GetStateTaxRatesEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResult();
        });
    }
}
