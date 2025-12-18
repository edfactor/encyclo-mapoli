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

public sealed class UpdateAnnuityRateEndpoint : ProfitSharingEndpoint<UpdateAnnuityRateRequest, Results<Ok<AnnuityRateDto>, NotFound, BadRequest, ProblemHttpResult>>
{
    private static readonly Error s_annuityRateNotFound = Error.EntityNotFound("Annuity rate");

    private readonly IAnnuityRatesService _annuityRatesService;
    private readonly ILogger<UpdateAnnuityRateEndpoint> _logger;

    public UpdateAnnuityRateEndpoint(IAnnuityRatesService annuityRatesService, ILogger<UpdateAnnuityRateEndpoint> logger)
        : base(Navigation.Constants.ManageAnnuityRates)
    {
        _annuityRatesService = annuityRatesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("annuity-rates");
        Validator<UpdateAnnuityRateRequestValidator>();
        Summary(s =>
        {
            s.Summary = "Updates a single annuity rate.";
            s.ExampleRequest = new UpdateAnnuityRateRequest { Year = 2024, Age = 65, SingleRate = 13.0000m, JointRate = 15.0000m };
        });
        Group<AdministrationGroup>();
    }

    public override Task<Results<Ok<AnnuityRateDto>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(UpdateAnnuityRateRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _annuityRatesService.UpdateAnnuityRateAsync(req, ct);

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "annuity-rate-update"),
                    new("endpoint", nameof(UpdateAnnuityRateEndpoint)));

                EndpointTelemetry.RecordCountsProcessed?.Record(1,
                    new("record_type", "annuity-rate"),
                    new("endpoint", nameof(UpdateAnnuityRateEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            return result.ToHttpResultWithValidation(s_annuityRateNotFound);
        });
    }
}
