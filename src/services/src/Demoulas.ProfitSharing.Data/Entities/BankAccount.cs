using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Represents a specific bank account for check printing and ACH transactions.
/// Each bank can have multiple accounts, with one marked as primary.
/// </summary>
public sealed class BankAccount : ModifiedBase
{
    /// <summary>
    /// Unique identifier for this bank account record (primary key).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the parent Bank entity.
    /// </summary>
    public int BankId { get; set; }

    /// <summary>
    /// The 9-digit ABA routing number for this account (leading zeros preserved).
    /// Required for ACH/wire transactions.
    /// </summary>
    public required string RoutingNumber { get; set; }

    /// <summary>
    /// The bank account number for MICR/printing and ACH transactions.
    /// SECURITY: This field contains sensitive financial information and must be protected.
    /// Maximum length 34 characters per IBAN standard.
    /// </summary>
    public required string AccountNumber { get; set; }

    /// <summary>
    /// Friendly name/description for this account (e.g., "Payroll Account", "Benefits Account").
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// Indicates whether this is the primary/default account for this bank.
    /// Only one account per bank should be marked as primary.
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Indicates whether this account is disabled and should not be used for new transactions.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Servicing Federal Reserve Bank routing number (9 digits).
    /// This is the FRB that serves this bank for ACH transactions.
    /// </summary>
    public string? ServicingFedRoutingNumber { get; set; }

    /// <summary>
    /// Address of the servicing Federal Reserve Bank.
    /// Example: "Federal Reserve Bank of Boston, 600 Atlantic Avenue, Boston, MA 02210"
    /// </summary>
    public string? ServicingFedAddress { get; set; }

    /// <summary>
    /// Bank name as it appears in Fedwire transactions (telegraphic name).
    /// This is the official name used for wire transfers.
    /// </summary>
    public string? FedwireTelegraphicName { get; set; }

    /// <summary>
    /// City and state where the bank is located for Fedwire purposes.
    /// Example: "CHELSEA, MA"
    /// </summary>
    public string? FedwireLocation { get; set; }

    /// <summary>
    /// Date when the FedACH routing information was last changed or added.
    /// This tracks when the routing number became effective for ACH transactions.
    /// </summary>
    public DateOnly? FedAchChangeDate { get; set; }

    /// <summary>
    /// Date when the Fedwire routing information was last revised.
    /// This tracks updates to wire transfer routing information.
    /// </summary>
    public DateOnly? FedwireRevisionDate { get; set; }

    /// <summary>
    /// Additional notes or comments about this bank account.
    /// Can be used for internal documentation about account usage, restrictions, etc.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Date when this account becomes/became effective for use.
    /// Accounts cannot be used before this date.
    /// </summary>
    public DateOnly? EffectiveDate { get; set; }

    /// <summary>
    /// Date when this account was discontinued and should no longer be used.
    /// Once set, the account should be marked as disabled.
    /// </summary>
    public DateOnly? DiscontinuedDate { get; set; }

    /// <summary>
    /// Username of the person who created this bank account record.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Username of the person who last modified this bank account record.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Navigation property to the parent Bank entity.
    /// </summary>
    public Bank Bank { get; set; } = null!;
}
