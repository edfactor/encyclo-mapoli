using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;

public class MasterInquiryDetailGroupingEndpoint : ProfitSharingEndpoint<MasterInquiryRequest, PaginatedResponseDto<GroupedProfitSummaryDto>>
{
    private readonly IMasterInquiryService _masterInquiryService;

    public MasterInquiryDetailGroupingEndpoint(IMasterInquiryService masterInquiryService)
        : base(Navigation.Constants.MasterInquiry)
    {
        _masterInquiryService = masterInquiryService;
    }

    public override void Configure()
    {
        Post("master-inquiry/grouping");
        Summary(s =>
        {
            s.Summary = "Search for profit sharing members with filters and pagination.";
            s.Description = "Returns a paginated list of members (employees or beneficiaries) matching the provided search criteria, such as profit year, name, SSN, and other filters. Use MemberType=1 for employees and MemberType=2 for beneficiaries.";
            s.ExampleRequest = MasterInquiryRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, new PaginatedResponseDto<GroupedProfitSummaryDto> { Results = new List<GroupedProfitSummaryDto> { } } } };
            s.Responses[400] = "Bad Request. Invalid search parameters provided.";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Not Found. No members matched the search criteria.";
        });
        Group<MasterInquiryGroup>();
    }

    public override Task<PaginatedResponseDto<GroupedProfitSummaryDto>> ExecuteAsync(MasterInquiryRequest req, CancellationToken ct)
    {
        if (req is { ProfitYear: 0, EndProfitYear: > 0 })
        {
            req.ProfitYear = req.EndProfitYear.Value;
        }
        return _masterInquiryService.GetGroupedProfitDetails(req, ct);
    }
}
