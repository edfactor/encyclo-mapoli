using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class FreezeDemographicsEndpoint : ProfitSharingEndpoint<SetFrozenStateRequest, FrozenStateResponse>
{
    private readonly IFrozenService _frozenService;
    private readonly IAppUser _appUser;
    private readonly ILogger<FreezeDemographicsEndpoint> _logger;

    public FreezeDemographicsEndpoint(IFrozenService frozenService, IAppUser appUser, ILogger<FreezeDemographicsEndpoint> logger) : base(Navigation.Constants.DemographicFreeze)
    {
        _frozenService = frozenService;
        _appUser = appUser;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("freeze");
        Summary(s =>
        {
            s.Summary = "Freezes demographics for a specific profit year";
            s.ResponseExamples = new Dictionary<int, object> { { 200, new FrozenStateResponse() { Id = 2, ProfitYear = Convert.ToInt16(DateTime.Now.Year) } } };
            s.ExampleRequest =
                new SetFrozenStateRequest { AsOfDateTime = DateTime.Today, ProfitYear = (short)DateTime.Today.Year };
        });
        Group<ItDevOpsGroup>();
    }

    public override Task<FrozenStateResponse> ExecuteAsync(SetFrozenStateRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _frozenService.FreezeDemographics(req.ProfitYear, req.AsOfDateTime, _appUser.UserName, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "freeze-demographics"),
                new("endpoint", "FreezeDemographicsEndpoint"));

            EndpointTelemetry.RecordCountsProcessed.Record(1,
                new("record_type", "demographics-frozen"),
                new("endpoint", "FreezeDemographicsEndpoint"));

            _logger.LogInformation("Demographics frozen for ProfitYear: {ProfitYear}, AsOfDateTime: {AsOfDateTime}, User: {UserName}, FrozenId: {FrozenId} (correlation: {CorrelationId})",
                req.ProfitYear, req.AsOfDateTime, _appUser.UserName, response?.Id, HttpContext.TraceIdentifier);

            return response ?? new FrozenStateResponse { Id = 0 };
        });
    }
}
