using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record DistributionsAndForfeitureResponse : IIsExecutive
{
    public required long BadgePsn { get; set; }
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
    [MaskSensitive] public required byte? Age { get; set; }
    public required char? TaxCode { get; set; }
    public string? OtherName { get; set; }
    public string? OtherSsn { get; set; }
    public required bool HasForfeited { get; set; }
    public bool IsExecutive { get; set; }

    public static DistributionsAndForfeitureResponse ResponseExample()
    {
        return new DistributionsAndForfeitureResponse()
        {
            BadgePsn = 123,
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
