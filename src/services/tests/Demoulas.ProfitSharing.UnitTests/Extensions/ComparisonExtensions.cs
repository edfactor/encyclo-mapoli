using System.Collections;
using FluentAssertions;

namespace Demoulas.ProfitSharing.IntegrationTests.Extensions;

public static class ComparisonExtensions
{
    public static void ShouldBeEquivalentTo<T, TU>(this T entity, TU dto)
    {
        var entityProperties = typeof(T).GetProperties();
        var dtoProperties = typeof(TU).GetProperties();

        foreach (var entityProperty in entityProperties)
        {
            // Skip navigation properties
            if (IsNavigationProperty(entityProperty.PropertyType))
            {
                continue;
            }

            var dtoProperty = dtoProperties.FirstOrDefault(p => p.Name == entityProperty.Name);

            if (dtoProperty == null)
            {
                // skip properties that don't match They will need to be checked manually.
                continue;
            }

            if (IsNavigationProperty(dtoProperty.PropertyType))
            {
                continue;
            }

            var entityValue = entityProperty.GetValue(entity);
            var dtoValue = dtoProperty.GetValue(dto);

            dtoValue.Should().Be(entityValue, because: $"{dtoProperty.Name} property should be equivalent.");
        }
    }

    private static bool IsNavigationProperty(Type propertyType)
    {
        // Consider properties that are classes (except string) or implement ICollection/IEnumerable as navigation properties
        return (propertyType.IsClass && propertyType != typeof(string)) ||
               (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string));
    }
}
