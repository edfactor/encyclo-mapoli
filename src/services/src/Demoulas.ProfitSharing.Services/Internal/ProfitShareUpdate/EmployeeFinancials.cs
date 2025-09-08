namespace Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;

/// <summary>
///     A summary of financial information about an Employee
/// </summary>
internal sealed record EmployeeFinancials
{
    public string? Name { get; set; }
    public int BadgeNumber { get; set; }
    public int Ssn { get; set; }
    public byte EnrolledId { get; set; }
    public short YearsInPlan { get; set; }
    public decimal CurrentAmount { get; set; }
    public byte EmployeeTypeId { get; set; }
    public int PointsEarned { get; set; }
    public decimal Contributions { get; set; }
    public decimal IncomeForfeiture { get; set; }
    public decimal Earnings { get; set; }
    public decimal Etva { get; set; } // Corresponds to PAYPROFIT.PY_PS_ETVA
    public decimal EarningsOnEtva { get; set; }
    public decimal SecondaryEarnings { get; set; }
    public decimal EarningsOnSecondaryEtva { get; set; }
    public byte? ZeroContributionReasonId { get; set; }
    public byte PayFrequencyId { get; set; }
    
    // Transactions for this year. 
    public decimal? DistributionsTotal { get; set; }
    public decimal? ForfeitsTotal { get; set; }
    public decimal? AllocationsTotal { get; set; }
    public decimal? PaidAllocationsTotal { get; set; }
    public decimal? MilitaryTotal { get; set; }
    public decimal? ClassActionFundTotal { get; set; }

    public bool HasTransactionAmounts()
    {
        if (DistributionsTotal == null)
        {
            return false;
        }
        return DistributionsTotal != 0 || ForfeitsTotal != 0 || AllocationsTotal != 0 || PaidAllocationsTotal != 0 || MilitaryTotal != 0 || ClassActionFundTotal != 0;
    }
}
