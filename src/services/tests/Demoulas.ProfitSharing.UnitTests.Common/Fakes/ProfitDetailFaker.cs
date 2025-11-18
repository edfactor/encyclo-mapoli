using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

public sealed class ProfitDetailFaker : Faker<ProfitDetail>
{
    private static int _profitDetailCounter = 1000;

    public ProfitDetailFaker(IList<Demographic> demographicFakes)
    {
        var demographicQueue = new Queue<Demographic>(demographicFakes);

        Demographic currentDemographic = demographicQueue.Dequeue();
        var taxCodeFaker = new TaxCodeFaker();
        var profitCodeFaker = new ProfitCodeFaker();
        RuleFor(d => d.Id, f => _profitDetailCounter++);

        RuleFor(d => d.Ssn, (f, o) =>
        {
            // This code is non-intuitive.   The idea is that when the demographic
            var rslt = currentDemographic.Ssn; // association is made, we want the badge number and SSN to relate to existing
            if (demographicQueue.Any()) // demographic record that contains the both of them
            {
                // So by keeping a state field outside the lamdba, we can refer to an existing demographic
                currentDemographic = demographicQueue.Dequeue(); // record and copy its values.
            }
            else
            {
                demographicQueue = new Queue<Demographic>(demographicFakes);
                currentDemographic = demographicQueue.Dequeue();
            }

            return rslt;
        });

        RuleFor(pd => pd.ProfitYear, fake => fake.Random.Short(2016, 2024))
            .RuleFor(pd => pd.ProfitYearIteration, fake => fake.Random.Byte(0, 25))
            .RuleFor(pd => pd.DistributionSequence, fake => fake.Random.Int(short.MaxValue, ushort.MaxValue))
            .RuleFor(pd => pd.ProfitCodeId, fake => fake.PickRandom<byte>(ProfitCode.Constants.IncomingContributions.Id,
                ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                ProfitCode.Constants.OutgoingForfeitures.Id,
                ProfitCode.Constants.OutgoingDirectPayments.Id,
                ProfitCode.Constants.OutgoingXferBeneficiary.Id,
                ProfitCode.Constants.IncomingQdroBeneficiary.Id,
                ProfitCode.Constants.Incoming100PercentVestedEarnings.Id,
                ProfitCode.Constants.Outgoing100PercentVestedPayment.Id))
            .RuleFor(pd => pd.ProfitCode, fake => profitCodeFaker.Generate())
            .RuleFor(pd => pd.Contribution, fake => fake.Finance.Amount(0, 10000))
            .RuleFor(pd => pd.Earnings, fake => fake.Finance.Amount(-5000, 10000))
            .RuleFor(pd => pd.Forfeiture, fake => fake.Finance.Amount(0, short.MaxValue))
            .RuleFor(pd => pd.MonthToDate, fake => fake.Random.Byte(0, 12))
            .RuleFor(pd => pd.YearToDate, fake => (short)2024)
            .RuleFor(pd => pd.Remark, fake => fake.Lorem.Slug())
            .RuleFor(pd => pd.ZeroContributionReasonId, fake => fake.PickRandom(
                ZeroContributionReason.Constants.Normal,
                ZeroContributionReason.Constants.Under21WithOver1Khours,
                ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested,
                ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay))
            .RuleFor(pd => pd.FederalTaxes, fake => fake.Finance.Amount(0, 10_000))
            .RuleFor(pd => pd.StateTaxes, fake => fake.Finance.Amount(0, 1000))
            .RuleFor(pd => pd.TaxCode, fake => taxCodeFaker.Generate())
            .RuleFor(pd => pd.TaxCodeId, fake => taxCodeFaker.Generate().Id)
            .RuleFor(pd => pd.CommentTypeId, fake => fake.PickRandom<byte>(CommentType.Constants.ClassAction.Id,
                CommentType.Constants.Dirpay.Id,
                CommentType.Constants.Distribution.Id,
                CommentType.Constants.Forfeit.Id,
                CommentType.Constants.Hardship.Id,
                CommentType.Constants.Military.Id,
                CommentType.Constants.OneHundredPercentEarnings.Id,
                CommentType.Constants.Over64OneYearVested.Id))
            .UseSeed(100);

    }
}
