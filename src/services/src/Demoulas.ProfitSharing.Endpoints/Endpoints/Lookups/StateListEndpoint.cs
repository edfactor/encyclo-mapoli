using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

/// <summary>
/// Endpoint for retrieving the list of states used in profit sharing operations.
/// Currently returns a hardcoded list of common states, but designed to be easily
/// converted to database-driven lookup in the future.
/// </summary>
public sealed class StateListEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<StateListResponse>>
{
    private readonly ILogger<StateListEndpoint> _logger;

    public StateListEndpoint(ILogger<StateListEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _logger = logger;
    }

    public override void Configure()
    {
        Get("states");
        Summary(s =>
        {
            s.Summary = "Gets all available states for filtering profit sharing transactions";
            s.ResponseExamples = new Dictionary<int, object> {
            {
                200, new ListResponseDto<StateListResponse>
                {
                    Results = new List<StateListResponse>
                    {
                        new StateListResponse { Abbreviation = "MA", Name = "Massachusetts" },
                        new StateListResponse { Abbreviation = "NH", Name = "New Hampshire" },
                        new StateListResponse { Abbreviation = "ME", Name = "Maine" },
                        new StateListResponse { Abbreviation = "VT", Name = "Vermont" },
                        new StateListResponse { Abbreviation = "RI", Name = "Rhode Island" },
                        new StateListResponse { Abbreviation = "CT", Name = "Connecticut" },
                        new StateListResponse { Abbreviation = "NY", Name = "New York" },
                        new StateListResponse { Abbreviation = "FL", Name = "Florida" }
                    }
                }
            } };
        });
        Group<LookupGroup>();
        if (!Env.IsTestEnvironment())
        {
            // Cache for 1 hour since state list rarely changes
            TimeSpan cacheDuration = TimeSpan.FromHours(1);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<ListResponseDto<StateListResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, new { }, async () =>
        {
            // TODO: Replace with database query when STATE_LIST table is created
            // For now, hardcoded list of common states used in profit sharing operations
            var states = GetStateList();

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "state-list-lookup"),
                new("endpoint", nameof(StateListEndpoint)));

            var response = new ListResponseDto<StateListResponse>
            {
                Results = states
            };

            return Result<ListResponseDto<StateListResponse>>.Success(response);
        });
    }

    /// <summary>
    /// Returns the hardcoded list of states.
    /// TODO: Replace this method with a database query:
    /// <code>
    /// return await _dataContextFactory.UseReadOnlyContext(c => c.States
    ///     .OrderBy(x => x.Abbreviation)
    ///     .Select(x => new StateListResponse { Abbreviation = x.Abbreviation, Name = x.Name })
    ///     .ToListAsync(ct), ct);
    /// </code>
    /// </summary>
    private static List<StateListResponse> GetStateList()
    {
        return new List<StateListResponse>
        {
            new StateListResponse { Abbreviation = "CT", Name = "Connecticut" },
            new StateListResponse { Abbreviation = "FL", Name = "Florida" },
            new StateListResponse { Abbreviation = "MA", Name = "Massachusetts" },
            new StateListResponse { Abbreviation = "ME", Name = "Maine" },
            new StateListResponse { Abbreviation = "NH", Name = "New Hampshire" },
            new StateListResponse { Abbreviation = "NY", Name = "New York" },
            new StateListResponse { Abbreviation = "RI", Name = "Rhode Island" },
            new StateListResponse { Abbreviation = "VT", Name = "Vermont" }
        };
    }
}
