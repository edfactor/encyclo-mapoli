using Demoulas.Common.Api.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

/// <summary>
/// FastEndpoints group for lookup endpoints (reference data, dropdowns, etc.).
/// Route prefix: /lookup
/// 
/// Most lookups use standard permissions. Individual endpoints may specify their own
/// policies as needed (e.g., UnmaskSsnEndpoint uses the CanUnmaskSsn policy for
/// SSN-Unmasking role only).
/// </summary>
public sealed class LookupGroup : GroupBase
{
    protected override string Route => "lookup";
    protected override string RouteName => "Lookup";
}
