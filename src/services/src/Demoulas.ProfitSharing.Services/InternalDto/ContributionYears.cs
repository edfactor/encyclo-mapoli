using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Services.InternalDto;
internal sealed record ContributionYears
{
    public int BadgeNumber { get; init; }
    public byte YearsInPlan { get; init; }
}
