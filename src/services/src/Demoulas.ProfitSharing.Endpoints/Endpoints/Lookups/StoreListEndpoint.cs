using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

/// <summary>
/// Endpoint for retrieving the list of stores with optional filtering.
/// Returns store information including store ID, display name, and associated departments.
/// Supports filtering by store status (Active/Unopened) and type (Retail/All).
/// </summary>
public sealed class StoreListEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<StoreResponse>>
{
    private readonly IStoreLookupService _storeLookupService;

    public StoreListEndpoint(IStoreLookupService storeLookupService) : base(Navigation.Constants.Inquiries)
    {
        _storeLookupService = storeLookupService;
    }

    public override void Configure()
    {
        Get("stores");
        Summary(s =>
        {
            s.Summary = "Gets stores with optional filtering";
            s.Description = "Returns a list of stores with their display names, departments, and spirits store status. " +
                           "Supports filtering by store status (Active/Unopened/All) and type (Retail/All).";
            s.Params["Status"] = "Filter by store status: 0=All, 1=Active, 2=Unopened";
            s.Params["StoreType"] = "Filter by store type: 0=All, 1=Retail (store number < 899)";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, new ListResponseDto<StoreResponse>
                    {
                        Items = new List<StoreResponse>
                        {
                            new()
                            {
                                StoreId = 1,
                                DisplayName = "1 - FLETCHER",
                                City = "Chelmsford",
                                State = "MA",
                                ZipCode = "01824",
                                HasSpirits = false
                            },
                            new()
                            {
                                StoreId = 2,
                                DisplayName = "2 - BRIDGE ST.",
                                City = "Salem",
                                State = "MA",
                                ZipCode = "01970",
                                HasSpirits = false
                            }
                        }
                    }
                }
            };
        });
        Group<LookupGroup>();
    }

    protected override async Task<Results<Ok<ListResponseDto<StoreResponse>>, NotFound, ProblemHttpResult>> HandleRequestAsync(CancellationToken ct)
    {
        // Create default request (all stores, all types)
        var request = new StoreListRequest();

        // Get stores using the lookup service
        var stores = await _storeLookupService.GetStoresAsync(request, ct);

        var response = new ListResponseDto<StoreResponse>
        {
            Items = stores
        };

        return Result<ListResponseDto<StoreResponse>>.Success(response).ToHttpResult();
    }
}
