using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services
{
    public sealed class FakeSsnService : IFakeSsnService
    {
        private readonly IProfitSharingDataContextFactory _dataContextFactory;
        private static readonly Random _random = new Random();
        private static readonly HashSet<int> _reservedSsns = [78051120, 219099999];

        private static readonly List<int> _reservedRange =
            Enumerable.Range(4320, 10).Select(n => int.Parse($"98765{n:D4}")).ToList();

        public FakeSsnService(IProfitSharingDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        /// <summary>
        /// Generates a fake Social Security Number (SSN) and stores it in the database.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
        /// with a result of the generated fake SSN as an <see cref="int"/>.
        /// </returns>
        public Task<int> GenerateFakeSsnAsync(CancellationToken cancellationToken)
        {
            return _dataContextFactory.UseWritableContext(async c =>
            {
                int ssn;
                do
                {
                    int area = 666; // Always using 666 for clear indication of fake SSNs
                    int group = _random.Next(1, 100);
                    int serial = _random.Next(1, 10000);

                    ssn = int.Parse($"{area:D3}{group:D2}{serial:D4}");
                } while (await IsReservedOrExists(ssn, c, cancellationToken));


                var fakeSsnEntry = new FakeSsn { Ssn = ssn };
                c.FakeSsns.Add(fakeSsnEntry);
                await c.SaveChangesAsync(cancellationToken);

                return ssn;
            }, cancellationToken);
        }

        private static async Task<bool> IsReservedOrExists(int ssn, IProfitSharingDbContext context, CancellationToken cancellationToken)
        {
            return _reservedSsns.Contains(ssn) || _reservedRange.Contains(ssn) || await context.FakeSsns.AnyAsync(f => f.Ssn == ssn, cancellationToken);
        }

        /// <summary>
        /// Tracks changes to an SSN (Social Security Number) by creating a history entry in the database.
        /// </summary>
        /// <typeparam name="THistory">
        /// The type of the history entry to be created. Must inherit from <see cref="SsnChangeHistory"/> and have a parameterless constructor.
        /// </typeparam>
        /// <param name="oldSsn">The original SSN before the change.</param>
        /// <param name="newSsn">The new SSN after the change.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task TrackSsnChangeAsync<THistory>(int oldSsn, int newSsn, CancellationToken cancellationToken) where THistory : SsnChangeHistory, new()
        {
            return _dataContextFactory.UseWritableContext(async c =>
            {
                var historyEntry = new THistory
                {
                    OldSsn = oldSsn, NewSsn = newSsn, ChangeDateUtc = DateTimeOffset.UtcNow
                };
                c.Entry(historyEntry);
                await c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
        }
    }
}
