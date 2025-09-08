using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class CreateMilitaryContributionRecord : ProfitSharingRequestEndpoint<CreateMilitaryContributionRequest>
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
                { 200, new MilitaryContributionResponse { ProfitYear = Convert.ToInt16(DateTime.Now.Year) } }
            };
            s.ExampleRequest = CreateMilitaryContributionRequest.RequestExample();
        });
    Group<MilitaryGroup>();
    }

    public override async Task HandleAsync(CreateMilitaryContributionRequest req, CancellationToken ct)
    {
        var response = await _militaryService.CreateMilitaryServiceRecordAsync(req, ct);

        await response.Match(
            async success =>
            {
                await Send.CreatedAtAsync<GetMilitaryContributionRecords>(
                    routeValues: new MilitaryContributionRequest
                    {
                        BadgeNumber = req.BadgeNumber,
                        ProfitYear = req.ProfitYear,
                    },
                    responseBody: success,
                    cancellation: ct
                );
            },
            async error =>
            {
                await Send.ResponseAsync(error, statusCode: 400, cancellation: ct);
            }
        );
    }

}
