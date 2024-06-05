using Demoulas.ProfitSharing.Common.Enums;
using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// https://demoulas.atlassian.net/wiki/spaces/~bherrmann/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables
/// </summary>
public sealed class Demographic : Member
{
    public required long SSN { get; set; }
    public required int BadgeNumber { get; set; }
    public required long OracleHcmId { get; set; }
    public string? FullName { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required short StoreNumber { get; set; }

    public required Department Department { get; set; }
    public PayClassification? PayClassification { get; set; }
    public required byte PayClassificationId { get; set; }

    public required ContactInfo ContactInfo { get; set; }
    public required Address Address { get; set; }
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
