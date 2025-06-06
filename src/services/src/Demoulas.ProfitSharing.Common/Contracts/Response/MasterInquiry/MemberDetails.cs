using System.Diagnostics.Eventing.Reader;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
public record MemberDetails : IdRequest
{
    public bool IsEmployee { get; init; }
    public int BadgeNumber { get; init; }
    public short PsnSuffix { get; init; }
    public string Ssn { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName => $"{LastName}, {FirstName}";
    public string Address { get; init; } = string.Empty;
    public string AddressCity { get; init; } = string.Empty;
    public string AddressState { get; init; } = string.Empty;
    public string AddressZipCode { get; init; } = string.Empty;
    public short Age { get; set; }
    public DateOnly DateOfBirth { get; init; }
    public DateOnly? HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; } = null;
    public DateOnly? ReHireDate { get; init; } = null;
    public string? EmploymentStatus { get; set; }
    public decimal YearToDateProfitSharingHours { get; init; }
    public byte? EnrollmentId { get; init; }
    public string? Enrollment { get; init; }
    public short StoreNumber { get; set; }
    public decimal CurrentEtva { get; set; }
    public decimal PreviousEtva { get; set; }
    
    public required string Department { get; set; }
    public required string Classification { get; set; }
    public required string Sex { get; set; }
    public required string PhoneNumber { get; set; }
    public required string WorkLocation { get; set; }
    public required bool ReceivedContributionsLastYear { get; set; }
    public required DateOnly FullTimeDate { get; set; }
    public required string TerminationReason { get; set; }
    

    public List<int> Missives { get; set; } = new List<int>();
}
