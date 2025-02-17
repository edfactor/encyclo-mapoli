using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IPayClassificationService
{
    Task<ISet<PayClassificationResponseDto>> GetAllPayClassificationsAsync(CancellationToken cancellationToken = default);
}
