using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Adjustments;

public sealed class SaveProfitSharingAdjustmentsEndpoint : ProfitSharingEndpoint<SaveProfitSharingAdjustmentsRequest, Results<Ok<GetProfitSharingAdjustmentsResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IProfitSharingAdjustmentsService _service;
    private readonly ILogger<SaveProfitSharingAdjustmentsEndpoint> _logger;

    public SaveProfitSharingAdjustmentsEndpoint(
        IProfitSharingAdjustmentsService service,
        ILogger<SaveProfitSharingAdjustmentsEndpoint> logger) : base(Navigation.Constants.ProfitSharingAdjustments)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("under21");
        Group<AdjustmentsGroup>();
        Validator<SaveProfitSharingAdjustmentsRequestValidator>();

        Summary(s =>
        {
            s.Summary = "Save Profit Sharing Adjustments";
            s.Description = "Updates existing Profit Detail rows and inserts new rows for the Profit Sharing Adjustments screen (TPR008-22 parity).";
        });
    }

    public override Task<Results<Ok<GetProfitSharingAdjustmentsResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(SaveProfitSharingAdjustmentsRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _service.SaveAsync(req, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "profit-sharing-adjustments-save"),
                new("endpoint", nameof(SaveProfitSharingAdjustmentsEndpoint)),
                new("profit_year", req.ProfitYear.ToString()));

            return result.ToHttpResult(Error.EmployeeNotFound);
        }, "Ssn", "DateOfBirth", "HireDate", "CurrentIncomeYear", "CurrentHoursYear");
    }
}
