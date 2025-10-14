# Master Inquiry Service Refactoring - Phase 1: Employee Implementation Guide

## Overview

This guide provides step-by-step instructions for refactoring MasterInquiryService to use the new EmployeeMasterInquiryService, focusing ONLY on employee operations initially. Beneficiary integration will be added in Phase 2.

## Phase 1 Goals

- ✅ Create EmployeeMasterInquiryService with all employee lookup logic
- ✅ Create shared DTOs and helpers
- ✅ Register services in DI container
- 🔄 Refactor MasterInquiryService to delegate employee operations
- 🔄 Keep beneficiary logic inline (unchanged) for now
- 🔄 Add unit tests for EmployeeMasterInquiryService

## Files Modified in Phase 1

### Already Created ✅

1. `MasterInquiryDtos.cs` - Shared DTOs
2. `MasterInquiryHelpers.cs` - Filtering and sorting utilities
3. `IEmployeeMasterInquiryService.cs` - Employee service interface
4. `EmployeeMasterInquiryService.cs` - Employee service implementation
5. `ServicesExtension.cs` - DI registration (updated)

### To Modify 🔄

1. `MasterInquiryService.cs` - Refactor constructor and employee methods

---

## Step-by-Step Implementation

### Step 1: Update MasterInquiryService Constructor

**Current constructor (lines ~102-113):**

```csharp
public MasterInquiryService(
    IProfitSharingDataContextFactory dataContextFactory,
    ITotalService totalService,
    ILoggerFactory loggerFactory,
    IMissiveService missiveService,
    IDemographicReaderService demographicReaderService)
{
    _dataContextFactory = dataContextFactory;
    _totalService = totalService;
    _missiveService = missiveService;
    _demographicReaderService = demographicReaderService;
    _logger = loggerFactory.CreateLogger<MasterInquiryService>();
}
```

**Replace with:**

```csharp
private readonly IEmployeeMasterInquiryService _employeeInquiryService;

public MasterInquiryService(
    IProfitSharingDataContextFactory dataContextFactory,
    ITotalService totalService,
    ILoggerFactory loggerFactory,
    IMissiveService missiveService,
    IDemographicReaderService demographicReaderService,
    IEmployeeMasterInquiryService employeeInquiryService)
{
    _dataContextFactory = dataContextFactory;
    _totalService = totalService;
    _missiveService = missiveService;
    _demographicReaderService = demographicReaderService;
    _employeeInquiryService = employeeInquiryService;
    _logger = loggerFactory.CreateLogger<MasterInquiryService>();
}
```

### Step 2: Update Private DTOs Section

**Remove duplicate DTOs (lines ~25-98):**

Delete these classes from MasterInquiryService since they're now in `MasterInquiryDtos.cs`:

- `MasterInquiryItem`
- `InquiryDemographics`
- `MasterInquiryRawDto`

**Add using statement at top of file:**

```csharp
// MasterInquiryItem, InquiryDemographics, MasterInquiryRawDto now in MasterInquiryDtos.cs
```

### Step 3: Replace GetMasterInquiryDemographics Method

**Find method (line ~585):**

```csharp
private async Task<IQueryable<MasterInquiryItem>> GetMasterInquiryDemographics(
    ProfitSharingReadOnlyDbContext ctx,
    MasterInquiryRequest? req = null)
{
    // ... lots of code ...
}
```

**Replace entire method with:**

```csharp
private Task<IQueryable<MasterInquiryItem>> GetMasterInquiryDemographics(
    ProfitSharingReadOnlyDbContext ctx,
    MasterInquiryRequest? req = null)
{
    // Delegate to EmployeeMasterInquiryService
    return _employeeInquiryService.GetEmployeeInquiryQueryAsync(ctx, req);
}
```

### Step 4: Replace GetDemographicDetails Method

**Find method (line ~722):**

