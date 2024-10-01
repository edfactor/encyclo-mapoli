using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;

/// <summary>
///   Faker for <c>Beneficiary</c>
/// </summary>
internal sealed class BeneficiaryFaker : Faker<Beneficiary>
{
    private static int _iDCounter = 1000;
    
    /// <summary>
    ///   Initializes a default instance of <c>BeneficiaryFaker</c>
    /// </summary>
    internal BeneficiaryFaker()
    {
        BeneficiaryContactFaker contactFaker = new BeneficiaryContactFaker();


        RuleFor(d => d.Id, f => _iDCounter++);
        RuleFor(b => b.Psn, f => f.Random.Long(100_000_000, 9_999_999_999));
        RuleFor(pc => pc.Distribution, f => f.Finance.Amount(min: 100, max: 20_000, decimals: 2));
        RuleFor(pc => pc.Amount, f => f.Finance.Amount(min: 100, max: 20_000, decimals: 2));
        RuleFor(pc => pc.Earnings, f => f.Finance.Amount(min: 100, max: 20_000, decimals: 2));
        RuleFor(pc => pc.SecondaryEarnings, f => f.Finance.Amount(min: 100, max: 2_000, decimals: 2));
        RuleFor(b => b.Kind, f => f.PickRandom(new List<BeneficiaryKind>
        {
            new BeneficiaryKind
            {
                Id = BeneficiaryKind.Constants.Primary, Name = "Primary",
            },
            new BeneficiaryKind
            {
                Id = BeneficiaryKind.Constants.Secondary, Name = "Secondary",

            }
        }));

        RuleFor(b => b.Contact, f =>
        {
            var contact = contactFaker.Generate();
            RuleFor(b => b.BeneficiaryContactId, _ => contact.Id); // Set BeneficiaryContactId to the generated Contact's ID
            return contact;
        });
    }
}
