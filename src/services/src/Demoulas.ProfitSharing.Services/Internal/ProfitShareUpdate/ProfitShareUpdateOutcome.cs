using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

namespace Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;

internal sealed record ProfitShareUpdateOutcome(
    List<MemberFinancials> MemberFinancials,
    AdjustmentsSummaryDto AdjustmentsSummaryData,
    ProfitShareUpdateTotals ProfitShareUpdateTotals,
    bool RerunNeeded
);
