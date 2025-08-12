
namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public record BeneficiaryDetailResponse
{
    public int BadgeNumber { get; set; }
    public int Psn { get; set; }
    public string? Name { get; set; }
    public string? Ssn { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public decimal? CurrentBalance { get; set; }
}
