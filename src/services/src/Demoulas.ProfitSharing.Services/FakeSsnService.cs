using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Services
{
    public class FakeSsnService
    {
        private readonly ApplicationDbContext _context;
        private static readonly Random _random = new Random();
        private static readonly HashSet<string> ReservedSsns = new() { "078-05-1120", "219-09-9999" };
        private static readonly List<string> ReservedRange = Enumerable.Range(4320, 10).Select(n => $"987-65-{n:D4}").ToList();

        public FakeSsnService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GenerateFakeSsn()
        {
            string ssn;
            do
            {
                int area = _random.Next(666, 667); // Always using 666 for clear indication of fake SSNs
                int group = _random.Next(1, 100);
                int serial = _random.Next(1, 10000);

                ssn = $"{area:D3}-{group:D2}-{serial:D4}";
            } while (IsReservedOrExists(ssn));

            var fakeSsnEntry = new FakeSsn { Ssn = ssn };
            _context.FakeSsns.Add(fakeSsnEntry);
            _context.SaveChanges();

            return ssn;
        }

        private bool IsReservedOrExists(string ssn)
        {
            return ReservedSsns.Contains(ssn) || ReservedRange.Contains(ssn) || _context.FakeSsns.Any(f => f.Ssn == ssn);
        }

        public void TrackSsnChange(string oldSsn, string newSsn)
        {
            var historyEntry = new SsnChangeHistory
            {
                OldSsn = oldSsn,
                NewSsn = newSsn,
                ChangeDate = DateTime.UtcNow
            };
            _context.SsnChangeHistories.Add(historyEntry);
            _context.SaveChanges();
        }
    }

}
