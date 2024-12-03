using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Common.Contracts.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IMasterInquiryService
{
    public Task<MasterInquiryWithDetailsResponseDto> GetMasterInquiry(MasterInquiryRequest req, CancellationToken cancellationToken = default);
}
