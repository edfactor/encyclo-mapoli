using System.Diagnostics.CodeAnalysis;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.ProfitShareEdit;

[SuppressMessage("ReSharper", "MergeConditionalExpression")]
internal static class BeneficiariesProcessingHelper
{
    internal static async Task ProcessBeneficiaries(IProfitSharingDataContextFactory dbContextFactory, TotalService totalsService, List<MemberFinancials> members,
        ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        // The BeginningBalance is the total balance from the previous year
        short profitYearPrior = (short)(profitShareUpdateRequest.ProfitYear - 1);

        List<BeneficiaryFinancials> benes = await dbContextFactory.UseReadOnlyContext(ctx =>
            {
                return ctx.Beneficiaries
                    .Include(b => b.Contact)
                    .ThenInclude(c => c!.ContactInfo)
                    .Join(totalsService.GetTotalBalanceSet(ctx, profitYearPrior), b => b.Contact!.Ssn, tbs => tbs.Ssn,
                        (bene, tbs) => new { bene = bene, BeginningBalance = tbs.Total })
                    .OrderBy(bt => bt.bene.Contact!.ContactInfo.FullName)
                    .ThenByDescending(bt => bt.bene.BadgeNumber * 10000 + bt.bene.PsnSuffix).Select(bt =>
                        new BeneficiaryFinancials
                        {
                            Psn = bt.bene.Psn,
                            Ssn = bt.bene.Contact!.Ssn,
                            Name = bt.bene.Contact.ContactInfo.FullName,
                            BeginningBalance = (bt.BeginningBalance == null) ? 0 : bt.BeginningBalance.Value,
                        }).ToListAsync(cancellationToken);
            }
        );

        Dictionary<int, ProfitDetailTotals> thisYearsTotalsBySSn =
            await TotalService.GetProfitDetailTotalsForASingleYear(dbContextFactory, profitShareUpdateRequest.ProfitYear, [.. benes.Select(ef => ef.Ssn)], cancellationToken);

        foreach (BeneficiaryFinancials bene in benes)
        {
            // See if this bene is also an employee
            MemberFinancials? employee = members.FirstOrDefault(m => m.Ssn == bene.Ssn);
            if (employee != null)
            {
                // If an employee has an ETVA amount and no years on the plan he is a beneficiary
                if (employee.Etva > 0 && employee.EmployeeTypeId == /*0*/ EmployeeType.Constants.NotNewLastYear && /* Beginning Amount */ employee.CurrentAmount == 0)
                {
                    // We flag the employee as also being a beneficiary
                    employee.TreatAsBeneficiary = true;
                }
                continue;
            }

            MemberFinancials memb = ProcessBeneficiary(bene, thisYearsTotalsBySSn.GetValueOrDefault(bene.Ssn) ?? ProfitDetailTotals.Zero, profitShareUpdateRequest);
            if (!memb.IsAllZeros())
            {
                members.Add(memb);
            }
        }
    }

    private static MemberFinancials ProcessBeneficiary(BeneficiaryFinancials bene, ProfitDetailTotals thisYearsTotals, ProfitShareUpdateRequest profitShareUpdateRequest)
    {
        MemberTotals memberTotals = new()
        {
            NewCurrentAmount = bene.BeginningBalance 
                               + thisYearsTotals.AllocationsTotal 
                               + thisYearsTotals.ClassActionFundTotal 
                               - thisYearsTotals.ForfeitsTotal 
                               - thisYearsTotals.PaidAllocationsTotal
                               - thisYearsTotals.DistributionsTotal
        };
        // Yea, adding and removing ClassActionFundTotal - is strange
        memberTotals.NewCurrentAmount -= thisYearsTotals.ClassActionFundTotal;

        if (memberTotals.NewCurrentAmount > 0)
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.NewCurrentAmount, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (int)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        memberTotals.EarningsAmount = Math.Round(profitShareUpdateRequest.EarningsPercent * memberTotals.EarnPoints, 2,
            MidpointRounding.AwayFromZero);

        memberTotals.SecondaryEarningsAmount =
            Math.Round(profitShareUpdateRequest.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);

        return new MemberFinancials(bene, thisYearsTotals, memberTotals);
    }
    
}
