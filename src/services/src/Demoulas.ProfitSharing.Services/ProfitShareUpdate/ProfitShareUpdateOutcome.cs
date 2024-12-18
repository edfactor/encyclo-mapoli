using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Services.ProfitShareUpdate;

public record ProfitShareUpdateOutcome(
    List<MemberFinancials> MemberFinancials,
    AdjustmentReportData AdjustmentReportData,
    bool RerunNeeded
);
