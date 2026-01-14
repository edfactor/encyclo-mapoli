

using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;

public record BeneficiarySearchFilterResponse
{
    public int BadgeNumber { get; set; }
    public short PsnSuffix { get; set; }
    [MaskSensitive] public string? FullName { get; set; }
    public string? Ssn { get; set; }
    [MaskSensitive] public string? Street { get; set; }
    [MaskSensitive] public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    [MaskSensitive] public short? Age { get; set; }

    public static BeneficiarySearchFilterResponse ResponseExample() => new()
    {
        BadgeNumber = 12345,
        PsnSuffix = 1,
        FullName = "John Smith",
        Ssn = "***-**-6789",
        Street = "123 Main St",
        City = "Springfield",
        State = "MA",
        Zip = "01101",
        Age = 44
    };
}
