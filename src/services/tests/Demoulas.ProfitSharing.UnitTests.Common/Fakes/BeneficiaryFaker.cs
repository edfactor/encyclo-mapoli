using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

/// <summary>
///   Faker for <c>Beneficiary</c>
/// </summary>
internal sealed class BeneficiaryFaker : Faker<Beneficiary>
{
    private static int _iDCounter = 1000;

    /// <summary>
    ///   Initializes a default instance of <c>BeneficiaryFaker</c>
    /// </summary>
    internal BeneficiaryFaker(IList<Demographic> demographicFakes)
    {
        BeneficiaryContactFaker contactFaker = new BeneficiaryContactFaker();

        var demographicQueue = new Queue<Demographic>(demographicFakes);

        Demographic currentDemographic = demographicQueue.Peek();

        RuleFor(pc => pc.DemographicId, (f, o) => (currentDemographic.Id))
            .RuleFor(pc => pc.BadgeNumber, (f, o) => (currentDemographic.BadgeNumber))
            .RuleFor(d => d.Demographic, (f, o) =>
        {
            if (demographicQueue.Any()) // demographic record that contains the both of them
            {
                // So by keeping a state field outside the lamdba, we can refer to an existing demographic
                currentDemographic = demographicQueue.Dequeue(); // record and copy its values.
            }
            else
            {
                demographicQueue = new Queue<Demographic>(demographicFakes); // Reset the queue if it's empty
                currentDemographic = demographicQueue.Dequeue(); // Start over with the first item
            }

            return currentDemographic;
        })

        .RuleFor(d => d.Id, f => _iDCounter++)
            .RuleFor(b => b.PsnSuffix, f => f.Random.Short(1_000, 9_999))
            .RuleFor(b => b.Kind,
            f => f.PickRandom(new List<BeneficiaryKind>
            {
                new BeneficiaryKind { Id = BeneficiaryKind.Constants.Primary, Name = "Primary", },
                new BeneficiaryKind { Id = BeneficiaryKind.Constants.Secondary, Name = "Secondary", }
            }))

            .RuleFor(b => b.Contact, f =>
        {
            var contact = contactFaker.Generate();
            return contact;
        })

        // Move the setting of BeneficiaryContactId to its own RuleFor block
        .RuleFor(b => b.BeneficiaryContactId, f =>
        {
            var contact = contactFaker.Generate();
            return contact.Id;
        })
        .UseSeed(100);
    }
}
