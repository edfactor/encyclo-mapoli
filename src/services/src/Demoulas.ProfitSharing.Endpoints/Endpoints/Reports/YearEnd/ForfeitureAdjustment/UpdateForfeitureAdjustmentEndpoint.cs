using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public class UpdateForfeitureAdjustmentEndpoint : Endpoint<ForfeitureAdjustmentUpdateRequest, ForfeitureAdjustmentReportDetail>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;

    public UpdateForfeitureAdjustmentEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService)
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
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    ForfeitureAdjustmentReportDetail.ResponseExample()
                }
            };
            s.Responses[200] = "Successfully updated the forfeiture adjustment";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Badge number not found";
        });
        Group<YearEndGroup>();
        Roles(Role.ADMINISTRATOR, Role.FINANCEMANAGER);
    }

    public override async Task HandleAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken ct)
    {
        var result = await _forfeitureAdjustmentService.UpdateForfeitureAdjustmentAsync(req, ct);
        
        if (result == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(result, ct);
    }
} 