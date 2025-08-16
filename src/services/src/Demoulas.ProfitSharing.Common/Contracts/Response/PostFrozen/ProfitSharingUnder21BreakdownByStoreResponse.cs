using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;

public sealed record ProfitSharingUnder21BreakdownByStoreResponse
{
    public short StoreNumber { get; set; }
    public int BadgeNumber { get; set; }
    [MaskSensitive] public required string FullName { get; set; }
    public decimal? BeginningBalance { get; set; } = 0;
    public decimal? Earnings { get; set; } = 0;
    public decimal? Contributions { get; set; } = 0;
    public decimal? Forfeitures { get; set; } = 0;
    public decimal? Distributions { get; set; } = 0;
    public decimal? EndingBalance { get; set; } = 0;
    public decimal? VestedAmount { get; set; } = 0;
    public decimal? VestingPercentage { get; set; } = 0;
    public DateOnly DateOfBirth { get; set; }
    public byte Age { get; set; }
    public byte EnrollmentId { get; set; }

    public static readonly string REPORT_NAME = "Under 21 Breakdown";
    public static ProfitSharingUnder21BreakdownByStoreResponse ResponseExample()
    {
        return new ProfitSharingUnder21BreakdownByStoreResponse()
        {
            StoreNumber = (short)42,
            BadgeNumber = 704234,
            FullName = "Johnson, Martha",
            BeginningBalance = 20014,
            Earnings = 199.25m,
            Contributions = 59.11m,
            Forfeitures = 0,
            Distributions = 0,
            VestedAmount = 20272.36m,
            EndingBalance = 20272.36m,
            VestingPercentage = 1,
            DateOfBirth = new DateOnly(1971,5,11),
            Age = 53,
            EnrollmentId = 1
        };
    }
}
