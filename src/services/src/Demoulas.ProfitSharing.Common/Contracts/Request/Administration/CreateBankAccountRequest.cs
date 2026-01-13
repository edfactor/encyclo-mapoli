namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

public sealed record CreateBankAccountRequest
{
    public required int BankId { get; init; }

    public required string RoutingNumber { get; init; }

    public required string AccountNumber { get; init; }

    public bool IsPrimary { get; init; }

    public string? ServicingFedRoutingNumber { get; init; }

    public string? ServicingFedAddress { get; init; }

    public string? FedwireTelegraphicName { get; init; }

    public string? FedwireLocation { get; init; }

    public DateOnly? FedAchChangeDate { get; init; }

    public DateOnly? FedwireRevisionDate { get; init; }

    public string? Notes { get; init; }

    public DateOnly? EffectiveDate { get; init; }

    public static CreateBankAccountRequest RequestExample() => new()
    {
        BankId = 1,
        RoutingNumber = "026004297",
        AccountNumber = "0375495656",
        IsPrimary = true,
        EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
    };
}
