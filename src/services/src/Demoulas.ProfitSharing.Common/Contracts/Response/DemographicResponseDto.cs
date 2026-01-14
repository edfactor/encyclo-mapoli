using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record DemographicResponseDto
{
    public required string Ssn { get; set; }
    public int BadgeNumber { get; set; }

    public required long OracleHcmId { get; set; }
    public required short StoreNumber { get; set; }

    public required DepartmentResponseDto? Department { get; set; }
    public required string PayClassificationId { get; set; }

    public required ContactInfoResponseDto ContactInfo { get; set; }
    public AddressResponseDto? Address { get; set; }
    [MaskSensitive] public required DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Date of full-time status
    /// </summary>
    public DateOnly? FullTimeDate { get; set; }

    public required DateOnly HireDate { get; set; }
    public DateOnly? ReHireDate { get; set; }
    public TerminationCodeResponseDto? TerminationCode { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public required EmploymentTypeResponseDto? EmploymentType { get; set; }
    public required PayFrequencyResponseDto? PayFrequency { get; set; }

    public required GenderResponseDto? Gender { get; set; }


    public static DemographicResponseDto ResponseExample()
    {
        return new DemographicResponseDto
        {
            Ssn = "123-45-6789",
            OracleHcmId = 0,
            StoreNumber = 0,
            Department = new DepartmentResponseDto { Id = 3, Name = "Produce" },
            PayClassificationId = "0",
            ContactInfo = ContactInfoResponseDto.ResponseExample(),
            Address = AddressResponseDto.ResponseExample(),
            DateOfBirth = default,
            HireDate = default,
            ReHireDate = default,
            EmploymentType = new EmploymentTypeResponseDto { Name = "Supreme Leader" },
            PayFrequency = new PayFrequencyResponseDto { Name = "Hourly" },
            Gender = new GenderResponseDto { Name = "Yes" }
        };
    }
}
