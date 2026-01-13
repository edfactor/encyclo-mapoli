namespace Demoulas.ProfitSharing.Data.Entities.Audit;

public sealed class BeneficiaryContactArchive
{
    public int ArchiveId { get; set; }
    public required int Id { get; set; }

    public required int Ssn { get; set; }

    public required DateOnly DateOfBirth { get; set; }

    public required Address Address { get; set; }
    public required ContactInfo ContactInfo { get; set; }

    public required DateOnly CreatedDate { get; set; }
    public DateOnly DeleteDate { get; set; }
    public required string DeletedBy { get; set; }
}
