using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

public sealed class StateTaxEndpoint : ProfitSharingEndpoint<StateTaxLookupRequest, StateTaxLookupResponse>
{
    private readonly IStateTaxLookupService _stateTaxLookupService;
    private readonly ILogger<StateTaxEndpoint> _logger;

    public StateTaxEndpoint(IStateTaxLookupService stateTaxLookupService, ILogger<StateTaxEndpoint> logger) : base(Navigation.Constants.Unknown)
    {
        _stateTaxLookupService = stateTaxLookupService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/state-taxes/{state}");
        Group<LookupGroup>();
        Summary(s =>
        {
            s.Description = "Returns the default state tax rate for a given state abbreviation.";
            s.Summary = "Lookup state tax rate by state abbreviation";
            s.ExampleRequest = new StateTaxLookupRequest { State = "MA" };
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new StateTaxLookupResponse
                {
                    State = "MA",
                    StateTaxRate = 5.05m
                }
            } };
        });
    }

    public override Task<StateTaxLookupResponse> ExecuteAsync(StateTaxLookupRequest req, CancellationToken ct) =>
        this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _stateTaxLookupService.LookupStateTaxRate(req.State, ct);

            // Record state tax lookup metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "state-tax-lookup"),
                new("endpoint", "StateTaxEndpoint"),
                new("state", req.State));

            _logger.LogInformation("State tax lookup completed for state {State}, rate: {TaxRate} (correlation: {CorrelationId})",
                req.State, response.StateTaxRate, HttpContext.TraceIdentifier);

            return response;
        });
}
