using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public class UpdateForfeitureAdjustmentBulkEndpoint : ProfitSharingEndpoint<List<ForfeitureAdjustmentUpdateRequest>, Results<NoContent, ProblemHttpResult>>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;

    public UpdateForfeitureAdjustmentBulkEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService) : base(Navigation.Constants.Forfeitures)
    {
        _forfeitureAdjustmentService = forfeitureAdjustmentService;
    }

    public override void Configure()
    {
        Put("forfeiture-adjustments/bulk-update");
        Summary(s =>
        {
            s.Summary = "Update multiple forfeiture adjustments";
            s.Description = "This endpoint updates multiple forfeiture adjustments in a single request";
            s.ExampleRequest =
            [
                ForfeitureAdjustmentUpdateRequest.RequestExample()
            ];
            s.Responses[204] = "Successfully updated the forfeiture adjustments";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "One or more badge numbers not found";
        });
        Group<YearEndGroup>();
    }
    public override async Task<Results<NoContent, ProblemHttpResult>> ExecuteAsync(List<ForfeitureAdjustmentUpdateRequest> req, CancellationToken ct)
    {
        try
        {
            await _forfeitureAdjustmentService.UpdateForfeitureAdjustmentBulkAsync(req, ct);
            return TypedResults.NoContent();
        }
        catch (ArgumentException aex)
        {
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
