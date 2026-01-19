using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

/// <summary>
/// Endpoint for retrieving the list of states used in profit sharing operations.
/// Returns states that are referenced in profit sharing comment records, joined with
/// full state names from the States lookup table.
/// </summary>
public sealed class StateListEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<StateListResponse>>
{
    private readonly IStateService _stateService;

    public StateListEndpoint(IStateService stateService) : base(Navigation.Constants.Inquiries)
    {
        _stateService = stateService;
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

    protected override async Task<Results<Ok<ListResponseDto<StateListResponse>>, NotFound, ProblemHttpResult>> HandleRequestAsync(CancellationToken ct)
    {
        var result = await _stateService.GetStatesAsync(ct);
        var response = new ListResponseDto<StateListResponse>
        {
            Items = result.ToList()
        };

        return Result<ListResponseDto<StateListResponse>>.Success(response).ToHttpResult();
    }
}
