namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public sealed record BalanceEndpointResponse
{
    public required int Id { get; set; }
    public required string Ssn { get; set; }
    public decimal VestedBalance { get; set; }
    public decimal TotalDistributions { get; set; }
    public decimal Etva { get; set; }
    public decimal VestingPercent { get; set; }
    public decimal CurrentBalance { get; set; }
    public short YearsInPlan { get; set; }

    public static BalanceEndpointResponse ResponseExample() {
        return new BalanceEndpointResponse
        {
            Id = 123456789,
            Ssn = "xxx-xx-6789",
            VestedBalance = 2030,
            TotalDistributions = 200,
            Etva = 250,
            VestingPercent = .4m,
            CurrentBalance = 5000,
            YearsInPlan = 4,
        };
    }
}
