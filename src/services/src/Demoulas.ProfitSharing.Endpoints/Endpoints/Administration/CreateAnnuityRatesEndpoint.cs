using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

public sealed class CreateAnnuityRatesEndpoint
    : ProfitSharingEndpoint<CreateAnnuityRatesRequest, Results<Ok<IReadOnlyList<AnnuityRateDto>>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IAnnuityRatesService _annuityRatesService;
    private readonly ILogger<CreateAnnuityRatesEndpoint> _logger;

    public CreateAnnuityRatesEndpoint(IAnnuityRatesService annuityRatesService, ILogger<CreateAnnuityRatesEndpoint> logger)
        : base(Navigation.Constants.ManageAnnuityRates)
    {
        _annuityRatesService = annuityRatesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("annuity-rates");
        Summary(s =>
        {
            s.Summary = "Creates annuity rates for a new year.";
            s.ExampleRequest = CreateAnnuityRatesRequest.RequestExample();
        });
        Group<AdministrationGroup>();
    }

    protected override Task<Results<Ok<IReadOnlyList<AnnuityRateDto>>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(
        CreateAnnuityRatesRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _annuityRatesService.CreateAnnuityRatesAsync(req, ct);

            if (result.IsSuccess)
            {
                EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new("operation", "annuity-rate-create"),
                    new("endpoint", nameof(CreateAnnuityRatesEndpoint)),
                    new("year", req.Year.ToString()));

                EndpointTelemetry.RecordCountsProcessed.Record(result.Value?.Count ?? 0,
                    new("record_type", "annuity-rates"),
                    new("endpoint", nameof(CreateAnnuityRatesEndpoint)));
            }

            return result.ToHttpResultWithValidation();
        });
    }
}
