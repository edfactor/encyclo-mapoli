using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

public sealed class DemographicSyncAuditFaker : Faker<DemographicSyncAudit>
{
    public DemographicSyncAuditFaker()
    {
        RuleFor(x => x.Id, f => f.IndexFaker + 1)
            .RuleFor(x => x.BadgeNumber, f => f.Random.Int(100000, 999999))
            .RuleFor(x => x.OracleHcmId, f => f.Random.Long(1000, 999999))
            .RuleFor(x => x.InvalidValue, f => f.Random.Bool() ? null : f.Lorem.Word())
            .RuleFor(x => x.UserName, f => $"user{f.Random.Int(1, 100)}")
            .RuleFor(x => x.PropertyName, f => f.Random.ArrayElement(new[] { "Badge", "Email", "Phone", "Address", "Name" }))
            .RuleFor(x => x.Message, f => f.Lorem.Sentence())
            .RuleFor(x => x.Created, f => f.Date.RecentOffset(days: 30));
    }
}
