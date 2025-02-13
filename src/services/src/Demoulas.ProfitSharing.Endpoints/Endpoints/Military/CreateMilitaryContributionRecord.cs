using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class CreateMilitaryContributionRecord : Endpoint<MilitaryContributionRequest, MilitaryContributionResponse>
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

    public override Task<MilitaryContributionResponse> ExecuteAsync(MilitaryContributionRequest req, CancellationToken ct)
    {
        return Task.FromResult(new MilitaryContributionResponse());
    }
}
