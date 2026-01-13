namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request DTO for Duplicate Names and Birthdays report.
/// Extends ProfitYearRequest to provide profit year and pagination parameters.
/// </summary>
public sealed record DuplicateNamesAndBirthdaysRequest : ProfitYearRequest
{
    /// <inheritdoc />
    public override string ToString() => base.ToString()!;

    public static new DuplicateNamesAndBirthdaysRequest RequestExample()
    {
        return new DuplicateNamesAndBirthdaysRequest
        {
            ProfitYear = 2024
        };
    }
}
