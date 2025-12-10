using Demoulas.ProfitSharing.Data.Contexts;

namespace Demoulas.ProfitSharing.OracleHcm.Commands;

/// <summary>
/// Represents a database operation command that can be executed within a transaction context.
/// Allows domain services to return operations without directly executing them.
/// </summary>
public interface IDemographicCommand
{
    /// <summary>
    /// Executes the command within the provided database context.
    /// </summary>
    /// <param name="context">Database context for the operation</param>
    /// <param name="ct">Cancellation token</param>
    Task ExecuteAsync(ProfitSharingDbContext context, CancellationToken ct);
}
