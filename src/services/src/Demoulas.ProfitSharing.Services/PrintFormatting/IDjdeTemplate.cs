namespace Demoulas.ProfitSharing.Services.PrintFormatting;

/// <summary>
/// Template for generating Xerox DJDE (Dynamic Job Descriptor Entry) directives.
/// </summary>
public interface IDjdeTemplate
{
    /// <summary>
    /// Generates DJDE directives for Xerox printer from check data.
    /// </summary>
    /// <param name="checkData">Check data to format for printing.</param>
    /// <returns>DJDE directive string ready for printer spool.</returns>
    string GenerateDirectives(CheckData checkData);
}
