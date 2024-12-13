using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;
/// <summary>
/// Totals extracted from Profit Detail records
/// </summary>
public record ProfitDetailTotals(
    decimal DistributionsTotal,
    decimal ForfeitsTotal,
    decimal AllocationsTotal,
    decimal PaidAllocationsTotal,
    decimal MilitaryTotal,
    decimal ClassActionFundTotal);
