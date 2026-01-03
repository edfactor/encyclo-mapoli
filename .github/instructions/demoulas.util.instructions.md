---
applyTo: "src/services/src/**/*.*"
paths: "src/services/src/**/*.*"
---

# Demoulas.Util - Core Utility Extensions

**Package:** `Demoulas.Util`  
**Namespace:** `Demoulas.Util.Extensions`

This package provides essential extension methods for common .NET types including strings, dates, objects, and HTTP requests. These utilities enhance productivity and code consistency across all Demoulas applications.

## Table of Contents

1. [String Extensions](#string-extensions)
2. [DateTime Extensions](#datetime-extensions)
3. [DateOnly Extensions](#dateonly-extensions)
4. [Object Extensions](#object-extensions)
5. [IQueryable Extensions](#iqueryable-extensions)
6. [HttpRequestMessage Extensions](#httprequestmessage-extensions)
7. [Environment Extensions](#environment-extensions)

---

## String Extensions

**Class:** `StringExtension`

### FirstCharToUpper()

Converts the first character of a string to uppercase while leaving the rest unchanged.

**Syntax:**

```csharp
public static string FirstCharToUpper(this string input)
```

**Parameters:**

- `input` (string): The input string to process

**Returns:** A string with its first character converted to uppercase

**Exceptions:**

- `ArgumentNullException`: Thrown when input is null
- `ArgumentException`: Thrown when input is empty

**Example:**

```csharp
string text = "hello world";
string result = text.FirstCharToUpper(); // "Hello world"
```

**Performance Note:** The .NET 9 implementation uses `string.Create` for a single allocation, providing 10-15% performance improvement over earlier versions.

---

### Contains()

Performs a culture-sensitive string containment check.

**Syntax:**

```csharp
public static bool Contains(this string input, string searched, StringComparison comparison)
```

**Parameters:**

- `input` (string): The source string to search in
- `searched` (string): The substring to search for
- `comparison` (StringComparison): The comparison type (e.g., `OrdinalIgnoreCase`)

**Returns:** True if the substring is found; otherwise, false

**Example:**

```csharp
string text = "Hello World";
bool found = text.Contains("world", StringComparison.OrdinalIgnoreCase); // true
```

---

### Base64Encode()

Encodes a plain text string to Base64.

**Syntax:**

```csharp
public static string Base64Encode(this string plainText)
```

**Parameters:**

- `plainText` (string): The plain text string to encode

**Returns:** A Base64-encoded string

**Example:**

```csharp
string encoded = "Hello World".Base64Encode(); // "SGVsbG8gV29ybGQ="
```

---

### Base64Decode()

Decodes a Base64-encoded string back to plain text.

**Syntax:**

```csharp
public static string Base64Decode(this string base64EncodedData)
```

**Parameters:**

- `base64EncodedData` (string): The Base64-encoded string to decode

**Returns:** The decoded plain text string

**Example:**

```csharp
string decoded = "SGVsbG8gV29ybGQ=".Base64Decode(); // "Hello World"
```

---

## DateTime Extensions

**Class:** `DateTimeExtension`

### ToEndOfDay()

Calculates the end of the given date (23:59:59).

**Syntax:**

```csharp
public static DateTime ToEndOfDay(this DateTime dateTime)
```

**Parameters:**

- `dateTime` (DateTime): The date to process

**Returns:** A DateTime representing 11:59:59 PM on the given date

**Example:**

```csharp
DateTime date = new DateTime(2024, 12, 13, 10, 30, 0);
DateTime endDay = date.ToEndOfDay(); // 2024-12-13 23:59:59
```

---

### ZeroTime()

Gets a date with the time zeroed out (00:00:00).

**Syntax:**

```csharp
public static DateTime ZeroTime(this DateTime dateTime)
```

**Parameters:**

- `dateTime` (DateTime): The date to process

**Returns:** A DateTime with time set to 00:00:00

**Example:**

```csharp
DateTime date = new DateTime(2024, 12, 13, 10, 30, 45);
DateTime zeroed = date.ZeroTime(); // 2024-12-13 00:00:00
```

---

### NullOnUnixStart()

Returns null if the date is on or before January 1, 1970 (Unix epoch).

**Syntax:**

```csharp
public static DateTime? NullOnUnixStart(this DateTime? date)
```

**Parameters:**

- `date` (DateTime?): The nullable date to check

**Returns:** The original date if after Unix epoch, otherwise null

**Example:**

```csharp
DateTime? oldDate = new DateTime(1960, 1, 1);
DateTime? result = oldDate.NullOnUnixStart(); // null

DateTime? validDate = new DateTime(2024, 12, 13);
DateTime? result2 = validDate.NullOnUnixStart(); // 2024-12-13
```

---

### LastYearDay()

Calculates the corresponding date of the same day and week number in the previous year.

**Syntax:**

```csharp
public static DateTime LastYearDay(this DateTime todayDate)
```

**Parameters:**

- `todayDate` (DateTime): The current date

**Returns:** The date of the same day and week number in the previous year

**Example:**

```csharp
DateTime date = new DateTime(2024, 12, 13); // Friday, week 50
DateTime lastYear = date.LastYearDay(); // Friday of week 50 in 2023
```

---

### Age()

Calculates the age in years based on a birth date.

**Syntax:**

```csharp
// Overload 1: Using DateTime with current date
public static short Age(this DateTime birthDate)

// Overload 2: Using DateTime with specific reference date
public static short Age(this DateTime birthDate, DateTime fromDateTime)

// Overload 3: Using DateTimeOffset with current UTC date
public static short Age(this DateTimeOffset birthDate)

// Overload 4: Using DateTimeOffset with specific reference date
public static short Age(this DateTimeOffset birthDate, DateTimeOffset fromDateTime)
```

**Parameters:**

- `birthDate` (DateTime/DateTimeOffset): The date of birth
- `fromDateTime` (DateTime/DateTimeOffset, optional): Reference date for calculation (defaults to today/now)

**Returns:** The calculated age as a short integer

**Example:**

```csharp
DateTime birthDate = new DateTime(2000, 5, 15);
short age = birthDate.Age(); // Current age

short ageAt = birthDate.Age(new DateTime(2024, 12, 13)); // Age at specific date
```

---

### DsmMinDate()

Gets the Demoulas system minimum date (January 1, 1917). This is the founding date of Market Basket.

**Syntax:**

```csharp
public static DateTime DsmMinDate()
```

**Returns:** A DateTime representing January 1, 1917

**Example:**

```csharp
DateTime minDate = DateTimeExtension.DsmMinDate(); // 1917-01-01
```

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

- `dateTime` (DateTime): The DateTime to convert
- `dateTimeOffset` (DateTimeOffset): The DateTimeOffset to convert
- `kind` (DateTimeKind, optional): The DateTimeKind to use for conversion

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

- `source` (object): The object to retrieve the property from
- `propertyName` (string): The name of the property

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

- `source` (object): The object to retrieve the property from
- `propertyName` (string): The name of the property

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

- `source` (object): The object to retrieve the property from
- `propertyName` (string): The name of the property
- `defaultValue` (decimal, optional): Default value if conversion fails

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

- `TEntity`: The type of elements in the sequence

**Parameters:**

- `source` (IQueryable<TEntity>): The source queryable
- `orderByProperty` (string): The property name to order by (case-insensitive for strings)
- `descending` (bool, optional): Whether to sort in descending order
- `ignoreNullValues` (bool, optional): Whether to move null values to the bottom of the sort

**Returns:** An ordered IQueryable<TEntity>

**Exceptions:**

- `ArgumentException`: Thrown if the property doesn't exist on the entity type

**Features:**

- Case-insensitive sorting for string properties
- Null value handling to push them to bottom of results
- Supports multi-column sorting with comma-separated property names

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

- `request` (HttpRequestMessage): The HTTP request message
- `url` (string): The complete URL being requested

**Returns:** A formatted cURL command string that can be copied to Postman or terminal

**Features:**

- Includes all headers
- Properly escapes quotes in header values
- Adds compression and insecure flags for testing
- Ready to paste into terminal or Postman

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

- `environment` (IHostEnvironment/IWebHostEnvironment): The host environment

**Returns:** True if the environment is a test environment, otherwise false

**Recognized Test Environments:**

- Environment name: "Testing" or "Test"
- Application names: "ReSharperTestRunner" or "testHost"

**Side Effect:**

- Sets the environment name to "Testing" if a test environment is detected

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

- [Main Documentation](./demoulas.common.instructions.md)
- [Data Extensions](./demoulas.common.data.instructions.md)
- [API Extensions](./demoulas.common.api.instructions.md)
