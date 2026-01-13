namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

public sealed record UpdateBankRequest
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public string? OfficeType { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }

    public string? Phone { get; init; }

    public string? Status { get; init; }

    public bool IsDisabled { get; init; }
}
