using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;

/// <summary>
/// Endpoint for retrieving a single store by its ID.
/// Returns store information including display name, departments, and spirits store status.
/// </summary>
public sealed class StoreByIdEndpoint : ProfitSharingEndpoint<StoreByIdRequest, Results<Ok<StoreResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IStoreLookupService _storeLookupService;

    public StoreByIdEndpoint(IStoreLookupService storeLookupService) : base(Navigation.Constants.Inquiries)
    {
        _storeLookupService = storeLookupService;
    }

    public override void Configure()
    {
        Get("stores/{StoreId}");
        Summary(s =>
        {
            s.Summary = "Gets a single store by ID";
            s.Description = "Returns detailed information for a specific store including display name, departments, and spirits store status";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, new StoreResponse
                    {
                        StoreId = 1,
                        DisplayName = "1 - FLETCHER",
                        City = "Chelmsford",
                        State = "MA",
                        ZipCode = "01824",
                        HasSpirits = false
                    }
                },
                {
                    404, new { message = "Store not found" }
                }
            };
        });
        Group<LookupGroup>();
    }

    protected override async Task<Results<Ok<StoreResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(StoreByIdRequest req, CancellationToken ct)
    {
        // Retrieve store from the lookup service
        var store = await _storeLookupService.GetStoreByIdAsync(req.StoreId, ct);

        if (store == null)
        {
            return Result<StoreResponse>.Failure(Error.StoreNotFound).ToHttpResult();
        }

        // Convert to detail response DTO
        var response = new StoreResponse
        {
            StoreId = store.StoreId,
            DisplayName = store.DisplayName,
            City = store.City,
            State = store.State,
            ZipCode = store.ZipCode,
            HasSpirits = store.HasSpirits
        };

        return Result<StoreResponse>.Success(response).ToHttpResult();
    }
}

