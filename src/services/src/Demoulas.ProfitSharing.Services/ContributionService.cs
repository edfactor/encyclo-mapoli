using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.InternalDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public sealed class ContributionService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public ContributionService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }


    internal Task<Dictionary<int, byte>> GetContributionYears(short profitYear, ISet<int> badgeNumberSet, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(context => GetContributionYears(context, profitYear, badgeNumberSet, cancellationToken));
    }

    internal static Task<Dictionary<int, byte>> GetContributionYears(IProfitSharingDbContext context, short profitYear, ISet<int> badgeNumberSet, CancellationToken cancellationToken)
    {
        return GetContributionYearsQuery(context, profitYear, badgeNumberSet)
            .ToDictionaryAsync(arg => arg.BadgeNumber, arg => arg.YearsInPlan, cancellationToken);
    }

    internal static IQueryable<ContributionYears> GetContributionYearsQuery(IProfitSharingDbContext context, short profitYear, IEnumerable<int> badgeNumberSet)
    {
        return context.PayProfits
            .Include(p => p.Demographic)
            .Where(p => p.ProfitYear <= profitYear && badgeNumberSet.Contains(p.Demographic!.BadgeNumber))
            .GroupBy(p => p.Demographic!.BadgeNumber)
            .Select(p => new ContributionYears { BadgeNumber = p.Key, YearsInPlan = (byte)p.Count() });
    }

    internal Task<Dictionary<int, InternalProfitDetailDto>> GetNetBalance(short profitYear, IEnumerable<int> badgeNumbers, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx => GetNetBalance(ctx, profitYear, badgeNumbers, cancellationToken));
    }

    internal Task<Dictionary<int, InternalProfitDetailDto>> GetNetBalance(IProfitSharingDbContext context, short profitYear, IEnumerable<int> badgeNumbers,
        CancellationToken cancellationToken)
    {
        var badgeHash = badgeNumbers.ToHashSet();
        var pdQuery = context.ProfitDetails
            .Where(details => details.ProfitYear <= profitYear)
            .GroupBy(details => details.Ssn)
            .Select(g => new
            {
                Ssn = g.Key,
                TotalContributions = g.Sum(x => x.Contribution),
                TotalEarnings = g.Sum(x => x.Earnings),
                TotalForfeitures = g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.IncomingContributions ? x.Forfeiture : 0),
                TotalPayments = g.Sum(x => x.ProfitCodeId != ProfitCode.Constants.IncomingContributions ? x.Forfeiture : 0),
                TotalFedTaxes = g.Sum(x => x.FederalTaxes),
                TotalStateTaxes = g.Sum(x => x.StateTaxes)
            });



        var demoQuery = context.Demographics
            .Where(d => badgeHash.Contains(d.BadgeNumber))
            .Select(d => new { d.OracleHcmId, d.BadgeNumber, d.Ssn });

        var query = from d in demoQuery
            join r in pdQuery on d.Ssn equals r.Ssn
            select new InternalProfitDetailDto
            {
                OracleHcmId = d.OracleHcmId,
                BadgeNumber = d.BadgeNumber,
                TotalContributions = r.TotalContributions,
                TotalEarnings = r.TotalEarnings,
                TotalForfeitures = r.TotalForfeitures,
                TotalPayments = r.TotalPayments,
                TotalFederalTaxes = r.TotalFedTaxes,
                TotalStateTaxes = r.TotalStateTaxes,
                CurrentAmount = r.TotalContributions + r.TotalEarnings + r.TotalForfeitures - r.TotalPayments
            };

        return query.ToDictionaryAsync(d => d.BadgeNumber, cancellationToken);
    }

    public static decimal CalculateCurrentVested(List<ProfitDetail> payments, decimal currentAmount, decimal vestedPercentage)
    {
        if (vestedPercentage == 100)
        {
            return currentAmount;
        }

        // First, adjust for before vesting
        decimal vestedAmount = CalculatePayments(payments, currentAmount, true);

        // Multiply by the vesting percentage
        decimal percentage = vestedPercentage / 100;
        vestedAmount *= percentage;

        // Then, adjust for after vesting
        vestedAmount = CalculatePayments(payments, vestedAmount, false);

        return Math.Round(vestedAmount, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal CalculatePayments(List<ProfitDetail> payments, decimal currentAmount, bool beforeVesting)
    {
        decimal vestedAmount = currentAmount;
        decimal codeEightEarnings = 0;
        decimal codeNineForfeiture = 0;

        foreach (var payment in payments)
        {
            switch (payment.ProfitCodeId)
            {
                case var _ when payment.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id:  // Partial withdrawal
                case var _ when payment.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id:  // Outgoing forfeitures
                case var _ when payment.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments.Id:  // Direct payments / rollovers
                case var _ when payment.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id:  // XFER beneficiary / QDRO allocation
                    vestedAmount = beforeVesting ? vestedAmount + payment.Forfeiture : vestedAmount - payment.Forfeiture;
                    break;

                case var _ when payment.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id:  // Incoming QDRO beneficiary allocation
                    vestedAmount = beforeVesting ? vestedAmount - payment.Contribution : vestedAmount + payment.Contribution;
                    break;

                case var _ when payment.ProfitCodeId == ProfitCode.Constants.Incoming100PercentVestedEarnings.Id:  // Incoming 100% vested earnings
                    codeEightEarnings += payment.Earnings;
                    break;

                case var _ when payment.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id:  // Outgoing payment from 100% vested amount (ETVA)
                    codeNineForfeiture += payment.Forfeiture;
                    break;
            }
        }

        // Calculate the ETVA value (Code 8 minus Code 9)
        decimal evta = codeEightEarnings - codeNineForfeiture;

        // Adjust the vested amount based on ETVA
        vestedAmount = beforeVesting ? vestedAmount - evta : vestedAmount + evta;

        return Math.Round(vestedAmount, 2, MidpointRounding.AwayFromZero);
    }

}
