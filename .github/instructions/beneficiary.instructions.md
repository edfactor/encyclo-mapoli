---
applyTo: "src/ui/src/pages/Beneficiaries/**/*.*"
---
# Beneficiaries Module - Technical Summary

A comprehensive reference guide for the Beneficiaries page component and its ecosystem.

## Table of Contents

1. [Overview](#overview)
2. [Component Architecture](#component-architecture)
3. [Custom Hooks](#custom-hooks)
4. [Utilities & Helpers](#utilities--helpers)
5. [State Management](#state-management)
6. [Data Structures](#data-structures)
7. [API Integration](#api-integration)
8. [Core Workflows](#core-workflows)
9. [File Structure](#file-structure)
10. [Dependencies](#dependencies)

---

## Overview

The Beneficiaries module enables complex member relationship management within the profit-sharing system. It provides:

- **Member Search**: Find employees or beneficiaries by SSN, name, or badge/PSN number
- **Detail View**: Display comprehensive member information and relationship data
- **Beneficiary Management**: Create, update, and delete beneficiary records
- **Relationship Tracking**: View and manage "beneficiary of" and "beneficiaries" relationships
- **Percentage Allocation**: Track and validate beneficiary allocation percentages

### Key Features
- Auto-detection of member type (employee vs beneficiary) based on badge number
- Real-time percentage validation (must sum ≤ 100%)
- Two-step beneficiary creation (contact + beneficiary record)
- Pagination and sorting for large datasets
- Comprehensive form validation (SSN, dates, addresses)

---

## Component Architecture

### Main Page Container

#### `BeneficiaryInquiry.tsx`
The primary orchestrator component that manages the search-and-detail workflow.

**Responsibilities**:
- Manages three sub-workflows: search filter, member results, and detail view
- Tracks search request state and selected member
- Handles auto-selection when single member found
- Implements page reset functionality
- Uses `useBeneficiarySearch` hook for pagination/sorting state

**Component Tree**:
```
BeneficiaryInquiry
├── BeneficiaryInquirySearchFilter
├── MemberResultsGrid
└── IndividualBeneficiaryView
    ├── MemberDetailsPanel
    ├── BeneficiaryRelationshipsGrids
    │   ├── BeneficiariesListGrid
    │   └── BeneficiaryOfGrid
    └── CreateBeneficiaryDialog
```

---

### Search & Filtering Components

#### `BeneficiaryInquirySearchFilter.tsx`
Complex search form with three mutually-exclusive search criteria.

**Features**:
- **Search Modes**: SSN, Name, or Badge/PSN number
- **Auto-Detection**: Determines member type from badge length
  - 1-7 digits: Employee
  - 8+ digits: Beneficiary
- **Field Disabling**: Prevents simultaneous use of multiple search fields
- **Validation**: Yup schema for SSN (9 digits) and badge format
- **Pagination**: Includes page size and sort parameters in submission

**Props**:
- `onSearchChange?: (request: BeneficiarySearchAPIRequest) => void`
- `isLoading?: boolean`

**Output**:
Returns `BeneficiarySearchAPIRequest` with search criteria, pagination, and sorting.

---

#### `MemberResultsGrid.tsx`
Paginated display of search results with row selection.

**Features**:
- AG Grid with configurable column definitions
- Row click triggers detail fetch
- Pagination controls integrated with search state
- Auto-row-select when single result
- Loading state during fetch

**Props**:
- `members: BeneficiaryDetail[]`
- `totalCount: number`
- `isLoading: boolean`
- `onRowClick: (member: BeneficiaryDetail) => void`
- `pageNumber, pageSize, onPaginationChange`

---

### Detail View Components

#### `IndividualBeneficiaryView.tsx`
Top-level wrapper for member detail workflow.

**Responsibilities**:
- Manages dialog state for create/update operations
- Coordinates between detail panel, relationship grids, and dialogs
- Tracks refresh triggers via counter
- Passes member data to child components

**Child Components**:
- `MemberDetailsPanel`: Read-only member info
- `BeneficiaryRelationshipsGrids`: Two relationship grids
- `CreateBeneficiaryDialog`: Create/edit form
- `DeleteBeneficiaryDialog`: Delete confirmation

---

#### `MemberDetailsPanel.tsx`
Read-only display of selected member information.

**Displayed Fields**:
- Full name
- Address (street, city, state, zip)
- Social Security Number
- Badge/PSN numbers
- Age

**Layout**: Uses `LabelValueSection` component for consistent info display

---

### Grid & Relationship Components

#### `BeneficiaryRelationshipsGrids.tsx`
Complex grid manager for bidirectional beneficiary relationships.

**Manages Two Sections**:
1. **"Beneficiary Of"**: People who list this member as beneficiary
2. **"Beneficiaries"**: People this member is beneficiary for

**Features**:
- **Percentage Editing**: Inline percentage field with real-time validation
- **Validation**: Sum of percentages must not exceed 100%
- **Actions**: Edit/Delete buttons per row
- **Pagination**: Configurable page size with totals
- **Sorting**: Support for multiple sort fields
- **Counts**: Display total counts for each relationship type
- **Error Recovery**: Failed updates restore previous value

**Custom Hooks Used**:
- `useBeneficiaryRelationshipData`: Data fetching and pagination
- `useBeneficiaryPercentageUpdate`: Validation and update

**Props**:
- `selectedMember: BeneficiaryDetail`
- `externalRefreshTrigger: number` (counter for forced refresh)
- `onAddBeneficiary: () => void`
- `onEditBeneficiary: (dto: BeneficiaryDto) => void`

---

#### Grid Column Definitions

##### `MemberResultsGridColumns.tsx`
Search results grid (8 columns):
- Badge, PSN Suffix, Name, SSN, City, State, Zip, Age

##### `BeneficiariesListGridColumns.ts`
Beneficiary list grid (14 columns):
- Badge, PSN, Full Name, SSN, Percentage (editable), Kind, Current Balance
- Street, City, State, Zip, DOB, Relationship, Phone

##### `BeneficiaryOfGridColumns.tsx`
"Beneficiary of" grid (11 columns):
- Badge, PSN, Full Name, SSN, DOB, Street, City, State, Percentage, Zip, Current Balance

---

### Dialog Components

#### `CreateBeneficiaryDialog.tsx`
Material-UI Dialog wrapper for beneficiary creation/editing.

**Responsibilities**:
- Manages dialog lifecycle (open/close)
- Passes form logic to child `CreateBeneficiary` component
- Provides close button and title

---

#### `CreateBeneficiary.tsx`
Complex form component for creating and updating beneficiary records.

**Form Fields**:
- First Name (required)
- Last Name (required)
- Beneficiary SSN (required, 9 digits)
- Date of Birth (required, no future dates)
- Address (street, city, state, zip)
- Copy Address from Employee (checkbox)
- Beneficiary Kind (dropdown, fetched via `useBeneficiaryKinds`)
- Relationship (dropdown)
- Phone Number (optional, 9 digits max)

**Implementation Pattern**:
- Uses React Hook Form with Yup schema validation
- Creation flow: Creates contact record first, then beneficiary record (2 API calls)
- Update flow: Uses single update query
- Phone validation: 9 digits maximum, numeric only
- Error handling: Displays field-level validation errors

**Hooks Used**:
- `useForm` (React Hook Form)
- `useLazyCreateBeneficiaryContactQuery`
- `useLazyCreateBeneficiariesQuery`
- `useLazyUpdateBeneficiaryQuery`
- `useBeneficiaryKinds`

**Props**:
- `initialData?: BeneficiaryDetail` (for edit mode)
- `onSuccess: () => void`
- `employeeAddress?: Address` (for address copy)

---

#### `DeleteBeneficiaryDialog.tsx`
Simple confirmation dialog for beneficiary deletion.

**Features**:
- Confirmation prompt
- Loading state during deletion
- Success/error handling

---

## Custom Hooks

### `useBeneficiarySearch.ts`

**Purpose**: Encapsulates pagination and sorting state for member search

**Returns**:
```typescript
{
  pageNumber: number
  pageSize: number
  sortParams: SortParam[]
  handlePaginationChange: (pageNumber: number, pageSize: number) => void
  handleSortChange: (sortParams: SortParam[]) => void
  reset: () => void
}
```

**Key Characteristics**:
- Manages pagination state independent of search execution
- Parent component retains control over when search is triggered
- Supports multi-column sorting
- Reset clears pagination to defaults

**Usage Pattern**:
```typescript
const search = useBeneficiarySearch({ defaultPageSize: 50 });
// Parent component calls search.handlePaginationChange() when needed
```

---

### `useBeneficiaryKinds.ts`

**Purpose**: Fetches and caches beneficiary kind lookup data

**Returns**:
```typescript
{
  beneficiaryKinds: BeneficiaryKindDto[]
  isLoading: boolean
  error: string | null
}
```

**Implementation Details**:
- Uses RTK Query's `useLazyGetBeneficiaryKindQuery` hook
- Single-fetch guarantee via `hasAttempted` ref flag (prevents duplicate API calls)
- Local state caching (candidate for Redux optimization)
- Token-dependent: only fetches if user is authenticated
- Error handling: Logs errors to console and returns error state

**Performance**: Once fetched, data is cached for the component lifecycle

---

### `useBeneficiaryRelationshipData.ts`

**Purpose**: Manages bidirectional beneficiary relationship data fetching with pagination

**Returns**:
```typescript
{
  beneficiaryList: {
    data: BeneficiaryDto[]
    totalCount: number
    isLoading: boolean
  }
  beneficiaryOfList: {
    data: BeneficiaryDto[]
    totalCount: number
    isLoading: boolean
  }
  refresh: () => void
}
```

**Features**:
- Lazy evaluation: only fetches when member and token available
- Handles two simultaneous API queries (beneficiaries + beneficiary-of)
- Supports pagination and sorting for each relationship type
- Manual refresh via `refresh()` method
- External refresh trigger via `externalRefreshTrigger` prop (counter pattern)

**Validation**:
- Requires valid `badgeNumber` and `psnSuffix`
- Skips fetch if identifiers missing

**Configuration**:
```typescript
useBeneficiaryRelationshipData({
  selectedMember: BeneficiaryDetail
  pageNumber: number
  pageSize: number
  sortParams: SortParam[]
  externalRefreshTrigger?: number  // Increment to trigger refresh
})
```

---

### `useBeneficiaryPercentageUpdate.ts`

**Purpose**: Validates and updates beneficiary percentage allocation with business rule enforcement

**Returns**:
```typescript
{
  validateAndUpdate: (
    beneficiaryId: number,
    newPercentage: number,
    currentBeneficiaries: BeneficiaryDto[]
  ) => Promise<ValidationResult>
  isUpdating: boolean
}
```

**Validation Process**:
1. Calculates new sum with proposed percentage
2. Validates sum ≤ 100%
3. Returns validation result with error message if invalid
4. Returns previous value for UI restoration on failure
5. Executes API update if valid
6. Calls optional `onUpdateSuccess` callback after successful update

**Error Handling**:
- Catches API failures and returns error message
- Does NOT throw exceptions (error returned in result)
- Provides previous value for UI rollback

**Result Structure**:
```typescript
{
  valid: boolean
  sum?: number
  previousValue?: number
  error?: string
}
```

---

## Utilities & Helpers

### `badgeUtils.ts`

Badge/PSN parsing and member type detection utilities.

#### `parseBadgeAndPSN(badgeInput: string)`
Separates combined badge/PSN number into components.

**Logic**:
- Badges: 1-7 digits
- PSNs: 8+ digits (badge + suffix)

**Example**:
```typescript
parseBadgeAndPSN("12345678")
// Returns: { badge: 1234567, psn: 8 }
```

#### `detectMemberTypeFromBadge(badge: number)`
Determines member type based on badge length.

**Returns**:
- `0`: All (no specific type)
- `1`: Employees (1-7 digits)
- `2`: Beneficiaries (8+ digits)

**Usage**: Auto-detect in search filter when user enters badge

#### `isValidBadgeIdentifiers(badgeNumber: number, psnSuffix: number)`
Validates both badge and PSN identifiers.

**Rules**:
- Both must be present and non-null
- Badge must be > 0
- PSN can be 0

#### `decomposePSNSuffix(psnSuffix: number)`
Breaks PSN into three beneficiary hierarchy levels.

**Returns**:
```typescript
{
  firstLevel: number
  secondLevel: number
  thirdLevel: number
}
```

**Usage**: Constructs beneficiary payload during creation

**Example**:
```typescript
decomposePSNSuffix(123)
// Returns: { firstLevel: 1, secondLevel: 2, thirdLevel: 3 }
```

---

### `percentageUtils.ts`

Percentage validation utilities for beneficiary allocation.

#### `calculatePercentageSum(items: BeneficiaryDto[], updatedId: number, newPercentage: number)`
Computes sum with one item's percentage changed.

**Returns**:
```typescript
{
  sum: number
  previousValue: number
}
```

**Purpose**: Allows validation of proposed change before API call

#### `validatePercentageAllocation(sum: number)`
Validates percentage sum constraint.

**Rules**:
- Sum must be ≤ 100%

**Returns**:
```typescript
{
  sum: number
  valid: boolean
  error?: string
}
```

---

## State Management

### Redux Integration

#### RTK Query Hooks (from `reduxstore/api/BeneficiariesApi`)

**Query Hooks (Read Operations)**:
- `useLazyBeneficiarySearchFilterQuery` - Primary search endpoint
- `useLazyGetBeneficiaryDetailQuery` - Fetch single member details
- `useLazyGetBeneficiariesQuery` - Fetch beneficiary relationships
- `useLazyGetBeneficiaryKindQuery` - Fetch beneficiary kinds lookup

**Mutation Hooks (Write Operations)**:
- `useLazyCreateBeneficiariesQuery` - Create new beneficiary
- `useLazyCreateBeneficiaryContactQuery` - Create contact record
- `useLazyUpdateBeneficiaryQuery` - Update beneficiary percentage/info
- `useLazyDeleteBeneficiaryQuery` - Delete beneficiary record

#### Redux Store Access
- **Authentication**: `state.security.token` (via `useSelector`)
- All hooks check for valid token before API calls

### State Management Pattern

**Local Component State** (React `useState`):
- Dialog open/close state
- Form data (Create/Edit form)
- Member selection
- Search criteria

**RTK Query Cache** (Distributed):
- Server data (automatic deduplication)
- Automatic cache invalidation on mutations
- Manual refresh via hook `refresh()` method

**Custom Hooks** (Business Logic):
- Encapsulate complex state logic (pagination, validation)
- Promote code reuse across components
- Facilitate unit testing

---

## Data Structures

### Request DTOs

#### `BeneficiarySearchAPIRequest`
Primary search filter request.

```typescript
{
  ssn?: string
  firstName?: string
  lastName?: string
  badgeInput?: string
  memberType: number  // 0=All, 1=Employee, 2=Beneficiary
  pageNumber: number
  pageSize: number
  sortParams: SortParam[]
}
```

#### `BeneficiaryDetailAPIRequest`
Request for single member details.

```typescript
{
  badgeNumber: number
  psnSuffix: number
}
```

#### `CreateBeneficiaryRequest`
Beneficiary creation payload.

```typescript
{
  badgeNumber: number
  psnSuffix: number
  psnFirstLevel: number
  psnSecondLevel: number
  psnThirdLevel: number
  firstName: string
  lastName: string
  ssn: string
  dateOfBirth: DateTime
  relationship: string
  beneficiaryKind: string
  currentBalance: decimal?
}
```

#### `CreateBeneficiaryContactRequest`
Contact information creation.

```typescript
{
  phone?: string
  street?: string
  city?: string
  state?: string
  zip?: string
}
```

#### `UpdateBeneficiaryRequest`
Beneficiary update payload.

```typescript
{
  badgeNumber: number
  psnSuffix: number
  psnFirstLevel: number
  psnSecondLevel: number
  psnThirdLevel: number
  percentage?: decimal
  phone?: string
  street?: string
  city?: string
  state?: string
  zip?: string
}
```

### Response DTOs

#### `BeneficiaryDetail`
Member search result.

```typescript
{
  badgeNumber: number
  psnSuffix: number
  firstName: string
  lastName: string
  ssn: string
  street?: string
  city?: string
  state?: string
  zip?: string
  dateOfBirth: DateTime
  age: number
}
```

#### `BeneficiaryDto`
Beneficiary relationship record.

```typescript
{
  id: number
  badgeNumber: number
  psnSuffix: number
  firstName: string
  lastName: string
  ssn: string
  percentage?: decimal
  relationship?: string
  kind?: string
  currentBalance?: decimal
  street?: string
  city?: string
  state?: string
  zip?: string
  dateOfBirth: DateTime
  phone?: string
}
```

#### `Paged<T>`
Paginated response wrapper.

```typescript
{
  results: T[]
  totalCount: number
}
```

#### `BeneficiaryKindDto`
Beneficiary kind lookup.

```typescript
{
  id: number
  name: string
}
```

---

## API Integration

### Lazy Query Pattern

All API calls use RTK Query's lazy query pattern for manual trigger control.

```typescript
const [trigger, { isFetching, data, error }] = useLazyXxxQuery();

// Later: trigger(params).unwrap().then(res => {
//   // handle response
// }).catch(err => {
//   // handle error
// })
```

**Advantages**:
- Component controls when API calls execute
- No automatic fetches on mount
- Clear error handling with `.unwrap()` + `.catch()`

### Error Handling Strategy

**Error Handling Pattern**:
```typescript
trigger(params)
  .unwrap()
  .then(response => {
    // Success path
    onSuccess?.(response)
  })
  .catch(error => {
    // Error path - both network and API errors
    console.error('API failed:', error)
    setErrorMessage(error.message || 'Operation failed')
  })
```

**Current Implementation**:
- Console error logging (candidate for improvement)
- Error messages propagated to UI (toasts or inline messages)
- No automatic retry (user-initiated retry via re-trigger)

### Data Refresh Strategy

**Pattern 1: Custom Hook Refresh**
```typescript
const { data, refresh } = useBeneficiaryRelationshipData(...)
// After mutation: refresh()
```

**Pattern 2: External Trigger Counter**
```typescript
const [refreshCounter, setRefreshCounter] = useState(0)
const data = useBeneficiaryRelationshipData({
  externalRefreshTrigger: refreshCounter
})
// After mutation: setRefreshCounter(c => c + 1)
```

**Pattern 3: RTK Query Auto-Invalidation**
- Mutations automatically invalidate related queries
- Manual cache control via tag-based invalidation (if configured)

---

## Core Workflows

### Workflow 1: Search Member

**Steps**:
1. User enters search criteria (SSN, Name, or Badge/PSN) in `BeneficiaryInquirySearchFilter`
2. Form validates inputs (SSN format, badge format)
3. User submits form
4. `BeneficiaryInquiry` calls search API with pagination/sort params
5. Results displayed in `MemberResultsGrid` with pagination controls
6. **Auto-select**: If single result, automatically selects member and fetches details
7. **Manual select**: If multiple results, user clicks row to fetch details

---

### Workflow 2: View Member Details

**Precondition**: Member selected from search results

**Steps**:
1. User clicks row in `MemberResultsGrid`
2. `BeneficiaryInquiry` triggers detail fetch for selected badge/PSN
3. `IndividualBeneficiaryView` displays:
   - Member demographics via `MemberDetailsPanel` (name, address, SSN, age)
   - Two relationship grids via `BeneficiaryRelationshipsGrids`:
     - **"Beneficiary Of"**: People who have this member as beneficiary
     - **"Beneficiaries"**: People this member is beneficiary for
4. Grids display with pagination and sorting support
5. User can edit percentage, add new, or delete relationships

---

### Workflow 3: Create Beneficiary

**Precondition**: Member selected with detail view displayed

**Steps**:
1. User clicks "Add Beneficiary" button in detail view
2. `CreateBeneficiaryDialog` opens with `CreateBeneficiary` form
3. User fills form fields:
   - Personal info (name, SSN, DOB)
   - Address (optionally copy from employee)
   - Relationship and kind
4. Form validates on blur and submit:
   - SSN: 9 digits required
   - DOB: No future dates
   - Address: Optional but if provided, all fields required
5. **On Submit**:
   - Step 1: Create contact record via `useLazyCreateBeneficiaryContactQuery`
   - Step 2: Create beneficiary record via `useLazyCreateBeneficiariesQuery`
   - Both must succeed (sequential API calls)
6. **On Success**:
   - Dialog closes
   - Beneficiary grids refresh via `refresh()` call
   - New beneficiary appears in grid
7. **On Error**:
   - Error message displayed
   - Dialog remains open for retry

---

### Workflow 4: Update Percentage Allocation

**Precondition**: Beneficiary grid visible with edit-enabled cells

**Steps**:
1. User clicks percentage field in beneficiary grid
2. User enters new percentage value
3. User tabs/clicks away (blur event)
4. `BeneficiaryRelationshipsGrids` calls `useBeneficiaryPercentageUpdate.validateAndUpdate()`
5. **Validation**:
   - Hook calculates: new percentage + sum of other percentages
   - Validates: sum ≤ 100%
   - Returns `{ valid: boolean, error?: string, previousValue: number }`
6. **If Invalid**:
   - Field reverts to previous value
   - Error message displayed below field
   - No API call made
7. **If Valid**:
   - API update triggered via `useLazyUpdateBeneficiaryQuery`
   - Loading state shown
   - Grid data refreshed after success
8. **On Error**:
   - Field reverts to previous value
   - Error message displayed

---

### Workflow 5: Delete Beneficiary

**Precondition**: Beneficiary grid visible with delete button per row

**Steps**:
1. User clicks delete icon in beneficiary grid row
2. `DeleteBeneficiaryDialog` opens with confirmation prompt
3. User clicks "Confirm Delete"
4. `useLazyDeleteBeneficiaryQuery` triggered
5. Loading state displayed during deletion
6. **On Success**:
   - Dialog closes
   - Grid refreshes via `refresh()` call
   - Deleted beneficiary removed from grid
7. **On Error**:
   - Error message displayed
   - Dialog remains open for retry

---

### Workflow 6: Edit Beneficiary Details

**Similar to Create Workflow but**:
- Form pre-populated with existing beneficiary data
- Uses `useLazyUpdateBeneficiaryQuery` instead of create queries
- Single API call instead of two

---

## File Structure

```
Beneficiaries/
│
├── BeneficiaryInquiry.tsx                 # Main page orchestrator
├── BeneficiaryInquirySearchFilter.tsx     # Search form component
├── IndividualBeneficiaryView.tsx          # Member detail wrapper
├── MemberDetailsPanel.tsx                 # Read-only member info display
├── MemberResultsGrid.tsx                  # Search results grid display
├── MemberResultsGridColumns.tsx           # Search grid column definitions
│
├── BeneficiaryRelationshipsGrids.tsx      # Bidirectional relationship grids
├── BeneficiariesListGridColumns.ts        # Beneficiary list column definitions
├── BeneficiaryOfGridColumns.tsx           # "Beneficiary of" column definitions
│
├── CreateBeneficiary.tsx                  # Create/edit form component
├── CreateBeneficiaryDialog.tsx            # Dialog wrapper for form
├── DeleteBeneficiaryDialog.tsx            # Delete confirmation dialog
│
├── hooks/
│   ├── index.ts                           # Hook exports
│   ├── useBeneficiarySearch.ts            # Pagination/sort state hook
│   ├── useBeneficiarySearch.test.ts       # Tests (3 test cases)
│   ├── useBeneficiaryKinds.ts             # Lookup data fetching hook
│   ├── useBeneficiaryKinds.test.ts        # Tests (7+ test cases)
│   ├── useBeneficiaryRelationshipData.ts  # Data fetching + pagination hook
│   ├── useBeneficiaryRelationshipData.test.ts  # Tests
│   ├── useBeneficiaryPercentageUpdate.ts  # Validation + update hook
│   └── useBeneficiaryPercentageUpdate.test.ts  # Tests
│
└── utils/
    ├── index.ts                           # Utility exports
    ├── badgeUtils.ts                      # Badge parsing + validation
    ├── badgeUtils.test.ts                 # Tests (58 test cases)
    ├── percentageUtils.ts                 # Percentage validation utilities
    └── percentageUtils.test.ts            # Tests (39 test cases)
```


