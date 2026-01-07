using Demoulas.Common.Api.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

/// <summary>
/// FastEndpoints group for system-level endpoints (health, time, diagnostics).
/// Route prefix: /system
/// 
/// These endpoints are accessible to all authenticated users for basic system information.
/// </summary>
public sealed class SystemGroup : GroupBase
{
    protected override string Route => "system";
    protected override string RouteName => "System";
}
