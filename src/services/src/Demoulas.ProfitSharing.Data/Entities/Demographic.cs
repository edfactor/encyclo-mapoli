using Demoulas.ProfitSharing.Common.Enums;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// https://demoulas.atlassian.net/wiki/spaces/~bherrmann/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables
/// </summary>
public sealed class Demographic
{
    public required int BadgeNumber { get; set; }
    public required long OracleHcmId { get; set; }
    public required string FullName { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required short StoreNumber { get; set; }

    public required DepartmentEnum Department { get; set; }
    public required PayClassification PayClassification { get; set; }

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
    public required EmploymentTypeEnum EmploymentType { get; set; }
    public required PayFrequencyEnum PayFrequency { get; set; }
    public required GenderEnum Gender { get; set; }
}
