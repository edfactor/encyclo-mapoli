using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.CheckRun;
using Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.CheckRun;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.CheckRun;

/// <summary>
/// Endpoint to start a new profit sharing check run.
/// Orchestrates: workflow creation → file generation (MICR, DJDE, PositivePay) → FTP transfer → audit logging → status update.
/// </summary>
public sealed class CheckRunStartEndpoint : ProfitSharingEndpoint<CheckRunStartRequest, Results<Ok<CheckRunWorkflowResponse>, NotFound, ProblemHttpResult>>
{
    private readonly ICheckRunWorkflowService _workflowService;
    private readonly ICheckRunOrchestrator _orchestrator;
    private readonly ILogger<CheckRunStartEndpoint> _logger;

    public CheckRunStartEndpoint(
        ICheckRunWorkflowService workflowService,
        ICheckRunOrchestrator orchestrator,
        ILogger<CheckRunStartEndpoint> logger)
        : base(Navigation.Constants.CheckRun)
    {
        _workflowService = workflowService;
        _orchestrator = orchestrator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("start");
        Summary(s =>
        {
            s.Summary = "Start a new profit sharing check run";
            s.Description = "Initiates complete check run workflow: creates workflow record, generates files (MICR, DJDE, PositivePay), transfers via FTP, logs operations, updates workflow status.";
            s.ExampleRequest = CheckRunStartRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, CheckRunWorkflowResponse.ResponseExample() }
            };
            s.Responses[400] = "Bad Request. Invalid input parameters or validation errors.";
            s.Responses[500] = "Internal Server Error. Workflow execution failed.";
        });
        Group<CheckRunGroup>();
    }

    public override Task<Results<Ok<CheckRunWorkflowResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
        CheckRunStartRequest req,
        CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            // Step 1: Start new workflow (creates workflow record in Pending state)
            var workflowResult = await _workflowService.StartNewRunAsync(
                req.ProfitYear,
                req.CheckRunDate,
                req.CheckNumber,
                req.UserName,
                ct);
            if (!workflowResult.IsSuccess)
            {
                _logger.LogError("Failed to start new check run workflow: {Error} (correlation: {CorrelationId})",
                    workflowResult.Error, HttpContext.TraceIdentifier);
                return workflowResult.ToHttpResult(Error.Unexpected("Failed to start workflow"));
            }

            var runId = workflowResult.Value!.Id;
            _logger.LogInformation("Check run workflow started with runId: {RunId} (correlation: {CorrelationId})",
                runId, HttpContext.TraceIdentifier);

            try
            {
                // Step 2: Execute complete check run orchestration (file generation + transfer)
                var orchestrationResult = await _orchestrator.ExecuteCheckRunAsync(
                    req.ProfitYear,
                    req.CheckNumber.ToString(),
                    req.UserName,
                    runId,
                    ct);

                if (!orchestrationResult.IsSuccess)
                {
                    _logger.LogError("Check run orchestration failed for runId {RunId}: {Error} (correlation: {CorrelationId})",
                        runId, orchestrationResult.Error, HttpContext.TraceIdentifier);

                    // Step 3a: Record failure in workflow
                    await _workflowService.RecordStepCompletionAsync(runId, stepNumber: 1, req.UserName, ct);

                    return TypedResults.Problem(orchestrationResult.Error!.Description, statusCode: 500);
                }

                // Step 3b: Record success in workflow
                await _workflowService.RecordStepCompletionAsync(runId, stepNumber: 1, req.UserName, ct);

                // Step 4: Get updated workflow status
                var finalWorkflowResult = await _workflowService.GetCurrentRunAsync(req.ProfitYear, ct);

                // Record business metrics
                EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new("operation", "check-run-start"),
                    new("endpoint", nameof(CheckRunStartEndpoint)),
                    new("profit_year", req.ProfitYear.ToString()),
                    new("success", "true"));

                _logger.LogInformation(
                    "Check run completed successfully for profit year {ProfitYear}, runId: {RunId}, check number: {CheckNumber} (correlation: {CorrelationId})",
                    req.ProfitYear, runId, req.CheckNumber, HttpContext.TraceIdentifier);

                return finalWorkflowResult.IsSuccess
                    ? TypedResults.Ok(finalWorkflowResult.Value!)
                    : TypedResults.Problem(finalWorkflowResult.Error!.Description);
            }
#pragma warning disable S2139 // Exception is logged with full context before rethrowing
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error during check run execution for runId {RunId}, profit year {ProfitYear} (correlation: {CorrelationId})",
                    runId, req.ProfitYear, HttpContext.TraceIdentifier);

                // Record failure in workflow
                await _workflowService.RecordStepCompletionAsync(runId, stepNumber: 1, req.UserName, ct);

                // Record business metrics (failure)
                EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new("operation", "check-run-start"),
                    new("endpoint", nameof(CheckRunStartEndpoint)),
                    new("profit_year", req.ProfitYear.ToString()),
                    new("success", "false"));

                throw;
            }
#pragma warning restore S2139
        });
    }
}
