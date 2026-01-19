using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Adjustments;

public sealed class SaveProfitSharingAdjustmentsEndpoint : ProfitSharingEndpoint<SaveProfitSharingAdjustmentsRequest, Results<Ok<GetProfitSharingAdjustmentsResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IProfitSharingAdjustmentsService _service;

    public SaveProfitSharingAdjustmentsEndpoint(
        IProfitSharingAdjustmentsService service) : base(Navigation.Constants.ProfitSharingAdjustments)
    {
        _service = service;
    }

    public override void Configure()
    {
        Post("under21");
        Group<AdjustmentsGroup>();
        Validator<SaveProfitSharingAdjustmentsRequestValidator>();

        Summary(s =>
        {
            s.Summary = "Save Profit Sharing Adjustments";
            s.ExampleRequest = SaveProfitSharingAdjustmentsRequest.RequestExample();
            s.Description = "Updates existing Profit Detail rows and inserts new rows for the Profit Sharing Adjustments screen (TPR008-22 parity).";
        });
    }

    protected override async Task<Results<Ok<GetProfitSharingAdjustmentsResponse>, NotFound, ProblemHttpResult>> HandleRequestAsync(
        SaveProfitSharingAdjustmentsRequest req,
        CancellationToken ct)
    {
        var result = await _service.SaveAdjustmentsAsync(req, ct);
        return result.ToHttpResult(Error.EmployeeNotFound);
    }
}
