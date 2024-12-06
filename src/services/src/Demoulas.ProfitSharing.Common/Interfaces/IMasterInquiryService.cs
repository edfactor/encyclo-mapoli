using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IMasterInquiryService
{
    public Task<MasterInquiryWithDetailsResponseDto> GetMasterInquiryAsync(MasterInquiryRequest req, CancellationToken cancellationToken = default);
}
