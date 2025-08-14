using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;

public record MemberDetails : IdRequest
{
    public bool IsEmployee { get; init; }
    public int BadgeNumber { get; init; }
    public short PsnSuffix { get; init; }
    public string Ssn { get; init; } = string.Empty;

    [MaskSensitive] public string FirstName { get; init; } = string.Empty;
    [MaskSensitive] public string LastName { get; init; } = string.Empty;
    [MaskSensitive] public string FullName => $"{LastName}, {FirstName}";
    [MaskSensitive] public string Address { get; init; } = string.Empty;
    public string AddressCity { get; init; } = string.Empty;
    public string AddressState { get; init; } = string.Empty;
    [MaskSensitive] public string AddressZipCode { get; init; } = string.Empty;
    public short Age { get; set; }
    public DateOnly DateOfBirth { get; init; }
    public DateOnly? HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; } = null;
    public DateOnly? ReHireDate { get; init; } = null;
    public string? EmploymentStatus { get; set; }
    [Unmask] public decimal YearToDateProfitSharingHours { get; init; }
    public byte? EnrollmentId { get; init; }
    public string? Enrollment { get; init; }
    public short StoreNumber { get; set; }
    [Unmask] public decimal CurrentEtva { get; set; }
    [Unmask] public decimal PreviousEtva { get; set; }

    public string? Department { get; set; }
    public string? PayClassification { get; set; }
    [MaskSensitive] public string? Gender { get; set; }
    [MaskSensitive] public string? PhoneNumber { get; set; }
    public string? WorkLocation { get; set; }
    public bool ReceivedContributionsLastYear { get; set; }
    public DateOnly? FullTimeDate { get; set; }

    [MaskSensitive] public string? TerminationReason { get; set; }

    public List<int> Missives { get; set; } = new List<int>();
}
