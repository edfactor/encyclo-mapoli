using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public class UpdateForfeitureAdjustmentEndpoint : ProfitSharingEndpoint<ForfeitureAdjustmentUpdateRequest, Results<NoContent, ProblemHttpResult>>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;

    public UpdateForfeitureAdjustmentEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService)
        : base(Navigation.Constants.ProfitShareForfeit)
    {
        _forfeitureAdjustmentService = forfeitureAdjustmentService;
    }

    public override void Configure()
    {
        Put("forfeiture-adjustments/update");
        Summary(s =>
        {
            s.Summary = "Update forfeiture adjustment for a badge number";
            s.Description = "This endpoint updates the forfeiture adjustment for a specific badge number";
            s.ExampleRequest = ForfeitureAdjustmentUpdateRequest.RequestExample();
            s.Responses[204] = "Successfully updated the forfeiture adjustment";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Badge number not found";
        });
        Group<YearEndGroup>();
    }
    public override async Task<Results<NoContent, ProblemHttpResult>> ExecuteAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken ct)
    {
        try
        {
            await _forfeitureAdjustmentService.UpdateForfeitureAdjustmentAsync(req, ct);
            return TypedResults.NoContent();
        }
        catch (ArgumentException aex)
        {
            // Business rule / not found style errors -> 400 Problem for now (no dedicated NotFound union in this endpoint)
            return TypedResults.Problem(aex.Message);
        }
        catch (InvalidOperationException ioex)
        {
            return TypedResults.Problem(ioex.Message);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(Error.Unexpected(ex.Message).Description);
        }
    }
}
