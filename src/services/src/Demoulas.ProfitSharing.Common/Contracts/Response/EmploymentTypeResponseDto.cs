namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public class EmploymentTypeResponseDto
{
    public char Id { get; set; }
    public required string Name { get; set; }

    public static EmploymentTypeResponseDto ResponseExample()
    {
        return new EmploymentTypeResponseDto
        {
            Id = 'F',
            Name = "Full Time"
        };
    }
}
