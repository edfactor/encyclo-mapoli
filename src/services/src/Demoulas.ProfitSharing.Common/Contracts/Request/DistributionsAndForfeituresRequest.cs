using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record DistributionsAndForfeituresRequest : SortedPaginationRequestDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string[]? States { get; set; }
    public char[]? TaxCodes { get; set; }

    public static DistributionsAndForfeituresRequest RequestExample()
    {
        return new DistributionsAndForfeituresRequest
        {
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            States = ["MA", "NH", "CT"],
            TaxCodes = ['F', 'S'],
            Skip = 0,
            Take = 50,
            SortBy = "Date",
            IsSortDescending = true
        };
    }
}
