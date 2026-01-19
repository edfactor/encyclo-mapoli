using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
// ToResultOrNotFound
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;

public class MasterInquiryMemberEndpoint : ProfitSharingEndpoint<MasterInquiryMemberRequest, Results<Ok<MemberProfitPlanDetails>, NotFound, ProblemHttpResult>>
{
    private readonly IMasterInquiryService _masterInquiryService;

    public MasterInquiryMemberEndpoint(IMasterInquiryService masterInquiryService)
        : base(Navigation.Constants.MasterInquiry)
    {
        _masterInquiryService = masterInquiryService;
    }

    public override void Configure()
    {
        Post("master-inquiry/member");
        Summary(s =>
        {
            s.Summary = "Returns detailed information for a specific profit sharing member.";
            s.Description = "This endpoint retrieves master inquiry details for a single member, including personal, employment, and profit sharing information. Requires administrator or finance manager roles.";
            s.ExampleRequest = new MasterInquiryMemberRequest
            {
                MemberType = 1,
                Id = 123,
                ProfitYear = 2023
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new MemberProfitPlanDetails
                    {
                        Id = 1,
                        IsEmployee = true,
                        BadgeNumber = 12345,
                        PsnSuffix = 1,
                        Ssn = "123456789".MaskSsn(),
                        FirstName = "Jane",
                        LastName = "Doe",
                        MiddleName = "Q",
                        StoreNumber = 101,
                        EmploymentStatus = "Active"
                    }
                }
            };
            s.Responses[200] = "Returns member details for the specified request.";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Member not found.";
        });
        Group<MasterInquiryGroup>();
    }

    protected override async Task<Results<Ok<MemberProfitPlanDetails>, NotFound, ProblemHttpResult>> HandleRequestAsync(MasterInquiryMemberRequest req, CancellationToken ct)
    {
        try
        {
            var entity = await _masterInquiryService.GetMemberAsync(req, ct);
            var result = entity.ToResultOrNotFound(Error.EntityNotFound("Member"));
            return result.ToHttpResult(Error.EntityNotFound("Member"));
        }
        catch (Exception ex)
        {
            return Result<MemberProfitPlanDetails>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
// Moved to Endpoints/InquiriesAndAdjustments/Inquiries/MasterInquiryMemberEndpoint.cs
