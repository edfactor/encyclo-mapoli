using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;

[NoMemberDataExposed]
public sealed record UpdateBeneficiaryResponse : UpdateBeneficiaryContactResponse
{
    public int BeneficiaryContactId { get; set; }
    public char? KindId { get; set; }
    public string? Relationship { get; set; }
    public decimal Percentage { get; set; }
    public int DemographicId { get; set; }
    public int BadgeNumber { get; set; }
}
