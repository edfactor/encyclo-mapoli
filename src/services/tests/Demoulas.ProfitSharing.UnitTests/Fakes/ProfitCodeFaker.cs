using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;
internal sealed class ProfitCodeFaker:Faker<ProfitCode>
{
    internal ProfitCodeFaker()
    {
        RuleFor(pc => pc.Code, f => f.PickRandom<byte>(1, 2, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20))
            .RuleFor(pc => pc.Definition, f => f.Name.JobTitle())
            .RuleFor(pc => pc.Frequency, f => f.Lorem.Word());
    }
}
