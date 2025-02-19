using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services
{
    public class FakeSsnService<THistory> where THistory : SsnChangeHistory, new()
    {
        private readonly IProfitSharingDataContextFactory _dataContextFactory;
        private static readonly Random _random = new Random();
        private static readonly HashSet<int> ReservedSsns = new() { 78051120, 219099999 };

        private static readonly List<int> ReservedRange =
            Enumerable.Range(4320, 10).Select(n => int.Parse($"98765{n:D4}")).ToList();

        public FakeSsnService(IProfitSharingDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task<int> GenerateFakeSsnAsync(CancellationToken cancellationToken)
        {
            await _dataContextFactory.UseWritableContext(c =>
            {
                int ssn;
                do
                {
                    int area = 666; // Always using 666 for clear indication of fake SSNs
                    int group = _random.Next(1, 100);
                    int serial = _random.Next(1, 10000);

                    ssn = int.Parse($"{area:D3}{group:D2}{serial:D4}");
                } while (IsReservedOrExists(ssn, c));


                var fakeSsnEntry = new THistory { Ssn = ssn };
                c.FakeSsns.Add(fakeSsnEntry);
                return c.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
            return ssn;
        }

        private bool IsReservedOrExists(int ssn, IProfitSharingDbContext context)
        {
            return ReservedSsns.Contains(ssn) || ReservedRange.Contains(ssn) || context.FakeSsns.Any(f => f.Ssn == ssn);
        }

        public Task TrackSsnChangeAsync(int oldSsn, int newSsn, CancellationToken cancellationToken)
        {
            _dataContextFactory.UseWritableContext(c =>
            {
                var historyEntry = new SsnChangeHistory
                {
                    OldSsn = oldSsn, NewSsn = newSsn, ChangeDateUtc = DateTimeOffset.UtcNow
                };
                c.SsnChangeHistories.Add(historyEntry);
                c.SaveChanges();
            }, cancellationToken);
        }
    }
}
