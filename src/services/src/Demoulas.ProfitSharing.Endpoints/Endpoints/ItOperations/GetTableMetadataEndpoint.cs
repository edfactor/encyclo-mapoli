using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class GetTableMetadataEndpoint : EndpointWithoutRequest<List<RowCountResult>>
{
    private readonly ITableMetadataService _frozenService;

    public GetTableMetadataEndpoint(ITableMetadataService frozenService)
    {
        _frozenService = frozenService;
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
        Group<ItOperationsGroup>();
    }

    public override Task<List<RowCountResult>> ExecuteAsync(CancellationToken ct)
    {
        return _frozenService.GetAllTableRowCountsAsync(ct);
    }
}
