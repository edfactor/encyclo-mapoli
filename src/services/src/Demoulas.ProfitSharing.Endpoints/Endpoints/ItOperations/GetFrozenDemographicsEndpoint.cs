using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class GetFrozenDemographicsEndpoint : Endpoint<SortedPaginationRequestDto, PaginatedResponseDto<FrozenStateResponse>>
{
    private readonly IFrozenService _frozenService;

    public GetFrozenDemographicsEndpoint(IFrozenService frozenService)
    {
        _frozenService = frozenService;
    }

    public override void Configure()
    {
        Get("frozen");
        Summary(s =>
        {
            s.Summary = "Gets frozen demographic meta data";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new PaginatedResponseDto<FrozenStateResponse>
                    {
                        Results = new List<FrozenStateResponse>
                        {
                            new FrozenStateResponse
                            {
                                Id = 2,
                                ProfitYear = Convert.ToInt16(DateTime.Now.Year),
                                FrozenBy = "Somebody",
                                AsOfDateTime = DateTime.Today,
                                IsActive = false,
                            }
                        }
                    }
                }
            };
        });
        Group<ItOperationsGroup>();
    }

    public override Task<PaginatedResponseDto<FrozenStateResponse>> ExecuteAsync(SortedPaginationRequestDto req, CancellationToken ct)
    {
        return _frozenService.GetFrozenDemographics(req, ct);
    }
}
