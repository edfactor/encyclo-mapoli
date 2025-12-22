using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record VestedAmountsByAgeDetail
{
    [MaskSensitive] public required short Age { get; set; }
    public ushort FullTime100PercentCount { get; set; }
    public decimal FullTime100PercentAmount { get; set; }
    public ushort FullTimePartialCount { get; set; }
    public decimal FullTimePartialAmount { get; set; }
    public ushort FullTimeNotVestedCount { get; set; }
    public decimal FullTimeNotVestedAmount { get; set; }
    public ushort PartTime100PercentCount { get; set; }
    public decimal PartTime100PercentAmount { get; set; }
    public ushort PartTimePartialCount { get; set; }
    public decimal PartTimePartialAmount { get; set; }
    public ushort PartTimeNotVestedCount { get; set; }
    public decimal PartTimeNotVestedAmount { get; set; }
    public ushort BeneficiaryCount { get; set; }
    public decimal BeneficiaryAmount { get; set; }
    public ushort FullTimeCount { get; set; }
    public ushort NotVestedCount { get; set; }
    public ushort PartialVestedCount { get; set; }
}
