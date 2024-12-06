namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record VestedAmountsByAgeDetail
{
    public byte Age { get; set; }
    public short FullTimeCount { get; set; }
    public decimal FullTimeAmount { get; set; }
    public short NotVestedCount { get; set; }
    public decimal NotVestedAmount { get; set; }
    public short PartialVestedCount { get; set; }
    public decimal PartialVestedAmount { get; set; }
    public short BeneficiaryCount { get; set; }
    public decimal BeneficiaryAmount { get; set; }
}
