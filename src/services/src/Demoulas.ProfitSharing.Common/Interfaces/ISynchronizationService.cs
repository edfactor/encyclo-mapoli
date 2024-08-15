using Demoulas.ProfitSharing.Common.Contracts.Messaging;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ISynchronizationService
{
    Task<bool> SendSynchronizationRequest(OracleHcmJobRequest request, CancellationToken cancellationToken = default);
}
