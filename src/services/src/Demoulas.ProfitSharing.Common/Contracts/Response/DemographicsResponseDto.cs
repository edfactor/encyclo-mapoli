using Demoulas.ProfitSharing.Common.Enums;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record DemographicsResponseDto
{
    public int BadgeNumber { get; set; }

    public required long OracleHcmId { get; set; }
    public required string FullName { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required short StoreNumber { get; set; }

    public required Department Department { get; set; }
    public required byte PayClassification { get; set; }

    public required ContactInfoResponseDto ContactInfo { get; set; }
    public AddressResponseDto? Address { get; set; }
    public required DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Date of full-time status 
    /// </summary>
    public DateOnly FullTimeDate { get; set; }

    public required DateOnly HireDate { get; set; }
    public required DateOnly ReHireDate { get; set; }
    public TerminationCode TerminationCode { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public required EmploymentType EmploymentType { get; set; }
    public required PayFrequency PayFrequency { get; set; }

    public required Gender Gender { get; set; }

}
