using System.Data;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public sealed class FakeSsnService : IFakeSsnService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public FakeSsnService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    /// <summary>
    /// Generates a fake Social Security Number (SSN) using a database sequence and stores it in the database.
    /// Assumes a migration has created the sequence “FakeSsnSeq” starting at 666000001.
    /// </summary>
    public async Task<int> GenerateFakeSsnAsync(CancellationToken cancellationToken)
    {
        // Get the next SSN value from the database sequence.
        var ssns = await GenerateFakeSsnBatchAsync(1, cancellationToken);
        return ssns.FirstOrDefault(defaultValue: 0);
    }

    /// <summary>
    /// Generates multiple fake Social Security Numbers (SSNs) in a single database call using a sequence.
    /// Returns the generated SSNs in ascending order.
    /// </summary>
    /// <param name="batchSize">Number of SSNs to generate (max 255)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of generated SSN values</returns>
    public Task<int[]> GenerateFakeSsnBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseWritableContext(async c =>
        {
            // Get multiple SSN values in one database call
            int[] ssns = await GetNextSequenceSsnBatchAsync(c, batchSize, cancellationToken);

            // Create all FakeSsn entries at once
            var fakeSsnEntries = ssns.Select(ssn => new FakeSsn { Ssn = ssn }).ToArray();
#pragma warning disable VSTHRD103
            c.FakeSsns.AddRange(fakeSsnEntries);
#pragma warning restore VSTHRD103

            await c.SaveChangesAsync(cancellationToken);
            return ssns;
        }, cancellationToken);
    }

    /// <summary>
    /// Retrieves multiple SSN values from the sequence in a single database call.
    /// Uses Oracle's CONNECT BY to generate the desired number of rows.
    /// </summary>
    private static async Task<int[]> GetNextSequenceSsnBatchAsync(ProfitSharingDbContext context,
        int batchSize, CancellationToken cancellationToken)
    {

        if (batchSize == 0)
        {
            return [];
        }

        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = @"SELECT FAKE_SSN_SEQ.NEXTVAL 
        FROM dual 
        CONNECT BY LEVEL <= :batchSize";

        var parameter = command.CreateParameter();
        parameter.ParameterName = ":batchSize";
        parameter.Value = batchSize;
        command.Parameters.Add(parameter);

        if (command.Connection!.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync(cancellationToken);
        }

        var results = new List<int>(batchSize);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(reader.GetInt32(0));
        }

        return results.ToArray();
    }


    /// <summary>
    /// Tracks changes to an SSN by creating a history entry in the database.
    /// </summary>
    public Task TrackSsnChangeAsync<THistory>(int oldSsn, int newSsn, CancellationToken cancellationToken) where THistory : SsnChangeHistory, new()
    {
        return _dataContextFactory.UseWritableContext(async c =>
        {
            var historyEntry = new THistory
            {
                OldSsn = oldSsn,
                NewSsn = newSsn,
                ModifiedAtUtc = DateTimeOffset.UtcNow
            };
            c.Entry(historyEntry);
            await c.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
