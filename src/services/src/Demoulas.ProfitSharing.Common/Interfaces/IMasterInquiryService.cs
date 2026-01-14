using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IMasterInquiryService
{
    Task<PaginatedResponseDto<MemberDetails>> GetMembersAsync(MasterInquiryRequest req, CancellationToken cancellationToken = default);
    Task<MemberProfitPlanDetails?> GetMemberAsync(MasterInquiryMemberRequest req, CancellationToken cancellationToken = default);
    Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMemberProfitDetails(MasterInquiryMemberDetailsRequest req, CancellationToken cancellationToken = default);

}
