namespace Demoulas.ProfitSharing.Data.Entities;

#pragma warning disable S1133
[Obsolete("Use PayProfit Entity")]
#pragma warning restore S1133
public sealed class PayProfitLegacy
{
    
    /// <summary>
    /// Employee BadgeNumber number (0-7 digits)
    /// </summary>
    public required int BadgeNumber { get; set; }

    /// <summary>
    /// Employee social security number
    /// </summary>
    public required long Ssn { get; set; }

    /// <summary>
    /// Hours towards Profit Sharing in the current year (updated weekly)
    /// </summary>
    public decimal? HoursCurrentYear { get; set; }

    /// <summary>
    /// hours towards PS last year 
    /// </summary>
    public decimal HoursLastYear { get; set; }

    /// <summary>
    /// Income (Wage) accumulated so far in the current year (updated weekly)
    /// </summary>
    public decimal? IncomeCurrentYear { get; set; }

    /// <summary>
    /// Dollars earned by the employee last year
    /// </summary>
    public required decimal IncomeLastYear { get; set; }

    /// <summary>
    /// amount after applying vesting rules.This can be updated when new transactions occur.For example, QDRO.
    /// </summary>
    public required decimal EarningsAfterApplyingVestingRules { get; set; }

    /// <summary>
    /// earnings on the ETVA value 
    /// </summary>
    public required decimal EarningsEtvaValue { get; set; }

    /// <summary>
    /// secondary earnings 
    /// </summary>
    public decimal? SecondaryEarnings { get; set; }

    /// <summary>
    /// secondary ETVA earnings 
    /// </summary>
    public decimal? SecondaryEtvaEarnings { get; set; }

    /// <summary>
    /// populated when forfeiture happens 
    /// </summary>
    public decimal? EarningsPriorEtvaValue { get; set; }

    /// <summary>
    /// Number of weeks worked in the current year
    /// </summary>
    public required byte WeeksWorkedYear { get; set; }

    /// <summary>
    /// Number of weeks worked last year
    /// </summary>
    public required byte WeeksWorkedLastYear { get; set; }

    /// <summary>
    /// Years the company has contributed to the employee 
    /// </summary>
    public required byte CompanyContributionYears { get; set; }

    public bool CertificateIssuedLastYear { get; set; }

    /// <summary>
    /// Date the last PS Certificate was issued
    /// </summary>
    public DateOnly? PsCertificateIssuedDate { get; set; }

    /// <summary>
    /// Year of initial contribution
    /// </summary>
    public short InitialContributionYear { get; set; }

    /// <summary>
    /// Employee net balance as of last year
    /// </summary>
    public decimal NetBalanceLastYear { get; set; }

    /// <summary>
    /// number of dollars earning in PS last year
    /// </summary>
    public decimal EarningLastYear { get; set; }

    /// <summary>
    /// points earned last year
    /// </summary>
    public ushort PointsEarnedLastYear { get; set; }

    /// <summary>
    /// Employee vested balance as of last year
    /// </summary>
    public decimal VestedBalanceLastYear { get; set; }

    /// <summary>
    /// employees dollar contribution PS last year 
    /// </summary>
    public decimal ContributionAmountLastYear { get; set; }

    /// <summary>
    /// Employee amount forfeiture to plan last year 
    /// </summary>
    public decimal ForfeitureAmountLastYear { get; set; }

    /// <summary>
    /// Employee enrollment status
    /// </summary>
    /// 
    public byte EnrollmentId { get; set; }
    public required Enrollment? Enrollment { get; set; }

    /// <summary>
    /// 0=Employee, 1=Beneficiary
    /// </summary>
    ///
    public byte BeneficiaryTypeId { get; set; }
    public required BeneficiaryType? BeneficiaryType { get; set; }

    /// <summary>
    /// 0=NOT New in plan last year, 1=New last year
    /// </summary>
    /// 
    public byte EmployeeTypeId { get; set; }
    public required EmployeeType? EmployeeType { get; set; }

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
}
