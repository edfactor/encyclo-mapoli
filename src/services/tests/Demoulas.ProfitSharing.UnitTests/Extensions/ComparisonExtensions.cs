using System.Collections;
using System.Reflection;
using Castle.Core.Internal;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests.Extensions;

public static class ComparisonExtensions
{
    /// <summary>
    /// Asserts that the properties of the specified entity are equivalent to the properties of the specified DTO.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="Tu">The type of the DTO.</typeparam>
    /// <param name="entity">The entity to compare.</param>
    /// <param name="dto">The DTO to compare.</param>
    /// <remarks>
    /// This method compares the properties of the entity and the DTO, skipping navigation properties and properties that do not match by name or type.
    /// </remarks>
    public static void ShouldBeEquivalentTo<T, Tu>(this T entity, Tu dto)
    {
        PropertyInfo[] entityProperties = typeof(T).GetProperties();
        PropertyInfo[] dtoProperties = typeof(Tu).GetProperties();

        foreach (PropertyInfo entityProperty in entityProperties)
        {
            // Skip navigation properties
            if (IsNavigationProperty(entityProperty.PropertyType))
            {
                continue;
            }

            PropertyInfo? dtoProperty = dtoProperties.Find(p => p.Name == entityProperty.Name);

            if (dtoProperty == null)
            {
                // skip properties that don't match They will need to be checked manually.
                continue;
            }

            if (IsNavigationProperty(dtoProperty.PropertyType))
            {
                continue;
            }

            if (entityProperty.PropertyType != dtoProperty.PropertyType)
            {
                // Skip properties that don't have the same type
                continue;
            }

            object? entityValue = entityProperty.GetValue(entity);
            object? dtoValue = dtoProperty.GetValue(dto);

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
