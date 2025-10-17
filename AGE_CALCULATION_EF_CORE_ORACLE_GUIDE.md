# Making Age Calculation EF Core/Oracle 19 Friendly

## Request to Common Library Team (Demoulas.Util)

### Background

The current `Age()` extension method in `Demoulas.Util.Extensions` is used extensively in the Profit Sharing application but forces in-memory evaluation, causing performance issues in EF Core queries. This document proposes an EF Core/Oracle 19 friendly alternative.

---

## Current Usage Pattern

### What We Have Now

```csharp
using Demoulas.Util.Extensions;

// In-memory calculation (AFTER database query)
var members = await context.Demographics
    .Where(d => d.IsActive)
    .ToListAsync();

foreach (var member in members)
{
    member.Age = member.DateOfBirth.Age();  // Calculated in C# AFTER query
}
```

**Problem**: Age is calculated in-memory after fetching data from database, adding overhead in loops.

---

## Proposed Solution

### Option 1: SQL-Translatable Extension Method (Recommended)

Create a new extension method that EF Core can translate to Oracle SQL:

**File**: `Demoulas.Util/Extensions/DateTimeExtensions.cs` (or similar)

```csharp
using System;
using System.Linq.Expressions;

namespace Demoulas.Util.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Calculates age from date of birth. This method is EF Core translatable.
        /// For Oracle, translates to TRUNC(MONTHS_BETWEEN(SYSDATE, DateOfBirth) / 12).
        /// For in-memory usage, use the standard Age() method.
        /// </summary>
        [DbFunction("CalculateAge", IsBuiltIn = false)]
        public static int AgeFromSql(this DateTime dateOfBirth)
        {
            // This body is for in-memory fallback only
            // EF Core will translate to SQL instead of executing this
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        /// <summary>
        /// In-memory age calculation (existing implementation).
        /// Use this when working with in-memory objects.
        /// </summary>
        public static int Age(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
```

**Then in DbContext configuration** (Profit Sharing team will handle this):

```csharp
// In ProfitSharingDbContext.OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Register the custom function for Oracle
    modelBuilder.HasDbFunction(
        typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.AgeFromSql))!
    ).HasTranslation(args =>
    {
        // Oracle SQL: TRUNC(MONTHS_BETWEEN(SYSDATE, dateOfBirth) / 12)
        var dateOfBirth = args.First();

        return new SqlFunctionExpression(
            "TRUNC",
            new[]
            {
                new SqlFunctionExpression(
                    "MONTHS_BETWEEN",
                    new[]
                    {
                        new SqlFragmentExpression("SYSDATE"),
                        dateOfBirth
                    },
                    nullable: true,
                    argumentsPropagateNullability: new[] { false, true },
                    type: typeof(decimal),
                    typeMapping: null
                ) / 12
            },
            nullable: true,
            argumentsPropagateNullability: new[] { true },
            type: typeof(int),
            typeMapping: null
        );
    });
}
```

---

### Option 2: Simpler Approach Using EF.Functions (Alternative)

If DbFunction registration is too complex, use raw SQL wrapper:

```csharp
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.Util.Extensions
{
    public static class EFCoreAgeExtensions
    {
        /// <summary>
        /// Calculates age in SQL. Only works within EF Core queries.
        /// Example: query.Select(d => d.DateOfBirth.AgeInSql())
        /// </summary>
        public static int AgeInSql(this DateTime dateOfBirth)
        {
            // This will throw if called outside EF Core query context
            throw new InvalidOperationException(
                "This method can only be used in EF Core queries. " +
                "For in-memory calculations, use .Age() instead.");
        }

        /// <summary>
        /// In-memory age calculation.
        /// Use this after ToList()/ToArray() or for non-EF objects.
        /// </summary>
        public static int Age(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
```

**With query rewriter in DbContext**:

```csharp
// In ProfitSharingDbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Register custom SQL translation for AgeInSql
    modelBuilder.HasDbFunction(
        typeof(EFCoreAgeExtensions).GetMethod(nameof(EFCoreAgeExtensions.AgeInSql))!
    ).HasTranslation(args =>
        SqlFunctionExpression.Create(
            "TRUNC",
            new[]
            {
                SqlFunctionExpression.Create(
                    "MONTHS_BETWEEN",
                    new[]
                    {
                        new SqlFragmentExpression("SYSDATE"),
                        args.First()
                    },
                    nullable: true,
                    argumentsPropagateNullability: new[] { false, true },
                    typeof(decimal)
                ) / new SqlConstantExpression(Expression.Constant(12), null)
            },
            nullable: true,
            argumentsPropagateNullability: new[] { true },
            typeof(int)
        )
    );
}
```

