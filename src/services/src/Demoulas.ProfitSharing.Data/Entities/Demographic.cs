using System.Diagnostics;
using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// https://demoulas.atlassian.net/wiki/spaces/~bherrmann/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables
/// </summary>
[DebuggerDisplay("Id={Id} OracleHcmId={OracleHcmId} BadgeNumber={BadgeNumber} FullName={ContactInfo.FullName} StoreNumber={StoreNumber}")]
public sealed class Demographic : Member
{
    private int _ssn;

    /// <summary>
    /// Gets or sets the Oracle HCM (Human Capital Management) ID.
    /// This ID is used to uniquely identify an employee within the Oracle HCM system.
    /// </summary>
    /// <value>
    /// The People ID of the employee.
    /// </value>
    public required long OracleHcmId { get; set; }

    /// <summary>
    /// Gets or sets the Social Security Number (SSN) associated with the demographic entity.
    /// </summary>
    /// <remarks>
    /// Changing the value of this property will automatically record the change in the 
    /// <see cref="DemographicSsnChangeHistory"/> collection, capturing the old and new SSN values
    /// along with the timestamp of the change.
    /// </remarks>
    /// <value>
    /// An integer representing the Social Security Number (SSN) of the demographic entity.
    /// </value>
    public required int Ssn
    {
        get
        {
            return _ssn;
        }
        set
        {
            if (_ssn != value)
            {
                DemographicSsnChangeHistories.Add(new DemographicSsnChangeHistory
                {
                    OldSsn = _ssn,
                    NewSsn = value,
                    ModifiedAtUtc = DateTimeOffset.UtcNow
                });
                _ssn = value;
            }
        }
    }

    public required short StoreNumber { get; set; }

    public PayClassification? PayClassification { get; set; }
    public required string PayClassificationId { get; set; }

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

    public List<DemographicSsnChangeHistory> DemographicSsnChangeHistories { get; set; } = [];

    /// <summary>
    /// Compares two <see cref="Demographic"/> objects to determine if their historical data is equivalent.
    /// </summary>
    /// <param name="demo1">The first <see cref="Demographic"/> object to compare.</param>
    /// <param name="demo2">The second <see cref="Demographic"/> object to compare.</param>
    /// <returns>
    /// <c>true</c> if the historical data of the two <see cref="Demographic"/> objects is equivalent; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method compares key properties of the <see cref="Demographic"/> objects, excluding SSN due to potential inconsistencies 
    /// in the Oracle HCM process. It is used to determine whether a new history record should be created.
    /// </remarks>
    public static bool DemographicHistoryEqual(Demographic demo1, Demographic demo2)
    {
        return demo1.OracleHcmId == demo2.OracleHcmId &&
               demo1.BadgeNumber == demo2.BadgeNumber &&
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
