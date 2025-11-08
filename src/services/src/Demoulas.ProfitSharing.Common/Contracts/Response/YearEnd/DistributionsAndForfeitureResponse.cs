using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

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

public sealed record DistributionsAndForfeitureResponse : IIsExecutive
{
    public required int BadgeNumber { get; set; }
    public required short PsnSuffix { get; set; }
    [MaskSensitive] public required string EmployeeName { get; set; }
    public required string Ssn { get; set; }
    public DateOnly? Date { get; set; }
    public required decimal DistributionAmount { get; set; }
    public required decimal StateTax { get; set; }
    public required string? State { get; set; }
    public required decimal FederalTax { get; set; }
    public required decimal ForfeitAmount { get; set; }
    /// <summary>
    /// Forfeit type indicator: 'A' = Administrative, 'C' = Class Action, null = Regular
    /// </summary>
    public char? ForfeitType { get; set; }
    public required byte? Age { get; set; }
    public required char? TaxCode { get; set; }
    public string? OtherName { get; set; }
    public string? OtherSsn { get; set; }
    public required bool HasForfeited { get; set; }
    public bool IsExecutive { get; set; }

    public static DistributionsAndForfeitureResponse ResponseExample()
    {
        return new DistributionsAndForfeitureResponse()
        {
            BadgeNumber = 123,
            PsnSuffix = 1,
            EmployeeName = "Doe, John",
            Ssn = "124",
            DistributionAmount = 1250.25m,
            StateTax = 25.12m,
            State = "MA",
            FederalTax = 51.52m,
            ForfeitAmount = 0m,
            ForfeitType = null,
            Age = 33,
            TaxCode = '9',
            HasForfeited = false
        };
    }
}