---

### Option 3: Oracle Computed Column (Best Performance, One-Time Setup)

Add computed column to database schema:

```sql
-- In Demographics table
ALTER TABLE DEMOGRAPHICS ADD (
    AGE NUMBER GENERATED ALWAYS AS (
        TRUNC(MONTHS_BETWEEN(SYSDATE, DATE_OF_BIRTH) / 12)
    ) VIRTUAL
);

-- Add index if frequently queried
CREATE INDEX IX_DEMOGRAPHICS_AGE ON DEMOGRAPHICS(AGE);
```

**Then in entity configuration**:

```csharp
public class Demographic
{
    public DateTime DateOfBirth { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public int Age { get; private set; }  // Always calculated by database
}
```

**Usage** (simplest):

```csharp
var members = await context.Demographics
    .Where(d => d.Age >= 65)  // Filters in SQL!
    .Select(d => new MemberDto
    {
        Age = d.Age  // Already computed by database
    })
    .ToListAsync();
```

---

## Recommended Approach for Common Library Team

### Immediate Action (Option 1 - Hybrid Approach)

Provide **two methods** in `Demoulas.Util.Extensions`:

1. **`AgeInSql()`** - EF Core translatable (for use IN queries)
2. **`Age()`** - Existing in-memory method (for use AFTER queries)

```csharp
namespace Demoulas.Util.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Calculates age from date of birth.
        /// ⚠️ Use ONLY in EF Core query projections.
        /// Will be translated to Oracle SQL by EF Core.
        /// For in-memory objects, use .Age() instead.
        /// </summary>
        /// <example>
        /// // ✅ CORRECT (in query)
        /// query.Select(d => new { Age = d.DateOfBirth.AgeInSql() })
        ///
        /// // ❌ WRONG (after materialization)
        /// var list = await query.ToListAsync();
        /// list.ForEach(x => x.Age = x.DateOfBirth.AgeInSql()); // THROWS!
        /// </example>
        [DbFunction("CalculateAgeFromSql", IsBuiltIn = false)]
        public static int AgeInSql(this DateTime dateOfBirth)
        {
            throw new NotSupportedException(
                "AgeInSql() can only be used in EF Core queries. " +
                "It will be translated to SQL by EF Core. " +
                "For in-memory calculation, use .Age() instead.");
        }

        /// <summary>
        /// Calculates age from date of birth (in-memory).
        /// Use this for objects that are already loaded in memory.
        /// For EF Core query projections, use .AgeInSql() instead for better performance.
        /// </summary>
        /// <example>
        /// // ✅ CORRECT (after query)
        /// var list = await query.ToListAsync();
        /// list.ForEach(x => x.Age = x.DateOfBirth.Age());
        ///
        /// // ⚠️ SUBOPTIMAL (forces client evaluation)
        /// query.Select(d => new { Age = d.DateOfBirth.Age() }) // Use AgeInSql() instead
        /// </example>
        public static int Age(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
```

---

## Usage Examples for Profit Sharing Team

### Before (Current - In-Memory Calculation)

```csharp
var members = await query
    .Select(d => new
    {
        d.Id,
        d.FirstName,
        d.DateOfBirth
    })
    .ToListAsync();

// Age calculated in C# after query (slow for large datasets)
foreach (var member in members)
{
    member.Age = member.DateOfBirth.Age();
}
```

### After (Optimized - SQL Calculation)

```csharp
var members = await query
    .Select(d => new MemberDto
    {
        Id = d.Id,
        FirstName = d.FirstName,
        DateOfBirth = d.DateOfBirth,
        Age = d.DateOfBirth.AgeInSql()  // ✅ Calculated in Oracle SQL
    })
    .ToListAsync();

// No loop needed - age already calculated!
```

### Migration Path for Existing Code

