using Demoulas.Common.Contracts.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;

public record MasterInquiryMemberDetailsRequest : SortedPaginationRequestDto
{
    public required byte? MemberType { get; set; }
    public int? Id { get; set; }
    public short? ProfitYear { get; set; }
    public byte? MonthToDate { get; set; }

    public int? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }
    public string? Ssn { get; set; }
    public byte? ProfitCode { get; set; }
    public decimal? ContributionAmount { get; set; }
    public decimal? EarningsAmount { get; set; }
    public decimal? ForfeitureAmount { get; set; }
    public decimal? PaymentAmount { get; set; }
    public string? Name { get; set; }
    public byte? PaymentType { get; set; }
    public bool? Voids { get; set; }

    public static MasterInquiryMemberDetailsRequest RequestExample() => new()
    {
        MemberType = 1,
        Id = 500,
        ProfitYear = 2024,
        MonthToDate = 12,
        BadgeNumber = 123456,
        PsnSuffix = 1,
        Ssn = "123-45-6789",
        ProfitCode = 1,
        ContributionAmount = 1500.00m,
        EarningsAmount = 250.75m,
        ForfeitureAmount = 0m,
        PaymentAmount = 800.00m,
        Name = "Smith, John",
        PaymentType = 1,
        Voids = false,
        Skip = 0,
        Take = 50,
        SortBy = "BadgeNumber",
        IsSortDescending = false
    };
}
