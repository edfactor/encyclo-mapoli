---
applyTo: "src/services/src/**/*.*"
paths: "src/services/src/**/*.*"
---

# FullName Consolidation Pattern - Implementation Rules

## Overview

This file defines mandatory patterns for handling employee/beneficiary name fields across the Profit Sharing application. All new code must follow these rules to ensure consistency.

## Mandatory Patterns

### Rule 1: Backend Response DTOs MUST Use `FullName`

**REQUIRED:**

- All Response DTOs that display person names MUST have a property named `FullName` (NOT `Name`)
- Exception: Lookup table DTOs can have generic `name` properties (e.g., `FrequencyKindDto.Name`, `StatusDto.Name`)
- The `FullName` property MUST be marked with `[MaskSensitive]` attribute

**Pattern:**

```csharp
public sealed record MyResponse
{
    [MaskSensitive]
    public required string FullName { get; set; }  // ✅ Correct

    // NOT: public string Name { get; set; }       // ❌ Wrong
}
```

**Verification:** No Response DTO (except lookups) should have a property named just `Name` for person data.

---

### Rule 2: Backend Services MUST Compute FullName

**REQUIRED:**

- Services MUST compute `FullName` using `DtoCommonExtensions.ComputeFullNameWithInitial()`
- MUST NOT assign stored `ContactInfo.FullName` directly to response DTOs
- MUST include `LastName`, `FirstName`, `MiddleName` in LINQ SELECT clause before computing

**Pattern:**

```csharp
// Step 1: Fetch individual name parts in query
select new
{
    LastName = entity.ContactInfo.LastName,
    FirstName = entity.ContactInfo.FirstName,
    MiddleName = entity.ContactInfo.MiddleName
}

// Step 2: Compute FullName with middle initial
FullName = DtoCommonExtensions.ComputeFullNameWithInitial(
    lastName,
    firstName,
    middleName)
```

**Verification:** All services mapping to DTOs with FullName property should use `ComputeFullNameWithInitial()`.

---

### Rule 3: FullName Format Specification

**REQUIRED:**

- Format MUST be: `"LastName, FirstName"` (comma-separated)
- If middle name exists: `"LastName, FirstName M"` (middle initial only, not full name)
- If middle name missing: `"LastName, FirstName"` (no middle initial)

**Pattern:**

```csharp
// Use this helper for all FullName computation
public static string ComputeFullNameWithInitial(string lastName, string firstName, string? middleName)
{
    var middleInitial = string.IsNullOrWhiteSpace(middleName)
        ? string.Empty
        : $"{middleName[0]}";

    return string.IsNullOrWhiteSpace(middleInitial)
        ? $"{lastName}, {firstName}"
        : $"{lastName}, {firstName} {middleInitial}";
}
```

**Examples:**

- Smith, John (no middle name) ✅
- Smith, John M (middle name = Michael) ✅
- Doe, Jane R (middle name = Rose) ✅

**Verification:** Unit tests in `DtoCommonExtensionsTests` verify format.

---

### Rule 4: Frontend DTOs MUST Use `fullName`

**REQUIRED:**

- All TypeScript DTO interfaces for person data MUST have `fullName` property (NOT `name`)
- Property should be optional (`fullName?: string`) to allow partial updates
- Exception: Lookup DTOs can have `name?: string` for generic purposes

**Pattern:**

```typescript
// ✅ Correct
export interface PersonResponse {
  fullName: string;
  badgeNumber: number;
}

// ❌ Wrong
export interface PersonResponse {
  name: string; // Should be fullName
  badgeNumber: number;
}
```

**Verification:** No person response DTO (except lookups) should have `name` property.

---

### Rule 5: Frontend Components MUST Use Backend-Provided fullName

**REQUIRED:**

- Components MUST use `object.fullName` directly from backend DTO
- MUST NOT manually concatenate `firstName` and `lastName`
- MUST NOT construct names in components using template literals or string addition
- Grid columns MUST use `field: "fullName"` mapping

**Pattern:**

```typescript
// ✅ Correct - Use backend-provided fullName
<span>{person.fullName}</span>

// ✅ Correct - Grid column with proper field mapping
createNameColumn({ field: "fullName" })

// ❌ Wrong - Manual concatenation
<span>{person.firstName} {person.lastName}</span>

// ❌ Wrong - Template literal construction
<span>{`${person.lastName}, ${person.firstName}`}</span>

// ❌ Wrong - String addition
<span>{person.firstName + ' ' + person.lastName}</span>
```

**Verification:** No concatenation of firstName/lastName in display/rendering code.

---

### Rule 6: Grid Columns MUST Specify fullName Field

**REQUIRED:**

- When using `createNameColumn()` factory, MUST explicitly specify `field: "fullName"`
- If no field specified, defaults to `"employeeName"` which causes type mismatch
- Ensure DTO being bound to grid has `fullName` property

**Pattern:**

```typescript
// ✅ Correct - Explicit field mapping
createNameColumn({ field: "fullName" });

// ❌ Wrong - No field specified (will default to employeeName)
createNameColumn({});

// ❌ Wrong - Wrong field name
createNameColumn({ field: "name" });
```

**Verification:** Grid definitions reviewed to ensure correct field binding.

---

### Rule 7: New Endpoints MUST Include FullName Computation

