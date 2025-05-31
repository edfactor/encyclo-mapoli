using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;

public class MasterInquiryMemberDetailsEndpoint : Endpoint<MasterInquiryMemberDetailsRequest, PaginatedResponseDto<MasterInquiryResponseDto>>
{
    private readonly IMasterInquiryService _masterInquiryService;

    public MasterInquiryMemberDetailsEndpoint(IMasterInquiryService masterInquiryService)
    {
        _masterInquiryService = masterInquiryService;
    }

    public override void Configure()
    {
        Post("master-inquiry/member/{type}/{id}/details", request => new {request.MemberType, request.Id} );
        Summary(s =>
        {
            s.Summary = "PS Master Inquiry (008-10)";
            s.Description =
                "This endpoint returns master inquiry details.";

            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new MasterInquiryWithDetailsResponseDto
                    {
                        EmployeeDetails = null,
                        InquiryResults = new PaginatedResponseDto<MasterInquiryResponseDto>
                        {
                            Results = new List<MasterInquiryResponseDto> { MasterInquiryResponseDto.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<MasterInquiryGroup>();
    }

    public override Task<PaginatedResponseDto<MasterInquiryResponseDto>> ExecuteAsync(MasterInquiryMemberDetailsRequest req, CancellationToken ct)
    {
        return _masterInquiryService.GetMemberProfitDetails(req, ct);
    }
}
