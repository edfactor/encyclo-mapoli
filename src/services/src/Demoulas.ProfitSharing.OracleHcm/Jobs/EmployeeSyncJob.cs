using Demoulas.ProfitSharing.Common.Interfaces;
using Quartz;

namespace Demoulas.ProfitSharing.OracleHcm.Jobs;
internal sealed class EmployeeSyncJob : IJob
{
    private readonly IEmployeeSyncService _employeeSyncService;

    public EmployeeSyncJob(IEmployeeSyncService employeeSyncService)
    {
        _employeeSyncService = employeeSyncService;
    }

    public Task Execute(IJobExecutionContext context)
    {
        return _employeeSyncService.SynchronizeEmployees(requestedBy: "System", context.CancellationToken);
    }
}