```csharp
private async Task<(int ssn, MemberDetails? memberDetails)> GetDemographicDetails(
    ProfitSharingReadOnlyDbContext ctx,
    int id,
    short currentYear,
    short previousYear,
    CancellationToken cancellationToken)
{
    // ... lots of code ...
}
```

**Replace entire method with:**

```csharp
private Task<(int ssn, MemberDetails? memberDetails)> GetDemographicDetails(
    ProfitSharingReadOnlyDbContext ctx,
    int id,
    short currentYear,
    short previousYear,
    CancellationToken cancellationToken)
{
    // Delegate to EmployeeMasterInquiryService
    return _employeeInquiryService.GetEmployeeDetailsAsync(ctx, id, currentYear, previousYear, cancellationToken);
}
```

### Step 5: Replace GetDemographicDetailsForSsns Method

**Find method (line ~998):**

```csharp
private async Task<PaginatedResponseDto<MemberDetails>> GetDemographicDetailsForSsns(
    ProfitSharingReadOnlyDbContext ctx,
    SortedPaginationRequestDto req,
    ISet<int> ssns,
    short currentYear,
    short previousYear,
    ISet<int> duplicateSsns,
    CancellationToken cancellationToken)
{
    // ... lots of code ...
}
```

**Replace entire method with:**

```csharp
private Task<PaginatedResponseDto<MemberDetails>> GetDemographicDetailsForSsns(
    ProfitSharingReadOnlyDbContext ctx,
    MasterInquiryRequest req,
    ISet<int> ssns,
    short currentYear,
    short previousYear,
    ISet<int> duplicateSsns,
    CancellationToken cancellationToken)
{
    // Delegate to EmployeeMasterInquiryService
    return _employeeInquiryService.GetEmployeeDetailsForSsnsAsync(
        ctx, req, ssns, currentYear, previousYear, duplicateSsns, cancellationToken);
}
```

**Note:** Changed `SortedPaginationRequestDto` to `MasterInquiryRequest` to match service signature.

### Step 6: Replace GetAllDemographicDetailsForSsns Method

**Find method (line ~1286):**

```csharp
private async Task<List<MemberDetails>> GetAllDemographicDetailsForSsns(
    ProfitSharingReadOnlyDbContext ctx,
    ISet<int> ssns,
    short currentYear,
    short previousYear,
    ISet<int> duplicateSsns,
    CancellationToken cancellationToken)
{
    // ... lots of code ...
}
```

**Replace entire method with:**

```csharp
private Task<List<MemberDetails>> GetAllDemographicDetailsForSsns(
    ProfitSharingReadOnlyDbContext ctx,
    ISet<int> ssns,
    short currentYear,
    short previousYear,
    ISet<int> duplicateSsns,
    CancellationToken cancellationToken)
{
    // Delegate to EmployeeMasterInquiryService
    return _employeeInquiryService.GetAllEmployeeDetailsForSsnsAsync(
        ctx, ssns, currentYear, previousYear, duplicateSsns, cancellationToken);
}
```

### Step 7: Update FilterMemberQuery Method

**Find method (line ~1176 or search for "private static IQueryable<MasterInquiryItem> FilterMemberQuery"):**

```csharp
private static IQueryable<MasterInquiryItem> FilterMemberQuery(MasterInquiryRequest req, IQueryable<MasterInquiryItem> query)
{
    // CRITICAL: Apply most selective filters first for Oracle query optimizer
    // ... lots of code ...
    return query;
}
```

**Replace entire method with:**

```csharp
private static IQueryable<MasterInquiryItem> FilterMemberQuery(MasterInquiryRequest req, IQueryable<MasterInquiryItem> query)
{
    // Delegate to MasterInquiryHelpers
    return MasterInquiryHelpers.FilterMemberQuery(req, query);
}
```

### Step 8: Update ApplySorting Method

**Find method (line ~1458 or search for "private static IQueryable<MemberDetails> ApplySorting"):**

