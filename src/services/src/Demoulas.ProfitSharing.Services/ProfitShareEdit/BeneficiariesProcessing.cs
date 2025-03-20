using System.Diagnostics.CodeAnalysis;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.ProfitShareEdit;

internal static class BeneficiariesProcessingHelper
{
    internal static async Task ProcessBeneficiaries(IProfitSharingDataContextFactory dbContextFactory, List<MemberFinancials> members,
        ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        List<BeneficiaryFinancials> benes = await dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.Beneficiaries
                .Include(b => b.Contact)
                .ThenInclude(c => c!.ContactInfo)
                .OrderBy(b => b.Contact!.ContactInfo.FullName)
                .ThenByDescending(b => b.BadgeNumber * 10000 + b.PsnSuffix).Select(b =>
                    new BeneficiaryFinancials
                    {
                        Psn = b.Psn,
                        Ssn = b.Contact!.Ssn,
                        Name = b.Contact.ContactInfo.FullName,
                        CurrentAmount = b.Amount // Should be computing this from the ProfitDetail via TotalService
                    }).ToListAsync(cancellationToken)
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
            // Yea, the next two lines - adding and removing ClassActionFundTotal - is strange
            NewCurrentAmount = thisYearsTotals.AllocationsTotal + thisYearsTotals.ClassActionFundTotal +
                               (bene.CurrentAmount - thisYearsTotals.ForfeitsTotal -
                                thisYearsTotals.PaidAllocationsTotal) -
                               thisYearsTotals.DistributionsTotal
        };
        memberTotals.NewCurrentAmount -= thisYearsTotals.ClassActionFundTotal;

        if (memberTotals.NewCurrentAmount > 0)
        {
            memberTotals.PointsDollars = Math.Round(memberTotals.NewCurrentAmount, 2, MidpointRounding.AwayFromZero);
            memberTotals.EarnPoints = (int)Math.Round(memberTotals.PointsDollars / 100, MidpointRounding.AwayFromZero);
        }

        ComputeEarningsForBeneficiary(memberTotals, bene, profitShareUpdateRequest);

        return new MemberFinancials(bene, thisYearsTotals, memberTotals);
    }

    private static void ComputeEarningsForBeneficiary(MemberTotals memberTotals, BeneficiaryFinancials bene, ProfitShareUpdateRequest profitShareUpdateRequest)
    {
        memberTotals.EarningsAmount = Math.Round(profitShareUpdateRequest.EarningsPercent * memberTotals.EarnPoints, 2,
            MidpointRounding.AwayFromZero);

        bene!.Earnings = memberTotals.EarningsAmount;

        memberTotals.SecondaryEarningsAmount =
            Math.Round(profitShareUpdateRequest.SecondaryEarningsPercent * memberTotals.EarnPoints, 2,
                MidpointRounding.AwayFromZero);

        bene.SecondaryEarnings = memberTotals.SecondaryEarningsAmount;
    }
}
