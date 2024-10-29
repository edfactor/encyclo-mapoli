using System.Diagnostics;
using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// https://demoulas.atlassian.net/wiki/spaces/~bherrmann/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables
/// </summary>
[DebuggerDisplay("OracleHcmId={OracleHcmId} BadgeNumber={BadgeNumber} FullName={ContactInfo.FullName} StoreNumber={StoreNumber}")]
public sealed class Demographic : Member
{
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the Oracle HCM (Human Capital Management) ID.
    /// This ID is used to uniquely identify an employee within the Oracle HCM system.
    /// </summary>
    /// <value>
    /// The Oracle HCM ID of the employee.
    /// </value>
    public required long OracleHcmId { get; set; }
    public required long Ssn { get; set; }
    public required int BadgeNumber { get; set; }
    public DateTime LastModifiedDate { get; set; }

    public required short StoreNumber { get; set; }

    public PayClassification? PayClassification { get; set; }
    public required byte PayClassificationId { get; set; }

    public required ContactInfo ContactInfo { get; set; }
    public required Address Address { get; set; }
    public required DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Date of full-time status
    /// </summary>
    public DateOnly? FullTimeDate { get; set; }

    public required DateOnly HireDate { get; set; }
    public DateOnly? ReHireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }


    public byte DepartmentId { get; set; }
    public Department? Department { get; set; }

    public char EmploymentTypeId { get; set; }
    public EmploymentType? EmploymentType { get; set; }

    public char GenderId { get; set; }
    public Gender? Gender { get; set; }

    public byte PayFrequencyId { get; set; }
    public PayFrequency? PayFrequency { get; set; }

    public char? TerminationCodeId { get; set; }
    public TerminationCode? TerminationCode { get; set; }

    public char EmploymentStatusId { get; set; }
    public EmploymentStatus? EmploymentStatus { get; set; }
    
    
    public List<PayProfit> PayProfits { get; set; } = [];
    public List<Beneficiary> Beneficiaries { get; set; } = [];

    public List<ProfitShareCheck> Checks { get; set; } = [];
}
