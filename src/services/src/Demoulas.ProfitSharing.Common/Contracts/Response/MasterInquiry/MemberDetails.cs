namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
public record MemberDetails
{
    public int Id { get; set; }
    public bool IsEmployee { get; init; }
    public int BadgeNumber { get; init; }
    public short PsnSuffix { get; init; }
    public string Ssn { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string AddressCity { get; init; } = string.Empty;
    public string AddressState { get; init; } = string.Empty;
    public string AddressZipCode { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; }
    
    public List<int> Missives { get; set; } = new List<int>();
    public string? EmploymentStatus { get; set; }
}
