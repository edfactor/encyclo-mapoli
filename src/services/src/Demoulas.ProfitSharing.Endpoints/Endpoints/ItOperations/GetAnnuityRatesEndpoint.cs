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

public sealed class GetAnnuityRatesEndpoint : ProfitSharingResultResponseEndpoint<IReadOnlyList<AnnuityRateDto>>
{
    private readonly IAnnuityRatesService _annuityRatesService;
    private readonly ILogger<GetAnnuityRatesEndpoint> _logger;

    public GetAnnuityRatesEndpoint(IAnnuityRatesService annuityRatesService, ILogger<GetAnnuityRatesEndpoint> logger)
        : base(Navigation.Constants.ManageAnnuityRates)
    {
        _annuityRatesService = annuityRatesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("annuity-rates");
        Summary(s =>
        {
            s.Summary = "Gets all annuity rates.";
        });
        Group<ItDevOpsGroup>();
    }

    public override Task<Results<Ok<IReadOnlyList<AnnuityRateDto>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, new { }, async () =>
        {
            var result = await _annuityRatesService.GetAnnuityRatesAsync(ct);

            try
            {
                var count = result.Value?.Count ?? 0;

                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "annuity-rates-list"),
                    new("endpoint", nameof(GetAnnuityRatesEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(count,
                    new("record_type", "annuity-rates"),
                    new("endpoint", nameof(GetAnnuityRatesEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResult();
        });
    }
}
