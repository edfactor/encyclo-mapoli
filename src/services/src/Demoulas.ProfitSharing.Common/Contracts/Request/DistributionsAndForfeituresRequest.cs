using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record DistributionsAndForfeituresRequest : SortedPaginationRequestDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? State { get; set; }
    public string? TaxCode { get; set; }
}
