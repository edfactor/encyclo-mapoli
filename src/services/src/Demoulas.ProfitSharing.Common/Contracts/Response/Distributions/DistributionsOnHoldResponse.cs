namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

public record DistributionsOnHoldResponse
{
    public required string Ssn { get; set; }
    public required string PayTo { get; set; }
    public required decimal CheckAmount { get; set; }

    public static DistributionsOnHoldResponse SampleResponse()
    {
        return new DistributionsOnHoldResponse
        {
            Ssn = "XXX-XX-6789",
            PayTo = "Jane Smith",
            CheckAmount = 1250.75M
        };
    }
}
