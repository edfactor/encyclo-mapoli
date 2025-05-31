using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface IMasterInquiryService
{
    public Task<MasterInquiryWithDetailsResponseDto> GetMasterInquiryAsync(MasterInquiryRequest req, CancellationToken cancellationToken = default);
    public Task<PaginatedResponseDto<MemberDetails>> GetMembersAsync(MasterInquiryMemberRequest req, CancellationToken cancellationToken = default);
    public Task<MemberDetails?> GetMemberAsync(MasterInquiryMemberRequest req, CancellationToken cancellationToken = default);
    public Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMemberProfitDetails(MasterInquiryMemberDetailsRequest req, CancellationToken cancellationToken = default);
}
