namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record EmployeeTypeResponseDto
{
    public byte Id { get; set; }
    public required string Name { get; set; }
}
