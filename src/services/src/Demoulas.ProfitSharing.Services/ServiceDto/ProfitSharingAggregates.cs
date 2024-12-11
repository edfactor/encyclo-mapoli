namespace Demoulas.ProfitSharing.Services.ServiceDto;
internal sealed record ProfitSharingAggregates
{
    public short RegularTotalEmployees { get; init; }
    public decimal RegularAmount { get; init; }

    public short HardshipTotalEmployees { get; init; }
    public decimal HardshipTotalAmount { get; init; }
}
