using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;
public sealed class CreateDistributionEndpoint : ProfitSharingEndpoint<CreateDistributionRequest, CreateDistributionResponse>
{
    private readonly IDistributionService _distributionService;

    public CreateDistributionEndpoint(IDistributionService distributionService):base(Navigation.Constants.Distributions) {
        _distributionService = distributionService;
    }

    public override void Configure()
    {
        Post("/");
        Group<DistributionGroup>();
        Summary(s =>
        {
            s.Summary = "Create a new profit sharing distribution in the current profit year.";
            s.Description = "Creates a new profit sharing distribution record for the current profit year. ";
            s.ExampleRequest = CreateDistributionRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new CreateDistributionResponse
                    {
                        Id = 1,
                        BadgeNumber = 12345,
                        StatusId = 'A',
                        FrequencyId = 'M',
                        FederalTaxPercentage = 15.0m,
                        FederalTaxAmount = 150.00m,
                        StateTaxPercentage = 5.0m,
                        StateTaxAmount = 50.00m,
                        GrossAmount = 1000.00m,
                        CheckAmount = 800.00m,
                        TaxCodeId = '1',
                        MaskSsn = "XXX-XX-6789",
                        PaymentSequence = 1,
                        CreatedAt = DateTime.UtcNow,
                        Memo = "Distribution for June 2024"
                    }
                }
            };
        });
    }

    public override Task<CreateDistributionResponse> ExecuteAsync(CreateDistributionRequest req, CancellationToken ct)
    {
        return _distributionService.CreateDistribution(req, ct);
    }
}
