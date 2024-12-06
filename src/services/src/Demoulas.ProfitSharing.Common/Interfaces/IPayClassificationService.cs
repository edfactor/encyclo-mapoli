using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IPayClassificationService
{
    Task<ISet<PayClassificationResponseDto>> GetAllPayClassificationsAsync(CancellationToken cancellationToken = default);
}
