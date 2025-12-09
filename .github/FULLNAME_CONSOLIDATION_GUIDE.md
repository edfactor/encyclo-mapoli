# FullName Consolidation Guide

## Overview

This document establishes the standard pattern for handling employee/beneficiary name fields across the Profit Sharing application. All name information must be consolidated into a backend-provided `fullName` property formatted consistently with middle initials.

## ✅ Correct Pattern

### Backend (C#)

**1. DTO Response Definition**

```csharp
public sealed record MyResponse
{
    [MaskSensitive]
    public required string FullName { get; set; }  // ← Always use FullName (not Name)
}
```

**2. Service Mapping (Query Results)**

- Fetch individual name parts from the database: `LastName`, `FirstName`, `MiddleName`
- Compute FullName using the helper method:

```csharp
FullName = DtoCommonExtensions.ComputeFullNameWithInitial(
    lastName,
    firstName,
    middleName)
```

**3. Format Specification**

- Format: `"LastName, FirstName M"` (with middle initial only)
- Format: `"LastName, FirstName"` (without middle name)
- Helper method: `DtoCommonExtensions.ComputeFullNameWithInitial(lastName, firstName, middleName?)`

### Frontend (TypeScript)

**1. DTO Interface Definition**

```typescript
export interface MyResponseDto {
  fullName: string; // ← Always use fullName (not name)
}
```

**2. Component Usage**

```typescript
// ✅ CORRECT: Use backend-provided fullName
<span>{person.fullName}</span>

// ❌ WRONG: Manual concatenation
<span>{person.firstName} {person.lastName}</span>
```

## 📋 Implementation Checklist

When adding a new endpoint that includes person names:

- [ ] **Backend DTO**: Add `FullName` property (NOT `Name`)
- [ ] **Service Layer**: Compute FullName using `ComputeFullNameWithInitial()`
- [ ] **Query**: Include `LastName`, `FirstName`, `MiddleName` in SELECT
- [ ] **TypeScript DTO**: Import and add `fullName?: string` property
- [ ] **Grid Columns**: Use `createNameColumn({ field: "fullName" })`
- [ ] **Components**: Reference `object.fullName` directly (no concatenation)
- [ ] **Tests**: Verify format is "LastName, FirstName M" with middle initial

## 🔍 Search Commands for Auditing

### Find remaining stragglers in backend:

```powershell
# Search for properties named "Name" in response DTOs (excluding lookup types)
grep -r "public string Name\b" src/services/src/Demoulas.ProfitSharing.Common/Contracts/Response --include="*.cs" | grep -v "Kind\|Type\|Frequency\|Status\|Code"

# Search for direct FullName assignment without ComputeFullNameWithInitial
grep -r "FullName\s*=\s*" src/services/src/Demoulas.ProfitSharing.Services --include="*.cs" | grep -v "ComputeFullNameWithInitial"
```

### Find remaining stragglers in frontend:

```bash
# Search for .name property access on person objects
grep -r "\.name\b" src/ui/src --include="*.tsx" --include="*.ts" | grep -v "headerName\|displayName\|path.*Name\|kind\.name\|status.*Name"

# Search for manual firstName/lastName concatenation
grep -r "\[firstName\|lastName\]\|firstName\s*\+\|lastName\s*\+" src/ui/src --include="*.tsx" --include="*.ts"
```

## ⚠️ Common Mistakes to Avoid

### ❌ Backend Mistakes

1. **Using stored FullName directly without middle initial**

```csharp
// WRONG - database FullName may not have middle initial formatting
FullName = entity.ContactInfo.FullName

// CORRECT - compute with middle initial
FullName = DtoCommonExtensions.ComputeFullNameWithInitial(
    entity.ContactInfo.LastName,
    entity.ContactInfo.FirstName,
    entity.ContactInfo.MiddleName)
```

2. **Named property "Name" instead of "FullName"**

```csharp
// WRONG
public string Name { get; set; }

// CORRECT
public string FullName { get; set; }
```

3. **Forgetting to fetch individual name parts in queries**

```csharp
// WRONG - FullName field only
select new { FullName = e.ContactInfo.FullName }

// CORRECT - fetch parts for computation
select new {
    LastName = e.ContactInfo.LastName,
    FirstName = e.ContactInfo.FirstName,
    MiddleName = e.ContactInfo.MiddleName
}
```

### ❌ Frontend Mistakes

1. **Using .name property on objects**

```typescript
// WRONG
<span>{person.name}</span>

// CORRECT
<span>{person.fullName}</span>
```

2. **Manual concatenation of firstName/lastName**

```typescript
// WRONG
const displayName = `${person.firstName} ${person.lastName}`;

// CORRECT
const displayName = person.fullName;
```

3. **Grid columns with wrong field mapping**

```typescript
// WRONG - field defaults to "employeeName"
createNameColumn({});

// CORRECT - explicitly map to fullName
createNameColumn({ field: "fullName" });
```

## 📝 Affected Services/Endpoints

These endpoints correctly implement the FullName pattern:

- **ExecutiveHoursAndDollarsEndpoint** - Uses `ComputeFullNameWithInitial`
- **BreakdownReportEndpoint** - Uses `ComputeFullNameWithInitial` with null-safe pattern
- **BeneficiaryDetailEndpoint** - Uses `ComputeFullNameWithInitial`
- **DistributionSearchEndpoint** - Uses `ComputeFullNameWithInitial` with dual source (Demographic/Beneficiary)
- **BeneficiaryInquiryService** - All queries use computed FullName with middle initial

## 🛠️ Troubleshooting

### If names display without middle initials:

1. Check if service is using `ComputeFullNameWithInitial`
2. Verify query is fetching `MiddleName` field
3. Check database has MiddleName populated for test records

### If TypeScript compilation fails:

1. Verify DTO has `fullName` property (not `name`)
2. Check grid column uses correct field binding: `field: "fullName"`
3. Ensure all response objects mapped to updated DTO

### If old pattern appears in new code:

1. Run search commands above to identify location
2. Refactor to use `DtoCommonExtensions.ComputeFullNameWithInitial`
3. Update TypeScript DTO to use `fullName`
4. Remove manual concatenation in components

## References

- **Backend Helper**: `DtoCommonExtensions.ComputeFullNameWithInitial()`
- **Location**: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Shared/DtoCommonExtensions.cs`
- **Tests**: `DtoCommonExtensionsTests.cs`, `ExecutiveHoursAndDollarsTests.cs`
- **Related Ticket**: PS-1829 (FullName formatting with middle initial)

---

**Last Updated**: November 19, 2025  
**Maintained By**: Development Team
