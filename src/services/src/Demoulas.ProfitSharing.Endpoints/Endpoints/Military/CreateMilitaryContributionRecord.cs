using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class CreateMilitaryContributionRecord : Endpoint<CreateMilitaryContributionRequest>
{
    public CreateMilitaryContributionRecord()
    {
    
    }

    public override void Configure()
    {
        Post(string.Empty);
        Summary(s =>
        {
            s.Summary = "Create Military Contribution Record";
            s.ResponseExamples = new Dictionary<int, object> { { 200, new MilitaryContributionResponse { ProfitYear = Convert.ToInt16(DateTime.Now.Year) } } };
        });
        Group<MilitaryGroup>();
    }

    public override Task HandleAsync(CreateMilitaryContributionRequest req, CancellationToken ct)
    {
        var response = new MilitaryContributionResponse
        {
            BadgeNumber = req.BadgeNumber,
            Amount = req.Amount,
            CommentTypeId = req.CommentTypeId,
            ContributionDate = req.ContributionDate,
            ProfitYear = req.ProfitYear
        };

        return SendCreatedAtAsync<GetMilitaryContributionRecord>(
            routeValues: new { id = 5 }, // Assuming 5 is the ID of the created record
            responseBody: response,
            cancellation: ct
        );
    }
}
