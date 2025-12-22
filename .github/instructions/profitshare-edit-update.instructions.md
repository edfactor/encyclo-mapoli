---
applyTo: "src/ui/src/pages/FiscalClose/ProfitShareEditUpdate/**/*.*"
---

# ProfitShareEditUpdate Component Architecture

## Overview

The `ProfitShareEditUpdate` component is a complex, multi-step React workflow page that enables users to edit and update profit-sharing data for a fiscal year. The component follows the "Complex Workflow Page" pattern with refactored architecture using a custom hook for business logic encapsulation.

**Location**: `src/ui/src/pages/FiscalClose/ProfitShareEditUpdate/`

**Key Responsibility**: Manage profit-sharing master data updates with validation, confirmation, and reversion capabilities.

---

## Directory Structure

```
ProfitShareEditUpdate/
├── ProfitShareEditUpdate.tsx                      # Main page component (329 lines)
├── hooks/
│   ├── useProfitShareEditUpdate.ts               # Custom hook with business logic (421 lines)
│   └── useProfitShareEditUpdate.test.tsx         # Hook unit tests (781 lines, 24 tests)
├── utils/
│   └── formValidation.ts                         # Form validation utilities
├── ProfitShareEditUpdateSearchFilter.tsx          # Search/filter form component
├── ProfitShareEditUpdateTabs.tsx                  # Tabbed section (Edit vs Update)
├── ProfitShareEditConfirmation.tsx                # Confirmation modal
├── ProfitShareRevertButton.tsx                    # Revert action button
├── ProfitShareSaveButton.tsx                      # Save action button
├── ChangesList.tsx                                # Display previous changes
├── MasterUpdateSummaryTable.tsx                   # Summary table (with test)
├── MasterUpdateSummaryTable.test.tsx              # Summary table tests
├── ProfitShareEditGrid.tsx                        # Edit data grid
├── ProfitShareEditGridColumns.tsx                 # Edit grid column definitions
├── ProfitShareUpdateGrid.tsx                      # Update data grid
├── ProfitShareUpdateGridColumns.tsx               # Update grid column definitions
├── ProfitShareUpdateSummaryTable.tsx              # Update summary display
└── Claude.md                                      # (Empty refactoring plan file)
```

### Statistics

- **Total files**: 17
- **Main component**: 329 lines (after refactoring from 640)
- **Custom hook**: 421 lines
- **Hook tests**: 781 lines with 24 test cases
- **Size reduction**: 49% smaller after moving business logic to hook

---

## Architecture Pattern

---

## Component Details

### 1. ProfitShareEditUpdate.tsx

**Purpose**: Render the page UI with all sections and modals.

**Size**: 329 lines (refactored from 640 - 49% reduction)

**Key Sections**:

```
- Page header with Page wrapper & StatusDropdownActionNode
- Alert banner (changesApplied notification)
- Filter accordion (Search section)
- Previous changes display (conditional)
- PAY444 Summary section
  - Summary table
  - Totals grid
  - Forfeitures warning
- PAY447 Summary section
  - Grand totals grid
- Adjustments entered section (conditional)
- Tabbed grids (Edit/Update tabs)
- 3 Confirmation modals:
  1. Save confirmation
  2. Revert confirmation
  3. Empty validation modal
```

**Local State** (UI only):

```typescript
const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
const [pageNumberReset, setPageNumberReset] = useState(false);
const [openValidationField, setOpenValidationField] = useState<string | null>(
  null,
);
```

**Hook Usage**:

```typescript
const {
  changesApplied,
  openSaveModal,
  openRevertModal,
  openEmptyModal,
  minimumFieldsEntered,
  adjustedBadgeOneValid,
  adjustedBadgeTwoValid,
  updatedBy,
  updatedTime,
  // ... 20+ more values from hook
} = useProfitShareEditUpdate();
```

---

### 2. useProfitShareEditUpdate.ts (Custom Hook)

