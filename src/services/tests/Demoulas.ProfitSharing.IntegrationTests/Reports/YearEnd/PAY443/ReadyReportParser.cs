using System.Text.RegularExpressions;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY443;

// Processes the PAY443 report from READY and returns it as a ForfeituresAndPointsForYearResponseWithTotals response.
public static class ReadyReportParser
{
    public static ForfeituresAndPointsForYearResponseWithTotals ParseReport(string expectedReport)
    {
        const short currentYear = 2024;

        List<string> lines = expectedReport.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).ToList();

        List<ForfeituresAndPointsForYearResponse> expectedMembers = [];

        decimal totalForfeitures = 0;
        int totalEarningPoints = 0;
        int totalContForfeitPoints = 0;

        foreach (string line in lines)
        {
            // Grab lines starting with a badge number, ignore rest
            if (!Regex.IsMatch(line, @"^ \d{7} "))
            {
                if (line.Contains(" T O T A L S"))
                {
                    string[] totalParts = line.Substring("                                     T O T A L S  ".Length)
                        .Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    totalForfeitures = ExtractMoney(totalParts[0]);
                    totalContForfeitPoints = ExtractInt(totalParts[1]);
                    totalEarningPoints = ExtractInt(totalParts[2]);
                }

                continue;
            }

            ForfeituresAndPointsForYearResponse record = ParseLine(line);
            expectedMembers.Add(record);
        }

        totalForfeitures.ShouldBeGreaterThan(0); // indicates parsing failure

        string lastLine = lines[^1];
        string[] verifyParts = lastLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        decimal totalProfitSharingBalanceLastYear = ExtractMoney(verifyParts[0]);
        decimal distributionTotals = ExtractMoney(verifyParts[1]);
        decimal allocationsTo = ExtractMoney(verifyParts[2]);
        decimal allocationsFrom = ExtractMoney(verifyParts[3]);

        PaginatedResponseDto<ForfeituresAndPointsForYearResponse> prd = new() { Total = expectedMembers.Count, Results = expectedMembers };


        return new ForfeituresAndPointsForYearResponseWithTotals()
        {
            TotalForfeitures = totalForfeitures,
            TotalEarningPoints = totalEarningPoints,
            TotalForfeitPoints = totalContForfeitPoints,
            TotalProfitSharingBalance = totalProfitSharingBalanceLastYear,
            DistributionTotals = distributionTotals,
            AllocationToTotals = allocationsTo,
            AllocationsFromTotals = allocationsFrom,
            ReportDate = DateTimeOffset.UtcNow,
            ReportName = $"PROFIT SHARING FORFEITURES AND POINTS FOR {currentYear}",
            StartDate = new DateOnly(currentYear, 1, 1), // transactions are bound to the year
            EndDate = new DateOnly(currentYear, 12, 31),
            Response = prd
        };
    }

    private static int ExtractInt(string intPart)
    {
        return int.Parse(intPart.Replace(",", ""));
    }

    private static decimal ExtractMoney(string moneyPart)
    {
        return decimal.Parse(moneyPart.Trim().Replace(",", ""));
    }

    private static ForfeituresAndPointsForYearResponse ParseLine(string line)
    {
        string badgeStr = line.Substring(0, 8).Trim();
        string name = line.Substring(10, 26).Trim();
        string ssn = line.Substring(37, 11).Trim().Replace("000-00-", "XXX-XX-");
        string forfeiture = line.Substring(51, 13).Trim();
        if (line.Length >= 65 && line[64] == '-')
        {
            forfeiture = "-" + forfeiture;
        }

        string contPts = line.Length > 67 ? line.Substring(67, 12).Replace(",", "").Trim() : "";
        string earnPts = line.Length > 79 ? line.Substring(80, 12).Replace(",", "").Trim() : "";
        string? benePsn = line.Length > 92 ? line.Substring(92).Trim() : null;

        return new ForfeituresAndPointsForYearResponse
        {
            BadgeNumber = int.TryParse(badgeStr, out int badge) ? badge : 0,
            EmployeeName = name,
            Ssn = ssn,
            Forfeitures = decimal.TryParse(forfeiture.Replace(",", ""), out decimal f) ? f : null,
            EarningPoints = int.TryParse(earnPts, out int e) ? e : 0,
            ContForfeitPoints = short.TryParse(contPts, out short c) ? c : (short)0,
            BeneficiaryPsn = benePsn,
            IsExecutive = false,
        };
    }
}
