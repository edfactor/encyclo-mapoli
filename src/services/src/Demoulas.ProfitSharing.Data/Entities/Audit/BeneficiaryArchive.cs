namespace Demoulas.ProfitSharing.Data.Entities.Audit;

public sealed class BeneficiaryArchive
{
    public int ArchiveId { get; set; }
    public required int Id { get; set; }
    public required int BadgeNumber { get; set; }
    public required short PsnSuffix { get; set; }
    public required int DemographicId { get; set; }
    public required int BeneficiaryContactId { get; set; }
    public string? Relationship { get; set; }
    public required decimal Percent { get; set; }
    public DateOnly DeleteDate { get; set; }
    public required string DeletedBy { get; set; }
}
