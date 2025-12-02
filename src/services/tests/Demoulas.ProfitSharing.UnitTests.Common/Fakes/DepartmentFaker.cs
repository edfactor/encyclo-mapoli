using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
internal sealed class DepartmentFaker : Faker<Department>
{
    internal DepartmentFaker()
    {
        RuleFor(x => x.Id, f => f.PickRandom<byte>(Department.Constants.Grocery, Department.Constants.Bakery, Department.Constants.BeerAndWine, Department.Constants.Dairy, Department.Constants.Deli, Department.Constants.Meat, Department.Constants.Produce));
        RuleFor(x => x.Name, f => f.Commerce.Department());
    }
}
