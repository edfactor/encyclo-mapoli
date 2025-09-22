using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
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

public class GetForfeitureAdjustmentsEndpoint : ProfitSharingEndpoint<SuggestedForfeitureAdjustmentRequest, Results<Ok<SuggestedForfeitureAdjustmentResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;

    public GetForfeitureAdjustmentsEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService) : base(Navigation.Constants.Forfeitures)
    {
        _forfeitureAdjustmentService = forfeitureAdjustmentService;
    }

    public override void Configure()
    {
        Get("forfeiture-adjustments");
        Summary(s =>
        {
            s.Summary = "Get forfeiture suggested adjustments for  badge number or ssn.";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Description = "This endpoint is used to get a suggested forfeiture adjustment for a badge number or ssn.";
        });
        Group<YearEndGroup>();
    }
    public override async Task<Results<Ok<SuggestedForfeitureAdjustmentResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(SuggestedForfeitureAdjustmentRequest req, CancellationToken ct)
    {
        try
        {
            var response = await _forfeitureAdjustmentService.GetSuggestedForfeitureAmount(req, ct);
            return Result<SuggestedForfeitureAdjustmentResponse>.Success(response).ToHttpResult();
        }
        catch (ArgumentException aex) when (aex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return Result<SuggestedForfeitureAdjustmentResponse>.Failure(Error.EmployeeNotFound).ToHttpResult(Error.EmployeeNotFound);
        }
        catch (Exception ex)
        {
            return Result<SuggestedForfeitureAdjustmentResponse>.Failure(Error.Unexpected(ex.Message)).ToHttpResult();
        }
    }
}
