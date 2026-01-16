using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class CreateMilitaryContributionRecord : ProfitSharingEndpoint<CreateMilitaryContributionRequest, Results<Created<MilitaryContributionResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IMilitaryService _militaryService;

    public CreateMilitaryContributionRecord(IMilitaryService militaryService) : base(Navigation.Constants.MilitaryContributions)
    {
        _militaryService = militaryService ?? throw new ArgumentNullException(nameof(militaryService));
    }

    public override void Configure()
    {
        Post(string.Empty);
        Summary(s =>
        {
            s.Summary = "Create Military Contribution Record";
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 201, new MilitaryContributionResponse { ProfitYear = Convert.ToInt16(DateTime.Now.Year), BadgeNumber = 1234567 } }
            };
            s.ExampleRequest = CreateMilitaryContributionRequest.RequestExample();
            s.Responses[404] = "Not Found. Employee not found.";
        });
        Group<MilitaryGroup>();
    }

    protected override async Task<Results<Created<MilitaryContributionResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        CreateMilitaryContributionRequest req,
        CancellationToken ct)
    {
        var result = await _militaryService.CreateMilitaryServiceRecordAsync(req, ct);
        return result.Match<Results<Created<MilitaryContributionResponse>, NotFound, ProblemHttpResult>>(
            success => TypedResults.Created($"/military-contributions/{success.BadgeNumber}/{success.ProfitYear}", success),
            error => error.Detail == Error.EmployeeNotFound.Description
                ? TypedResults.NotFound()
                : TypedResults.Problem(error.Detail));
    }
}
