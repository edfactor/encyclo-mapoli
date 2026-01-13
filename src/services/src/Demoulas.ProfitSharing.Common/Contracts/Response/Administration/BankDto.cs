using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Administration;

public sealed record BankDto
{
    public required int Id { get; init; }

    [UnmaskSensitive]
    public required string Name { get; init; }

    public string? OfficeType { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }

    public string? Phone { get; init; }

    public string? Status { get; init; }

    public bool IsDisabled { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string CreatedBy { get; init; } = string.Empty;

    public DateTimeOffset? ModifiedAtUtc { get; init; }

    public string? ModifiedBy { get; init; }

    public int AccountCount { get; init; }

    public static BankDto ResponseExample()
    {
        return new BankDto
        {
            Id = 1,
            Name = "Newtek Bank, NA",
            OfficeType = "Main Office",
            City = "Lake Success",
            State = "NY",
            Phone = "516-254-7586",
            Status = "Active",
            IsDisabled = false,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CreatedBy = "SYSTEM",
            ModifiedAtUtc = null,
            ModifiedBy = null,
            AccountCount = 1
        };
    }
}
