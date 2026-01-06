using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// Validator for DistributionSearchRequest that validates SortBy against DistributionSearchResponse properties.
/// </summary>
public sealed class DistributionSearchRequestValidator : SortedPaginationValidator<DistributionSearchRequest, DistributionSearchResponse>
{
    public DistributionSearchRequestValidator()
    {
        // Base class validates SortBy against DistributionSearchResponse properties
        // Additional validation can be added here if needed
    }
}
