using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IPayProfitService
{
    Task<PaginatedResponseDto<PayProfitResponseDto>?> GetAllProfits(PaginationRequestDto req, CancellationToken cancellationToken = default);
    Task<ISet<PayProfitResponseDto>?> AddProfit(IEnumerable<PayProfitRequestDto> profitRequest, CancellationToken cancellationToken);
}