**Purpose**: Encapsulate all business logic, state management, and API interaction.

**Size**: 421 lines

**Architecture**: Uses `useReducer` pattern for complex state transitions.

#### State Interface

```typescript
interface ProfitShareEditUpdateState {
  changesApplied: boolean;
  modals: {
    saveModalOpen: boolean;
    revertModalOpen: boolean;
    emptyModalOpen: boolean;
  };
  validation: {
    minimumFieldsEntered: boolean;
    adjustedBadgeOneValid: boolean;
    adjustedBadgeTwoValid: boolean;
  };
  profitMasterStatus: {
    updatedBy: string | null;
    updatedTime: string | null;
  };
}
```

#### Key Functions

**saveAction()** - Apply changes to year-end master data:

1. Build request parameters from Redux query params
2. Call `applyMaster` mutation
3. Update local state (SAVE_SUCCESS)
4. Dispatch Redux actions (clear, show messages)
5. Show success/failure message with employee/beneficiary counts

**revertAction()** - Revert previous changes:

1. Call `triggerRevert` with profit year
2. Update local state (REVERT_SUCCESS)
3. Dispatch Redux actions to clear all data
4. Show success/failure message with affected counts

**validateForm()** - Check form field validity:

1. Check minimum fields: contribution%, earnings%, max allowed contributions
2. Check badge 1 adjustment: if badge present, all adjustment fields required
3. Check badge 2 adjustment: if badge present, earnings amount required
4. Dispatch validation state update
5. Return true/false for overall form validity

**onStatusSearch()** - Fetch current master status:

1. Trigger status query with profit year
2. Update local status fields (updatedBy, updatedTime)
3. If changes were previously applied, set changesApplied = true

#### Reducer Actions

```typescript
OPEN_SAVE_MODAL / CLOSE_SAVE_MODAL;
OPEN_REVERT_MODAL / CLOSE_REVERT_MODAL;
OPEN_EMPTY_MODAL / CLOSE_EMPTY_MODAL;
SET_CHANGES_APPLIED;
SET_VALIDATION;
SET_STATUS;
SAVE_SUCCESS;
REVERT_SUCCESS;
```

#### Redux Integration

**Selectors** (read from store):

- `profitEditUpdateChangesAvailable`
- `profitEditUpdateRevertChangesAvailable`
- `profitShareEditUpdateShowSearch`
- `profitSharingEdit`
- `profitSharingUpdate`
- `profitSharingEditQueryParams`
- `profitShareApplyOrRevertLoading`
- `totalForfeituresGreaterThanZero`
- `invalidProfitShareEditYear`
- `profitMasterStatus`

**Dispatches**:

- `setProfitShareApplyOrRevertLoading`
- `setProfitEditUpdateChangesAvailable`
- `setProfitEditUpdateRevertChangesAvailable`
- `setProfitShareEditUpdateShowSearch`
- `clearProfitSharingUpdate`
- `clearProfitSharingEditQueryParams`
- `clearProfitSharingEdit`
- `setResetYearEndPage`
- `setInvalidProfitShareEditYear`
- `setMessage`

#### RTK Query Integration

```typescript
const [applyMaster] = useGetMasterApplyMutation();
const [triggerRevert] = useLazyGetMasterRevertQuery();
const [triggerStatus] = useLazyGetProfitMasterStatusQuery();
```

#### Additional Hooks

**useChecksumValidation**: Validates accounting balances

- Input: profitYear and current totals
- Output: validationData and getFieldValidation function

**useFiscalCloseProfitYear**: Gets selected profit year from context

---

### 3. useProfitShareEditUpdate.test.tsx

**Purpose**: Comprehensive unit test suite for the custom hook.

**Size**: 781 lines with 24 test cases

**Test Coverage**:

