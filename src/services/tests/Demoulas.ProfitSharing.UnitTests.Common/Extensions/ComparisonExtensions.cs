using System.Collections;
using System.Reflection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Common.Extensions;

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
        if (entity is IEnumerable entityEnumerable && dto is IEnumerable dtoEnumerable && entity is not string && dto is not string)
        {
            var entityList = entityEnumerable.Cast<object>().ToList();
            var dtoList = dtoEnumerable.Cast<object>().ToList();
            entityList.Count.ShouldBe(dtoList.Count, "Collections should have the same count");
            // Check that every item in entityList has an equivalent in dtoList, and vice versa, ignoring order
            foreach (var entityItem in entityList)
            {
                dtoList.Any(dtoItem =>
                {
                    try { entityItem.ShouldBeEquivalentTo(dtoItem); return true; } catch { return false; }
                }).ShouldBeTrue("An equivalent item was not found in the expected collection.");
            }
            foreach (var dtoItem in dtoList)
            {
                entityList.Any(entityItem =>
                {
                    try { entityItem.ShouldBeEquivalentTo(dtoItem); return true; } catch { return false; }
                }).ShouldBeTrue("An equivalent item was not found in the actual collection.");
            }
            return;
        }

        PropertyInfo[] entityProperties = entity?.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0).ToArray() ?? typeof(T).GetProperties().Where(p => p.GetIndexParameters().Length == 0).ToArray();
        PropertyInfo[] dtoProperties = dto?.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0).ToArray() ?? typeof(Tu).GetProperties().Where(p => p.GetIndexParameters().Length == 0).ToArray();

        foreach (PropertyInfo entityProperty in entityProperties)
        {
            if (IsNavigationProperty(entityProperty.PropertyType))
            {
                continue;
            }

            PropertyInfo? dtoProperty = dtoProperties.FirstOrDefault(p => p.Name == entityProperty.Name);
            if (dtoProperty == null)
            {
                continue;
            }
            if (IsNavigationProperty(dtoProperty.PropertyType))
            {
                continue;
            }
            if (entityProperty.PropertyType != dtoProperty.PropertyType)
            {
                continue;
            }

            object? entityValue = entityProperty.GetValue(entity);
            object? dtoValue = dtoProperty.GetValue(dto);

            if (entityValue is IEnumerable entityValEnum && dtoValue is IEnumerable dtoValEnum && entityValue is not string && dtoValue is not string)
            {
                var entityValList = entityValEnum.Cast<object>().ToList();
                var dtoValList = dtoValEnum.Cast<object>().ToList();
                entityValList.Count.ShouldBe(dtoValList.Count, $"{dtoProperty.Name} collection count should match");
                // Check that every item in entityValList has an equivalent in dtoValList, and vice versa, ignoring order
                foreach (var entityItem in entityValList)
                {
                    dtoValList.Any(dtoItem =>
                    {
                        try { entityItem.ShouldBeEquivalentTo(dtoItem); return true; } catch { return false; }
                    }).ShouldBeTrue("An equivalent item was not found in the expected collection.");
                }
                foreach (var dtoItem in dtoValList)
                {
                    entityValList.Any(entityItem =>
                    {
                        try { entityItem.ShouldBeEquivalentTo(dtoItem); return true; } catch { return false; }
                    }).ShouldBeTrue("An equivalent item was not found in the actual collection.");
                }
            }
            else
            {
                if (dtoValue?.GetType() == entityValue?.GetType())
                {
                    dtoValue.ShouldBe(entityValue, $"{dtoProperty.Name} property should be equivalent.");
                }
                else
                {
                    (dtoValue?.Equals(entityValue) == true).ShouldBeTrue($"{dtoProperty.Name} property should be equivalent.");
                }
            }
        }
    }

    /// <summary>
    /// Asserts that the properties of the specified entity are equivalent to the properties of the specified DTO, excluding specified properties.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="Tu">The type of the DTO.</typeparam>
    /// <param name="entity">The entity to compare.</param>
    /// <param name="dto">The DTO to compare.</param>
    /// <param name="excludedProperties">The properties to exclude from comparison.</param>
    /// <remarks>
    /// This method compares the properties of the entity and the DTO, skipping navigation properties and properties that do not match by name or type.
    /// </remarks>
    public static void ShouldBeEquivalentTo<T, Tu>(this T entity, Tu dto, params string[] excludedProperties)
    {
        if (entity is IEnumerable entityEnumerable && dto is IEnumerable dtoEnumerable && entity is not string && dto is not string)
        {
            var entityList = entityEnumerable.Cast<object>().ToList();
            var dtoList = dtoEnumerable.Cast<object>().ToList();
            entityList.Count.ShouldBe(dtoList.Count, "Collections should have the same count");
            foreach (var entityItem in entityList)
            {
                dtoList.Any(dtoItem =>
                {
                    try { entityItem.ShouldBeEquivalentTo(dtoItem, excludedProperties ?? []); return true; } catch { return false; }
                }).ShouldBeTrue("An equivalent item was not found in the expected collection.");
            }
            foreach (var dtoItem in dtoList)
            {
                entityList.Any(entityItem =>
                {
                    try { entityItem.ShouldBeEquivalentTo(dtoItem, excludedProperties ?? []); return true; } catch { return false; }
                }).ShouldBeTrue("An equivalent item was not found in the actual collection.");
            }
            return;
        }

        PropertyInfo[] entityProperties = entity?.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0).ToArray() ?? typeof(T).GetProperties().Where(p => p.GetIndexParameters().Length == 0).ToArray();
        PropertyInfo[] dtoProperties = dto?.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0).ToArray() ?? typeof(Tu).GetProperties().Where(p => p.GetIndexParameters().Length == 0).ToArray();

        foreach (PropertyInfo entityProperty in entityProperties)
        {
            if (excludedProperties != null && excludedProperties.Contains(entityProperty.Name))
            {
                continue;
            }
            if (IsNavigationProperty(entityProperty.PropertyType))
            {
                continue;
            }

            PropertyInfo? dtoProperty = dtoProperties.FirstOrDefault(p => p.Name == entityProperty.Name);
            if (dtoProperty == null)
            {
                continue;
            }
            if (IsNavigationProperty(dtoProperty.PropertyType))
            {
                continue;
            }
            if (entityProperty.PropertyType != dtoProperty.PropertyType)
            {
                continue;
            }

            object? entityValue = entityProperty.GetValue(entity);
            object? dtoValue = dtoProperty.GetValue(dto);

            if (entityValue is IEnumerable entityValEnum && dtoValue is IEnumerable dtoValEnum && entityValue is not string && dtoValue is not string)
            {
                var entityValList = entityValEnum.Cast<object>().ToList();
                var dtoValList = dtoValEnum.Cast<object>().ToList();
                entityValList.Count.ShouldBe(dtoValList.Count, $"{dtoProperty.Name} collection count should match");
                // Check that every item in entityValList has an equivalent in dtoValList, and vice versa, ignoring order
                foreach (var entityItem in entityValList)
                {
                    dtoValList.Any(dtoItem =>
                    {
                        try { entityItem.ShouldBeEquivalentTo(dtoItem, excludedProperties ?? []); return true; } catch { return false; }
                    }).ShouldBeTrue("An equivalent item was not found in the expected collection.");
                }
                foreach (var dtoItem in dtoValList)
                {
                    entityValList.Any(entityItem =>
                    {
                        try { entityItem.ShouldBeEquivalentTo(dtoItem, excludedProperties ?? []); return true; } catch { return false; }
                    }).ShouldBeTrue("An equivalent item was not found in the actual collection.");
                }
            }
            else
            {
                if (dtoValue?.GetType() == entityValue?.GetType())
                {
                    dtoValue.ShouldBe(entityValue, $"{dtoProperty.Name} property should be equivalent.");
                }
                else
                {
                    (dtoValue?.Equals(entityValue) == true).ShouldBeTrue($"{dtoProperty.Name} property should be equivalent.");
                }
            }
        }
    }

    private static bool IsNavigationProperty(Type propertyType)
    {
        // Consider properties that are classes (except string) or implement ICollection/IEnumerable as navigation properties
        return (propertyType.IsClass && propertyType != typeof(string)) ||
               (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string));
    }
}
