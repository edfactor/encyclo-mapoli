---
applyTo: "src/services/src/**/*.*"
paths: "src/services/src/**/*.*"
---

# Demoulas.Util - Utility Extensions

Rules

-   Use provided extensions instead of reimplementing common helpers.
-   Keep extension methods side-effect free and well tested.

See also

-   demoulas.common.instructions.md

---

## DateOnly Extensions

**Class:** `DateOnlyExtensions`

### ToDateOnly()

Converts DateTime or DateTimeOffset to DateOnly.

**Syntax:**

```csharp
// Overload 1: From DateTime (uses local kind by default)
public static DateOnly ToDateOnly(this DateTime dateTime)

// Overload 2: From DateTime with specified kind
public static DateOnly ToDateOnly(this DateTime dateTime, DateTimeKind kind)

// Overload 3: From DateTimeOffset
public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset)

// Overload 4: From DateTimeOffset with specified kind
public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset, DateTimeKind kind)
```

**Parameters:**

-   `dateTime` (DateTime): The DateTime to convert
-   `dateTimeOffset` (DateTimeOffset): The DateTimeOffset to convert
-   `kind` (DateTimeKind, optional): The DateTimeKind to use for conversion

**Returns:** A DateOnly representation of the provided date/time

**Example:**

```csharp
DateTime dt = new DateTime(2024, 12, 13, 10, 30, 0);
DateOnly dateOnly = dt.ToDateOnly(); // 2024-12-13

DateTimeOffset dto = new DateTimeOffset(2024, 12, 13, 10, 30, 0, TimeSpan.Zero);
DateOnly dateOnly2 = dto.ToDateOnly(); // 2024-12-13
```

---

## Object Extensions

**Class:** `ObjectExtension`

### GetPropertyValue()

Dynamically retrieves the value of a property from an object.

**Syntax:**

```csharp
public static object? GetPropertyValue(this object source, string propertyName)
```

**Parameters:**

-   `source` (object): The object to retrieve the property from
-   `propertyName` (string): The name of the property

**Returns:** The property value, or null if the property doesn't exist

**Example:**

```csharp
var customer = new { Name = "John", Age = 30 };
object? name = customer.GetPropertyValue("Name"); // "John"
```

---

### GetPropertyValueAsString()

Retrieves a property value and converts it to a string.

**Syntax:**

```csharp
public static string? GetPropertyValueAsString(this object source, string propertyName)
```

**Parameters:**

-   `source` (object): The object to retrieve the property from
-   `propertyName` (string): The name of the property

**Returns:** A string representation of the property value, or null

**Example:**

```csharp
var product = new { Price = 19.99m };
string? price = product.GetPropertyValueAsString("Price"); // "19.99"
```

---

### GetPropertyValueAsDecimal()

Retrieves a property value and converts it to a decimal.

**Syntax:**

```csharp
public static decimal? GetPropertyValueAsDecimal(this object source, string propertyName, decimal defaultValue = 0)
```

**Parameters:**

-   `source` (object): The object to retrieve the property from
-   `propertyName` (string): The name of the property
-   `defaultValue` (decimal, optional): Default value if conversion fails

**Returns:** A decimal representation of the property value, or the default value

**Example:**

```csharp
var order = new { Total = "150.50" };
decimal? total = order.GetPropertyValueAsDecimal("Total"); // 150.50

decimal? invalid = order.GetPropertyValueAsDecimal("NonExistent", 0); // 0
```

---

## IQueryable Extensions

**Class:** `IQueryableExtension`

### OrderByProperty()

Orders IQueryable results by a property name dynamically.

**Syntax:**

```csharp
public static IQueryable<TEntity> OrderByProperty<TEntity>(
    this IQueryable<TEntity> source,
    string orderByProperty,
    bool descending = false,
    bool ignoreNullValues = false)
```

**Type Parameters:**

-   `TEntity`: The type of elements in the sequence

**Parameters:**

