namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;
internal sealed record ProfitSharingAggregates
{
    internal short TotalEmployees { get; init; }
    internal short BothHardshipAndRegularEmployees { get; init; }
    internal decimal BothHardshipAndRegularAmount { get; init; }
    internal short RegularTotalEmployees { get; init; }
    internal decimal RegularAmount { get; init; }

    internal short HardshipTotalEmployees { get; init; }
    internal decimal HardshipTotalAmount { get; init; }
}
