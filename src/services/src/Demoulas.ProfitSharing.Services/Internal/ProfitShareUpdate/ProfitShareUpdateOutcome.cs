namespace Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;

internal sealed record ProfitShareUpdateOutcome(
    List<MemberFinancials> MemberFinancials,
    AdjustmentReportData AdjustmentReportData,
    bool RerunNeeded
);
