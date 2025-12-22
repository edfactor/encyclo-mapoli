using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record GetEligibleEmployeesResponse : ReportResponseBase<EligibleEmployee>
{
    [YearEndArchiveProperty]
    public required int NumberReadOnFrozen { get; set; }
    [YearEndArchiveProperty]
    public required int NumberNotSelected { get; set; }
    [YearEndArchiveProperty]
    public required int NumberWritten { get; set; }
}
