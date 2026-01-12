namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

public sealed record CreateBankRequest
{
    public required string Name { get; init; }

    public string? OfficeType { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }

    public string? Phone { get; init; }

    public string? Status { get; init; }

    public static CreateBankRequest RequestExample() => new()
    {
        Name = "Newtek Bank, NA",
        OfficeType = "Main Office",
        City = "Lake Success",
        State = "NY",
        Phone = "516-254-7586",
        Status = "Active"
    };
}
