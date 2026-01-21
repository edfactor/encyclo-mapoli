namespace Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;

/// <summary>
/// Factory for building DJDE templates based on check type.
/// </summary>
public sealed class DjdeFormatBuilder
{
    private readonly IMicrFormatterFactory _micrFormatterFactory;

    public DjdeFormatBuilder(IMicrFormatterFactory micrFormatterFactory)
    {
        _micrFormatterFactory = micrFormatterFactory;
    }

    /// <summary>
    /// Creates a DJDE template for profit sharing checks using Newtek bank MICR format.
    /// </summary>
    /// <returns>Configured DJDE template for profit sharing checks.</returns>
    public IDjdeTemplate CreateProfitShareCheckTemplate()
    {
        var micrFormatter = _micrFormatterFactory.GetFormatter("026004297"); // Newtek routing
        return new ProfitShareCheckTemplate(micrFormatter);
    }
}
