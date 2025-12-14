using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public sealed class UpdateStateTaxRateEndpoint : ProfitSharingEndpoint<UpdateStateTaxRateRequest, Results<Ok<StateTaxRateDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private static readonly Error s_stateTaxNotFound = Error.EntityNotFound("State tax");

    private readonly IStateTaxRatesService _stateTaxRatesService;
    private readonly ILogger<UpdateStateTaxRateEndpoint> _logger;

    public UpdateStateTaxRateEndpoint(IStateTaxRatesService stateTaxRatesService, ILogger<UpdateStateTaxRateEndpoint> logger)
        : base(Navigation.Constants.ManageStateTaxRates)
    {
        _stateTaxRatesService = stateTaxRatesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("state-tax-rates");
        Validator<UpdateStateTaxRateRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Updates a single state tax rate.";
            s.ExampleRequest = new UpdateStateTaxRateRequest { Abbreviation = "MA", Rate = 5.00m };
        });
        Group<ItDevOpsGroup>();
    }

    public override Task<Results<Ok<StateTaxRateDto>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(UpdateStateTaxRateRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _stateTaxRatesService.UpdateStateTaxRateAsync(req, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "state-tax-rate-update"),
                    new("endpoint", nameof(UpdateStateTaxRateEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "state-tax-rate"),
                    new("endpoint", nameof(UpdateStateTaxRateEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResultWithValidation(s_stateTaxNotFound);
        });
    }
}
