---
applyTo: "src/ui/src/pages/Beneficiaries/**/*.*"
paths: "src/ui/src/pages/Beneficiaries/**/*.*"
---

# Beneficiaries Module Technical Documentation

Technical reference for `./src/ui/src/pages/Beneficiaries/`

## Overview

The Beneficiaries module provides functionality for managing profit sharing beneficiaries, including searching for members, viewing beneficiary relationships, creating/editing beneficiaries, and managing percentage allocations.

## Directory Structure

```
Beneficiaries/
├── BeneficiaryInquiry.tsx              # Main page component
├── BeneficiaryInquirySearchFilter.tsx  # Search filter form
├── MemberResultsGrid.tsx               # Member search results grid
├── MemberResultsGridColumns.tsx        # Column definitions for member grid
├── IndividualBeneficiaryView.tsx       # Individual member view container
├── MemberDetailsPanel.tsx              # Member details display panel
├── BeneficiaryRelationshipsGrids.tsx   # Displays beneficiary relationships
├── BeneficiariesListGridColumns.ts     # Column definitions for beneficiaries list
├── BeneficiaryOfGridColumns.tsx        # Column definitions for "beneficiary of" grid
├── BeneficiaryActions.tsx              # Action cell renderer (edit/delete/distribute)
├── CreateBeneficiary.tsx               # Beneficiary create/edit form
├── CreateBeneficiaryDialog.tsx         # Modal wrapper for create/edit form
├── DeleteBeneficiaryDialog.tsx         # Delete confirmation dialog
├── hooks/
│   ├── index.ts                        # Exports all hooks
│   ├── useBeneficiarySearch.ts         # Search pagination/sort state
│   ├── useBeneficiaryRelationshipData.ts # Relationship data fetching
│   ├── useBeneficiaryPercentageUpdate.ts # Percentage update validation
│   └── __test__/
│       ├── useBeneficiarySearch.test.ts
│       ├── useBeneficiaryRelationshipData.test.ts
│       └── useBeneficiaryPercentageUpdate.test.ts
└── utils/
    ├── index.ts                        # Exports all utilities
    ├── badgeUtils.ts                   # Badge/PSN parsing utilities
    ├── percentageUtils.ts              # Percentage allocation utilities
    ├── Claude.md                       # Utility documentation
    └── __test__/
        ├── badgeUtils.test.ts
        └── percentageUtils.test.ts
```

## Component Hierarchy

```
BeneficiaryInquiry
├── MissiveAlertProvider (context)
├── BeneficiaryInquirySearchFilter
├── MemberResultsGrid (shown when multiple results)
└── IndividualBeneficiaryView (shown when member selected)
    ├── MemberDetailsPanel
    ├── CreateBeneficiaryDialog
    │   └── CreateBeneficiary (form)
    └── BeneficiaryRelationshipsGrids
        ├── "Beneficiary Of" grid
        ├── "Beneficiaries" grid (with inline percentage editing)
        ├── DeleteBeneficiaryDialog
        └── Snackbar (percentage update feedback)
```

## Key Components

### BeneficiaryInquiry.tsx

Main page component wrapped in `MissiveAlertProvider`. Handles:

- Member search via `useLazyBeneficiarySearchFilterQuery`
- Auto-selection when single result returned
- Navigation state for distributions

### IndividualBeneficiaryView.tsx

Container for viewing a selected member's beneficiary information:

- Manages create/edit dialog state
- Tracks beneficiary changes for grid refresh
- Coordinates between `MemberDetailsPanel` and `BeneficiaryRelationshipsGrids`

### BeneficiaryRelationshipsGrids.tsx

Displays two grids showing bidirectional beneficiary relationships:

- **Beneficiary Of**: Members for whom the selected person is a beneficiary
- **Beneficiaries**: People designated as beneficiaries by the selected member
- Inline editable percentage field with validation
- Action column (New Distribution, Edit, Delete)

### CreateBeneficiary.tsx

Form for creating/editing beneficiaries using `react-hook-form` with Yup validation:

- Fields: First/Last Name, SSN, DOB, Address, Relationship
- Checkbox for copying address from employee
- Uses mutations: `useCreateBeneficiariesMutation`, `useCreateBeneficiaryContactMutation`, `useUpdateBeneficiaryMutation`

### BeneficiaryActions.tsx

