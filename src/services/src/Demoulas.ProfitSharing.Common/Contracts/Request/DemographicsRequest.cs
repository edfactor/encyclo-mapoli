
namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record DemographicsRequest : IMemberRequest
{
    public required int Ssn { get; set; }
    public int BadgeNumber { get; set; }

    public required long OracleHcmId { get; set; }
    
    public required short StoreNumber { get; set; }

    public required byte DepartmentId { get; set; }
    public required byte PayClassificationId { get; set; }

    public required ContactInfoRequestDto ContactInfo { get; set; }
    public required AddressRequestDto Address { get; set; }
    public required DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Date of full-time status 
    /// </summary>
    public DateOnly? FullTimeDate { get; set; }

    public required DateOnly HireDate { get; set; }
    public DateOnly? ReHireDate { get; set; }
    public char? TerminationCodeId { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public required char EmploymentTypeCode { get; set; }
    public required byte PayFrequencyId { get; set; }

    public required char GenderCode { get; set; }
    public required char EmploymentStatusId { get; set; }

}
