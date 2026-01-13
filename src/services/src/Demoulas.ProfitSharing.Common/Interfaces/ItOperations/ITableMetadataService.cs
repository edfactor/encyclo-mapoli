using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

namespace Demoulas.ProfitSharing.Common.Interfaces.ItOperations;

public interface ITableMetadataService
{
    Task<List<RowCountResult>> GetAllTableRowCountsAsync(CancellationToken cancellationToken);
}
