using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;

namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IMasterInquiryService
{
    public Task<PaginatedResponseDto<MemberDetails>> GetMembersAsync(MasterInquiryRequest req, CancellationToken cancellationToken = default);
    public Task<MemberProfitPlanDetails?> GetMemberVestingAsync(MasterInquiryMemberRequest req, CancellationToken cancellationToken = default);
    public Task<PaginatedResponseDto<MasterInquiryResponseDto>> GetMemberProfitDetails(MasterInquiryMemberDetailsRequest req, CancellationToken cancellationToken = default);
    public Task<PaginatedResponseDto<GroupedProfitSummaryDto>> GetGroupedProfitDetails(MasterInquiryRequest req, CancellationToken cancellationToken = default);
}
