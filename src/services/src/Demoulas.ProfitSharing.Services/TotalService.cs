using Demoulas.ProfitSharing.Common.Contracts.Services;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services;
public sealed class TotalService
{
    public IQueryable<ParticipantTotalDto> GetTotalBalanceSet(IProfitSharingDbContext ctx, short profitYear)
    {
        var sumAllFieldProfitCodeTypes = new[] {
                                       ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                                       ProfitCode.Constants.OutgoingForfeitures.Id,
                                       ProfitCode.Constants.OutgoingDirectPayments.Id,
                                       ProfitCode.Constants.OutgoingXferBeneficiary.Id
        };

#pragma warning disable S3358 // Ternary operators should not be nested
        return (from pd in ctx.ProfitDetails
                where pd.ProfitYear <= profitYear
                group pd by pd.Ssn into pd_g
                select new ParticipantTotalDto
                {
                    Ssn = pd_g.Key,
                    Total = pd_g.Sum(x=> x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment ? x.Forfeiture * -1 : //Just look at forfeiture
                                          sumAllFieldProfitCodeTypes.Contains(x.ProfitCodeId) ? - x.Forfeiture + x.Contribution + x.Earnings : //Invert forfeiture, and add columns
                                          x.Contribution + x.Earnings + x.Forfeiture) //Just add the columns
                });
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    public IQueryable<ParticipantTotalDto> GetTotalEtva(IProfitSharingDbContext ctx, short profitYear)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear <= profitYear
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

    public IQueryable<ParticipantTotalDto> GetTotalDistributions(IProfitSharingDbContext ctx, short profitYear)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear <= profitYear
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

    public IQueryable<ParticipantTotalDto> GetGrossAmount(IProfitSharingDbContext ctx, short profitYear)
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

    public IQueryable<ParticipantTotalYearsDto> GetYearsOfService(IProfitSharingDbContext ctx, short profitYear)
    {
        return (from pd in ctx.ProfitDetails
                where pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions &&
                      pd.ProfitYearIteration == 0 &&
                      pd.ProfitYear <= profitYear
                group pd by pd.Ssn into g
                select new ParticipantTotalYearsDto { Ssn = g.Key, Years = (short)g.Count() }); //Need to verify logic here
    }

    public IQueryable<ParticipantTotalRatioDto> GetVestingRatio(IProfitSharingDbContext ctx, short profitYear, DateOnly asOfDate)
    {

        var BirthDate65 = asOfDate.AddYears(-65);
        var BeginningOfYear = asOfDate.AddYears(-1).AddDays(1);

        var demoInfo = (
            from d in ctx.Demographics
            join pp in ctx.PayProfits on new { d.Id, ProfitYear = profitYear } equals new { Id = pp.DemographicId, pp.ProfitYear }
            join cy in GetYearsOfService(ctx, profitYear) on d.Ssn equals cy.Ssn
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
                Years = (short)0,
                Hours = (decimal)0
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
                Ratio = db.FromBeneficiary == 1 ? 1.0 :
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
