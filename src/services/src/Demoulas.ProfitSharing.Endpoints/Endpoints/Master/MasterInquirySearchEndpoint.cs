using System.Linq;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Master;

public class MasterInquirySearchEndpoint : ProfitSharingEndpoint<MasterInquiryRequest, Results<Ok<PaginatedResponseDto<MemberDetails>>, NotFound, ProblemHttpResult>>
{
    private readonly IMasterInquiryService _masterInquiryService;
    private readonly ILogger<MasterInquirySearchEndpoint> _logger;

    public MasterInquirySearchEndpoint(IMasterInquiryService masterInquiryService, ILogger<MasterInquirySearchEndpoint> logger)
        : base(Navigation.Constants.MasterInquiry)
    {
        _masterInquiryService = masterInquiryService;
        _logger = logger;
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
                                MiddleName = "Q",
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

    public override Task<Results<Ok<PaginatedResponseDto<MemberDetails>>, NotFound, ProblemHttpResult>> ExecuteAsync(MasterInquiryRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            if (req is { ProfitYear: 0, EndProfitYear: > 0 })
            {
                req.ProfitYear = req.EndProfitYear.Value;
            }

            var data = await _masterInquiryService.GetMembersAsync(req, ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "search"),
                new KeyValuePair<string, object?>("endpoint.category", "master-inquiry"));

            // Record result count for monitoring bulk operations
            var resultCount = data.Results?.Count() ?? 0;
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new KeyValuePair<string, object?>("endpoint.name", nameof(MasterInquirySearchEndpoint)),
                new KeyValuePair<string, object?>("operation", "search"));

            // Log search operation with filter details (no sensitive data)
            _logger.LogInformation("Master inquiry search completed: {ResultCount} records found for profit year {ProfitYear} (correlation: {CorrelationId})",
                resultCount, req.ProfitYear, HttpContext.TraceIdentifier);

            // Tests expect success (200) even when no members match filters; return empty list as success.
            return Result<PaginatedResponseDto<MemberDetails>>.Success(data).ToHttpResult();
        });
    }
}
