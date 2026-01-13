namespace Demoulas.ProfitSharing.Common.Contracts.Request.CheckRun;

/// <summary>
/// Determines which printer/output format to generate for check run printing.
/// </summary>
public enum CheckRunPrinterType
{
    /// <summary>
    /// Xerox 4635-compatible DJDE output.
    /// </summary>
    XeroxDjde = 1,

    /// <summary>
    /// Standard (non-DJDE) printer output.
    /// </summary>
    Standard = 2
}
