
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

public record BeneficiaryDetailResponse : IIsExecutive, IFullNameProperty
{
    public int BadgeNumber { get; set; }
    public short PsnSuffix { get; set; }
    [MaskSensitive] public string? FullName { get; set; }
    public string? Ssn { get; set; }
    [MaskSensitive] public string? Street { get; set; }
    [MaskSensitive] public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    [MaskSensitive] public DateOnly? DateOfBirth { get; set; }
    public decimal? CurrentBalance { get; set; }
    public bool IsExecutive { get; set; }

    public static BeneficiaryDetailResponse ResponseExample() => new()
    {
        BadgeNumber = 12345,
        PsnSuffix = 1,
        FullName = "John Smith",
        Ssn = "***-**-6789",
        Street = "123 Main St",
        City = "Springfield",
        State = "MA",
        Zip = "01101",
        DateOfBirth = new DateOnly(1980, 5, 15),
        CurrentBalance = 150000.00m,
        IsExecutive = false
    };
}
