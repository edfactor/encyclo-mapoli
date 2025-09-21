using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;

public class MasterInquirySearchEndpoint : ProfitSharingEndpoint<MasterInquiryRequest, Results<Ok<PaginatedResponseDto<MemberDetails>>, NotFound, ProblemHttpResult>>
{
    private readonly IMasterInquiryService _masterInquiryService;

    public MasterInquirySearchEndpoint(IMasterInquiryService masterInquiryService)
        : base(Navigation.Constants.MasterInquiry)
    {
        _masterInquiryService = masterInquiryService;
    }

    public override void Configure()
    {
        Post("master-inquiry/search");
        Summary(s =>
        {
            s.Summary = "Search for profit sharing members with filters and pagination.";
            s.Description =
                "Returns a paginated list of members (employees or beneficiaries) matching the provided search criteria, such as profit year, name, SSN, and other filters. Use MemberType=1 for employees and MemberType=2 for beneficiaries.";
            s.ExampleRequest = MasterInquiryRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new PaginatedResponseDto<MemberDetails>
                    {
                        Results = new List<MemberDetails>
                        {
                            new MemberDetails
                            {
                                Id = 1,
                                Ssn = "XXX-XX-1234",
                                FirstName = "John",
                                LastName = "Doe",
                                BadgeNumber = 1001,
                                PayFrequencyId = 1,
                                Address = "123 Main St",
                                AddressCity = "Boston",
                                AddressState = "MA",
                                AddressZipCode = "02110",
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
                        },
                        Total = 1
                    }
                }
            };
            s.Responses[400] = "Bad Request. Invalid search parameters provided.";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Not Found. No members matched the search criteria.";
        });
        Group<MasterInquiryGroup>();
    }

    public override async Task<Results<Ok<PaginatedResponseDto<MemberDetails>>, NotFound, ProblemHttpResult>> ExecuteAsync(MasterInquiryRequest req, CancellationToken ct)
    {
        try
        {
            if (req is { ProfitYear: 0, EndProfitYear: > 0 })
            {
                req.ProfitYear = req.EndProfitYear.Value;
            }

            var data = await _masterInquiryService.GetMembersAsync(req, ct);
            // Tests expect success (200) even when no members match filters; return empty list as success.
            return Result<PaginatedResponseDto<MemberDetails>>.Success(data).ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<PaginatedResponseDto<MemberDetails>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
