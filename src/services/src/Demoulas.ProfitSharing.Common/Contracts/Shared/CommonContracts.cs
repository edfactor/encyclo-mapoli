using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Shared;

// Granular interfaces so DTOs can opt-in without altering existing property names/shapes.
// Note: FirstName/LastName modeled as non-nullable to satisfy DTOs that mark them 'required'.
// DTOs that truly allow null can still implement by permitting null assignment (compiler will warn if misused).
public interface IFirstName { [MaskSensitive] string FirstName { get; } }
public interface ILastName { [MaskSensitive] string LastName { get; } }
public interface IMiddleName { [MaskSensitive] string? MiddleName { get; } }
public interface IFullNameProperty { [MaskSensitive] string? FullName { get; } }
public interface IPhoneNumber { [MaskSensitive] string? PhoneNumber { get; } }
public interface IEmailAddress { [MaskSensitive] string? EmailAddress { get; } }
public interface ICity { [MaskSensitive] string? City { get; } }

// Grouping marker
public interface INameParts : IFirstName, ILastName, IMiddleName;

public static class DtoCommonExtensions
{
    public static string ComputeFullName(this INameParts parts, bool lastNameFirst = true)
    {
        string firstBlock = string.Join(" ", new[] { parts.FirstName, parts.MiddleName }
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
}
