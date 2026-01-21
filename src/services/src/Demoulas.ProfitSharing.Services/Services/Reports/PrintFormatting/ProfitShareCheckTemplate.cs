using System.Text;
using Demoulas.ProfitSharing.Common.Extensions;

namespace Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;

/// <summary>
/// DJDE template for Demoulas profit sharing checks on Xerox 4635 printer.
/// Based on PROFCHKS.cbl COBOL program DJDE pattern.
/// </summary>
public sealed class ProfitShareCheckTemplate : IDjdeTemplate
{
    private readonly IMicrFormatter _micrFormatter;

    public ProfitShareCheckTemplate(IMicrFormatter micrFormatter)
    {
        _micrFormatter = micrFormatter;
    }

    /// <summary>
    /// Generates DJDE directives matching PROFCHKS.cbl format:
    /// - Main directive: DJDE JDE=PSCK,JDL=PSCK,SHIFT=NO,;
    /// - Header directive: RNA/DPT/LDT/RSQ/DDE/DAR/RRA/NSE/PRD parameters
    /// - Page directive: SEQ/PRA/TXT parameters
    /// </summary>
    public string GenerateDirectives(CheckData checkData)
    {
        var sb = new StringBuilder();

        // Main DJDE directive - identifies job and library
        sb.AppendLine("DJDE JDE=PSCK,JDL=PSCK,SHIFT=NO,;");

        // Header directive - report metadata
        sb.AppendLine($"DJDE RNA=PROFCKS,DPT=PAYROLL,LDT={checkData.IssueDate:MM/dd/yyyy},RSQ=Y,DDE=Y,DAR=DET-BOTH,RRA,NSE,PRD;");

        // Page directive - sequencing and text layout
        sb.AppendLine("DJDE SEQ=1,PRA,TXT;");

        // Check content with MICR line
        var micrLine = _micrFormatter.FormatMicrLine(checkData.CheckNumber, checkData.Amount);
        var maskedSsn = checkData.Ssn.MaskSsn();

        sb.AppendLine($"Check #{checkData.CheckNumber}");
        sb.AppendLine($"Date: {checkData.IssueDate:MM/dd/yyyy}");
        sb.AppendLine($"Pay to: {checkData.RecipientName}");
        sb.AppendLine($"Amount: ${checkData.Amount:N2}");
        sb.AppendLine($"Badge: {checkData.BadgeNumber}");
        sb.AppendLine($"SSN: {maskedSsn}");
        sb.AppendLine();
        sb.AppendLine(micrLine);

        return sb.ToString();
    }
}
