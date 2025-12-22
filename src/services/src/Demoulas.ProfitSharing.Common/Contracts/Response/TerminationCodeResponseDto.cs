namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public class TerminationCodeResponseDto
{
    public char Id { get; set; }
    public required string Name { get; set; }

    public static TerminationCodeResponseDto ResponseExample()
    {
        return new TerminationCodeResponseDto
        {
            Id = 'V',
            Name = "Voluntary"
        };
    }
}
