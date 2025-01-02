using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;

/// <summary>
/// A single year of totals extracted from Profit Detail records.
/// </summary>
public record ProfitDetailTotals(
    decimal DistributionsTotal,
    decimal ForfeitsTotal,
    decimal AllocationsTotal,
    decimal PaidAllocationsTotal,
    decimal MilitaryTotal,
    decimal ClassActionFundTotal)
{
    /// <summary>
    /// Extracts a single year of profit_detail transactions.   Ignores any 0 records, special handling for ClassActionFund and Military.
    /// </summary>
    /// <param name="dbFactory"></param>
    /// <param name="profitYear"></param>
    /// <param name="ssn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static async Task<ProfitDetailTotals> GetProfitDetailTotals(IProfitSharingDataContextFactory dbFactory, short profitYear, int ssn, CancellationToken cancellationToken)
    {
        decimal distributionsTotal = 0;
        decimal forfeitsTotal = 0;
        decimal allocationsTotal = 0;
        decimal paidAllocationsTotal = 0;
        decimal militaryTotal = 0;
        decimal classActionFundTotal = 0;

        List<ProfitDetail> pds = await dbFactory.UseReadOnlyContext(ctx => ctx.ProfitDetails.Where(pd => pd.Ssn == ssn && pd.ProfitYear == profitYear)
            .OrderBy(pd => pd.ProfitYear).ThenBy(pd => pd.ProfitYearIteration).ThenBy(pd => pd.MonthToDate)
            .ThenBy(pd => pd.FederalTaxes).Include(profitDetail => profitDetail.CommentType)
            .ToListAsync(cancellationToken));

        foreach (ProfitDetail pd in pds)
        {
            if (pd.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal /*1*/ ||
                pd.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments /*3*/)
            {
                distributionsTotal += pd.Forfeiture;
            }

            if (pd.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment /*9*/)
            {
                if (Equals(pd.CommentType, CommentType.Constants.TransferOut) /* "XFER >" or "XFER>" */ ||
                    Equals(pd.CommentType, CommentType.Constants.QdroOut) /* "QDRO >" or "QDRO>" */)
                {
                    paidAllocationsTotal += pd.Forfeiture;
                }
                else
                {
                    distributionsTotal += pd.Forfeiture;
                }
            }

            if (pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures /*2*/)
            {
                forfeitsTotal += pd.Forfeiture;
            }

            if (pd.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary /*5*/)
            {
                paidAllocationsTotal += pd.Forfeiture;
            }

            if (pd.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary /*6*/)
            {
                allocationsTotal += pd.Contribution;
            }

            if (pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationMilitary /*1*/)
            {
                militaryTotal += pd.Contribution;
            }

            if (pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationClassActionFund /*2*/)
            {
                classActionFundTotal += pd.Earnings;
            }
        }

        return new ProfitDetailTotals(
            distributionsTotal,
            forfeitsTotal,
            allocationsTotal,
            paidAllocationsTotal,
            militaryTotal,
            classActionFundTotal);
    }
}
