using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Data.Contexts;
using System.Data;

namespace Demoulas.ProfitSharing.Services
{
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
        public Task<int> GenerateFakeSsnAsync(CancellationToken cancellationToken)
        {
            return _dataContextFactory.UseWritableContext(async c =>
            {
                // Get the next SSN value from the database sequence.
                int ssn = await GetNextSequenceSsnAsync(c, cancellationToken);

                var fakeSsnEntry = new FakeSsn { Ssn = ssn };
                c.FakeSsns.Add(fakeSsnEntry);

                await c.SaveChangesAsync(cancellationToken);
                return ssn;
            }, cancellationToken);
        }

        /// <summary>
        /// Retrieves the next available SSN value using a database sequence.
        /// A migration must create the sequence “FakeSsnSeq” (e.g., via migrationBuilder.Sql("CREATE SEQUENCE FakeSsnSeq AS INT START WITH 666000001 INCREMENT BY 1;")).
        /// </summary>
        private static async Task<int> GetNextSequenceSsnAsync(ProfitSharingDbContext context,
            CancellationToken cancellationToken)
        {
            await using var command = context.Database.GetDbConnection().CreateCommand();
            // Use Oracle syntax to retrieve the next value.
            command.CommandText = "SELECT FAKE_SSN_SEQ.NEXTVAL FROM dual";
            if (command.Connection!.State != ConnectionState.Open)
            {
                await command.Connection.OpenAsync(cancellationToken);
            }
            object? result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result);
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
                    ChangeDateUtc = DateTimeOffset.UtcNow
                };
                c.Entry(historyEntry);
                await c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
        }
    }
}
