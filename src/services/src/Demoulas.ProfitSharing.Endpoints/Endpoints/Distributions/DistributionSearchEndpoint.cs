using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;
public sealed class DistributionSearchEndpoint : ProfitSharingEndpoint<DistributionSearchRequest, PaginatedResponseDto<DistributionSearchResponse>>
{
    private readonly IDistributionService _distributionService;
    public DistributionSearchEndpoint(IDistributionService distributionService) : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
    }
    public override void Configure()
    {
        Post("search");
        Group<DistributionGroup>();
        Summary(s =>
        {
            s.Summary = "Search for profit sharing members with filters and pagination.";
            s.Description =
                "Returns a paginated list of members (employees or beneficiaries) matching the provided search criteria, such as profit year, name, SSN, and other filters. Use MemberType=1 for employees and MemberType=2 for beneficiaries.";
            s.ExampleRequest = DistributionSearchRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new PaginatedResponseDto<DistributionSearchResponse>
                    {
                        Results = new List<DistributionSearchResponse>
                        {
                            DistributionSearchResponse.SampleResponse()
                        },
                        Total = 1
                    }
                }
            };
            s.Responses[400] = "Bad Request. Invalid search parameters provided.";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Not Found. No members matched the search criteria.";
        });
    }

    public override Task<PaginatedResponseDto<DistributionSearchResponse>> ExecuteAsync(DistributionSearchRequest req, CancellationToken ct)
    {
        return _distributionService.SearchAsync(req, ct);
    }
}
