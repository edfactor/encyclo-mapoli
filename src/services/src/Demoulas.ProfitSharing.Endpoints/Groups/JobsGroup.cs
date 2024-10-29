using Demoulas.Common.Api.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class JobsGroup : GroupBase
{
    protected override string RouteName => "Jobs";
    protected override string Route => "jobs";
}
