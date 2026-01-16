using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

/// <summary>
/// Endpoint for unmasking SSN numbers for demographic records.
/// Restricted to users with SSN-Unmasking role for compliance and verification purposes.
/// Returns the formatted SSN (e.g., "123-45-6789") for a given demographic ID.
/// </summary>
public sealed class UnmaskSsnEndpoint : ProfitSharingEndpoint<UnmaskSsnRequest, Results<Ok<UnmaskSsnResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IUnmaskingService _unmaskingService;

    public UnmaskSsnEndpoint(IUnmaskingService unmaskingService)
        : base(Navigation.Constants.Inquiries)
    {
        _unmaskingService = unmaskingService;
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
                    new UnmaskSsnResponse { UnmaskedSsn = "123-45-6789" }
                }
            };
            s.Responses[404] = "Demographic not found";
            s.Responses[403] = "Forbidden. Requires SSN-Unmasking role";
        });
        Group<SsnUnmaskingGroup>();
    }

    protected override async Task<Results<Ok<UnmaskSsnResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        UnmaskSsnRequest req,
        CancellationToken ct)
    {
        var ssnResult = await _unmaskingService.GetUnmaskedSsnAsync(req.DemographicId, ct);
        return !ssnResult.IsSuccess
            ? TypedResults.NotFound()
            : TypedResults.Ok(new UnmaskSsnResponse { UnmaskedSsn = ssnResult.Value! });
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
    public int DemographicId { get; init; }
}

/// <summary>
/// Response containing unmasked (formatted) SSN.
/// </summary>
public sealed record UnmaskSsnResponse
{
    /// <summary>
    /// The formatted SSN (e.g., "123-45-6789").
    /// </summary>
    public string UnmaskedSsn { get; init; } = string.Empty;
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
