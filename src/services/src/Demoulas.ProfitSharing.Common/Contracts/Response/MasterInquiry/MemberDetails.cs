using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;

public record MemberDetails : IdRequest<int>, INameParts, IFullNameProperty, IPhoneNumber, IIsExecutive
{
    public bool IsEmployee { get; init; }
    public int BadgeNumber { get; init; }
    public short PsnSuffix { get; init; }
    public byte PayFrequencyId { get; init; }
    public bool IsExecutive { get; set; }
    public string Ssn { get; init; } = string.Empty;

    [MaskSensitive] public required string FirstName { get; init; } = string.Empty;
    [MaskSensitive] public required string LastName { get; init; } = string.Empty;
    [MaskSensitive] public required string? MiddleName { get; init; } = null;
    [MaskSensitive]
    public string FullName => string.IsNullOrEmpty(MiddleName)
        ? $"{LastName}, {FirstName}"
        : $"{LastName}, {FirstName} {MiddleName[0]}";
    [MaskSensitive] public string Address { get; init; } = string.Empty;
    public string AddressCity { get; init; } = string.Empty;
    public string AddressState { get; init; } = string.Empty;
    [MaskSensitive] public string AddressZipCode { get; init; } = string.Empty;
    [MaskSensitive] public short Age { get; set; }

    [MaskSensitive]
    public DateOnly DateOfBirth { get; init; }
    public DateOnly? HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; } = null;
    public DateOnly? ReHireDate { get; init; } = null;
    public string? EmploymentStatus { get; set; }
    [UnmaskSensitive] public decimal YearToDateProfitSharingHours { get; init; }
    public byte? EnrollmentId { get; init; }
    public string? Enrollment { get; init; }
    public short StoreNumber { get; set; }
    public decimal CurrentEtva { get; set; }
    public decimal PreviousEtva { get; set; }

    public string? Department { get; set; }
    public string? PayClassification { get; set; }
    [MaskSensitive] public string? Gender { get; set; }
    [MaskSensitive] public string? PhoneNumber { get; set; }
    public string? WorkLocation { get; set; }
    public bool? ReceivedContributionsLastYear { get; set; }
    public DateOnly? FullTimeDate { get; set; }

    [MaskSensitive] public string? TerminationReason { get; set; }

    public List<int> Missives { get; set; } = [];
    public List<int> BadgesOfDuplicateSsns { get; set; } = [];

    public static MemberDetails ResponseExample()
    {
        return new MemberDetails
        {
            Id = 1,
            IsEmployee = true,
            BadgeNumber = 1001,
            PsnSuffix = 0,
            PayFrequencyId = 1,
            IsExecutive = false,
            Ssn = "123456789",
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "M",
            Address = "123 Main St",
            AddressCity = "Boston",
            AddressState = "MA",
            AddressZipCode = "02101",
            Age = 35,
            DateOfBirth = new DateOnly(1989, 1, 15),
            HireDate = new DateOnly(2015, 3, 1),
            TerminationDate = null,
            ReHireDate = null,
            EmploymentStatus = "Active",
            YearToDateProfitSharingHours = 1040.00m,
            EnrollmentId = 1,
            Enrollment = "Eligible",
            StoreNumber = 5,
            CurrentEtva = 50000.00m,
            PreviousEtva = 48000.00m,
            Department = "Grocery",
            PayClassification = "Full Time",
            Gender = "M",
            PhoneNumber = "555-1234",
            WorkLocation = "Store 5",
            ReceivedContributionsLastYear = true,
            FullTimeDate = new DateOnly(2015, 3, 1),
            TerminationReason = null,
            Missives = [],
            BadgesOfDuplicateSsns = []
        };
    }
}
