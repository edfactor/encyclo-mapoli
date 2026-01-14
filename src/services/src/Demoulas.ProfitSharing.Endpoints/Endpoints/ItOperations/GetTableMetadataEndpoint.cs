using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class GetTableMetadataEndpoint : ProfitSharingResponseEndpoint<List<RowCountResult>>
{
    private readonly ITableMetadataService _frozenService;
    private readonly ILogger<GetTableMetadataEndpoint> _logger;

    public GetTableMetadataEndpoint(ITableMetadataService frozenService, ILogger<GetTableMetadataEndpoint> logger) : base(Navigation.Constants.ItDevOps)
    {
        _frozenService = frozenService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("metadata");
        Summary(s =>
        {
            s.Summary = "Gets frozen demographic meta data";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new List<RowCountResult>
                    {
                        new RowCountResult
                        {
                            TableName = "TABLE_NAME",
                            RowCount = byte.MaxValue
                        }
                    }
                }
            };
        });
        Group<ItDevOpsAllUsersGroup>();
    }

    public override async Task<List<RowCountResult>> ExecuteAsync(CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, new { });

            var response = await _frozenService.GetAllTableRowCountsAsync(ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "table-metadata-query"),
                new("endpoint", "GetTableMetadataEndpoint"));

            var tableCount = response?.Count ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(tableCount,
                new("record_type", "table-metadata"),
                new("endpoint", "GetTableMetadataEndpoint"));

            _logger.LogInformation("Table metadata query completed, returned {TableCount} tables (correlation: {CorrelationId})",
                tableCount, HttpContext.TraceIdentifier);

            var safeResponse = response ?? new List<RowCountResult>();
            this.RecordResponseMetrics(HttpContext, _logger, safeResponse);

            return safeResponse;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
