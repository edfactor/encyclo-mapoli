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

public class MasterInquiryMemberDetailsEndpoint : ProfitSharingEndpoint<MasterInquiryMemberDetailsRequest, Results<Ok<PaginatedResponseDto<MasterInquiryResponseDto>>, NotFound, ProblemHttpResult>>
{
    private readonly IMasterInquiryService _masterInquiryService;

    public MasterInquiryMemberDetailsEndpoint(IMasterInquiryService masterInquiryService)
        : base(Navigation.Constants.MasterInquiry)
    {
        _masterInquiryService = masterInquiryService;
    }

    public override void Configure()
    {
        Get("master-inquiry/member/{MemberType}/{Id}/details");
        Summary(s =>
        {
            s.Summary = "Get paginated profit details for a specific member (employee or beneficiary).";
            s.Description = "Returns paginated profit sharing details for a specific member, filtered by MemberType and Id. Use MemberType=1 for employees and MemberType=2 for beneficiaries.";
            s.ExampleRequest = new MasterInquiryMemberDetailsRequest { MemberType = 1, Id = 123 };
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
            s.Responses[404] = "Not Found. The requested member or details could not be found.";
        });
        Group<MasterInquiryGroup>();
    }

    public override async Task<Results<Ok<PaginatedResponseDto<MasterInquiryResponseDto>>, NotFound, ProblemHttpResult>> ExecuteAsync(MasterInquiryMemberDetailsRequest req, CancellationToken ct)
    {
        try
        {
            var data = await _masterInquiryService.GetMemberProfitDetails(req, ct);
            if (data.Total == 0)
            {
                return Result<PaginatedResponseDto<MasterInquiryResponseDto>>.Failure(Error.EntityNotFound("Member details")).ToHttpResult(Error.EntityNotFound("Member details"));
            }
            return Result<PaginatedResponseDto<MasterInquiryResponseDto>>.Success(data).ToHttpResult();
        }
        catch (Exception ex)
        {
            return Result<PaginatedResponseDto<MasterInquiryResponseDto>>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
