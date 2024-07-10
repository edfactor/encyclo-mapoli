namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record ZeroContributionReasonResponseDto
{
    public required byte Id { get; set; }
    public required string Name { get; set; }
}
