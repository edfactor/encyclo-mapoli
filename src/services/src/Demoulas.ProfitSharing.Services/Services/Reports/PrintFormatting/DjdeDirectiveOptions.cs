namespace Demoulas.ProfitSharing.Services.PrintFormatting;

/// <summary>
/// Configurable Xerox DJDE directives used for report and certificate print output.
/// Defaults match existing production directives but can be overridden via configuration.
/// </summary>
public sealed class DjdeDirectiveOptions
{
    /// <summary>
    /// Configuration section name for DJDE directives.
    /// </summary>
    public const string SectionName = "DjdeDirectives";

    /// <summary>
    /// DJDE header used for certificate print files.
    /// </summary>
    public string CertificateHeader { get; init; } = "\fDJDE JDE=PROFNEW,JDL=DFLT5,END,;\r";

    /// <summary>
    /// DJDE header used for terminated employee letters.
    /// </summary>
    public string TerminatedLettersHeader { get; init; } = "DJDE JDE=QPS003,JDL=PAYROL,END,;";

    /// <summary>
    /// DJDE header used for the PROF-LETTER73 report.
    /// </summary>
    public string ProfitsOver73Header { get; init; } = "DJDE JDE=QPS073,JDL=PAYROL,END,;";

    /// <summary>
    /// DJDE printer control directives appended at the end of each terminated letter.
    /// </summary>
    public string[] TerminatedLettersPrinterControls { get; init; } =
    [
        "DJDE JDE=DISNO1,JDL=PAYROL,END,;",
        "DJDE JDE=DISNO2,JDL=PAYROL,END,;",
        "DJDE JDE=DISNO3,JDL=PAYROL,END,;",
        "DJDE JDE=DISNO4,JDL=PAYROL,END,;",
        "DJDE JDE=DISNO5,JDL=PAYROL,END,;",
        "DJDE JDE=BENDS1,JDL=PAYROL,END,;",
        "DJDE JDE=ACKNRC,JDL=PAYROL,END,;"
    ];
}
