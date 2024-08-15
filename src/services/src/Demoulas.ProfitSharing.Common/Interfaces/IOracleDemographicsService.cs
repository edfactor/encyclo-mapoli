using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IOracleDemographicsService
{
    /// <summary>
    /// Will retrieve all employees from OracleHCM
    /// https://docs.oracle.com/en/cloud/saas/human-resources/24c/farws/op-workers-get.html
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<OracleEmployee?> GetAllEmployees(CancellationToken cancellationToken = default);
}