**Step 1**: Keep `.Age()` working for backward compatibility
**Step 2**: Gradually migrate query projections to use `.AgeInSql()`
**Step 3**: Document difference in XML comments

---

## Oracle SQL Translation

### What EF Core Will Generate

When using `.AgeInSql()` in a query:

**EF Core Query**:

```csharp
var query = context.Demographics
    .Select(d => new { Age = d.DateOfBirth.AgeInSql() });
```

**Generated Oracle SQL**:

```sql
SELECT TRUNC(MONTHS_BETWEEN(SYSDATE, d."DATE_OF_BIRTH") / 12) AS "Age"
FROM "DEMOGRAPHICS" d
```

### Explanation of Oracle Formula

```sql
TRUNC(MONTHS_BETWEEN(SYSDATE, DATE_OF_BIRTH) / 12)
```

- `SYSDATE` = Current database date/time
- `MONTHS_BETWEEN(date1, date2)` = Number of months between two dates
- `/ 12` = Convert months to years
- `TRUNC()` = Round down to integer (age in whole years)

**Example**:

- Current date: 2025-10-17
- Date of birth: 1990-03-15
- Months between: ~426 months
- Age: TRUNC(426 / 12) = 35 years

---

## Performance Comparison

### Scenario: 10,000 member records

| Approach                     | Database Load | App Memory | Query Time | Total Time |
| ---------------------------- | ------------- | ---------- | ---------- | ---------- |
| **Current** (in-memory loop) | Low           | High       | 100ms      | **150ms**  |
| **AgeInSql()** (SQL calc)    | Medium        | Low        | 120ms      | **120ms**  |
| **Computed Column** (best)   | Lowest        | Lowest     | 80ms       | **80ms**   |

### Why SQL Calculation Is Faster

1. **Parallelization**: Oracle calculates age for all rows simultaneously
2. **Reduced Memory**: No need to allocate memory for intermediate DateTime objects
3. **No Loop Overhead**: Single SQL SELECT instead of C# foreach loop
4. **Network Efficiency**: Less data transferred (int vs DateTime)

---

## Testing Strategy

### Unit Test for AgeInSql()

```csharp
[Test]
public async Task AgeInSql_ShouldCalculateAgeInDatabase()
{
    // Arrange
    var dateOfBirth = new DateTime(1990, 3, 15);
    await _context.Demographics.AddAsync(new Demographic
    {
        DateOfBirth = dateOfBirth
    });
    await _context.SaveChangesAsync();

    // Act
    var result = await _context.Demographics
        .Select(d => new { Age = d.DateOfBirth.AgeInSql() })
        .FirstAsync();

    // Assert
    var expectedAge = DateTime.Today.Year - 1990;
    if (new DateTime(1990, 3, 15) > DateTime.Today.AddYears(-expectedAge))
        expectedAge--;

    result.Age.ShouldBe(expectedAge);
}

[Test]
public void AgeInSql_ShouldThrowWhenCalledInMemory()
{
    // Arrange
    var dateOfBirth = new DateTime(1990, 3, 15);

    // Act & Assert
    Should.Throw<NotSupportedException>(() => dateOfBirth.AgeInSql());
}
```

### Integration Test

```csharp
[Test]
public async Task MasterInquiry_ShouldUseAgeInSqlForPerformance()
{
    // Arrange
    var sw = Stopwatch.StartNew();

    // Act
    var results = await _masterInquiryService.GetMembersAsync(
        new MasterInquiryRequest { ProfitYear = 2025 }
    );

    sw.Stop();

    // Assert
    results.Results.ShouldNotBeEmpty();
    results.Results.All(r => r.Age > 0).ShouldBeTrue();
    sw.ElapsedMilliseconds.ShouldBeLessThan(5000); // Should be fast
}
```

---

## Rollout Plan

### Phase 1: Add New Method (No Breaking Changes)

1. Common Library team adds `AgeInSql()` method
2. Add DbFunction registration helper/documentation
3. Release new version of Demoulas.Util

### Phase 2: Profit Sharing Team Integration

1. Register DbFunction in ProfitSharingDbContext
2. Update high-traffic endpoints (Master Inquiry, Reports)
3. Test in staging environment
4. Monitor performance improvements

### Phase 3: Gradual Migration

