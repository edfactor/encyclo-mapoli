namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record ZeroContributionReasonRequestDto
{
    public required byte Id { get; set; }
    public required string Name { get; set; }
}