Cell renderer for action column with icon buttons:

- New Distribution (navigates to Add Distribution)
- Edit (opens edit dialog)
- Delete (opens confirmation dialog)

## Custom Hooks

### useBeneficiarySearch

Manages search pagination and sort state for member search grid.

```typescript
const search = useBeneficiarySearch({
  defaultPageSize: 10,
  defaultSortBy: "name",
});
// Returns: pageNumber, pageSize, sortParams, handlePaginationChange, reset
```

### useBeneficiaryRelationshipData

Fetches beneficiary relationship data with pagination support.

```typescript
const relationships = useBeneficiaryRelationshipData({
  selectedMember,
  pageNumber,
  pageSize,
  sortParams,
  externalRefreshTrigger: count,
});
// Returns: beneficiaryList, beneficiaryOfList, isLoading, refresh()
```

### useBeneficiaryPercentageUpdate

Validates and updates beneficiary percentage allocations.

```typescript
const percentageUpdate = useBeneficiaryPercentageUpdate(onRefresh);
const result = await percentageUpdate.validateAndUpdate(
  id,
  newValue,
  beneficiaryList,
);
// Validates: 0-100 range, sum <= 100%
// Returns: { success, previousValue, error?, warning? }
```

## Utility Functions

### badgeUtils.ts

- `parseBadgeAndPSN(input)` - Separates badge (1-7 digits) from PSN (8+ digits)
- `detectMemberTypeFromBadge(badge)` - Returns member type (0=All, 1=Employee, 2=Beneficiary)
- `isValidBadgeIdentifiers(badge, psn)` - Validates required identifiers
- `decomposePSNSuffix(psn)` - Extracts three-level beneficiary hierarchy

### percentageUtils.ts

- `calculatePercentageSum(items, id, newValue)` - Calculates sum with proposed change
- `validatePercentageAllocation(sum)` - Validates sum doesn't exceed 100%

## API Integration

The module uses RTK Query hooks from `reduxstore/api/BeneficiariesApi.ts`:

| Hook                                  | Type     | Purpose                       |
| ------------------------------------- | -------- | ----------------------------- |
| `useLazyBeneficiarySearchFilterQuery` | Query    | Search members                |
| `useLazyGetBeneficiaryDetailQuery`    | Query    | Get member details            |
| `useLazyGetBeneficiariesQuery`        | Query    | Get beneficiary relationships |
| `useLazyGetBeneficiarytypesQuery`     | Query    | Get beneficiary type options  |
| `useCreateBeneficiariesMutation`      | Mutation | Create beneficiary record     |
| `useCreateBeneficiaryContactMutation` | Mutation | Create contact record         |
| `useUpdateBeneficiaryMutation`        | Mutation | Update beneficiary/percentage |
| `useDeleteBeneficiaryMutation`        | Mutation | Delete beneficiary            |

## Key Types

```typescript
interface BeneficiaryDetail {
  badgeNumber: number;
  psnSuffix: number;
  ssn: string;
  firstName: string;
  lastName: string;
  street: string;
  city: string;
  state: string;
  zip: string;
  // ... additional fields
}

interface BeneficiaryDto {
  id: number;
  badgeNumber: number;
  psnSuffix: number;
  psn: string;
  firstName: string;
  lastName: string;
  percent: number;
  relationship: string;
  beneficiaryContactId: number;
  // ... additional fields
}
```

## Workflow

1. **Search**: User enters badge/SSN in filter, clicks Search
2. **Select**: If multiple results, user clicks row in `MemberResultsGrid`
3. **View**: `IndividualBeneficiaryView` displays member details and relationships
4. **Create/Edit**: "Add Beneficiary" button or Edit action opens `CreateBeneficiaryDialog`
5. **Percentage Update**: Inline editing in grid with real-time validation
6. **Delete**: Delete action opens confirmation dialog
7. **Navigate**: New Distribution action navigates to Add Distribution page

## Testing

```bash
# Run all Beneficiaries tests
npm run test -- --run Beneficiaries

# Run specific test files
npm run test -- useBeneficiaryPercentageUpdate.test.ts
npm run test -- badgeUtils.test.ts percentageUtils.test.ts
```

Test coverage includes:

- All custom hooks (97 tests across 3 hooks)
- All utility functions (97 tests across 2 utilities)
- Edge cases for badge parsing, percentage validation, and API error handling
