using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record DemographicsResponseDto : MemberResponseDto
{
    public required string SSN { get; set; }
    public int BadgeNumber { get; set; }

    public required long OracleHcmId { get; set; }
    public required string FullName { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required short StoreNumber { get; set; }

    public required DepartmentResponseDto? Department { get; set; }
    public required byte PayClassificationId { get; set; }

    public required ContactInfoResponseDto ContactInfo { get; set; }
    public AddressResponseDto? Address { get; set; }
    public required DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Date of full-time status 
    /// </summary>
    public DateOnly FullTimeDate { get; set; }

    public required DateOnly HireDate { get; set; }
    public required DateOnly ReHireDate { get; set; }
    public TerminationCodeResponseDto? TerminationCode { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public required EmploymentTypeResponseDto? EmploymentType { get; set; }
    public required PayFrequencyResponseDto? PayFrequency { get; set; }

    public required GenderResponseDto? Gender { get; set; }

}
