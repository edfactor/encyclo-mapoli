# Master Inquiry Service Refactoring Summary

## Overview

The `MasterInquiryService` has been refactored from a monolithic 1483-line service into a cleaner architecture with separated concerns:

### New Architecture

```
MasterInquiryService (Orchestrator)
├── EmployeeMasterInquiryService (Employee/Demographic lookups)
├── BeneficiaryMasterInquiryService (Beneficiary lookups)
└── MasterInquiryHelpers (Shared filtering & sorting utilities)
```

## Files Created

### 1. **MasterInquiryDtos.cs**

Shared DTOs used across all inquiry services:

- `MasterInquiryItem` - Core inquiry item with profit details
- `InquiryDemographics` - Simplified demographic representation
- `MasterInquiryRawDto` - SQL-translatable projection DTO

### 2. **IEmployeeMasterInquiryService.cs**

Interface defining employee/demographic lookup operations:

- `GetEmployeeInquiryQueryAsync()` - Get queryable employee inquiry items
- `GetEmployeeDetailsAsync()` - Get detailed info for specific employee ID
- `GetEmployeeDetailsForSsnsAsync()` - Paginated employee details for SSN collection
- `GetAllEmployeeDetailsForSsnsAsync()` - All employee details (no pagination)
- `FindEmployeeSsnByBadgeAsync()` - Find SSN by badge number

### 3. **EmployeeMasterInquiryService.cs**

Implementation handling all employee-specific master inquiry logic:

- Optimized EF Core 9 queries with `AsSplitQuery`, `TagWith`
- Missive lookups via `IMissiveService`
- Duplicate SSN detection and badge mapping
- Demographics via `IDemographicReaderService`
- ~500 lines focused solely on employee operations

### 4. **IBeneficiaryMasterInquiryService.cs**

Interface defining beneficiary lookup operations:

- `GetBeneficiaryInquiryQuery()` - Get queryable beneficiary inquiry items
- `GetBeneficiaryDetailsAsync()` - Get detailed info for specific beneficiary ID
- `GetBeneficiaryDetailsForSsnsAsync()` - Paginated beneficiary details
- `GetAllBeneficiaryDetailsForSsnsAsync()` - All beneficiary details
- `FindBeneficiarySsnByBadgeAndPsnAsync()` - Find SSN by badge + PSN suffix

### 5. **BeneficiaryMasterInquiryService.cs**

Implementation handling all beneficiary-specific master inquiry logic:

- Optimized queries for beneficiary contacts
- No dependency on `IMissiveService` or `IDemographicReaderService`
- Simpler than employee service (~250 lines)
- Focused solely on beneficiary operations

### 6. **MasterInquiryHelpers.cs**

Static utility class with shared logic:

- `FilterMemberQuery()` - Apply comprehensive filters (selectivity-ordered)
- `ApplySorting()` - Apply sorting to member details
- `ShouldUseOptimizedSsnQuery()` - Determine if fast SSN-first path should be used

## Refactored MasterInquiryService

The new `MasterInquiryService` will be significantly simplified:

- **Remove:** All private helper methods moved to specialized services
- **Keep:** Public interface implementation (`GetMembersAsync`, `GetMemberVestingAsync`, etc.)
- **Add:** Dependencies on `IEmployeeMasterInquiryService` and `IBeneficiaryMasterInquiryService`
- **Role:** Pure orchestration - delegates to specialized services, combines results

### Key Methods Remaining in MasterInquiryService

1. `GetMembersAsync()` - Orchestrates employee/beneficiary lookups, combines results
2. `GetMemberVestingAsync()` - Calls employee/beneficiary services, adds vesting calculations
3. `GetMemberProfitDetails()` - Delegates to appropriate service based on member type
4. `GetGroupedProfitDetails()` - Groups profit details across member types
5. Private helpers:
   - `GetSsnsFromProfitDetails()` - Optimized SSN extraction from profit details
   - `HandleExactBadgeOrSsn()` - Handle exact lookups for members without transactions
   - `GetVestingDetails()` - Calculate vesting information using `ITotalService`

## Benefits of Refactoring

### 1. **Separation of Concerns**

- Employee logic isolated from beneficiary logic
- Each service has a single, clear responsibility
- Easier to reason about and modify individual components

### 2. **Testability**

- Can unit test employee lookups independently
- Can unit test beneficiary lookups independently
- Can mock specialized services when testing orchestrator
- Smaller services = easier to achieve high code coverage

### 3. **Maintainability**

- Finding bugs easier with focused services
- Changes to employee logic don't risk breaking beneficiary logic
- Clearer code ownership and boundaries

### 4. **Performance Optimization**

- Each service can optimize for its specific data model
- No unnecessary dependencies (e.g., beneficiaries don't need missive service)
- Easier to identify and fix performance bottlenecks in isolated code

### 5. **Code Reusability**

- Employee/beneficiary services can be used by other features
- Shared helpers available for other inquiry-related features
- DTOs can be reused across multiple services

## Testing Strategy

### Unit Tests for EmployeeMasterInquiryService

- Test query building with various filters
- Test demographic detail retrieval
- Test missive integration
- Test duplicate SSN handling
- Test pagination logic

### Unit Tests for BeneficiaryMasterInquiryService

- Test query building for beneficiaries
- Test beneficiary detail retrieval
- Test contact information projection
- Test badge + PSN suffix lookups

### Integration Tests for MasterInquiryService

- Test orchestration of employee + beneficiary services
- Test combined result sets
- Test sorting across combined results
- Test pagination with mixed member types
- Test vesting details calculation

## Migration Notes

### Breaking Changes

- None - public interface (`IMasterInquiryService`) remains unchanged
- All existing endpoints continue to work without modification

### Internal Changes

- Private methods moved to specialized services
- DTOs extracted to shared file
- New service registrations required in DI container

### Deployment Considerations

- Register new services in `ServicesExtension.cs`
- No database changes required
- No API contract changes
- Backward compatible with existing clients

## Next Steps

1. ✅ Create shared DTOs (`MasterInquiryDtos.cs`)
2. ✅ Create employee inquiry interface and implementation
3. ✅ Create beneficiary inquiry interface and implementation
4. ✅ Create shared helpers (`MasterInquiryHelpers.cs`)
5. ⏳ Refactor `MasterInquiryService` to use new services
6. ⏳ Register new services in DI container
7. ⏳ Create comprehensive unit tests
8. ⏳ Verify existing endpoints still work
9. ⏳ Update documentation

## File Sizes (Approximate)

| File                                | Lines     | Purpose                |
| ----------------------------------- | --------- | ---------------------- |
| **Original**                        |           |                        |
| MasterInquiryService.cs (old)       | 1483      | Monolithic service     |
| **Refactored**                      |           |                        |
| MasterInquiryService.cs (new)       | ~400      | Orchestrator only      |
| EmployeeMasterInquiryService.cs     | ~500      | Employee operations    |
| BeneficiaryMasterInquiryService.cs  | ~250      | Beneficiary operations |
| MasterInquiryHelpers.cs             | ~180      | Shared utilities       |
| MasterInquiryDtos.cs                | ~95       | Shared DTOs            |
| IEmployeeMasterInquiryService.cs    | ~100      | Employee interface     |
| IBeneficiaryMasterInquiryService.cs | ~85       | Beneficiary interface  |
| **Total (new)**                     | **~1610** | Separated & testable   |

The slight increase in total lines is due to interfaces, XML documentation, and cleaner separation - but each file is now focused, testable, and maintainable.
