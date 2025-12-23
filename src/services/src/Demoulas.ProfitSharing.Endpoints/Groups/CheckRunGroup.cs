using Demoulas.Common.Api.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

/// <summary>
/// FastEndpoints group for check run operations.
/// Routes: /check-run/*
/// </summary>
public sealed class CheckRunGroup : GroupBase
{
    protected override string Route => "check-run";
    protected override string RouteName => "Check Run";
}
