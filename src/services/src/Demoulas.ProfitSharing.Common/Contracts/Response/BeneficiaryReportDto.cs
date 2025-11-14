using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record BeneficiaryReportDto(
    int BeneficiaryId,
    string FullName,
    string Ssn,
    string? Relationship,
    decimal? Balance,
    int BadgeNumber,
    short PsnSuffix,
    List<ProfitDetailDto>? ProfitDetails
) : IIsExecutive
{
    public required bool IsExecutive { get; set; }
};