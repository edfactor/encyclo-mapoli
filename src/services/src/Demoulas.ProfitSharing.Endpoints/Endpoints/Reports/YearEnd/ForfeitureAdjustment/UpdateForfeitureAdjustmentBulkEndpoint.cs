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

public class UpdateForfeitureAdjustmentBulkEndpoint : Endpoint<List<ForfeitureAdjustmentUpdateRequest>, List<ForfeitureAdjustmentReportDetail>>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;

    public UpdateForfeitureAdjustmentBulkEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService)
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
            s.ExampleRequest = new List<ForfeitureAdjustmentUpdateRequest> 
            { 
                ForfeitureAdjustmentUpdateRequest.RequestExample() 
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new List<ForfeitureAdjustmentReportDetail> 
                    { 
                        ForfeitureAdjustmentReportDetail.ResponseExample() 
                    }
                }
            };
            s.Responses[200] = "Successfully updated the forfeiture adjustments";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "One or more badge numbers not found";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(List<ForfeitureAdjustmentUpdateRequest> req, CancellationToken ct)
    {
        var results = await _forfeitureAdjustmentService.UpdateForfeitureAdjustmentBulkAsync(req, ct);
        
        if (results == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(results, ct);
    }
} 