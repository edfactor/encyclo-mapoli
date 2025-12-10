using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayServices;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.PayServices;

/// <summary>
/// Endpoint for PayServices operations.
/// Handles demographic retrieval requests following the Result&lt;T&gt; pattern.
/// </summary>
public sealed class PayServicesPartTimeEndpoint : ProfitSharingEndpoint<PayServicesRequest, Results<Ok<PayServicesResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IPayService _payService;
    private readonly ILogger<PayServicesPartTimeEndpoint> _logger;

    public PayServicesPartTimeEndpoint(
        IPayService payService,
        ILogger<PayServicesPartTimeEndpoint> logger) : base(Navigation.Constants.QPAY600)
    {
        _payService = payService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("parttime/{ProfitYear}");
        Group<PayServicesGroup>();

        Validator<PayServicesRequestValidator>();
        Summary(s =>
        {
            s.Description = "Retrieves Pay Services for part-time employees.";
            s.Summary = "Get Pay Services";
            s.ExampleRequest = PayServicesRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, PayServicesResponse.ResponseExample() },
                { 400, new { detail = "Validation error", title = "Validation Failed" } },
                { 404, new { detail = "Pay Service not found" } }
            };
        });
    }

    public override Task<Results<Ok<PayServicesResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(PayServicesRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            // Call the service to get demographics
            var result = await _payService.GetPayServices(req, EmploymentType.Constants.PartTime, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "get-demographics-parttime"),
                new("endpoint", nameof(PayServicesPartTimeEndpoint)));

            // Convert Result<PayServicesResponse> to proper HTTP response
            return result.Match<Results<Ok<PayServicesResponse>, NotFound, ProblemHttpResult>>(
                demographicValue =>
                {
                    return TypedResults.Ok(demographicValue);
                },
                error =>
                {
                    // Return problem details for all other errors
                    return TypedResults.Problem(error);
                });
        });
    }
}
