using Demoulas.Common.Api.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class LookupGroup : GroupBase
{
    protected override string Route => "lookup";
    protected override string RouteName => "Lookup";

}
