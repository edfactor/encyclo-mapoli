using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class TaxCodeFaker : Faker<TaxCode>
{
    internal TaxCodeFaker()
    {
        var taxCodes = new List<char> { '1',
                                       '2',
                                       '3',
                                       '4',
                                       '5',
                                       '6',
                                       '7',
                                       '8',
                                       '9',
                                       'A',
                                       'B',
                                       'C',
                                       'D',
                                       'E',
                                       'F',
                                       'G',
                                       'H',
                                       'P'};

        RuleFor(tc => tc.Id, fake => fake.Random.ListItem(taxCodes)).
            RuleFor(tc => tc.Name, fake => fake.Music.Genre())
            .UseSeed(100);
    }
}
