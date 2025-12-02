using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
internal sealed class BeneficiaryListFaker
{
    internal List<Beneficiary> GetDummyBeneficiary()
    {
        return new List<Beneficiary>()
        {
            new Beneficiary()
            {
                Id = 145,
                PsnSuffix = 1000,
                BadgeNumber = 703244,
                DemographicId = 3173,
                Contact = new BeneficiaryContact()
                {
                    Id = 145,
                    Ssn = 700010692,
                    DateOfBirth = DateOnly.FromDateTime(new DateTime(1984,3,4,0,0,0,DateTimeKind.Utc)),
                    Address =new Address()
                    {
                        Street = "243 SECOND COURT",
                        Street2 = null,
                        City = "PEPPERELL",
                        State ="MA",
                        PostalCode = "2318",
                        CountryIso = "US"
                    },
                    ContactInfo = new ContactInfo()
                    {
                        FullName ="DELAROSA, ZOE",
                        LastName = "DELAROSA",
                        FirstName = "ZOE",
                        MiddleName =null,
                        PhoneNumber = null,
                        MobileNumber = null,
                        EmailAddress = null
                    },
                    CreatedDate = DateOnly.FromDateTime(new DateTime(2025,5,8,0,0,0,DateTimeKind.Utc))
                },
                BeneficiaryContactId = 145,
                Relationship = "DAUGHTER",
                Percent = 100
            }
        };
    }
}
