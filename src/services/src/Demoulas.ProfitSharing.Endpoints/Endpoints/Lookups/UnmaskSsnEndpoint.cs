using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

/// <summary>
/// Endpoint for unmasking SSN numbers for demographic records.
/// Restricted to users with SSN-Unmasking role for compliance and verification purposes.
/// Returns the formatted SSN (e.g., "123-45-6789") for a given demographic ID.
/// </summary>
public sealed class UnmaskSsnEndpoint : ProfitSharingEndpoint<UnmaskSsnRequest, Results<Ok<UnmaskSsnResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IUnmaskingService _unmaskingService;
    private readonly ILogger<UnmaskSsnEndpoint> _logger;

    public UnmaskSsnEndpoint(
        IUnmaskingService unmaskingService,
        ILogger<UnmaskSsnEndpoint> logger)
        : base(Navigation.Constants.Inquiries)
    {
        _unmaskingService = unmaskingService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("");
        Summary(s =>
        {
            s.Summary = "Unmask SSN for a demographic record";
            s.Description = "Returns the unmasked (formatted) SSN for a specific demographic ID. Restricted to users with SSN-Unmasking role.";
            s.ExampleRequest = new UnmaskSsnRequest { DemographicId = 12345 };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new UnmaskSsnResponse { Ssn = "123-45-6789" }
                }
            };
            s.Responses[404] = "Demographic not found";
            s.Responses[403] = "Forbidden. Requires SSN-Unmasking role";
        });
        Group<SsnUnmaskingGroup>();
    }

    public override Task<Results<Ok<UnmaskSsnResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
        UnmaskSsnRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry<UnmaskSsnRequest, Results<Ok<UnmaskSsnResponse>, NotFound, ProblemHttpResult>>(
            HttpContext, _logger, req, async () =>
        {
            var ssnResult = await _unmaskingService.GetUnmaskedSsnAsync(req.DemographicId, ct);

            // Handle not found case
            if (!ssnResult.IsSuccess)
            {
                return TypedResults.NotFound();
            }

            // Record sensitive field access for compliance/audit
            EndpointTelemetry.SensitiveFieldAccessTotal.Add(1,
                new("field", "Ssn"),
                new("endpoint", nameof(UnmaskSsnEndpoint)));

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "ssn-unmasking"),
                new("endpoint", nameof(UnmaskSsnEndpoint)));

            _logger.LogInformation("SSN unmasked for demographic ID {DemographicId} (correlation: {CorrelationId})",
                req.DemographicId, HttpContext.TraceIdentifier);

            var response = new UnmaskSsnResponse { Ssn = ssnResult.Value! };
            return TypedResults.Ok(response);
        }, "Ssn");
    }
}

/// <summary>
/// Request for unmasking SSN lookup endpoint.
/// </summary>
public sealed record UnmaskSsnRequest
{
    /// <summary>
    /// The demographic ID to unmask SSN for.
    /// </summary>
    public long DemographicId { get; init; }
}

/// <summary>
/// Response containing unmasked (formatted) SSN.
/// </summary>
public sealed record UnmaskSsnResponse
{
    /// <summary>
    /// The formatted SSN (e.g., "123-45-6789").
    /// </summary>
    public string Ssn { get; init; } = string.Empty;
}

/// <summary>
/// Validator for UnmaskSsnRequest.
/// </summary>
public sealed class UnmaskSsnRequestValidator : FluentValidation.AbstractValidator<UnmaskSsnRequest>
{
    public UnmaskSsnRequestValidator()
    {
        RuleFor(x => x.DemographicId)
            .GreaterThan(0)
            .WithMessage("DemographicId must be a positive integer.");
    }
}
