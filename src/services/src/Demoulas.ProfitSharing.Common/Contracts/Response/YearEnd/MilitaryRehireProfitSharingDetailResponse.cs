
using Demoulas.Util.Extensions;
using System.Data.SqlTypes;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record MilitaryRehireProfitSharingDetailResponse
{
    public required short Year { get; set; }
    public required decimal Forfeiture { get; set; }
    public required string Comment { get; set; }
}
