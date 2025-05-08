namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record DistributionsAndForfeituresRequest: ProfitYearRequest
{
    public int? StartMonth { get; set; } = 1;
    public int? EndMonth { get; set; } = 12;

}
