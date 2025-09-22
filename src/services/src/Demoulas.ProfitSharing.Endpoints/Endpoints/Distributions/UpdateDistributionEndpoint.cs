using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;
public sealed class UpdateDistributionEndpoint : ProfitSharingEndpoint<UpdateDistributionRequest, CreateOrUpdateDistributionResponse>
{
    private readonly IDistributionService _distributionService;

    public UpdateDistributionEndpoint(IDistributionService distributionService) : base(Navigation.Constants.Distributions) 
    {
        _distributionService = distributionService;
    }

    public override void Configure()
    {
        Put("/");
        Group<DistributionGroup>();
        Summary(s =>
        {
            s.Summary = "Updates an existing profit sharing distribution in the current profit year.";
            s.Description = "Updates an existing profit sharing distribution record for the current profit year. ";
            s.ExampleRequest = UpdateDistributionRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    CreateOrUpdateDistributionResponse.ResponseExample()
                }
            };
        });
    }

    public override Task<CreateOrUpdateDistributionResponse> ExecuteAsync(UpdateDistributionRequest req, CancellationToken ct)
    {
        return _distributionService.UpdateDistribution(req, ct);
    }
}
