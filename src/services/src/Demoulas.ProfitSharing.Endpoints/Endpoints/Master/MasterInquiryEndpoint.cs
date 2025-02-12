using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;

public class MasterInquiryEndpoint : Endpoint<MasterInquiryRequest, MasterInquiryWithDetailsResponseDto>
{
    private readonly IMasterInquiryService _masterInquiryService;

    public MasterInquiryEndpoint(IMasterInquiryService masterInquiryService)
    {
        _masterInquiryService = masterInquiryService;
    }

    public override void Configure()
    {
        Get("master-inquiry");
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

    public override Task<MasterInquiryWithDetailsResponseDto> ExecuteAsync(MasterInquiryRequest req, CancellationToken ct)
    {
        return _masterInquiryService.GetMasterInquiryAsync(req, ct);
    }
}
