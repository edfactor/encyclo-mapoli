using System.Diagnostics;
using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

[DebuggerDisplay("Id={DemographicId} ProfitYear={ProfitYear} EmploymentStatus={Demographic.EmploymentStatusId} TerminationCodeId={Demographic.TerminationCodeId} Name:{Demographic.ContactInfo.FullName}")]
public sealed class PayProfit : ModifiedBase
{
    /// <summary>
    /// Gets or sets the Oracle HCM (Human Capital Management) identifier.
    /// This identifier is used to uniquely associate the PayProfit entity with a Demographic/Oracle HCM record.
    /// </summary>
    public required int DemographicId { get; set; }

    /// <summary>
    /// Gets or sets the year for which the profit is being calculated.
    /// </summary>
    /// <value>
    /// The year represented as a short integer.
    /// </value>
    public short ProfitYear { get; set; }

    /// <summary>
    /// Hours towards Profit Sharing in the current year (updated weekly)
    /// </summary>
    public decimal CurrentHoursYear { get; set; }


    /// <summary>
    /// Income (Wage) accumulated so far in the current year (updated weekly)
    /// </summary>
    public decimal CurrentIncomeYear { get; set; }

    /// <summary>
    /// Amount of money which is outside the time changing Vesting Amount.
    /// This amount fully belongs to the member.   This number can not be derived (yet) from the PROFIT_DETAIL transactions.   There is an enhancement request to consider
    /// changing this behavior.
    /// </summary>
    public required decimal Etva { get; set; }

    /// <summary>
    /// Number of weeks worked in the current year
    /// </summary>
    public byte WeeksWorkedYear { get; set; }

    /// <summary>
    /// Date the last PS Certificate was issued
    /// </summary>
    public DateOnly? PsCertificateIssuedDate { get; set; }

    // EnrollmentId and Enrollment properties removed - use Demographic.EnrollmentId instead
    // Enrollment is now stored on DEMOGRAPHIC table (VestingScheduleId + HasForfeited)

    /// <summary>
    /// 0=Employee, 1=Beneficiary
    /// </summary>
    public byte BeneficiaryTypeId { get; set; }
    public BeneficiaryType? BeneficiaryType { get; set; }

    /// <summary>
    /// 0=NOT New in plan last year, 1=New last year
    /// </summary>
    public byte EmployeeTypeId { get; set; }
    public EmployeeType? EmployeeType { get; set; }


    public byte? ZeroContributionReasonId { get; set; }
    public ZeroContributionReason? ZeroContributionReason { get; set; }

    /// <summary>
    /// Executive hours
    /// </summary>
    public decimal HoursExecutive { get; set; }

    /// <summary>
    /// Executive earnings
    /// </summary>
    public decimal IncomeExecutive { get; set; }

    /// <summary>
    /// Points Earned (for the ProfitYear).
    /// </summary>
    public decimal? PointsEarned { get; set; }

    /// <summary>
    /// Total hours including both executive and current year hours.
    /// Computed as: HoursExecutive + CurrentHoursYear
    /// </summary>
    public decimal TotalHours { get; set; }

    /// <summary>
    /// Total income including both executive and current year income.
    /// Computed as: IncomeExecutive + CurrentIncomeYear
    /// </summary>
    public decimal TotalIncome { get; set; }

    public Demographic? Demographic { get; set; }
}
