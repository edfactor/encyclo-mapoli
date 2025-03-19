using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;

/// <summary>
/// A single year of totals extracted from Profit Detail records.
/// </summary>
internal sealed record ProfitDetailTotals(
    decimal DistributionsTotal,
    decimal ForfeitsTotal,
    decimal AllocationsTotal,
    decimal PaidAllocationsTotal,
    decimal MilitaryTotal,
    decimal ClassActionFundTotal)
{
    public static ProfitDetailTotals Zero { get; set; } = new ProfitDetailTotals(0m, 0m, 0m, 0m, 0m, 0m);

    /// <summary>
    /// Extracts a single year of profit_detail transactions.   Ignores any 0 records, special handling for ClassActionFund and Military.
    /// </summary>
    internal static Task<Dictionary<int, ProfitDetailTotals>> GetProfitDetailTotalsForASingleYear(IProfitSharingDataContextFactory dbFactory, short profitYear, HashSet<int> ssns,
        CancellationToken cancellationToken)
    {
        return dbFactory.UseReadOnlyContext(ctx =>
        {
            var query = ctx.ProfitDetails
                .Where(pd => ssns.Contains(pd.Ssn))
                .Where(pd => pd.ProfitYear == profitYear)
                .GroupBy(pd => pd.Ssn) // Grouping by Ssn
                .Select(g => new
                {
                    Ssn = g.Key,
                    DistributionsTotal = g.Where(pd =>
                            pd.ProfitCodeId == /*1*/ ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal ||
                            pd.ProfitCodeId == /*3*/ ProfitCode.Constants.OutgoingDirectPayments ||
                            (pd.ProfitCodeId == /*9*/ProfitCode.Constants.Outgoing100PercentVestedPayment &&
                             !(pd.CommentType == CommentType.Constants.TransferOut ||
                               pd.CommentType == CommentType.Constants.QdroOut)))
                        .Sum(pd => pd.Forfeiture),
                    PaidAllocationsTotal = g.Where(pd =>
                            (pd.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment &&
                             (pd.CommentType == CommentType.Constants.TransferOut ||
                              pd.CommentType == CommentType.Constants.QdroOut)) ||
                            pd.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary)
                        .Sum(pd => pd.Forfeiture),
                    ForfeitsTotal = g.Where(pd => pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures)
                        .Sum(pd => pd.Forfeiture),
                    AllocationsTotal = g.Where(pd => pd.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary)
                        .Sum(pd => pd.Contribution),
                    MilitaryTotal = g.Where(pd => pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationMilitary)
                        .Sum(pd => pd.Contribution),
                    ClassActionFundTotal = g.Where(pd => pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationClassActionFund)
                        .Sum(pd => pd.Earnings)
                });
            
            return query.ToDictionaryAsync(k => k.Ssn, v => new ProfitDetailTotals
                (v.DistributionsTotal,
                    v.ForfeitsTotal,
                    v.AllocationsTotal,
                    v.PaidAllocationsTotal,
                    v.MilitaryTotal,
                    v.ClassActionFundTotal
                )
                , cancellationToken);
        });
    }
}
