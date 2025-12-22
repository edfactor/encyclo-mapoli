namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record EmployeeTypeResponseDto
{
    public byte Id { get; set; }
    public required string Name { get; set; }

    public static EmployeeTypeResponseDto ResponseExample()
    {
        return new EmployeeTypeResponseDto
        {
            Id = 1,
            Name = "Full Time"
        };
    }
}
