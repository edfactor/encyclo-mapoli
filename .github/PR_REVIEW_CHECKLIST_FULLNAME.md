# Pull Request Checklist: FullName Pattern

Use this checklist when reviewing PRs that involve person names, employee data, or beneficiary information.

---

## Pre-Review Quick Check

- [ ] Does this PR add new Response DTOs with person names? → See "New Response DTOs" section
- [ ] Does this PR update existing DTOs? → See "DTO Updates" section  
- [ ] Does this PR add new components displaying names? → See "Frontend Components" section
- [ ] Does this PR add new endpoints? → See "New Endpoints" section

---

## ✅ New Response DTOs Checklist

When PR adds new Response DTO for person data:

### Design Review
- [ ] Property is named `FullName` (NOT `Name`)
- [ ] `FullName` is marked with `[MaskSensitive]` attribute
- [ ] No manual firstName/lastName properties in DTO
- [ ] Not a lookup table (lookup tables can have generic `name` property)

**Example of CORRECT DTO:**
```csharp
public sealed record MyPersonResponse
{
    [MaskSensitive]
    public required string FullName { get; set; }  // ✅ Correct
    public required int Id { get; set; }
}
```

**Example of WRONG DTO:**
```csharp
public sealed record MyPersonResponse
{
    public required string Name { get; set; }      // ❌ Should be FullName
    public required int Id { get; set; }
}
```

---

## ✅ DTO Updates Checklist

When PR updates existing Response DTO:

### Design Review
- [ ] If adding name field, uses `FullName` (not `Name`)
- [ ] If removing name field, confirms backend doesn't break consumers
- [ ] `[MaskSensitive]` attribute applied if PII field

---

## ✅ Backend Service Checklist

When PR updates services that map to name DTOs:

### Service Implementation
- [ ] Service includes `LastName`, `FirstName`, `MiddleName` in query SELECT
- [ ] Service uses `DtoCommonExtensions.ComputeFullNameWithInitial()` when mapping
- [ ] NOT assigning `ContactInfo.FullName` directly
- [ ] Handles null/empty middle name correctly
- [ ] Format verified: "LastName, FirstName M" with middle initial only

**Code Review Questions:**
- Where is FullName computed?
- Are all three name parts (LastName, FirstName, MiddleName) available to the mapping?
- Is `ComputeFullNameWithInitial()` being called?

**Example of CORRECT pattern:**
```csharp
Results = data.Results.Select(d => new MyResponse
{
    FullName = DtoCommonExtensions.ComputeFullNameWithInitial(
        d.LastName,
        d.FirstName,
        d.MiddleName)
}).ToList()
```

**Example of WRONG pattern:**
```csharp
Results = data.Results.Select(d => new MyResponse
{
    FullName = d.ContactInfo.FullName  // ❌ No middle initial computation
}).ToList()
```

---

## ✅ Frontend TypeScript Checklist

When PR adds/updates TypeScript DTOs:

### Type Definition
- [ ] Property is named `fullName` (NOT `name`)
- [ ] Property optional if appropriate (`fullName?: string`)
- [ ] Not a lookup type (lookup types can have `name?: string`)

**Example of CORRECT DTO:**
```typescript
export interface MyPersonResponse {
  fullName: string;  // ✅ Correct
  badgeNumber: number;
}
```

**Example of WRONG DTO:**
```typescript
export interface MyPersonResponse {
  name: string;      // ❌ Should be fullName
  badgeNumber: number;
}
```

---

## ✅ Frontend Component Checklist

When PR adds/updates components displaying names:

### Component Usage
- [ ] Components use `object.fullName` (not `object.name`)
- [ ] NO manual concatenation of firstName/lastName
- [ ] NO template literals constructing names
- [ ] NO string addition for name building

**Code Review Questions:**
- Is fullName property being used?
- Are firstName/lastName being manually concatenated anywhere?
- Could this name display be simplified?

**Example of CORRECT usage:**
```typescript
// ✅ Correct
<span>{person.fullName}</span>
```

**Example of WRONG usage:**
```typescript
// ❌ Wrong - manual concatenation
<span>{person.firstName} {person.lastName}</span>

// ❌ Wrong - template literal
<span>{`${person.lastName}, ${person.firstName}`}</span>

// ❌ Wrong - string addition
<span>{person.firstName + ' ' + person.lastName}</span>
```

---

## ✅ Grid Column Checklist

When PR adds/updates grid columns for name display:

### Grid Configuration
- [ ] Uses `createNameColumn()` factory function
- [ ] Specifies `field: "fullName"` explicitly (NOT defaulting)
- [ ] No custom valueFormatter for name construction
- [ ] DTO being bound has `fullName` property

**Code Review Questions:**
- What field is this column bound to?
- Is the field named correctly in the DTO?
- Could this be using a wrong field name like "employeeName"?

**Example of CORRECT column:**
```typescript
// ✅ Correct
createNameColumn({ field: "fullName" })
```

**Example of WRONG column:**
```typescript
// ❌ Wrong - no field specified (defaults to employeeName)
createNameColumn({})

// ❌ Wrong - wrong field name
createNameColumn({ field: "name" })

// ❌ Wrong - manual valueFormatter
createNameColumn({ 
  field: "firstName",
  valueFormatter: (p) => `${p.data.lastName}, ${p.data.firstName}`
})
```

---

## ✅ Unit Tests Checklist

When PR adds tests for name formatting:

