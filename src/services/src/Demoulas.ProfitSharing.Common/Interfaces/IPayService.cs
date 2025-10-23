using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayServices;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service interface for PayServices operations.
/// Provides methods for retrieving demographic information.
/// </summary>
public interface IPayService
{
    /// <summary>
    /// method to get pay services based on the provided request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<PayServicesResponse>> GetPayServices(PayServicesRequest request, char employmentType, CancellationToken cancellationToken = default);
}
