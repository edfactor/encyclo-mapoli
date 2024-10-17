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

    public IQueryable<ParticipantTotalRatioDto> GetVestingRatio(IProfitSharingDbContext ctx, DateOnly asOfDate)
    {
        var profitYear = (short)asOfDate.Year;

        var BirthDate65 = asOfDate.AddYears(-65);
        var BeginningOfYear = asOfDate.AddYears(-1).AddDays(1);

        var contributionYears = (from pd in ctx.ProfitDetails
                                 where pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions
                                 group pd by pd.Ssn into g
                                 select new { Ssn = g.Key, Years = g.Count() }); //Need to verify logic here

        var demoInfo = (
            from d in ctx.Demographics
            join pp in ctx.PayProfits on new { d.OracleHcmId, ProfitYear = profitYear } equals new { pp.OracleHcmId, pp.ProfitYear }
            join cy in contributionYears on d.Ssn equals cy.Ssn
            select new
            {
                d.Ssn,
                pp.EnrollmentId,
                d.TerminationCodeId,
                d.TerminationDate,
                pp.ZeroContributionReasonId,
                d.DateOfBirth,
                FromBeneficiary = (short)0,
                Years = cy.Years,
                Hours = pp.CurrentHoursYear,
            }
        );

        var beneficiaryInfo = (
            from b in ctx.Beneficiaries
            join dt in ctx.Demographics on b.Contact!.Ssn equals dt.Ssn into d_join
            where !d_join.Any()
            select new
            {
                b.Contact!.Ssn,
                EnrollmentId = (byte)0,
                TerminationCodeId = (char?)null,
                TerminationDate = (DateOnly?)null,
                ZeroContributionReasonId = (byte?)null,
                b.Contact.DateOfBirth,
                FromBeneficiary = (short)1,
                Years = 0,
                Hours = (Decimal?)0
            }
        );

        var demoOrBeneficiary = demoInfo.Union(beneficiaryInfo);

#pragma warning disable S1244 // Floating point numbers should not be tested for equality
#pragma warning disable S3358 // Ternary operators should not be nested
        return (
            from db in demoOrBeneficiary
            select new ParticipantTotalRatioDto()
            {
                Ssn = db.Ssn,
                Ratio = (double)db.FromBeneficiary == 1 ? 1.0 :
                        db.DateOfBirth <= BirthDate65 && (db.TerminationDate == null || db.TerminationDate < BeginningOfYear) ? 1 :
                        db.EnrollmentId == 3 || db.EnrollmentId == 4 ? 1 :
                        db.TerminationCodeId == 'Z' ? 1 :
                        db.ZeroContributionReasonId == 6 ? 1 :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years < 3 ? 0 :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years == 3 ? .2 :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years == 4 ? .4 :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years == 5 ? .6 :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years == 6 ? .8 :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years > 6 ? 1 : 0
            }
        );
#pragma warning restore S3358 // Ternary operators should not be nested
#pragma warning restore S1244 // Floating point numbers should not be tested for equality
    }
}
