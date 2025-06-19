namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record DistributionsAndForfeituresRequest: ProfitYearRequest
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
