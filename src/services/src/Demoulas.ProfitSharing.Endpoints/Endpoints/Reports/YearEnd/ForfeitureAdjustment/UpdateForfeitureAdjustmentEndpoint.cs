// Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

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
        Group<AdhocReportsGroup>();
    }
    protected override async Task<Results<NoContent, ProblemHttpResult>> HandleRequestAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken ct)
    {
        var result = await _forfeitureAdjustmentService.UpdateForfeitureAdjustmentAsync(req, ct);
        if (result.IsError)
        {
            Microsoft.AspNetCore.Mvc.ProblemDetails pd = result.Error!;
            return TypedResults.Problem(pd.Detail);
        }
        return TypedResults.NoContent();
    }
}
