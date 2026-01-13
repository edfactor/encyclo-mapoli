namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

public sealed record UpdateBankAccountRequest
{
    public required int Id { get; init; }

    public required int BankId { get; init; }

    public required string RoutingNumber { get; init; }

    public required string AccountNumber { get; init; }

    public bool IsPrimary { get; init; }

    public bool IsDisabled { get; init; }

    public string? ServicingFedRoutingNumber { get; init; }

    public string? ServicingFedAddress { get; init; }

    public string? FedwireTelegraphicName { get; init; }

    public string? FedwireLocation { get; init; }

    public DateOnly? FedAchChangeDate { get; init; }

    public DateOnly? FedwireRevisionDate { get; init; }

    public string? Notes { get; init; }

    public DateOnly? EffectiveDate { get; init; }

    public DateOnly? DiscontinuedDate { get; init; }
}
