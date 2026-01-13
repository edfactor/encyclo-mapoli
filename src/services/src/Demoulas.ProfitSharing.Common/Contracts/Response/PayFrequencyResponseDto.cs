namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public class PayFrequencyResponseDto
{
    public byte Id { get; set; }
    public required string Name { get; set; }

    public static PayFrequencyResponseDto ResponseExample()
    {
        return new PayFrequencyResponseDto
        {
            Id = 1,
            Name = "Weekly"
        };
    }
}
