using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public class CreateBeneficiaryAndContactEndpoint : ProfitSharingEndpoint<CreateBeneficiaryRequest, Results<Ok<CreateBeneficiaryResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;
    private readonly ILogger<CreateBeneficiaryAndContactEndpoint> _logger;

    public CreateBeneficiaryAndContactEndpoint(IBeneficiaryService beneficiaryService, ILogger<CreateBeneficiaryAndContactEndpoint> logger) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }
    public override void Configure()
    {
        Post("/");
        Summary(s =>
        {
            s.Summary = "Adds a new beneficiary";
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, CreateBeneficiaryResponse.SampleResponse()}
            };
        });
        Group<BeneficiariesGroup>();
    }

    public override Task<Results<Ok<CreateBeneficiaryResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(CreateBeneficiaryRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var created = await _beneficiaryService.CreateBeneficiary(req, ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "create-beneficiary"),
                new KeyValuePair<string, object?>("endpoint.category", "beneficiaries"));

            _logger.LogInformation("Beneficiary created successfully, ID: {BeneficiaryId} (correlation: {CorrelationId})",
                created.BeneficiaryId, HttpContext.TraceIdentifier);

            return Result<CreateBeneficiaryResponse>.Success(created).ToHttpResult();
        });
    }
}
