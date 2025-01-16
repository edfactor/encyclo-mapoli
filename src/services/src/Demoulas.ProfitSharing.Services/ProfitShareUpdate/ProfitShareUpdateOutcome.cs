namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;

public record ProfitShareUpdateOutcome(
    List<MemberFinancials> MemberFinancials,
    AdjustmentReportData AdjustmentReportData,
    bool RerunNeeded
);
