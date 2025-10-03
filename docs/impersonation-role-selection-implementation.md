# Impersonation Role Selection - Implementation Summary

## Overview

Implemented validation logic for impersonation role selection in the Profit Sharing UI that enforces the following business rules:

1. **Default behavior**: Only one impersonation role can be selected at a time
2. **Exception**: The `Executive-Administrator` role can be combined with any of these roles:
   - `Finance-Manager`
   - `Distributions-Clerk`
   - `Hardship-Administrator`
   - `System-Administrator` (ProfitSharingAdministrator)

## Changes Made

### 1. Role Validation Utilities (`src/ui/src/utils/roleUtils.ts`)

Added two new validation functions:

#### `validateImpersonationRoles()`
Validates whether a new role can be added to the current selection based on business rules.

**Logic:**
- If no roles are currently selected, allow the new role
- If adding `Executive-Administrator` to an existing combinable role, allow the combination
- If adding a combinable role to existing `Executive-Administrator`, allow the combination
- For all other cases, replace all existing roles with the new role (single selection)

#### `validateRoleRemoval()`
Validates role removal and ensures consistency when Executive-Administrator is removed.

**Logic:**
- If removing `Executive-Administrator` and multiple combinable roles remain, keep only the first combinable role
- Otherwise, simply remove the specified role

**Combinable roles list:**
```typescript
const COMBINABLE_WITH_EXECUTIVE_ADMIN = [
  ImpersonationRoles.FinanceManager,
  ImpersonationRoles.DistributionsClerk,
  ImpersonationRoles.HardshipAdministrator,
  ImpersonationRoles.ProfitSharingAdministrator
];
```

### 2. Router Integration (`src/ui/src/components/router/RouterSubAssembly.tsx`)

Updated the `ImpersonationMultiSelect` component's `setCurrentRoles` callback to:
- Detect whether a role was added or removed
- Apply appropriate validation logic
- Update Redux state and localStorage with validated roles

**Implementation details:**
- Compares array lengths to determine if role was added or removed
- Finds the specific role that changed
- Calls appropriate validation function
- Ensures UI reflects validated role selection

### 3. Unit Tests (`src/ui/src/utils/roleUtils.test.ts`)

Created comprehensive test coverage for:
- Single role selection scenarios
- Executive-Administrator with combinable roles
- Non-combinable scenarios
- Multiple combinable roles without Executive-Administrator
- Role removal scenarios

**Test coverage:**
- ✅ All tests passing
- ✅ Covers all business rule scenarios
- ✅ Edge cases handled

## User Experience

### Scenario 1: Selecting a Single Role
- User selects "Auditor" → Only "Auditor" is selected
- User then selects "IT-DevOps" → "Auditor" is deselected, only "IT-DevOps" is selected

### Scenario 2: Combining with Executive-Administrator
- User selects "Finance-Manager" → Only "Finance-Manager" is selected
- User then selects "Executive-Administrator" → Both roles are now selected
- User can continue selecting other combinable roles while Executive-Administrator is selected

### Scenario 3: Breaking a Combination
- User has "Executive-Administrator" and "Finance-Manager" selected
- User selects "Auditor" → All previous roles are deselected, only "Auditor" is selected

### Scenario 4: Removing Executive-Administrator
- User has "Executive-Administrator", "Finance-Manager", and "Distributions-Clerk" selected
- User deselects "Executive-Administrator" → Only "Finance-Manager" remains (first combinable role)

## Technical Implementation Notes

### Why This Approach?

1. **Validation at the UI layer**: Prevents invalid states before they reach Redux/backend
2. **Explicit business rules**: Clear documentation of which roles can be combined
3. **User-friendly**: UI automatically adjusts selection based on rules
4. **Maintainable**: Centralized validation logic in `roleUtils.ts`

### Design Decisions

1. **Array comparison approach**: Determines intent (add vs remove) by comparing old and new arrays
2. **Keep-first strategy**: When Executive-Administrator is removed from multiple combinable roles, keeps the first one for predictable behavior
3. **Immutable updates**: Creates new arrays rather than mutating existing ones
4. **Type-safe**: Uses TypeScript enums for role names

## Testing Strategy

### Unit Tests
- Comprehensive test suite in `roleUtils.test.ts`
- Tests all business rule scenarios
- Tests edge cases and error conditions

### Manual Testing Checklist
- [ ] Verify single role selection works
- [ ] Verify Executive-Administrator can combine with Finance-Manager
- [ ] Verify Executive-Administrator can combine with Distributions-Clerk
- [ ] Verify Executive-Administrator can combine with Hardship-Administrator
- [ ] Verify Executive-Administrator can combine with System-Administrator
- [ ] Verify non-combinable roles replace existing selection
- [ ] Verify removing Executive-Administrator keeps only one combinable role
- [ ] Verify localStorage persists validated roles correctly
- [ ] Verify navigation restrictions apply correctly with combined roles

## Future Considerations

### Potential Enhancements
1. **Toast notifications**: Show user-friendly message when roles are auto-deselected
2. **Hover tooltips**: Explain which roles can be combined when hovering over role options
3. **Visual indicators**: Mark combinable roles with a special icon or badge
4. **Order preservation**: Maintain user's selection order rather than keeping first role

### Backend Integration
- Ensure backend validates role combinations for security
- Update API documentation to reflect role combination rules
- Add telemetry to track role combination usage patterns

## Files Modified

1. `src/ui/src/utils/roleUtils.ts` - Added validation functions
2. `src/ui/src/components/router/RouterSubAssembly.tsx` - Integrated validation logic
3. `src/ui/src/utils/roleUtils.test.ts` - Added comprehensive unit tests

## Documentation References

- Main project instructions: `.github/copilot-instructions.md`
- TypeScript enums: `src/ui/src/types/common/enums.ts`
- Redux security slice: `src/ui/src/reduxstore/slices/securitySlice.ts`

---

**Implementation Date**: October 3, 2025
**Status**: ✅ Complete
**Tests**: ✅ All passing
