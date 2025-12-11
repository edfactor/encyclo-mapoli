namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record EnrollmentResponseDto
{
    public required byte Id { get; set; }
    public required string Name { get; set; }
}