-   `source` (IQueryable<TEntity>): The source queryable
-   `orderByProperty` (string): The property name to order by (case-insensitive for strings)
-   `descending` (bool, optional): Whether to sort in descending order
-   `ignoreNullValues` (bool, optional): Whether to move null values to the bottom of the sort

**Returns:** An ordered IQueryable<TEntity>

**Exceptions:**

-   `ArgumentException`: Thrown if the property doesn't exist on the entity type

**Features:**

-   Case-insensitive sorting for string properties
-   Null value handling to push them to bottom of results
-   Supports multi-column sorting with comma-separated property names

**Example:**

```csharp
var customers = dbContext.Customers.AsQueryable();

// Simple ascending sort
var sorted = customers.OrderByProperty("LastName");

// Descending sort
var sortedDesc = customers.OrderByProperty("CreatedDate", descending: true);

// Ignore nulls
var sortedIgnoreNulls = customers.OrderByProperty("Email", ignoreNullValues: true);

// Multi-column sort
var multiSort = customers.OrderByProperty("LastName,FirstName");
```

---

## HttpRequestMessage Extensions

**Class:** `HttpRequestMessageExtension`

### GenerateCurlCommand()

Generates a cURL command string from an HttpRequestMessage.

**Syntax:**

```csharp
public static string GenerateCurlCommand(this HttpRequestMessage request, string url)
```

**Parameters:**

-   `request` (HttpRequestMessage): The HTTP request message
-   `url` (string): The complete URL being requested

**Returns:** A formatted cURL command string that can be copied to Postman or terminal

**Features:**

-   Includes all headers
-   Properly escapes quotes in header values
-   Adds compression and insecure flags for testing
-   Ready to paste into terminal or Postman

**Example:**

```csharp
using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/users");
request.Headers.Add("Authorization", "Bearer token123");

string curl = request.GenerateCurlCommand("https://api.example.com/users");
// curl -X GET "https://api.example.com/users" -H "Authorization: Bearer token123" --compressed --insecure
```

---

## Environment Extensions

**Class:** `EnvironmentExtensions`

### IsTestEnvironment()

Determines if the current environment is a test environment and updates the environment name to "Testing".

**Syntax:**

```csharp
// Overload 1: For IHostEnvironment
public static bool IsTestEnvironment(this IHostEnvironment environment)

// Overload 2: For IWebHostEnvironment
public static bool IsTestEnvironment(this IWebHostEnvironment environment)
```

**Parameters:**

-   `environment` (IHostEnvironment/IWebHostEnvironment): The host environment

**Returns:** True if the environment is a test environment, otherwise false

**Recognized Test Environments:**

-   Environment name: "Testing" or "Test"
-   Application names: "ReSharperTestRunner" or "testHost"

**Side Effect:**

-   Sets the environment name to "Testing" if a test environment is detected

**Example:**

```csharp
public void Configure(IHostEnvironment environment)
{
    if (environment.IsTestEnvironment())
    {
        // Configure test-specific settings
        // environment.EnvironmentName is now "Testing"
    }
}
```

---

## Best Practices

1. **String Operations**: Use `FirstCharToUpper()` for consistent capitalization instead of manual string manipulation
2. **Date Handling**: Use `ToEndOfDay()` and `ZeroTime()` for consistent date range queries
3. **Age Calculations**: Use the `Age()` extension method rather than manual date arithmetic
4. **Dynamic Sorting**: Use `OrderByProperty()` for user-driven sorting requirements
5. **Test Detection**: Always use `IsTestEnvironment()` for conditional test configuration
6. **Property Access**: Use `GetPropertyValue()` extensions for safe dynamic property access

## Import Statement

```csharp
using Demoulas.Util.Extensions;
```

---

**See Also:**

-   [Main Documentation](./demoulas.common.instructions.md)
-   [Data Extensions](./demoulas.common.data.instructions.md)
-   [API Extensions](./demoulas.common.api.instructions.md)
