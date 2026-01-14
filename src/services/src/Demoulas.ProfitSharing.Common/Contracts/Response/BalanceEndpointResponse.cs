using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record BalanceEndpointResponse : IdRequest<int>
{
    public required string Ssn { get; set; }
    public decimal VestedBalance { get; set; }
    public decimal Etva { get; set; }
    public decimal VestingPercent { get; set; }
    public decimal CurrentBalance { get; set; }
    public short YearsInPlan { get; set; }
    public decimal AllocationsToBeneficiary { get; set; }
    public decimal AllocationsFromBeneficiary { get; set; }


    public static BalanceEndpointResponse ResponseExample()
    {
        return new BalanceEndpointResponse
        {
            Id = 123456789,
            Ssn = "xxx-xx-6789",
            VestedBalance = 2030,
            Etva = 250,
            VestingPercent = .4m,
            CurrentBalance = 5000,
            YearsInPlan = 4,
            AllocationsToBeneficiary = 5,
            AllocationsFromBeneficiary = 6,
        };
    }
}
