using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;

internal sealed class ProfitDetailFaker : Faker<ProfitDetail>
{
    private static int _profitDetailCounter = 1000;

    internal ProfitDetailFaker(HashSet<long> ssnSet)
    {
        var ssnQueue = new Queue<long>(ssnSet);

        long currentSsn = ssnQueue.Peek();
        var taxCodeFaker = new TaxCodeFaker();

        RuleFor(d => d.Id, f => _profitDetailCounter++);
        
        RuleFor(d => d.Ssn, (f, o) =>
        {
            // This code is non-intuitive.   The idea is that when the demographic
            var rslt = currentSsn; // association is made, we want the badge number and SSN to relate to existing
            if (ssnQueue.Any()) // demographic record that contains the both of them
            {
                // So by keeping a state field outside the lamdba, we can refer to an existing demographic
                currentSsn = ssnQueue.Dequeue(); // record and copy its values.
            }
            else
            {
                ssnQueue = new Queue<long>(ssnSet);
                currentSsn = ssnQueue.Dequeue();
            }

            return rslt;
        });
        
        RuleFor(pd => pd.ProfitYear, fake => fake.Random.Short(Convert.ToInt16(DateTime.Now.Year-5), Convert.ToInt16(DateTime.Now.Year)))
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
            .RuleFor(pd => pd.Contribution, fake => fake.Finance.Amount(0, 10000))
            .RuleFor(pd => pd.Earnings, fake => fake.Finance.Amount(-5000, 10000))
            .RuleFor(pd => pd.Forfeiture, fake => fake.Finance.Amount(0, short.MaxValue))
            .RuleFor(pd => pd.MonthToDate, fake => fake.Random.Byte(0, 12))
            .RuleFor(pd => pd.YearToDate, fake => Convert.ToInt16(DateTime.Now.Year))
            .RuleFor(pd => pd.Remark, fake => fake.Lorem.Slug())
            .RuleFor(pd => pd.ZeroContributionReasonId, fake => (byte?)0)
            .RuleFor(pd => pd.FederalTaxes, fake => fake.Finance.Amount(0, 10_000))
            .RuleFor(pd => pd.StateTaxes, fake => fake.Finance.Amount(0, 1000))
            .RuleFor(pd => pd.TaxCode, fake => taxCodeFaker.Generate())
            .RuleFor(pd => pd.TaxCodeId, fake => taxCodeFaker.Generate().Code);

    }
}
