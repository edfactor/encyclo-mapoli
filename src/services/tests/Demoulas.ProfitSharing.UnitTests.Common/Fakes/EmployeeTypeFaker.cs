using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
internal sealed class EmployeeTypeFaker : Faker<EmploymentType>
{
    internal EmployeeTypeFaker()
    {
        RuleFor(et => et.Id, f => f.PickRandom<char>('P', 'H', 'G', 'F'));
        RuleFor(et => et.Name, f => f.Commerce.Random.AlphaNumeric(10));
    }
}
