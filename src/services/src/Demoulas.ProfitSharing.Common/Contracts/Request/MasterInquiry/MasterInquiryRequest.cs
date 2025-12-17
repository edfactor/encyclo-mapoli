using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;

public record MasterInquiryRequest : MasterInquiryMemberRequest
{
    [DefaultValue(2025)]
    public short? EndProfitYear { get; set; }
    public byte? ProfitCode { get; set; }
    public decimal? ContributionAmount { get; set; }
    public decimal? EarningsAmount { get; set; }
    public decimal? ForfeitureAmount { get; set; }
    public decimal? PaymentAmount { get; set; }
    public string? Name { get; set; }
    public byte? PaymentType { get; set; }
    public bool? Voids { get; set; }
#pragma warning disable DSM001
    public int Ssn { get; set; }
#pragma warning restore DSM001

    public int? BadgeNumber { get; set; }
    public short? PsnSuffix { get; set; }

    public static new MasterInquiryRequest RequestExample()
    {
        return new MasterInquiryRequest
        {
            ProfitYear = 2024,
            MemberType = 1,
            Id = 123,
            Ssn = 123456987,
            EndProfitYear = 2025,
            ProfitCode = 2,
            ContributionAmount = 1000.00m,
            EarningsAmount = 150.50m,
            ForfeitureAmount = 0m,
            PaymentAmount = 500.00m,
            Name = "John Doe",
            PaymentType = 1
        };
    }
}
