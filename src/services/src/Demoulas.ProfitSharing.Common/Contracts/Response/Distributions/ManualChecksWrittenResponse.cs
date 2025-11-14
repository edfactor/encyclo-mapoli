namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
public sealed record ManualChecksWrittenResponse : DistributionsOnHoldResponse
{
    public string? CheckNumber { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal FederalTaxAmount { get; set; }
    public decimal StateTaxAmount { get; set; }

    public static ManualChecksWrittenResponse ResponseExample()
    {
        return new ManualChecksWrittenResponse()
        {
            Ssn = "XXX-XX-1234",
            PayTo = "John Doe",
            CheckAmount = 1200.00m,
            CheckNumber = "1001",
            GrossAmount = 1500.00m,
            FederalTaxAmount = 150.00m,
            StateTaxAmount = 75.00m
        };
    }
}
