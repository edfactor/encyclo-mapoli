using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record VestedAmountsByAgeDetail
{
    [MaskSensitive] public required short Age { get; set; }
    public short FullTime100PercentCount { get; set; }
    public decimal FullTime100PercentAmount { get; set; }
    public short FullTimePartialCount { get; set; }
    public decimal FullTimePartialAmount { get; set; }
    public short FullTimeNotVestedCount { get; set; }
    public decimal FullTimeNotVestedAmount { get; set; }
    public short PartTime100PercentCount { get; set; }
    public decimal PartTime100PercentAmount { get; set; }
    public short PartTimePartialCount { get; set; }
    public decimal PartTimePartialAmount { get; set; }
    public short PartTimeNotVestedCount { get; set; }
    public decimal PartTimeNotVestedAmount { get; set; }
    public short BeneficiaryCount { get; set; }
    public decimal BeneficiaryAmount { get; set; }
    public short FullTimeCount { get; set; }
    public short NotVestedCount { get; set; }
    public short PartialVestedCount { get; set; }
}
