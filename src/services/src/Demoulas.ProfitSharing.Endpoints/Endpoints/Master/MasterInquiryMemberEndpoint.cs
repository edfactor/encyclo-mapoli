using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Extensions; // ToResultOrNotFound
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http.HttpResults;

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
                        FirstName = "John",
                        MiddleName = "J",
                        LastName = "Doe",
                        Address = "123 Main St",
                        AddressCity = "Lowell",
                        AddressState = "MA",
                        AddressZipCode = "01850",
                        DateOfBirth = new DateOnly(1980,
                            1,
                            1),
                        YearToDateProfitSharingHours = 1200.5m,
                        YearsInPlan = 10,
                        PercentageVested = 100m,
                        EnrollmentId = 2,
                        Enrollment = "Active",
                        HireDate = new DateOnly(2010,
                            5,
                            1),
                        TerminationDate = null,
                        ReHireDate = null,
                        StoreNumber = 101,
                        BeginPSAmount = 5000m,
                        CurrentPSAmount = 15000m,
                        BeginVestedAmount = 5000m,
                        CurrentVestedAmount = 15000m,
                        CurrentEtva = 1000m,
                        PreviousEtva = 900m,
                        Missives = new List<int>
                        {
                            1,
                            2
                        },
                        EmploymentStatus = "Active",
                        Department = "Grocery",
                        PayClassification = "Unknown",
                        Gender = "M",
                        PhoneNumber = "603-555-5555",
                        WorkLocation = "Store 4",
                        ReceivedContributionsLastYear = false,
                        FullTimeDate = default,
                        TerminationReason = "Retired"
                    }
                }
            };
            s.Responses[200] = "Returns member details for the specified request.";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Member not found.";
        });
        Group<MasterInquiryGroup>();
    }

    public override async Task<Results<Ok<MemberProfitPlanDetails>, NotFound, ProblemHttpResult>> ExecuteAsync(MasterInquiryMemberRequest req, CancellationToken ct)
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
