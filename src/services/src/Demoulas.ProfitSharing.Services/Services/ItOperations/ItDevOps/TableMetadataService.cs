using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.Services.Services.ItOperations.ItDevOps;

public sealed class TableMetadataService : ITableMetadataService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public TableMetadataService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public Task<List<RowCountResult>> GetAllTableRowCountsAsync(CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseWritableContextAsync(async (context, transaction) =>
        {
            var results = new List<RowCountResult>();
            var designTimeModel = context.GetService<IDesignTimeModel>().Model;


            var entityTypes = designTimeModel.GetEntityTypes()
                .Where(t => !t.IsOwned()
                            && !t.IsTableExcludedFromMigrations()
                            && !typeof(System.Collections.IDictionary).IsAssignableFrom(t.ClrType))
                .ToList();


            foreach (var entityType in entityTypes)
            {

                var clrType = entityType.ClrType;
                var tableName = entityType.GetTableName();

                try
                {
                    // Get DbSet<T>
                    var set = context.GetType()
                        .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
                        .MakeGenericMethod(clrType)
                        .Invoke(context, null);

                    if (set is IQueryable queryable)
                    {
                        var countMethod = typeof(EntityFrameworkQueryableExtensions)
                            .GetMethods()
                            .First(m => m.Name == nameof(EntityFrameworkQueryableExtensions.CountAsync) &&
                                        m.GetParameters().Length == 2)
                            .MakeGenericMethod(clrType);

                        var task = (Task<int>)countMethod.Invoke(null, [queryable, cancellationToken])!;
                        var count = await task;

                        results.Add(new RowCountResult { TableName = tableName ?? clrType.Name, RowCount = count });
                    }
                }
                catch (OracleException e) when (e.Number == 942) // https://docs.oracle.com/en/error-help/db/ora-00942/?r=23ai
                {
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(tableName);
                }
            }


            // No changes are being made, but we are being extra cautious  
            await transaction.RollbackAsync(cancellationToken);

            return results;
        }, cancellationToken);
    }
}