**REQUIRED CHECKLIST:**

When adding a new endpoint returning person names:

- [ ] Response DTO has `FullName` property (not `Name`)
- [ ] Service includes `LastName`, `FirstName`, `MiddleName` in query SELECT
- [ ] Service uses `ComputeFullNameWithInitial()` when mapping to DTO
- [ ] TypeScript DTO has `fullName` property
- [ ] Components use `object.fullName` (no concatenation)
- [ ] Grid columns specify `field: "fullName"` if applicable
- [ ] Format verified: "LastName, FirstName M" pattern with middle initial only
- [ ] Unit tests verify format for edge cases (no middle name, etc.)

**Example Checklist Item (in PR):**

```markdown
- [x] FullName pattern implemented per FULLNAME_CONSOLIDATION_GUIDE.md
  - Backend DTO: FullName property ✓
  - Service: ComputeFullNameWithInitial() ✓
  - Frontend DTO: fullName property ✓
  - Components: Using .fullName (no concatenation) ✓
  - Format verified: "LastName, FirstName M" ✓
```

---

## Violation Consequences

### During Code Review

- [ ] Any Response DTO using `Name` for person data → **Request Changes**
- [ ] Any service assigning FullName without `ComputeFullNameWithInitial()` → **Request Changes**
- [ ] Any frontend component concatenating firstName/lastName → **Request Changes**
- [ ] Any grid column using wrong field name → **Request Changes**

### In Pre-commit Hooks (if enabled)

- Scripts will detect violations and **block commit** with error message
- Developer must fix violations before proceeding

### In CI/CD Pipeline (if enabled)

- Build will **fail** if violations detected
- PR will be marked as **status check failed**

---

## How to Implement for New Endpoints

### Step-by-Step Example: Adding a New MyPerson Endpoint

#### 1. Create Response DTO

```csharp
// File: src/services/src/Demoulas.ProfitSharing.Common/Contracts/Response/MyPersonResponse.cs
public sealed record MyPersonResponse
{
    public required int Id { get; set; }
    [MaskSensitive]
    public required string FullName { get; set; }  // ← Must be FullName
    public required string Ssn { get; set; }
}
```

#### 2. Implement Service Mapping

```csharp
// File: MyService.cs
public async Task<MyPersonResponse> GetPersonAsync(int id, CancellationToken ct)
{
    var person = await _context.People
        .Where(p => p.Id == id)
        .Select(p => new
        {
            p.Id,
            p.Ssn,
            p.ContactInfo.LastName,      // ← Include parts
            p.ContactInfo.FirstName,     // ← Include parts
            p.ContactInfo.MiddleName     // ← Include parts
        })
        .FirstOrDefaultAsync(ct);

    if (person is null)
        return null;

    return new MyPersonResponse
    {
        Id = person.Id,
        Ssn = person.Ssn,
        FullName = DtoCommonExtensions.ComputeFullNameWithInitial(  // ← Compute
            person.LastName,
            person.FirstName,
            person.MiddleName)
    };
}
```

#### 3. Create TypeScript DTO

```typescript
// File: src/ui/src/types/myPerson.ts
export interface MyPersonResponse {
  id: number;
  fullName: string; // ← Must be fullName
  ssn: string;
}
```

#### 4. Use in Component

```typescript
// ✅ Correct
<div>{person.fullName}</div>

// ❌ Wrong (don't do this)
<div>{person.firstName} {person.lastName}</div>
```

#### 5. Add Unit Test

```csharp
[Fact(DisplayName = "PS-XXXX: FullName formatted with middle initial")]
public async Task GetPersonAsync_Should_Format_FullName_With_Middle_Initial()
{
    // Arrange
    var person = new Person {
        LastName = "Smith",
        FirstName = "John",
        MiddleName = "Michael"
    };

    // Act
    var result = await _service.GetPersonAsync(1, CancellationToken.None);

    // Assert
    result.FullName.Should().Be("Smith, John M");  // Format: "LastName, FirstName M"
}
```

---

## References

- **Helper Method**: `DtoCommonExtensions.ComputeFullNameWithInitial()`
- **Location**: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Shared/DtoCommonExtensions.cs`
- **Guide**: `.github/FULLNAME_CONSOLIDATION_GUIDE.md`
- **Prevention**: `.github/FULLNAME_PATTERN_PREVENTION.md`
- **Audit**: `.github/FULLNAME_QUICK_AUDIT.md`
- **Related Ticket**: PS-1829 (FullName formatting with middle initial)

---

## For AI Assistants/Copilot

**Important**: When implementing person name handling in new endpoints:

1. **Always** check if DTO should have `FullName` property
2. **Always** compute using `DtoCommonExtensions.ComputeFullNameWithInitial()`
3. **Always** include `LastName`, `FirstName`, `MiddleName` in query SELECT
4. **Always** update TypeScript DTO with `fullName` property
5. **Always** remove manual concatenation in components
6. **Always** verify format: "LastName, FirstName M" (middle initial only)
7. **Always** add test verifying the format

If you violate these patterns, code review will request changes. These are not suggestions—they are mandatory patterns.

---

**Last Updated**: November 19, 2025  
**Status**: Active - All new code must comply
**Enforcement**: Manual review + Optional automated checks
