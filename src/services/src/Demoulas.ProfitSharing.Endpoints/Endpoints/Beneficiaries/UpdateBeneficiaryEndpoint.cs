using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Beneficiaries;

public sealed class UpdateBeneficiaryEndpoint : ProfitSharingEndpoint<UpdateBeneficiaryRequest, Results<Ok<UpdateBeneficiaryResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;
    private readonly ILogger<UpdateBeneficiaryEndpoint> _logger;

    public UpdateBeneficiaryEndpoint(IBeneficiaryService beneficiaryService, ILogger<UpdateBeneficiaryEndpoint> logger) : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/");
        Summary(s =>
        {
            s.Summary = "Updates beneficiary information";
            s.ExampleRequest = UpdateBeneficiaryRequest.SampleRequest();
        });
        Group<BeneficiariesGroup>();
    }

    public override Task<Results<Ok<UpdateBeneficiaryResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(UpdateBeneficiaryRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var updated = await _beneficiaryService.UpdateBeneficiary(req, ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "update-beneficiary"),
                new KeyValuePair<string, object?>("endpoint.category", "beneficiaries"));

            // If service returns null when not found, map to Result failure with specific not-found error
            if (updated is null)
            {
                _logger.LogWarning("Beneficiary update failed - beneficiary not found, ID: {BeneficiaryId} (correlation: {CorrelationId})",
                    req.Id, HttpContext.TraceIdentifier);

                return Result<UpdateBeneficiaryResponse>.Failure(Error.EntityNotFound("Beneficiary")).ToHttpResult(Error.EntityNotFound("Beneficiary"));
            }

            _logger.LogInformation("Beneficiary updated successfully, ID: {BeneficiaryId} (correlation: {CorrelationId})",
                req.Id, HttpContext.TraceIdentifier);

            return Result<UpdateBeneficiaryResponse>.Success(updated).ToHttpResult();
        });
    }
}
