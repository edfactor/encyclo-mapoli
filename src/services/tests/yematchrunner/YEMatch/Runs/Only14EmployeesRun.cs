using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using YEMatch.Activities;
using YEMatch.ReadyActivities;
using YEMatch.SmartActivities;
using static YEMatch.Activities.ActivityName;

namespace YEMatch.Runs;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class Only14EmployeesRun : Runnable
{
    public Only14EmployeesRun(
        IActivityFactory activityFactory,
        IReadySshClientFactory readySshClientFactory,
        ISmartApiClientFactory smartApiClientFactory,
        ILogger<Only14EmployeesRun> logger)
        : base(activityFactory, readySshClientFactory, smartApiClientFactory, logger)
    {
    }

    public override async Task Exec()
    {
        await Run(Specify(
            R00_BuildDatabase, // import obfuscated
            TrimTo14Employees, // Reduces execution time to 1 minute
            R15_ProfitSharingYTDWagesExtract2,
            R16_ReadyScreen00809Second,
            R17_ProfitShareReportEditRun,
            R18_ProfitShareReportFinalRun,
            R19_GetEligibleEmployees,
            R20_ProfitForfeit,
            R21_ProfitShareUpdate
        ));
    }
}