### Test Coverage
- [ ] Tests verify format: "LastName, FirstName" (no middle name)
- [ ] Tests verify format: "LastName, FirstName M" (with middle initial)
- [ ] Tests use `ComputeFullNameWithInitial()` helper
- [ ] Tests cover edge cases (null/empty middle name)

**Example of CORRECT test:**
```csharp
[Fact(DisplayName = "PS-XXXX: FullName formatted with middle initial")]
public void ComputeFullNameWithInitial_With_MiddleName()
{
    var result = DtoCommonExtensions.ComputeFullNameWithInitial("Smith", "John", "Michael");
    result.Should().Be("Smith, John M");  // ← Format: "LastName, FirstName M"
}

[Fact(DisplayName = "PS-XXXX: FullName formatted without middle name")]
public void ComputeFullNameWithInitial_Without_MiddleName()
{
    var result = DtoCommonExtensions.ComputeFullNameWithInitial("Smith", "John", null);
    result.Should().Be("Smith, John");  // ← Format: "LastName, FirstName"
}
```

---

## ✅ New Endpoint Checklist

When PR adds a completely new endpoint for person data:

### Complete Implementation
- [ ] Response DTO has `FullName` property (not `Name`)
- [ ] Service computes FullName using `ComputeFullNameWithInitial()`
- [ ] Query includes `LastName`, `FirstName`, `MiddleName`
- [ ] TypeScript DTO has `fullName` property
- [ ] Components use `object.fullName` (no concatenation)
- [ ] Grid columns specify `field: "fullName"` if applicable
- [ ] Unit tests verify format
- [ ] Format is "LastName, FirstName M" with middle initial

**Verification**:
Run this in PR description:
```markdown
## FullName Pattern Compliance

- [x] Backend DTO: FullName property ✓
- [x] Service: ComputeFullNameWithInitial() ✓
- [x] Query: Includes LastName, FirstName, MiddleName ✓
- [x] Frontend DTO: fullName property ✓
- [x] Components: Using .fullName (no concatenation) ✓
- [x] Grid columns: field: "fullName" ✓
- [x] Tests: Format verified "LastName, FirstName M" ✓
```

---

## 🚫 Common Violations

### Violation #1: DTO Named "Name"
**Severity**: 🔴 **HIGH** - Request Changes
```csharp
// ❌ WRONG
public string Name { get; set; }

// ✅ CORRECT
public string FullName { get; set; }
```

**Reviewer Action**: 
> This Response DTO should use `FullName` (not `Name`) per the FullName consolidation pattern. See `.github/FULLNAME_CONSOLIDATION_GUIDE.md`

---

### Violation #2: Direct FullName Assignment
**Severity**: 🔴 **HIGH** - Request Changes
```csharp
// ❌ WRONG
FullName = entity.ContactInfo.FullName

// ✅ CORRECT
FullName = DtoCommonExtensions.ComputeFullNameWithInitial(
    entity.ContactInfo.LastName,
    entity.ContactInfo.FirstName,
    entity.ContactInfo.MiddleName)
```

**Reviewer Action**:
> FullName should be computed using `ComputeFullNameWithInitial()` to ensure middle initial formatting is applied. The service needs to include `LastName`, `FirstName`, and `MiddleName` in the query SELECT.

---

### Violation #3: Manual Concatenation
**Severity**: 🟡 **MEDIUM** - Request Changes
```typescript
// ❌ WRONG
<span>{person.firstName} {person.lastName}</span>

// ✅ CORRECT
<span>{person.fullName}</span>
```

**Reviewer Action**:
> Use the backend-provided `fullName` property instead of manual concatenation. This ensures consistent formatting and eliminates duplicate logic.

---

### Violation #4: Wrong Grid Field
**Severity**: 🟡 **MEDIUM** - Request Changes
```typescript
// ❌ WRONG
createNameColumn({})  // Defaults to employeeName

// ✅ CORRECT
createNameColumn({ field: "fullName" })
```

**Reviewer Action**:
> Grid column should explicitly specify `field: "fullName"` to match the DTO property.

---

## ℹ️ Reference Links

| Document | Purpose | Read Time |
|----------|---------|-----------|
| [FULLNAME_CONSOLIDATION_GUIDE.md](/.github/FULLNAME_CONSOLIDATION_GUIDE.md) | Implementation guide | 5 min |
| [fullname-pattern.instructions.md](/.github/instructions/fullname-pattern.instructions.md) | Mandatory rules (for developers) | 5 min |
| [FULLNAME_PATTERN_PREVENTION.md](/.github/FULLNAME_PATTERN_PREVENTION.md) | Automated checks setup | 10 min |
| [FULLNAME_QUICK_AUDIT.md](/.github/FULLNAME_QUICK_AUDIT.md) | Audit commands and status | 3 min |
| [FULLNAME_CONSOLIDATION_SUMMARY.md](/.github/FULLNAME_CONSOLIDATION_SUMMARY.md) | Complete summary | 5 min |

---

## 💡 Reviewer Tips

1. **Look for the helper**: Is `ComputeFullNameWithInitial()` being used?
2. **Check the format**: Does output match "LastName, FirstName M"?
3. **Search for concatenation**: Use `grep` to find manual string building
4. **Verify DTOs**: Are names in TypeScript DTO named `fullName`?
5. **Test the format**: Check unit tests verify the expected format

---

**Created**: November 19, 2025  
**For**: Pull Request Reviews  
**Related**: PS-1829 (FullName consolidation)