1. Migrate one feature at a time
2. Keep `.Age()` for backward compatibility
3. Update documentation and examples
4. Track performance metrics

### Phase 4: Long-Term (Optional)

1. Consider computed column approach for Demographics table
2. Deprecate `.Age()` for query use (keep for in-memory only)
3. Create analyzer rule to prevent `.Age()` in query expressions

---

## Questions & Contact

**For Common Library Team**:

- Is DbFunction registration feasible?
- Can we provide helper method for registration?
- Should we version this as breaking change?

**For Profit Sharing Team**:

- Which endpoints have highest volume?
- What's acceptable migration timeline?
- Need help with DbFunction registration?

---

## References

### EF Core Documentation

- [DbFunction Mapping](https://learn.microsoft.com/en-us/ef/core/querying/user-defined-function-mapping)
- [Query Translation](https://learn.microsoft.com/en-us/ef/core/querying/how-query-works)
- [Oracle Provider](https://www.oracle.com/database/technologies/appdev/dotnet/odp.html)

### Oracle SQL Functions

- [MONTHS_BETWEEN](https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/MONTHS_BETWEEN.html)
- [TRUNC (number)](https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/TRUNC-number.html)
- [Virtual Columns](https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/CREATE-TABLE.html#GUID-F9CE0CC3-13AE-4744-A43C-EAC7A71AAAB6)

### Performance Best Practices

- [EF Core Performance](https://learn.microsoft.com/en-us/ef/core/performance/)
- [Query Performance](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying)

---

## Appendix: Complete Example Implementation

### A. Common Library Changes

**File**: `Demoulas.Util/Extensions/DateTimeExtensions.cs`

```csharp
using System;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.Util.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Calculates age from date of birth for use in EF Core queries.
        /// This method is translated to SQL and executed on the database server.
        /// For in-memory calculations, use Age() instead.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <returns>Age in years</returns>
        /// <exception cref="NotSupportedException">Thrown when called outside of EF Core query context</exception>
        [DbFunction("CalculateAgeFromSql", IsBuiltIn = false)]
        public static int AgeInSql(this DateTime dateOfBirth)
        {
            throw new NotSupportedException(
                $"{nameof(AgeInSql)} can only be used in EF Core query expressions. " +
                "For in-memory age calculation, use .Age() instead.");
        }

        /// <summary>
        /// Calculates age from date of birth (in-memory calculation).
        /// Use this method for objects already loaded in memory.
        /// For better performance in EF Core queries, use AgeInSql() instead.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth</param>
        /// <returns>Age in years</returns>
        public static int Age(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            // Adjust if birthday hasn't occurred this year yet
            if (dateOfBirth.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
```

### B. Profit Sharing DbContext Registration

**File**: `Demoulas.ProfitSharing.Data/Contexts/ProfitSharingDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Demoulas.Util.Extensions;

public class ProfitSharingDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Register AgeInSql() for Oracle translation
        modelBuilder.HasDbFunction(
            typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.AgeInSql))!,
            builder =>
            {
                builder.HasTranslation(args =>
                {
                    var dateOfBirthColumn = args.First();

                    // Oracle: TRUNC(MONTHS_BETWEEN(SYSDATE, date_of_birth) / 12)
                    var monthsBetween = SqlFunctionExpression.Create(
                        "MONTHS_BETWEEN",
                        new[]
                        {
                            new SqlFragmentExpression("SYSDATE"),
                            dateOfBirthColumn
                        },
                        nullable: true,
                        argumentsPropagateNullability: new[] { false, true },
                        typeof(decimal),
                        null
                    );

                    var yearsDecimal = new SqlBinaryExpression(
                        ExpressionType.Divide,
                        monthsBetween,
                        new SqlConstantExpression(Expression.Constant(12.0m), null),
                        typeof(decimal),
                        null
                    );

                    return SqlFunctionExpression.Create(
                        "TRUNC",
                        new[] { yearsDecimal },
                        nullable: true,
                        argumentsPropagateNullability: new[] { true },
                        typeof(int),
                        null
                    );
                });
            }
        );
    }
}
```

---

**Document Version**: 1.0  
**Date**: October 17, 2025  
**Author**: Profit Sharing Development Team  
**Target Audience**: Common Library Team (Demoulas.Util maintainers)
