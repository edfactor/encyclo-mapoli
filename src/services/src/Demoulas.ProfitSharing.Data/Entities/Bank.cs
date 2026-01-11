namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Represents a bank record used for check printing, MICR configuration, and ACH/Fedwire reference.
/// </summary>
public sealed class Bank
{
    /// <summary>
    /// The 9-digit ABA routing number (leading zeros preserved).
    /// </summary>
    public required string RoutingNumber { get; set; }

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
    /// Optional bank account number used for MICR/printing.
    /// NOTE: If populated in the future, this should be treated as sensitive and secured appropriately.
    /// </summary>
    public string? AccountNumber { get; set; }
}
