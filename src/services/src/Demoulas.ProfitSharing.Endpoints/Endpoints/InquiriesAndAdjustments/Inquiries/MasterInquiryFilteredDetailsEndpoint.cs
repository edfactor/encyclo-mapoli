using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;

public class MasterInquiryFilteredDetailsEndpoint : ProfitSharingEndpoint<MasterInquiryMemberDetailsRequest, Results<Ok<PaginatedResponseDto<MasterInquiryResponseDto>>, NotFound, ProblemHttpResult>>
{
    private readonly IMasterInquiryService _masterInquiryService;

    public MasterInquiryFilteredDetailsEndpoint(IMasterInquiryService masterInquiryService)
        : base(Navigation.Constants.MasterInquiry)
    {
        _masterInquiryService = masterInquiryService;
    }

    public override void Configure()
    {
        Post("master-inquiry/member/details");
        Summary(s =>
        {
            s.Summary = "Get paginated profit details filtered by member type, year, and month.";
            s.Description = "Returns paginated profit sharing details filtered by MemberType and optional ProfitYear/MonthToDate query parameters.";
            s.ExampleRequest = new MasterInquiryMemberDetailsRequest { MemberType = 0, ProfitYear = 2024, MonthToDate = 3 };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new PaginatedResponseDto<MasterInquiryResponseDto>
                    {
                        Results = new List<MasterInquiryResponseDto> { MasterInquiryResponseDto.ResponseExample() },
                        Total = 1
                    }
                }
            };
            s.Responses[400] = "Bad Request. Invalid parameters provided.";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Not Found. No details found matching the criteria.";
        });
        Group<MasterInquiryGroup>();
    }

    protected override async Task<Results<Ok<PaginatedResponseDto<MasterInquiryResponseDto>>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        MasterInquiryMemberDetailsRequest req,
        CancellationToken ct)
    {
        var result = await _masterInquiryService.GetMemberProfitDetails(req, ct);
        return Result<PaginatedResponseDto<MasterInquiryResponseDto>>.Success(result).ToHttpResult();
    }
}
// Moved to Endpoints/InquiriesAndAdjustments/Inquiries/MasterInquiryFilteredDetailsEndpoint.cs
