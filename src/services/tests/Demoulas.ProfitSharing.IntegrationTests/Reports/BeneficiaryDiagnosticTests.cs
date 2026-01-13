using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports;

public class BeneficiaryDiagnosticTests : PristineBaseTest
{
    public BeneficiaryDiagnosticTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public Task DiagnoseBeneficiaryData()
    {
        return DbFactory.UseWritableContext(async ctx =>
        {
            // Get all beneficiaries
            var allBeneficiaries = await ctx.Beneficiaries
                .Include(b => b.Contact)
                .ThenInclude(c => c!.ContactInfo)
                .Include(b => b.Demographic)
                .ToListAsync();

            TestOutputHelper.WriteLine($"\n=== BENEFICIARY TABLE DIAGNOSTICS ===");
            TestOutputHelper.WriteLine($"Total beneficiaries in table: {allBeneficiaries.Count}");

            // Group by PSN suffix
            var bySuffix = allBeneficiaries
                .GroupBy(b => b.PsnSuffix)
                .OrderBy(g => g.Key)
                .ToList();

            TestOutputHelper.WriteLine($"\nBeneficiaries by PSN Suffix:");
            foreach (var group in bySuffix)
            {
                TestOutputHelper.WriteLine($"  PSN Suffix {group.Key}: {group.Count()} beneficiaries");
            }

            // Show beneficiaries with -1000 suffix
            var beneficiariesWith1000 = allBeneficiaries.Where(b => b.PsnSuffix == -1000).ToList();
            TestOutputHelper.WriteLine($"\n=== BENEFICIARIES WITH -1000 SUFFIX ({beneficiariesWith1000.Count}) ===");
            foreach (var ben in beneficiariesWith1000.Take(20))
            {
                var name = ben.Contact?.ContactInfo?.FullName ?? "N/A";
                var ssn = ben.Contact != null ? ben.Contact.Ssn.ToString() : "N/A";
                var badge = ben.BadgeNumber;
                var demoMatch = ben.Demographic != null ? "YES" : "NO";
                var demoStatus = ben.Demographic?.EmploymentStatusId.ToString() ?? "N/A";
                var demoTermDate = ben.Demographic?.TerminationDate?.ToString("yyyy-MM-dd") ?? "N/A";

                TestOutputHelper.WriteLine($"  Badge: {badge}, PSN: {ben.PsnSuffix}, Name: {name}");
                TestOutputHelper.WriteLine($"    SSN: {ssn}, Demo Match: {demoMatch}, Status: {demoStatus}, TermDate: {demoTermDate}");
            }
            if (beneficiariesWith1000.Count > 20)
            {
                TestOutputHelper.WriteLine($"  ... and {beneficiariesWith1000.Count - 20} more");
            }

            // Check for specific beneficiaries from READY report
            var expectedMissingBadges = new[] {
                7039171000, // ALLEN, RAYMOND (from READY line 18)
                7026461000, // AVILA, SAVANNAH (from READY line 29)
                7024451000, // BARTLETT, ATLAS
                7052601000, // BERNAL, BROOKLYN
                7006561000  // BERNARD, DARCY
            };

            TestOutputHelper.WriteLine($"\n=== CHECKING FOR EXPECTED MISSING BENEFICIARIES ===");
            foreach (var badgeWithSuffix in expectedMissingBadges)
            {
                var badge = (int)(badgeWithSuffix / 10000); // Extract badge number
                var psn = (short)(badgeWithSuffix % 10000 * -1); // Extract PSN as negative

                var found = allBeneficiaries.FirstOrDefault(b =>
                    b.BadgeNumber == badge && Math.Abs(b.PsnSuffix) == Math.Abs(psn));

                if (found != null)
                {
                    var name = found.Contact?.ContactInfo?.FullName ?? "N/A";
                    TestOutputHelper.WriteLine($"  ✓ FOUND: Badge {badge}, PSN {psn} ({name})");
                }
                else
                {
                    TestOutputHelper.WriteLine($"  ✗ MISSING: Badge {badge}, PSN {psn}");
                }
            }

            // Check beneficiaries who match terminated employees in date range
            var startDate = new DateOnly(2025, 01, 4);
            var endDate = new DateOnly(2025, 12, 27);

            var beneficiariesMatchingTerminatedInRange = allBeneficiaries
                .Where(b => b.Contact != null && b.Demographic != null &&
                           b.Contact.Ssn == b.Demographic.Ssn &&
                           b.Demographic.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                           b.Demographic.TerminationDate != null &&
                           b.Demographic.TerminationDate >= startDate &&
                           b.Demographic.TerminationDate <= endDate)
                .ToList();

            TestOutputHelper.WriteLine($"\n=== BENEFICIARIES THAT SHOULD BE FILTERED OUT (Match Terminated in Range) ===");
            TestOutputHelper.WriteLine($"Count: {beneficiariesMatchingTerminatedInRange.Count}");
            foreach (var ben in beneficiariesMatchingTerminatedInRange.Take(10))
            {
                var name = ben.Contact?.ContactInfo?.FullName ?? "N/A";
                TestOutputHelper.WriteLine($"  Badge: {ben.BadgeNumber}, PSN: {ben.PsnSuffix}, Name: {name}, TermDate: {ben.Demographic?.TerminationDate:yyyy-MM-dd}");
            }

            // Check beneficiaries who DON'T match the filter (should be included)
            var beneficiariesShouldBeIncluded = allBeneficiaries
                .Where(b => !(b.Contact != null && b.Demographic != null &&
                             b.Contact.Ssn == b.Demographic.Ssn &&
                             b.Demographic.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                             b.Demographic.TerminationDate != null &&
                             b.Demographic.TerminationDate >= startDate &&
                             b.Demographic.TerminationDate <= endDate))
                .ToList();

            TestOutputHelper.WriteLine($"\n=== BENEFICIARIES THAT SHOULD BE INCLUDED (After Filter) ===");
            TestOutputHelper.WriteLine($"Count: {beneficiariesShouldBeIncluded.Count}");

            var includedBy1000 = beneficiariesShouldBeIncluded.Count(b => b.PsnSuffix == -1000);
            TestOutputHelper.WriteLine($"  With -1000 suffix: {includedBy1000}");

            // Check if there are beneficiaries with transactions
            var beneficiarySSNs = allBeneficiaries.Select(b => b.Contact?.Ssn).Where(s => s != null).Distinct().ToHashSet();

            var beneficiariesWithCode5or6 = await ctx.ProfitDetails
                .Where(pd => pd.ProfitYear == 2025 &&
                            (pd.ProfitCodeId == 5 || pd.ProfitCodeId == 6) &&
                            beneficiarySSNs.Contains(pd.Ssn))
                .GroupBy(pd => pd.Ssn)
                .Select(g => new
                {
                    Ssn = g.Key,
                    Code5Count = g.Count(x => x.ProfitCodeId == 5),
                    Code6Count = g.Count(x => x.ProfitCodeId == 6),
                    Code5Total = g.Where(x => x.ProfitCodeId == 5).Sum(x => x.Forfeiture),
                    Code6Total = g.Where(x => x.ProfitCodeId == 6).Sum(x => x.Contribution)
                })
                .ToListAsync();

            TestOutputHelper.WriteLine($"\n=== BENEFICIARIES WITH CODE 5/6 TRANSACTIONS IN 2025 ===");
            TestOutputHelper.WriteLine($"Beneficiaries with Code 5 or 6 transactions: {beneficiariesWithCode5or6.Count}");
            foreach (var item in beneficiariesWithCode5or6.Take(10))
            {
                var ben = allBeneficiaries.FirstOrDefault(b => b.Contact?.Ssn == item.Ssn);
                var name = ben?.Contact?.ContactInfo?.FullName ?? "N/A";
                TestOutputHelper.WriteLine($"  SSN: {item.Ssn}, Name: {name}");
                TestOutputHelper.WriteLine($"    Code 5 (Outgoing): {item.Code5Count} txns, Total: -${item.Code5Total:N2}");
                TestOutputHelper.WriteLine($"    Code 6 (Incoming): {item.Code6Count} txns, Total: +${item.Code6Total:N2}");
                TestOutputHelper.WriteLine($"    Net Beneficiary Allocation: ${(item.Code6Total - item.Code5Total):N2}");
            }
        });
    }
}
