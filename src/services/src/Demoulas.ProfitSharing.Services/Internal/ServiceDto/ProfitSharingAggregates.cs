namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

internal sealed record ProfitSharingAggregates
{
    internal ushort TotalEmployees { get; init; }
    internal ushort BothHardshipAndRegularEmployees { get; init; }
    internal decimal BothHardshipAndRegularAmount { get; init; }
    internal ushort RegularTotalEmployees { get; init; }
    internal decimal RegularAmount { get; init; }

    internal ushort HardshipTotalEmployees { get; init; }
    internal decimal HardshipTotalAmount { get; init; }
}
