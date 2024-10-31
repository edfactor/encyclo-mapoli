using Demoulas.ProfitSharing.Common.Contracts.Services;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services;
public sealed class TotalService
{
    public IQueryable<ParticipantTotalDto> GetTotalBalanceSet(IProfitSharingDbContext ctx, DateOnly asOfDate)
    {
        return (from pd in ctx.ProfitDetails
                where pd.ProfitYear <= asOfDate.Year
                group pd by pd.Ssn into pd_g
                select new ParticipantTotalDto
                {
                    Ssn = pd_g.Key,
                    Total = pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment).Sum(x => x.Forfeiture * -1) +
                                   pd_g.Where(x => new[] {
                                       ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                                       ProfitCode.Constants.OutgoingForfeitures.Id,
                                       ProfitCode.Constants.OutgoingDirectPayments.Id,
                                       ProfitCode.Constants.OutgoingXferBeneficiary.Id
                                       }.Contains(x.ProfitCodeId)).Sum(x => x.Forfeiture + x.Contribution + x.Earnings) +
                                   pd_g.Where(x => !new[] {
                                       ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                                       ProfitCode.Constants.OutgoingForfeitures.Id,
                                       ProfitCode.Constants.OutgoingDirectPayments.Id,
                                       ProfitCode.Constants.OutgoingXferBeneficiary.Id,
                                       ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
                                       }.Contains(x.ProfitCodeId)).Sum(x => x.Earnings + x.Forfeiture)
                });
    }

    public IQueryable<ParticipantTotalDto> GetTotalEtva(IProfitSharingDbContext ctx, DateOnly asOfDate)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear <= asOfDate.Year
            group pd by pd.Ssn into pd_g
            select new ParticipantTotalDto
            {
                Ssn = pd_g.Key,
                Total = pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary).Sum(x => x.Contribution) +
                       pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.Incoming100PercentVestedEarnings.Id).Sum(x => x.Earnings) +
                       pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id).Sum(x => x.Forfeiture)
            }
        );
    }

    public IQueryable<ParticipantTotalDto> GetTotalDistributions(IProfitSharingDbContext ctx, DateOnly asOfDate)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear <= asOfDate.Year
            group pd by pd.Ssn into pd_g
            select new ParticipantTotalDto
            {
                Ssn = pd_g.Key,
                Total = pd_g.Where(x => new[] {
                        ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                        ProfitCode.Constants.OutgoingForfeitures.Id,
                        ProfitCode.Constants.OutgoingDirectPayments.Id,
                        ProfitCode.Constants.OutgoingXferBeneficiary.Id,
                        ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
                    }.Contains(x.ProfitCodeId)).Sum(x => x.Forfeiture)
            }
        );
    }

    public IQueryable<ParticipantTotalDto> GetGrossAmount(IProfitSharingDbContext ctx, DateOnly asOfDate)
    {
        return (
            from ds in ctx.Distributions
            group ds by ds.Ssn into ds_g
            select new ParticipantTotalDto()
            {
                Ssn = ds_g.Key,
                Total = ds_g.Where(x => x.StatusId != DistributionStatus.Constants.PurgeRecord && x.StatusId != DistributionStatus.Constants.PaymentMade).Sum(x => x.GrossAmount)
            }
        );
    }
}
