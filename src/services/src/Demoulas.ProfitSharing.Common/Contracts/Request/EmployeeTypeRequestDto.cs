namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record EmployeeTypeRequestDto
{
    public char Id { get; set; }
    public required string Name { get; set; }
}
