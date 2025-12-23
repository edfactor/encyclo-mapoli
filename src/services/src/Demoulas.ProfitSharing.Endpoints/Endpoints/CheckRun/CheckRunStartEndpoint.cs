using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.CheckRun;
using Demoulas.ProfitSharing.Common.Contracts.Response.CheckRun;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.CheckRun;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.CheckRun;

/// <summary>
/// Endpoint to start a new profit sharing check run.
/// Orchestrates: workflow creation → file generation (MICR, DJDE, PositivePay) → FTP transfer → audit logging → status update.
/// </summary>
public sealed class CheckRunStartEndpoint : ProfitSharingEndpoint<CheckRunStartRequest, Results<Ok<CheckRunWorkflowResponse>, BadRequest, ProblemHttpResult>>
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
        Policies(Security.Policy.CanRunYearEndProcesses);
        Group<CheckRunGroup>();
    }

    public override async Task<Results<Ok<CheckRunWorkflowResponse>, BadRequest, ProblemHttpResult>> ExecuteAsync(
        CheckRunStartRequest req,
        CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            // Step 1: Start new workflow (creates workflow record in Pending state)
            var workflowResult = await _workflowService.StartNewRunAsync(
                req.ProfitYear, 
                req.CheckRunDate, 
                req.CheckNumber, 
                req.UserId, 
                ct);
            if (!workflowResult.IsSuccess)
            {
                _logger.LogError("Failed to start new check run workflow: {Error} (correlation: {CorrelationId})",
                    workflowResult.Error, HttpContext.TraceIdentifier);
                return workflowResult;
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
                    req.UserId,
                    runId,
                    ct);

                if (!orchestrationResult.IsSuccess)
                {
                    _logger.LogError("Check run orchestration failed for runId {RunId}: {Error} (correlation: {CorrelationId})",
                        runId, orchestrationResult.Error, HttpContext.TraceIdentifier);

                    // Step 3a: Record failure in workflow
                    await _workflowService.RecordStepCompletionAsync(runId, stepNumber: 1, req.UserId, ct);

                    return Result<CheckRunWorkflowResponse>.Failure(orchestrationResult.Error!);
                }

                // Step 3b: Record success in workflow
                await _workflowService.RecordStepCompletionAsync(runId, stepNumber: 1, req.UserId, ct);

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

                return finalWorkflowResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error during check run execution for runId {RunId}, profit year {ProfitYear} (correlation: {CorrelationId})",
                    runId, req.ProfitYear, HttpContext.TraceIdentifier);

                // Record failure in workflow
                await _workflowService.RecordStepCompletionAsync(runId, stepNumber: 1, req.UserId, ct);

                // Record business metrics (failure)
                EndpointTelemetry.BusinessOperationsTotal.Add(1,
                    new("operation", "check-run-start"),
                    new("endpoint", nameof(CheckRunStartEndpoint)),
                    new("profit_year", req.ProfitYear.ToString()),
                    new("success", "false"));

                throw;
            }
        });
    }
}
