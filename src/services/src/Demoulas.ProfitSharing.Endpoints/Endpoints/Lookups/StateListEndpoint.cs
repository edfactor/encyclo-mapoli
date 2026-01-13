using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

/// <summary>
/// Endpoint for retrieving the list of states used in profit sharing operations.
/// Returns states that are referenced in profit sharing comment records, joined with
/// full state names from the States lookup table.
/// </summary>
public sealed class StateListEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<StateListResponse>>
{
    private readonly ILogger<StateListEndpoint> _logger;
    private readonly IStateService _stateService;

    public StateListEndpoint(IStateService stateService, ILogger<StateListEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _stateService = stateService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("states");
        Summary(s =>
        {
            s.Summary = "Gets all available states for filtering profit sharing transactions";
            s.Description = "Returns states that are referenced in profit sharing comment records, joined with full state names from the States lookup table";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new ListResponseDto<StateListResponse>
                {
                    Items = new List<StateListResponse>
                    {
                        new() { Abbreviation = "MA", Name = "Massachusetts" },
                        new() { Abbreviation = "NH", Name = "New Hampshire" },
                        new() { Abbreviation = "ME", Name = "Maine" },
                        new() { Abbreviation = "VT", Name = "Vermont" },
                        new() { Abbreviation = "RI", Name = "Rhode Island" },
                        new() { Abbreviation = "CT", Name = "Connecticut" },
                        new() { Abbreviation = "NY", Name = "New York" },
                        new() { Abbreviation = "FL", Name = "Florida" }
                    }
                }
            } };
        });
        Group<LookupGroup>();
    }

    public override async Task<Results<Ok<ListResponseDto<StateListResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, new { }, async () =>
        {
            // Retrieve states from database via StateService
            var states = await _stateService.GetStatesAsync(ct);

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "state-list-lookup"),
                new("endpoint", nameof(StateListEndpoint)));

            var response = new ListResponseDto<StateListResponse>
            {
                Items = states.ToList()
            };

            return Result<ListResponseDto<StateListResponse>>.Success(response);
        });
    }
}