| Category          | Tests | Focus                                    |
| ----------------- | ----- | ---------------------------------------- |
| Initial State     | 2     | Hook initialization, redux defaults      |
| Form Validation   | 6     | Minimum fields, badge validations        |
| Modal Management  | 5     | Open/close behavior, empty validation    |
| Save Action       | 3     | Success, failure, API interaction        |
| Revert Action     | 3     | Success, failure, state cleanup          |
| Status Fetching   | 3     | Fetch, update state, apply detection     |
| Redux Integration | 2     | State passthrough, dispatch verification |

**Test Example** (Form Validation):

```typescript
it("should validate minimum required fields", () => {
  const { result } = renderHook(() => useProfitShareEditUpdate(), {
    wrapper: createWrapper({
      profitSharingEditQueryParams: {
        contributionPercent: 0,
        earningsPercent: 0,
        maxAllowedContributions: 0,
        // ... all zero
      },
    }),
  });

  expect(result.current.minimumFieldsEntered).toBe(false);
  expect(result.current.adjustedBadgeOneValid).toBe(true);
  expect(result.current.adjustedBadgeTwoValid).toBe(true);
});
```

**Type Safety**: All `any` types replaced with proper TypeScript interfaces:

- `MockedYearsEndState`
- `MockedSecurityState`
- `MockedRootState`
- `ValidationFieldResult`
- `ChecksumValidationResult`

---

### 4. Supporting Components

#### ProfitShareEditUpdateSearchFilter.tsx

- Form to enter profit-sharing parameters
- Collapsible accordion section
- Triggers search which populates edit/update grids

#### ProfitShareEditUpdateTabs.tsx

- Two tabs: "Edit" and "Update"
- Uses tabbed grids for data entry

#### ProfitShareEditConfirmation.tsx

- Modal for confirming save/revert actions
- Displays what will change
- Warning message before execution

#### MasterUpdateSummaryTable.tsx

- Summary table of totals (PAY444)
- Displays validation details
- Collapsible validation sections

#### ChangesList.tsx

- Shows previously applied changes
- Displays badge adjustments

#### ProfitShareSaveButton.tsx & ProfitShareRevertButton.tsx

- Action buttons in page header
- Validation before enabling

#### Grid Components

- `ProfitShareEditGrid` / `ProfitShareEditGridColumns` - Edit data grid
- `ProfitShareUpdateGrid` / `ProfitShareUpdateGridColumns` - Update data grid
- Both support pagination and column sorting

---

## Data Flow

### Save Workflow

```
User clicks "APPLY" button
        ↓
┌─────────────────────────────┐
│ validateForm() checks       │
│ - minimumFieldsEntered      │
│ - adjustedBadgeOneValid     │
│ - adjustedBadgeTwoValid     │
└──────┬──────────────────────┘
       │
       ├─ FALSE → Show empty validation modal
       │
       └─ TRUE → Open save confirmation modal
              ↓
        User confirms
              ↓
┌─────────────────────────────┐
│ saveAction() executes       │
│ 1. Build ProfitShareMaster  │
│    ApplyRequest             │
│ 2. Call applyMaster()       │
│ 3. Dispatch success actions │
│ 4. Show success message     │
└─────────────────────────────┘
```

### Revert Workflow

```
User clicks "REVERT" button
        ↓
Open revert confirmation modal
        ↓
User confirms
        ↓
┌──────────────────────────────┐
│ revertAction() executes      │
│ 1. Call triggerRevert()      │
│ 2. Clear all profit-sharing  │
│    data from Redux           │
│ 3. Show revert message       │
└──────────────────────────────┘
```

### Status Check Workflow

```
Component mounts
        ↓
hasToken check + initialize check
        ↓
onStatusSearch() called
        ↓
triggerStatus() fetches current status
        ↓
If changesApplied found:
├─ Set changesApplied = true
├─ Update updatedBy, updatedTime
├─ Show alert banner
└─ Disable search accordion
```

---

## Redux State Dependencies

### YearsEnd Slice

The component depends on these Redux selectors from `yearsEnd` slice:

```typescript
profitEditUpdateChangesAvailable: boolean;
profitEditUpdateRevertChangesAvailable: boolean;
profitShareEditUpdateShowSearch: boolean;
profitSharingEdit: ProfitSharingEditData | null;
profitSharingUpdate: ProfitSharingUpdateData | null;
profitSharingEditQueryParams: ProfitShareEditUpdateQueryParams | null;
profitShareApplyOrRevertLoading: boolean;
totalForfeituresGreaterThanZero: boolean;
invalidProfitShareEditYear: boolean;
profitMasterStatus: {
  updatedBy: string | null;
  updatedTime: string | null;
}
profitSharingUpdateAdjustmentSummary: AdjustmentSummary | null;
```

### Actions Dispatched

- `setProfitShareApplyOrRevertLoading(boolean)`
- `setProfitEditUpdateChangesAvailable(boolean)`
- `setProfitEditUpdateRevertChangesAvailable(boolean)`
- `setProfitShareEditUpdateShowSearch(boolean)`
- `clearProfitSharingUpdate()`
- `clearProfitSharingEdit()`
- `clearProfitSharingEditQueryParams()`
- `setResetYearEndPage(boolean)`
- `setInvalidProfitShareEditYear(boolean)`

---

## Validation Logic

### Form Validation Rules

**Minimum Fields Required**:

- `contributionPercent > 0`
- `earningsPercent > 0`
- `maxAllowedContributions > 0`

**Badge 1 Adjustment** (if `badgeToAdjust > 0`):

- `adjustContributionAmount > 0`
- `adjustEarningsAmount > 0`
- `adjustIncomingForfeitAmount > 0`

**Badge 2 Adjustment** (if `badgeToAdjust2 > 0`):

- `adjustEarningsSecondaryAmount > 0`

### Utility Function: wasFormUsed()

Located in `utils/formValidation.ts`:

```typescript
export const wasFormUsed = (
  profitSharingEditQueryParams: ProfitShareEditUpdateQueryParams | null,
): boolean => {
  // Returns true if ANY field > 0
  // Used to detect if user has interacted with form
};
```

### Checksum Validation

Integrated via `useChecksumValidation` hook:

- Validates accounting balance (PAY444)
- Maps form values to checksum fields
- Provides `getFieldValidation(fieldName)` method
- Shows validation details in collapsible sections

---

## API Integration

### Mutations

**useGetMasterApplyMutation()**:

- Endpoint: Apply changes to profit-sharing master
- Request: `ProfitShareMasterApplyRequest`
- Response: Employee/beneficiary/ETVA counts affected
- Error handling: Catches and shows error message

### Lazy Queries

**useLazyGetMasterRevertQuery()**:

- Endpoint: Revert previous changes
- Request: `{ profitYear: number }`
- Response: Employee/beneficiary/ETVA counts affected

**useLazyGetProfitMasterStatusQuery()**:

- Endpoint: Fetch current status
- Request: `{ profitYear: number }`
- Response: `{ updatedBy: string; updatedTime: string }`
- Called on component mount to check for previous changes

---

## Key Features

### 1. Change Detection

- On mount, checks if changes were previously applied
- Shows alert banner with update timestamp
- Disables search to prevent new changes while showing previous ones

### 2. Multi-Step Validation

- Client-side validation before confirmation modal
- Server-side validation during API call
- Detailed validation messages in modal
- Empty modal for insufficient data

### 3. Dual Badging System

- Primary badge adjustment (Badge 1)
- Secondary badge adjustment (Badge 2)
- Independent validation for each

### 4. Summary Displays

- PAY444: Master file totals (contribution, earnings, forfeitures)
- PAY447: Grand totals and adjustments
- Checksum validation alerts
- Previous changes list

### 5. Revert Capability

- Undo previous changes
- Only available when changes exist
- Clears all related data from Redux

### 6. Year Validation

- Only allows current fiscal year (year-1)
- Shows warning if wrong year selected
- Prevents operations on invalid years

---
