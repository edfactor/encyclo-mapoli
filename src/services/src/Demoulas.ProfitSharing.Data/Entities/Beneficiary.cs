using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class Beneficiary : Member
{
    public required short PsnSuffix { get; set; } // Suffix for hierarchy: (1000, 2000, etc.)
    public required int DemographicId { get; set; }

    // Reconstruct the full PSN dynamically
    public string Psn => $"{BadgeNumber}{PsnSuffix:D4}"; // Reconstruct PSN w

    public BeneficiaryContact? Contact { get; set; }
    public required int BeneficiaryContactId { get; set; }

    public string? Relationship { get; set; }
    public required decimal Percent { get; set; }
    public Demographic? Demographic { get; set; }


}
