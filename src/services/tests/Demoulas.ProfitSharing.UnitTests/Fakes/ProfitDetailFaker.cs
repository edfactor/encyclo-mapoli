using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;
internal sealed class ProfitDetailFaker: Faker<ProfitDetail>
{
    internal ProfitDetailFaker()
    {
        var profitCodeFaker = new ProfitCodeFaker();
        var taxCodeFaker = new TaxCodeFaker();

        RuleFor(pd => pd.ProfitYear, fake => Convert.ToInt16(DateTime.Now.Year)).RuleFor(pd => pd.ProfitYearIteration, fake => (byte)0)
            .RuleFor(pd => pd.ProfitCodeId, fake => fake.PickRandom<byte>(1, 2, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20))
            .RuleFor(pd => pd.Contribution, fake => fake.Finance.Amount(0, 10000)).RuleFor(pd => pd.Earnings, fake => fake.Finance.Amount(-5000, 10000))
            .RuleFor(pd => pd.Forfeiture, fake => fake.Finance.Amount(0, 1000)).RuleFor(pd => pd.MonthToDate, fake => fake.Random.Byte(0, 12))
            .RuleFor(pd => pd.YearToDate, fake => Convert.ToInt16(DateTime.Now.Year)).RuleFor(pd => pd.Remark, fake => fake.Lorem.Slug())
            .RuleFor(pd => pd.ZeroCont, fake => '0').RuleFor(pd => pd.FederalTaxes, fake => fake.Finance.Amount(0, 1000))
            .RuleFor(pd => pd.StateTaxes, fake => fake.Finance.Amount(0, 1000)).RuleFor(pd => pd.TaxCode, fake => taxCodeFaker.Generate())
            .RuleFor(pd => pd.TaxCodeId, fake => taxCodeFaker.Generate().Code).RuleFor(pd => pd.SSN, fake => fake.Person.Ssn().ConvertSsnToLong());
    }
}