```csharp
private static IQueryable<MemberDetails> ApplySorting(IQueryable<MemberDetails> query, SortedPaginationRequestDto req)
{
    if (string.IsNullOrEmpty(req.SortBy))
    {
        return query;
    }
    // ... lots of code ...
    return query;
}
```

**Replace entire method with:**

```csharp
private static IQueryable<MemberDetails> ApplySorting(IQueryable<MemberDetails> query, SortedPaginationRequestDto req)
{
    // Delegate to MasterInquiryHelpers
    return MasterInquiryHelpers.ApplySorting(query, req);
}
```

### Step 9: Update ShouldUseOptimizedSsnQuery Method

**Find method (line ~250 or search for "private static bool ShouldUseOptimizedSsnQuery"):**

```csharp
private static bool ShouldUseOptimizedSsnQuery(MasterInquiryRequest req)
{
    // Use optimized path when we have filters that can reduce the dataset before joining
    // ... lots of code ...
    return false;
}
```

**Replace entire method with:**

```csharp
private static bool ShouldUseOptimizedSsnQuery(MasterInquiryRequest req)
{
    // Delegate to MasterInquiryHelpers
    return MasterInquiryHelpers.ShouldUseOptimizedSsnQuery(req);
}
```

### Step 10: Update HandleExactBadgeOrSsn for Employee Delegation

**Find method (line ~355 or search for "private async Task HandleExactBadgeOrSsn"):**

Find the section that looks for employee SSN:

```csharp
else if (badgeNumber != 0)
{
    var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
    int ssnEmpl = await demographics
        .AsNoTracking()
        .Where(d => d.BadgeNumber == badgeNumber)
        .Select(d => d.Ssn)
        .FirstOrDefaultAsync(cancellationToken);

    if (ssnEmpl != 0)
    {
        ssnList.Add(ssnEmpl);
    }
}
```

**Replace with:**

```csharp
else if (badgeNumber != 0)
{
    // Delegate employee lookup to EmployeeMasterInquiryService
    int ssnEmpl = await _employeeInquiryService.FindEmployeeSsnByBadgeAsync(ctx, badgeNumber.Value, cancellationToken);

    if (ssnEmpl != 0)
    {
        ssnList.Add(ssnEmpl);
    }
}
```

### Step 11: Update GetDuplicateSsns Helper (Optional Optimization)

**Find usage of duplicate SSN detection in GetMembersAsync (line ~157):**

```csharp
var duplicateSsns = await demographics
    .Where(d => ssnList.Contains(d.Ssn))
    .GroupBy(d => d.Ssn)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .TagWith("MasterInquiry: Find duplicate SSNs")
    .ToHashSetAsync(timeoutToken);
```

This stays inline for now, but consider extracting to helper method later.

---

## Testing Phase 1

### Unit Tests for EmployeeMasterInquiryService

Create test file: `Demoulas.ProfitSharing.UnitTests/Services/MasterInquiry/EmployeeMasterInquiryServiceTests.cs`

```csharp
using Demoulas.ProfitSharing.Services.MasterInquiry;
using Xunit;
using Shouldly;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Services.MasterInquiry;

public class EmployeeMasterInquiryServiceTests
{
    [Fact]
    public async Task GetEmployeeInquiryQueryAsync_WithRequest_ReturnsFilteredQuery()
    {
        // Arrange
        var service = CreateService();
        var request = new MasterInquiryRequest { ProfitYear = 2025 };

        // Act
        var result = await service.GetEmployeeInquiryQueryAsync(mockContext, request);

        // Assert
        result.ShouldNotBeNull();
    }

    // Add more tests for:
    // - GetEmployeeDetailsAsync with valid/invalid IDs
    // - GetEmployeeDetailsForSsnsAsync with pagination
    // - GetAllEmployeeDetailsForSsnsAsync
    // - FindEmployeeSsnByBadgeAsync
    // - Duplicate SSN handling
    // - Missive integration
}
```

