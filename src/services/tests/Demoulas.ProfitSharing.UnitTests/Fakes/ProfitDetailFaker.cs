using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;

internal sealed class ProfitDetailFaker : Faker<ProfitDetail>
{
    internal ProfitDetailFaker(IList<Demographic> demographicFakes)
    {
        var demographicQueue = new Queue<Demographic>(demographicFakes);

        Demographic currentDemographic = demographicQueue.Dequeue();
        var taxCodeFaker = new TaxCodeFaker();

        RuleFor(d => d.Ssn, (f, o) =>
        {
            // This code is non-intuitive.   The idea is that when the demographic
            var rslt = currentDemographic.Ssn; // association is made, we want the badge number and SSN to relate to existing
            if (demographicQueue.Any()) // demographic record that contains the both of them
            {
                // So by keeping a state field outside the lamdba, we can refer to an existing demographic
                currentDemographic = demographicQueue.Dequeue(); // record and copy its values.
            }

            return rslt;
        });
        RuleFor(pd => pd.ProfitYear, fake => Convert.ToInt16(DateTime.Now.Year)).RuleFor(pd => pd.ProfitYearIteration, fake => (byte)0)
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
            .RuleFor(pd => pd.FederalTaxes, fake => fake.PickRandom<byte>(1, 2, 3, 4, 5, 6, 7))
            .RuleFor(pd => pd.StateTaxes, fake => fake.Finance.Amount(0, 1000))
            .RuleFor(pd => pd.TaxCode, fake => taxCodeFaker.Generate())
            .RuleFor(pd => pd.TaxCodeId, fake => taxCodeFaker.Generate().Code);
    }
}
