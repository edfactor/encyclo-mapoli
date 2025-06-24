using System;
using System.Collections.Generic;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public record ProfitDetailDto(
    int Year,
    string Code,
    decimal Contributions,
    decimal Earnings,
    decimal Forfeitures,
    DateOnly Date,
    string? Comments
);

public record BeneficiaryReportDto(
    int BeneficiaryId,
    string FullName,
    string Ssn,
    string? Relationship,
    decimal? Balance,
    int? BadgeNumber,
    List<ProfitDetailDto>? ProfitDetails
);

public record AdhocBeneficiariesReportResponse : ReportResponseBase<BeneficiaryReportDto>
{
    public decimal TotalEndingBalance { get; init; }
    public int TotalCount { get; init; }
}
