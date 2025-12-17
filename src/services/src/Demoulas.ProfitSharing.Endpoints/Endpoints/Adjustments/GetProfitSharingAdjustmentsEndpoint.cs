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

public sealed class GetProfitSharingAdjustmentsEndpoint : ProfitSharingEndpoint<GetProfitSharingAdjustmentsRequest, Results<Ok<GetProfitSharingAdjustmentsResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IProfitSharingAdjustmentsService _service;
    private readonly ILogger<GetProfitSharingAdjustmentsEndpoint> _logger;

    public GetProfitSharingAdjustmentsEndpoint(
        IProfitSharingAdjustmentsService service,
        ILogger<GetProfitSharingAdjustmentsEndpoint> logger) : base(Navigation.Constants.ProfitSharingAdjustments)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("under21");
        Group<AdjustmentsGroup>();
        Validator<GetProfitSharingAdjustmentsRequestValidator>();

        Summary(s =>
        {
            s.Summary = "Get Profit Sharing Adjustments";
            s.Description = "Loads Profit Detail rows for the Profit Sharing Adjustments screen (TPR008-22 parity).";
        });
    }

    public override Task<Results<Ok<GetProfitSharingAdjustmentsResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(GetProfitSharingAdjustmentsRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _service.GetAsync(req, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "profit-sharing-adjustments-load"),
                new("endpoint", nameof(GetProfitSharingAdjustmentsEndpoint)),
                new("profit_year", req.ProfitYear.ToString()));

            return result.ToHttpResult(Error.EmployeeNotFound);
        }, "Ssn", "DateOfBirth", "HireDate", "CurrentIncomeYear", "CurrentHoursYear");
    }
}
