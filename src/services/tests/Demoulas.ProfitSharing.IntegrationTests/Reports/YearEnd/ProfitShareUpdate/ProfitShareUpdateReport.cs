using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;

internal  sealed class ProfitShareUpdateReport(OracleConnection connection, IProfitSharingDataContextFactory dbFactory, short profitYear)
{
    public DateTime TodaysDateTime { get; set; }
    public List<string> ReportLines { get; set; }

    public void ApplyAdjustments(decimal contributionPercent,
        decimal incomingForfeitPercent, decimal earningsPercent, decimal secondaryEarningsPercent,
        long badgeToAdjust, decimal adjustContributionAmount, decimal adjustIncomingForfeitAmount,
        decimal adjustEarningsAmount,
        long badgeToAdjust2, decimal adjustEarningsSecondary, long maxAllowedContributions)
    {

        ProfitShareUpdateService psu = new(connection, dbFactory, profitYear);

        psu.TodaysDateTime = TodaysDateTime;

        psu.ApplyAdjustments( contributionPercent,
             incomingForfeitPercent,  earningsPercent,  secondaryEarningsPercent,
             badgeToAdjust,  adjustContributionAmount,  adjustIncomingForfeitAmount,
             adjustEarningsAmount,
             badgeToAdjust2,  adjustEarningsSecondary,  maxAllowedContributions
            );

        ReportLines = psu.ReportLines;


    }

  
}
