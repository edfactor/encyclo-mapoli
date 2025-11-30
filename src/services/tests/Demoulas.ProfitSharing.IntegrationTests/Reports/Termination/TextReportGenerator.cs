namespace Demoulas.ProfitSharing.IntegrationTests.Reports.Termination;

/// <summary>
///     Produces a paper report identical to the QPAY066.pco program.
///     This class is only used in testing.
/// </summary>
public class TextReportGenerator
{
    private const int LinesOnPage = 52;
    private readonly string _effectiveRunDateFormatted;
    private readonly string _fiscalYearEndDateFormatted;
    private readonly string _fiscalYearStartDateFormatted;
    private readonly string _profitShareYearFormatted;
    private readonly StringWriter _reportWriter = new();
    private int _lineCounter;
    private int _pageCounter = 1;

    public TextReportGenerator(DateOnly effectiveRunDate, DateOnly startDate, DateOnly endDate, decimal profitSharingYearWithIteration)
    {
        _effectiveRunDateFormatted = effectiveRunDate.ToString("MMM dd, yyyy").ToUpper();
        _fiscalYearStartDateFormatted = startDate.ToString("MM/dd/yyyy");
        _fiscalYearEndDateFormatted = endDate.ToString("MM/dd/yyyy");
        _profitShareYearFormatted = profitSharingYearWithIteration.ToString("0000.0");
    }

    public void PrintReportHeader()
    {
        _reportWriter.WriteLine("DJDE JDE=LANIQS,JDL=DFLT4,END,;");
    }

    public void PrintPageHeader()
    {
        _reportWriter.WriteLine("DON MULLIGAN");
        _reportWriter.WriteLine("{0,-39}{1,24} {2}  {3}   {4}                         {5}   {6}", "QPAY066    TERMINATION - PROFIT SHARING", "DATE", _effectiveRunDateFormatted,
            "YEAR:", _profitShareYearFormatted, "PAGE:", $"{_pageCounter:000000}");
        _reportWriter.WriteLine($"    {"FROM",11} {_fiscalYearStartDateFormatted} TO {_fiscalYearEndDateFormatted}");
        _reportWriter.WriteLine("");
        _reportWriter.WriteLine(
            @"                                  BEGINNING  BENEFICIARY   DISTRIBUTION                 ENDING       VESTED    DATE      YTD VST     E
BADGE/PSN # EMPLOYEE NAME           BALANCE  ALLOCATION       AMOUNT       FORFEIT      BALANCE      BALANCE   TERM   PS HRS PCT AGE C");
        _reportWriter.WriteLine();
    }

    public void PrintDetails(long r2BadgePsnNp, string? r2EmployeeName, decimal r2PsAmt, decimal r2BenAlloc, decimal r2PsLoan,
        decimal r2PsForf, decimal r2PsDol, decimal r2Vest, DateOnly? wsDoTerm, decimal r2PsHrs,
        decimal wVestPert, int? age, byte? wEnrolled)
    {
        if (_lineCounter % LinesOnPage == 0)
        {
            PrintHeader();
        }

        if (wEnrolled == 2 || wEnrolled == 0)
        {
            wEnrolled = null;
        }

        _lineCounter++;
        string termDate = wsDoTerm == null || wsDoTerm == DateOnly.MinValue
            ? "".PadRight(6)
            : $"{wsDoTerm.Value.Year - 2000:00}{wsDoTerm.Value.Month:00}{wsDoTerm.Value.Day:00}";
        string ageStr = age.HasValue ? $"{age:00}" : "";
        string wVestPertStr = wVestPert == 100 ? "100" : $"{wVestPert:00}";

        string? name = r2EmployeeName?.PadRight(19).Substring(0, 19);

        _reportWriter.WriteLine(r2BadgePsnNp.ToString().PadLeft(11) + " " + // fmt
                                name + " " +
                                FormatWithSingleComma(r2PsAmt).PadLeft(12) + " " +
                                FormatWithSingleComma(r2BenAlloc).PadLeft(12) + " " +
                                FormatWithSingleComma(r2PsLoan).PadLeft(12) + " " +
                                FormatWithSingleComma(r2PsForf).PadLeft(12) + " " +
                                FormatWithSingleComma(r2PsDol).PadLeft(12) + " " +
                                FormatWithSingleComma(r2Vest).PadLeft(12) + " " +
                                termDate + " " +
                                $"{r2PsHrs:0.00}".PadLeft(7) + " " +
                                wVestPertStr.PadLeft(3) + " " +
                                ageStr.PadLeft(3) +
                                (wEnrolled == null ? "" : " " + wEnrolled));
    }

    // In order to print nicely, values over 1 million do not have a second comma.
    //   1,000,000 <-- bad
    //    1000,000 <--good
    private static string FormatWithSingleComma(decimal number)
    {
        string numberStr = number.ToString("#,##0.00 ;#,##0.00-");

        string[] parts = numberStr.Split(',');
        Console.WriteLine(parts);
        if (parts.Length == 3)
        {
            return $"{parts[0]}{parts[1]},{parts[2]}";
        }

        return numberStr;
    }


    public void PrintTotals(decimal totalProfitSharing, decimal totalVesting, decimal totalForfeitures, decimal totalBeneficiaryAllocations)
    {
        _reportWriter.WriteLine("");
        _reportWriter.WriteLine("");
        _reportWriter.WriteLine("TOTALS".PadRight(6));
        _reportWriter.WriteLine(("AMOUNT IN PROFIT SHARING".PadRight(34) + totalProfitSharing.ToString("#,##0.00 ;#,##0.00-").PadLeft(14)).Trim());
        _reportWriter.WriteLine(("VESTED AMOUNT".PadRight(34) + totalVesting.ToString(" #,##0.00 ;#,##0.00-").PadLeft(14)).Trim());
        _reportWriter.WriteLine(("TOTAL FORFEITURES".PadRight(34) + totalForfeitures.ToString(" #,##0.00 ;#,##0.00-").PadLeft(14)).Trim());
        // holding on to the misspelling of ALLOCTIONS to match the Ready system.
        _reportWriter.WriteLine(("TOTAL BENEFICIARY ALLOCTIONS".PadRight(34) + totalBeneficiaryAllocations.ToString("#,##0.00 ;#,##0.00-").PadLeft(14)).Trim());
        _reportWriter.WriteLine("\n\f");
    }

    public string GetReport()
    {
        return _reportWriter.ToString();
    }

    private void PrintHeader()
    {
        if (_lineCounter != 0)
        {
            _reportWriter.WriteLine("\n\f");
        }
        else
        {
            PrintReportHeader();
        }

        PrintPageHeader();
        _pageCounter++;
    }
}
