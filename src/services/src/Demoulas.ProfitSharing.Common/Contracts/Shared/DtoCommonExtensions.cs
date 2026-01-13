namespace Demoulas.ProfitSharing.Common.Contracts.Shared;

// Granular interfaces so DTOs can opt-in without altering existing property names/shapes.
// Note: FirstName/LastName modeled as non-nullable to satisfy DTOs that mark them 'required'.
// DTOs that truly allow null can still implement by permitting null assignment (compiler will warn if misused).

// Grouping marker

public static class DtoCommonExtensions
{
    public static string ComputeFullName(this INameParts parts, bool lastNameFirst = true, bool middleInitialOnly = true)
    {
        var middlePart = string.IsNullOrWhiteSpace(parts.MiddleName)
            ? string.Empty
            : (middleInitialOnly ? $"{parts.MiddleName[0]}" : parts.MiddleName);

        string firstBlock = string.Join(" ", new[] { parts.FirstName, middlePart }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
        string last = parts.LastName ?? string.Empty;
        if (lastNameFirst)
        {
            return string.Join(", ", new[] { last, firstBlock }.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        return string.Join(" ", new[] { firstBlock, last }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    public static bool HasBasicName(this INameParts parts) =>
        !string.IsNullOrWhiteSpace(parts.FirstName) && !string.IsNullOrWhiteSpace(parts.LastName);

    public static bool HasContactChannel(this IEmailAddress e, IPhoneNumber p) =>
        (!string.IsNullOrWhiteSpace(e.EmailAddress)) || (!string.IsNullOrWhiteSpace(p.PhoneNumber));

    /// <summary>
    /// Computes a full name string with middle initial (if available) in the format: "LastName, FirstName M"
    /// </summary>
    public static string ComputeFullNameWithInitial(string lastName, string firstName, string? middleName)
    {
        var middleInitial = string.IsNullOrWhiteSpace(middleName) ? string.Empty : $"{middleName[0]}";
        return string.IsNullOrWhiteSpace(middleInitial)
            ? $"{lastName}, {firstName}"
            : $"{lastName}, {firstName} {middleInitial}";
    }
}
