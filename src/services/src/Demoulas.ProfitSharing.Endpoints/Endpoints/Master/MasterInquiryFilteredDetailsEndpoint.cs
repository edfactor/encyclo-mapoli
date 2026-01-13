using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;

public class MasterInquiryFilteredDetailsEndpoint : ProfitSharingEndpoint<MasterInquiryMemberDetailsRequest, Results<Ok<PaginatedResponseDto<MasterInquiryResponseDto>>, NotFound, ProblemHttpResult>>
{
    private readonly IMasterInquiryService _masterInquiryService;
    private readonly ILogger<MasterInquiryFilteredDetailsEndpoint> _logger;

    public MasterInquiryFilteredDetailsEndpoint(IMasterInquiryService masterInquiryService, ILogger<MasterInquiryFilteredDetailsEndpoint> logger)
        : base(Navigation.Constants.MasterInquiry)
    {
        _masterInquiryService = masterInquiryService;
        _logger = logger;
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

    public override Task<Results<Ok<PaginatedResponseDto<MasterInquiryResponseDto>>, NotFound, ProblemHttpResult>> ExecuteAsync(MasterInquiryMemberDetailsRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var data = await _masterInquiryService.GetMemberProfitDetails(req, ct);

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "member-profit-details"),
                new("endpoint.category", "master-inquiry"));

            // Record result count for monitoring
            var resultCount = data.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "profit-details"),
                new("endpoint", nameof(MasterInquiryFilteredDetailsEndpoint)));

            _logger.LogInformation(
                "Master inquiry member profit details retrieved for member type {MemberType}, profit year {ProfitYear}, {ResultCount} records (correlation: {CorrelationId})",
                req.MemberType, req.ProfitYear, resultCount, HttpContext.TraceIdentifier);

            // Return 200 with empty collection; a lack of matching detail rows under current filters is not a not-found condition.
            return Result<PaginatedResponseDto<MasterInquiryResponseDto>>.Success(data).ToHttpResult();
        }, "Ssn", "BadgeNumber");
    }
}
