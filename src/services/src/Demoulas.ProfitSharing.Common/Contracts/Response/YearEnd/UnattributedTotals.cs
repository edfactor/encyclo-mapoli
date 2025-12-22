namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

/// <summary>
/// Totals for records with state taxes but no state code attribution.
/// These represent data quality issues in state extraction (PS-2031).
/// </summary>
public record UnattributedTotals
{
    /// <summary>Number of records without state code attribution</summary>
    public int Count { get; set; }

    /// <summary>Sum of federal taxes for unattributed records</summary>
    public decimal FederalTax { get; set; }

    /// <summary>Sum of state taxes for unattributed records</summary>
    public decimal StateTax { get; set; }

    /// <summary>Sum of net proceeds for unattributed records</summary>
    public decimal NetProceeds { get; set; }
}
