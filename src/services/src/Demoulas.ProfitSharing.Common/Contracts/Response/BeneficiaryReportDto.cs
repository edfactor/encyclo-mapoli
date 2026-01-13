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

    public static BeneficiaryReportDto ResponseExample() => new(
        BeneficiaryId: 1,
        FullName: "Jane Smith",
        Ssn: "***-**-6789",
        Relationship: "Spouse",
        Balance: 150000.00m,
        BadgeNumber: 12345,
        PsnSuffix: 1,
        ProfitDetails: new List<ProfitDetailDto> { ProfitDetailDto.ResponseExample() }
    )
    {
        IsExecutive = false
    };
};
