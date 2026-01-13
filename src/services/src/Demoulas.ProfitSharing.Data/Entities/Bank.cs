using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Represents a bank record used for check printing, MICR configuration, and ACH/Fedwire reference.
/// </summary>
public sealed class Bank : ModifiedBase
{
    /// <summary>
    /// Unique identifier for this bank record (primary key).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The 9-digit ABA routing number (leading zeros preserved).
    /// Kept for backwards compatibility during Phase 1 transition.
    /// </summary>
    public string? RoutingNumber { get; set; }

    /// <summary>
    /// The bank's name as used in operational/financial references.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Optional office type (e.g., Main Office).
    /// </summary>
    public string? OfficeType { get; set; }

    /// <summary>
    /// Optional city for the bank office.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Optional 2-character state abbreviation for the bank office.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Optional phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Optional status (e.g., Active).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// FedACH change date (when available).
    /// </summary>
    public DateOnly? FedAchChangeDate { get; set; }

    /// <summary>
    /// Servicing Federal Reserve routing number (when applicable).
    /// </summary>
    public string? ServicingFedRoutingNumber { get; set; }

    /// <summary>
    /// Servicing Federal Reserve office address (when applicable).
    /// </summary>
    public string? ServicingFedAddress { get; set; }

    /// <summary>
    /// Fedwire telegraphic name (when available).
    /// </summary>
    public string? FedwireTelegraphicName { get; set; }

    /// <summary>
    /// Fedwire location (when available).
    /// </summary>
    public string? FedwireLocation { get; set; }

    /// <summary>
    /// Fedwire revision date (when available).
    /// </summary>
    public DateOnly? FedwireRevisionDate { get; set; }

    /// <summary>
    /// Indicates whether this bank is disabled and should not be used for new transactions.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Username of the person who created this bank record.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Username of the person who last modified this bank record.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Navigation property to bank accounts associated with this bank.
    /// One bank can have multiple accounts.
    /// </summary>
    public ICollection<BankAccount> Accounts { get; set; } = new List<BankAccount>();
}
