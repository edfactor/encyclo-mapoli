using Demoulas.ProfitSharing.Common.Contracts.Request.Adjustments;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Adjustments;

public sealed class MergeProfitDetailsEndpoint : ProfitSharingRequestEndpoint<MergeProfitDetailsRequest>
{
    private readonly IMergeProfitDetailsService _mergeProfitDetailsService;
    private readonly ILogger<MergeProfitDetailsEndpoint> _logger;

    public MergeProfitDetailsEndpoint(IMergeProfitDetailsService mergeProfitDetailsService, ILogger<MergeProfitDetailsEndpoint> logger)
        : base(Navigation.Constants.Adjustments)
    {
        _mergeProfitDetailsService = mergeProfitDetailsService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/merge-profit-details");
        Group<AdjustmentsGroup>();
        Summary(s =>
        {
            s.Summary = "Merge profit details from source to target demographic.";
            s.Description = "Transfers all profit details from the source SSN to the destinatioin SSN.";

            // Add explicit parameter descriptions for Swagger
            s.Params["sourceSsn"] = "Source SSN from which profit details will be transferred";
            s.Params["destinationSsn"] = "Destination SSN to which profit details will be transferred";

            s.ExampleRequest = new MergeProfitDetailsRequest
            {
                SourceSsn = 1001,
                DestinationSsn = 1002
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ()
                }
            };
        });
    }
    protected override async Task HandleRequestAsync(MergeProfitDetailsRequest req, CancellationToken ct)
    {
        try
        {
            await _mergeProfitDetailsService.MergeProfitDetailsToDemographic(req.SourceSsn, req.DestinationSsn, ct);
            _logger.LogInformation("MergeProfitDetailsToDemographic successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MergeProfitDetailsToDemographic failed");
        }
    }
}