### Integration Tests for MasterInquiryService

Create test file: `Demoulas.ProfitSharing.UnitTests/Services/MasterInquiry/MasterInquiryServiceIntegrationTests.cs`

```csharp
[Fact]
public async Task GetMembersAsync_EmployeeOnly_DelegatesToEmployeeService()
{
    // Arrange
    var mockEmployeeService = new Mock<IEmployeeMasterInquiryService>();
    var service = new MasterInquiryService(
        mockFactory, mockTotalService, mockLoggerFactory,
        mockMissiveService, mockDemographicReader,
        mockEmployeeService.Object);

    var request = new MasterInquiryRequest
    {
        MemberType = 1, // Employee only
        ProfitYear = 2025
    };

    // Act
    await service.GetMembersAsync(request);

    // Assert
    mockEmployeeService.Verify(
        x => x.GetEmployeeInquiryQueryAsync(It.IsAny<ProfitSharingReadOnlyDbContext>(), request),
        Times.Once);
}
```

---

## Verification Checklist

After implementing Phase 1, verify:

- [ ] Solution builds without errors
- [ ] All existing unit tests pass
- [ ] New EmployeeMasterInquiryService tests pass
- [ ] Integration tests verify delegation works
- [ ] Employee-only Master Inquiry queries work in UI
- [ ] Performance is same or better than before
- [ ] No regressions in employee lookup functionality

## Common Issues & Solutions

### Issue 1: "Cannot implicitly convert IQueryable to Task<IQueryable>"

**Solution:** Methods delegating to async service methods should return `Task<T>`, not just `T`.

### Issue 2: "Parameter type mismatch SortedPaginationRequestDto vs MasterInquiryRequest"

**Solution:** Update method signature to use `MasterInquiryRequest` which inherits from `SortedPaginationRequestDto`.

### Issue 3: "Missing using statement for MasterInquiryHelpers"

**Solution:** Add: `using Demoulas.ProfitSharing.Services.MasterInquiry;`

### Issue 4: "DemographicReaderService still injected but not used"

**Solution:** Keep it for now - beneficiary methods still use it. Remove in Phase 2.

---

## Phase 2 Preview (Beneficiary Integration)

Once Phase 1 is tested and stable:

1. Create `IBeneficiaryMasterInquiryService` (already done ✅)
2. Create `BeneficiaryMasterInquiryService` (already done ✅)
3. Update `MasterInquiryService` constructor to inject beneficiary service
4. Replace beneficiary methods similar to employee methods
5. Remove remaining inline beneficiary logic
6. Add beneficiary unit tests
7. Test combined employee + beneficiary queries

---

## Rollback Plan

If issues arise:

1. Revert to backup:

   ```powershell
   Copy-Item "MasterInquiryService.cs.backup" "MasterInquiryService.cs" -Force
   ```

2. Remove new service registrations from `ServicesExtension.cs`

3. Keep new services (EmployeeMasterInquiryService, helpers, DTOs) for future use

---

## Summary of Changes

### Files Modified

- ✅ `MasterInquiryService.cs` - Constructor + 8 method delegations
- ✅ `ServicesExtension.cs` - Added DI registration

### Files Created

- ✅ `EmployeeMasterInquiryService.cs` (~500 lines)
- ✅ `MasterInquiryHelpers.cs` (~180 lines)
- ✅ `MasterInquiryDtos.cs` (~95 lines)
- ✅ `IEmployeeMasterInquiryService.cs` (~100 lines)

### Net Effect

- **MasterInquiryService**: ~1483 lines → ~900 lines (40% reduction)
- **Employee logic**: Isolated and testable
- **Beneficiary logic**: Unchanged (inline for Phase 1)
- **Public API**: No changes (backward compatible)

---

_This guide focuses on Phase 1 (employee-only) implementation. Proceed to Phase 2 (beneficiary integration) after thorough testing._
