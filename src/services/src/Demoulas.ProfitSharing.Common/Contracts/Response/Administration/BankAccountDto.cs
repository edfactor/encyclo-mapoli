using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Administration;

public sealed record BankAccountDto
{
    public required int Id { get; init; }

    public required int BankId { get; init; }

    [UnmaskSensitive]
    public required string BankName { get; init; }

    public required string RoutingNumber { get; init; }

    /// <summary>
    /// Masked account number for display (e.g., "****7890")
    /// </summary>
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

    public DateTimeOffset CreatedAtUtc { get; init; }

    public string CreatedBy { get; init; } = string.Empty;

    public DateTimeOffset? ModifiedAtUtc { get; init; }

    public string? ModifiedBy { get; init; }

    public static BankAccountDto ResponseExample()
    {
        return new BankAccountDto
        {
            Id = 1,
            BankId = 1,
            BankName = "Newtek Bank, NA",
            RoutingNumber = "026004297",
            AccountNumber = "******5656",
            IsPrimary = true,
            IsDisabled = false,
            ServicingFedRoutingNumber = "021001208",
            ServicingFedAddress = "100 Orchard Street, East Rutherford, NJ",
            FedwireTelegraphicName = "NEWTEK BANK, NA",
            FedwireLocation = "Miami, FL",
            FedAchChangeDate = new DateOnly(2024, 7, 30),
            FedwireRevisionDate = new DateOnly(2023, 7, 6),
            Notes = null,
            EffectiveDate = new DateOnly(2024, 1, 1),
            DiscontinuedDate = null,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            CreatedBy = "SYSTEM",
            ModifiedAtUtc = null,
            ModifiedBy = null
        };
    }
}
