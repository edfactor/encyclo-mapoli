using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;

public class MasterInquiryMemberEndpoint : Endpoint<MasterInquiryMemberRequest, MemberDetails>
{
    private readonly IMasterInquiryService _masterInquiryService;

    public MasterInquiryMemberEndpoint(IMasterInquiryService masterInquiryService)
    {
        _masterInquiryService = masterInquiryService;
    }

    public override void Configure()
    {
        Post("master-inquiry/member");
        Summary(s =>
        {
            s.Summary = "PS Master Inquiry (008-10)";
            s.Description =
                "This endpoint returns master inquiry details.";

            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new MemberDetails
                    {
                        
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<MasterInquiryGroup>();
    }

    public override async Task<MemberDetails> ExecuteAsync(MasterInquiryMemberRequest req, CancellationToken ct)
    {
        var result = await _masterInquiryService.GetMemberAsync(req, ct);
        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return null!;
        }
        return result;
    }
}
