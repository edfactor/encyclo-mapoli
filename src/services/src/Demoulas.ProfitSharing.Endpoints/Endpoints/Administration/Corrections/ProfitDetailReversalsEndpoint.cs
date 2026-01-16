using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ProfitDetails;

public sealed class ProfitDetailReversalsEndpoint : ProfitSharingEndpoint<IdsRequest, Results<Ok<IdsResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IProfitDetailReversalsService _profitDetailReversalsService;
    private readonly ILogger<ProfitDetailReversalsEndpoint> _logger;

    public ProfitDetailReversalsEndpoint(
        IProfitDetailReversalsService profitDetailReversalsService,
        ILogger<ProfitDetailReversalsEndpoint> logger) : base(Navigation.Constants.ProfitDetailReversals)
    {
        _profitDetailReversalsService = profitDetailReversalsService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/reversals");
        Group<ProfitDetailsGroup>();
        Policies(Security.Policy.CanReverseProfitDetails);
        Validator<IdsRequestValidator>();
        Summary(s =>
        {
            s.Description = "Reverses profit detail entries based on provided IDs. Creates reversal entries for eligible profit codes and updates ETVA balances where applicable.";
            s.Summary = "Reverse Profit Detail Entries";
            s.ExampleRequest = IdsRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, IdsResponse.ResponseExample() },
                { 400, new { detail = "Validation error", title = "Validation Failed" } },
                { 404, new { detail = "Profit details not found" } }
            };
        });
    }

    protected override async Task<Results<Ok<IdsResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(IdsRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Profit detail reversal requested for {Count} ids", req.Ids?.Length ?? 0);
        var profitDetailIds = req.Ids ?? Array.Empty<int>();
        var result = await _profitDetailReversalsService.ReverseProfitDetailsAsync(profitDetailIds, ct);
        return result.IsSuccess
            ? TypedResults.Ok(new IdsResponse { Ids = profitDetailIds })
            : (Results<Ok<IdsResponse>, NotFound, ProblemHttpResult>)TypedResults.Problem(result.Error!);
    }
}
