using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
internal sealed class BeneficiaryListFaker
{
    internal List<Beneficiary> GetDummyBeneficiary()
    {
        return new List<Beneficiary>()
        {
            new Beneficiary()
            {
                Id = 144,
                PsnSuffix = 1000,
                BadgeNumber = 701218,
                DemographicId = 1147,
                Contact = new BeneficiaryContact()
                {
                    Id = 144,
                    Ssn = 700010691,
                    DateOfBirth = DateOnly.FromDateTime(new DateTime(1966,09,11,0,0,0,DateTimeKind.Utc)),
                    Address =new Address()
                    {
                        Street = "12 PINE PARKWAY",
                        Street2 = null,
                        City = "SAUGUS",
                        State ="NH",
                        PostalCode = "3729",
                        CountryIso = "US"
                    },
                    ContactInfo = new ContactInfo()
                    {
                        FullName ="EVANS, VALENTINA",
                        LastName = "EVANS",
                        FirstName = "VALENTINA",
                        MiddleName =null,
                        PhoneNumber = null,
                        MobileNumber = null,
                        EmailAddress = null
                    },
                    CreatedDate = DateOnly.FromDateTime(new DateTime(2025,5,8,0,0,0,DateTimeKind.Utc))
                },
                BeneficiaryContactId = 144,
                Relationship = "HUSBAND",
                KindId ='P',
                Kind =new BeneficiaryKind()
                {
                    Id = 'P',
                    Name = "Primary"
                },
                Percent = 100
            },
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
                KindId ='P',
                Kind =new BeneficiaryKind()
                {
                    Id = 'P',
                    Name = "Primary"
                },
                Percent = 100
            }
        };
    }
}
