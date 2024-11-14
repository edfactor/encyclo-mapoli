using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;
public class MasterInquiryEndpoint : Endpoint<MasterInquiryRequest, ReportResponseBase<MasterInquiryResponseDto>>
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
            s.Summary = "PS Master Inquiry";
            s.Description =
                "This endpoint returns master inquiry details.";

            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<MasterInquiryResponseDto>
                    {
                        ReportName = "Example Response",
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<MasterInquiryResponseDto>
                        {
                            Results = new List<MasterInquiryResponseDto> { MasterInquiryResponseDto.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override Task<ReportResponseBase<MasterInquiryResponseDto>> ExecuteAsync(MasterInquiryRequest req, CancellationToken ct)
    {
        return _masterInquiryService.GetMasterInquiry(req, ct);
    }
}
