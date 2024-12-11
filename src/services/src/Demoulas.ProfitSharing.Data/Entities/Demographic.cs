using System.Diagnostics;
using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// https://demoulas.atlassian.net/wiki/spaces/~bherrmann/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables
/// </summary>
[DebuggerDisplay("Id={Id} OracleHcmId={OracleHcmId} EmployeeId={EmployeeId} FullName={ContactInfo.FullName} StoreNumber={StoreNumber}")]
public sealed class Demographic : Member
{
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the Oracle HCM (Human Capital Management) ID.
    /// This ID is used to uniquely identify an employee within the Oracle HCM system.
    /// </summary>
    /// <value>
    /// The People ID of the employee.
    /// </value>
    public required long OracleHcmId { get; set; }
    public required int Ssn { get; set; }
    public required int EmployeeId { get; set; }
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
    public List<DistributionRequest> DistributionRequests { get; set; } = [];

    public static bool DemographicHistoryEqual(Demographic demo1, Demographic demo2)
    {
        return demo1.OracleHcmId == demo2.OracleHcmId &&
               //The Oracle HCM process seems to have a random SSN.  Until that settles down, not including SSN changes as a reason to create a new history record.  demo1.Ssn == demo2.Ssn &&
               demo1.EmployeeId == demo2.EmployeeId &&
               demo1.StoreNumber == demo2.StoreNumber &&
               demo1.PayClassificationId == demo2.PayClassificationId &&
               demo1.DateOfBirth == demo2.DateOfBirth &&
               demo1.HireDate == demo2.HireDate &&
               demo1.ReHireDate == demo2.ReHireDate &&
               demo1.TerminationDate == demo2.TerminationDate &&
               demo1.DepartmentId == demo2.DepartmentId &&
               demo1.EmploymentTypeId == demo2.EmploymentTypeId &&
               demo1.PayFrequencyId == demo2.PayFrequencyId &&
               demo1.TerminationCodeId == demo2.TerminationCodeId &&
               demo1.EmploymentStatusId == demo2.EmploymentStatusId;
                
    }
}
