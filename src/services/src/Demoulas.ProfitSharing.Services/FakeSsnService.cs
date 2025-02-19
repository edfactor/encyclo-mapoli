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
        private static readonly HashSet<int> _reservedSsns = new() { 78051120, 219099999 };

        private static readonly List<int> _reservedRange =
            Enumerable.Range(4320, 10).Select(n => int.Parse($"98765{n:D4}")).ToList();

        public FakeSsnService(IProfitSharingDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

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

        public Task TrackSsnChangeAsync<THistory>(int oldSsn, int newSsn, CancellationToken cancellationToken) where THistory : SsnChangeHistory, new()
        {
            return _dataContextFactory.UseWritableContext(async c =>
            {
                var historyEntry = new THistory
                {
                    OldSsn = oldSsn, NewSsn = newSsn, ChangeDateUtc = DateTimeOffset.UtcNow
                };
                c.Entry<THistory>(historyEntry);
                await c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
        }
    }
}
