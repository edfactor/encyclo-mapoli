namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class PayProfit
{
    public required long OracleHcmId { get; set; }

    public short ProfitYear { get; set; }

    /// <summary>
    /// Hours towards Profit Sharing in the current year (updated weekly)
    /// </summary>
    public decimal? CurrentHoursYear { get; set; }

    
    /// <summary>
    /// Income (Wage) accumulated so far in the current year (updated weekly)
    /// </summary>
    public decimal? CurrentIncomeYear { get; set; }

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
    /// Number of weeks worked in the current year
    /// </summary>
    public required byte WeeksWorkedYear { get; set; }

    /// <summary>
    /// Date the last PS Certificate was issued
    /// </summary>
    public DateOnly? PsCertificateIssuedDate { get; set; }

   
    /// <summary>
    /// Employee enrollment status
    /// </summary>
    ///
    public byte EnrollmentId { get; set; }
    public required Enrollment? Enrollment { get; set; }

    /// <summary>
    /// 0=Employee, 1=Beneficiary
    /// </summary>
    public byte BeneficiaryTypeId { get; set; }
    public required BeneficiaryType? BeneficiaryType { get; set; }

    /// <summary>
    /// 0=NOT New in plan last year, 1=New last year
    /// </summary>
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


    public Demographic? Demographic { get; set; }
}
