
using System.ComponentModel;

namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record DemographicsRequest : IMemberRequest, IBadgeNumberRequest
{
#pragma warning disable DSM001
    public required int Ssn { get; set; }
#pragma warning restore DSM001
    public int BadgeNumber { get; set; }

    public required long OracleHcmId { get; set; }

    [DefaultValue(99)]
    public required short StoreNumber { get; set; }

    public required byte DepartmentId { get; set; }
    public required string PayClassificationId { get; set; }

    public required ContactInfoRequestDto ContactInfo { get; set; }
    public required AddressRequestDto Address { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public DateOnly? DateOfDeath { get; set; }

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
