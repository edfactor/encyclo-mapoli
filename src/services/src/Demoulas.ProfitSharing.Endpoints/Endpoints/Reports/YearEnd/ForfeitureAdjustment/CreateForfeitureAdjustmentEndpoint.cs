using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public class CreateForfeitureAdjustmentEndpoint : Endpoint<CreateForfeitureAdjustmentRequest, ForfeitureAdjustmentReportDetail>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;
    private readonly ILogger<CreateForfeitureAdjustmentEndpoint> _logger;

    public CreateForfeitureAdjustmentEndpoint(
        IForfeitureAdjustmentService forfeitureAdjustmentService,
        ILogger<CreateForfeitureAdjustmentEndpoint> logger)
    {
        _forfeitureAdjustmentService = forfeitureAdjustmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("forfeiture-adjustments/create");
        Summary(s =>
        {
            s.Summary = "Create a new forfeiture adjustment for a badge number";
            s.Description = "This endpoint creates a new forfeiture adjustment for a specific badge number";
            s.ExampleRequest = CreateForfeitureAdjustmentRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    ForfeitureAdjustmentReportDetail.ResponseExample()
                }
            };
            s.Responses[200] = "Successfully created the forfeiture adjustment";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Badge number not found";
        });
        Group<YearEndGroup>();
        Roles(Role.ADMINISTRATOR, Role.FINANCEMANAGER);
    }

    public override async Task HandleAsync(CreateForfeitureAdjustmentRequest req, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Creating forfeiture adjustment for badge: {Badge}, amount: {Amount}", 
                req.BadgeNumber, req.ForfeitureAmount);
                
            var result = await _forfeitureAdjustmentService.CreateForfeitureAdjustmentAsync(req, ct);
            
            if (result == null)
            {
                _logger.LogWarning("No result returned from forfeiture adjustment creation for badge: {Badge}", 
                    req.BadgeNumber);
                await SendNotFoundAsync(ct);
                return;
            }
            
            _logger.LogInformation("Successfully created forfeiture adjustment for badge: {Badge}", 
                req.BadgeNumber);
            await SendOkAsync(result, ct);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error during forfeiture adjustment creation: {Message}", ex.Message);
            
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            HttpContext.Response.ContentType = "application/json";
            
            var errorResponse = new { error = "Validation Error", message = ex.Message };
            var json = JsonSerializer.Serialize(errorResponse);
            
            await HttpContext.Response.WriteAsync(json, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during forfeiture adjustment creation: {Message}", ex.Message);
            
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            HttpContext.Response.ContentType = "application/json";
            
            var errorResponse = new { error = "Server Error", message = "An unexpected error occurred while processing your request." };
            var json = JsonSerializer.Serialize(errorResponse);
            
            await HttpContext.Response.WriteAsync(json, ct);
        }
    }
} 