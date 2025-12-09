---
applyTo: "src/ui/src/pages/FiscalClose/ProfitShareEditUpdate/**/*.*"
---

# Profit Share Edit/Update Technical Documentation

## Overview

The Profit Share Edit/Update feature is a critical fiscal close operation that allows users to preview, apply, and revert profit-sharing calculation adjustments. The implementation features a complex workflow with form-based calculations, dual-grid tabbed preview, prerequisite validation, checksum cross-reference validation, confirmation modals, and apply/revert operations with persistent state tracking.

## Architecture Overview

The Profit Share Edit/Update feature implements a **Complex Workflow Page** pattern with multiple states, prerequisite checking, checksum validation, and dual-grid previews.

```
┌────────────────────────────────────────────────────────────┐
│ ProfitShareEditUpdate.tsx (Main Page)                     │
│ - Prerequisite guard (frozen data required)               │
│ - Apply/Revert buttons with modals                        │
│ - Status tracking (applied vs not applied)                │
│ - Checksum validation integration                         │
│ - Year validation (must be current year - 1)              │
└──────────────────┬─────────────────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
┌───────▼──────────┐  ┌───────▼────────────────────────┐
│ Search Filter    │  │ Summary Tables & Grids         │
│ (Hidden after    │  │ - MasterUpdateSummaryTable     │
│  apply)          │  │   (PAY444 with validation)     │
│ - Contribution % │  │ - PAY447 Summary               │
│ - Earnings %     │  │ - Adjustments Table            │
│ - Forfeiture %   │  │ - Additional Totals           │
│ - Max Allowed    │  │                                │
│ - Badge Adjust   │  └────────────┬───────────────────┘
│ - Secondary %    │               │
└──────────────────┘      ┌────────▼──────────┐
                          │ Tabbed Grids      │
                          │ - Preview Updates │
                          │ - Preview Details │
                          └───────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ Custom Hooks                                                │
│ - useSaveAction: Apply changes to master                   │
│ - useRevertAction: Revert to previous state                │
│ - useChecksumValidation: Cross-reference PAY443/PAY444     │
└─────────────────────────────────────────────────────────────┘
```

---

## Component Hierarchy

```
PrerequisiteGuard
└── ProfitShareEditUpdate
    ├── Page (smart-ui-library)
    │   ├── label: "PROFIT SHARE EDIT/UPDATE"
    │   └── actionNode:
    │       ├── RenderRevertButton (with modal)
    │       ├── RenderSaveButton (with modal)
    │       └── StatusDropdownActionNode
    ├── Alert (Changes Applied Banner - persistent)
    │   └── Shows: Updated By, Date
    ├── DSMAccordion (Filter - hidden after apply)
    │   └── ProfitShareEditUpdateSearchFilter
    │       ├── Contribution % (TextField)
    │       ├── Earnings % (TextField)
    │       ├── Secondary Earnings % (TextField)
    │       ├── Incoming Forfeit % (TextField)
    │       ├── Max Allowed Contributions (TextField)
    │       ├── Badge to Adjust (TextField)
    │       ├── Adjust Contribution Amount (TextField)
    │       ├── Adjust Earnings Amount (TextField)
    │       ├── Adjust Incoming Forfeit Amount (TextField)
    │       ├── Badge to Adjust 2 (TextField)
    │       ├── Adjust Secondary Earnings Amount (TextField)
    │       └── Preview / Reset buttons
    ├── ChangesList (Conditional - shown after apply)
    │   └── Lists all applied parameters
    ├── Summary (PAY444)
    │   ├── Employee/Beneficiary counts
    │   ├── MasterUpdateSummaryTable
    │   │   └── Unified table with validation icons in headers
    │   └── TotalsGrid (Forfeitures, Points, Max Exceeded)
    ├── Summary (PAY447)
    │   └── TotalsGrid (Beginning, Contributions, Earnings, Forfeit)
    ├── Adjustments Entered Table (Conditional)
    │   └── Shows badge-specific adjustments
    ├── ProfitShareEditUpdateTabs
    │   ├── Tab: Preview Updates (PAY444)
    │   │   └── ProfitShareUpdateGrid
    │   │       ├── Column definitions
    │   │       ├── Pagination
    │   │       └── Summary table
    │   └── Tab: Preview Details (PAY447)
    │       └── ProfitShareEditGrid
    │           ├── Column definitions
    │           ├── Pagination
    │           └── Summary table
    ├── SmartModal (Save Confirmation)
    │   └── ProfitShareEditConfirmation
    │       ├── Shows parameters being applied
    │       └── "YES, SAVE" / "NO, CANCEL"
    ├── SmartModal (Revert Confirmation)
    │   └── ProfitShareEditConfirmation
    │       ├── Shows parameters being reverted
    │       └── "YES, REVERT" / "NO, CANCEL"
    └── SmartModal (Empty Form Warning)
        └── ProfitShareEditConfirmation
            └── "Minimum fields required" message
```

---

## Key Features

### 1. Prerequisite Enforcement

**Component**: `PrerequisiteGuard` wrapper

**Purpose**: Ensures frozen data exists before allowing profit share edit/update operations

```typescript
<PrerequisiteGuard
  navigationId={currentNavigationId}
  messageTemplate={Messages.ProfitSharePrerequisiteIncomplete}>
  {({ prerequisitesComplete }) => (
    <Page>
      {/* Page content */}
      {/* Save button disabled if !prerequisitesComplete */}
    </Page>
  )}
</PrerequisiteGuard>
```

**Behavior**:

- Checks prerequisite navigations on page load
- Disables save button if prerequisites incomplete
- Shows tooltip: "All prerequisite navigations must be complete before saving."
- Displays warning message if prerequisites missing

### 2. Year Validation

**Rule**: Can only process previous year (current year - 1)

**Implementation**:

```typescript
useEffect(() => {
  const currentYear = new Date().getFullYear();
  if (profitYear !== currentYear - 1) {
    dispatch(setInvalidProfitShareEditYear(true));
    dispatch(
      setMessage({
        key: MessageKeys.ProfitShareEditUpdate,
        message: {
          type: "warning",
          title: "Invalid Year Selected",
          message: `Please select a ${currentYear - 1} date in the drawer menu to proceed.`,
        },
      }),
    );
  } else {
    dispatch(setInvalidProfitShareEditYear(false));
  }
}, [profitYear, dispatch]);
```

**Purpose**: Prevents accidental processing of current year or historical years

**Effect**: Save button disabled when `invalidProfitShareEditYear === true`

### 3. Workflow States

The page operates in three distinct states:

#### State 1: Initial (No Changes Applied)

- **Search filter**: Visible
- **Apply button**: Enabled (after preview)
- **Revert button**: Disabled
- **Banner**: Hidden
- **Grids**: Show preview data after search

#### State 2: Changes Applied

- **Search filter**: Hidden
- **Apply button**: Disabled (changes already applied)
- **Revert button**: Enabled
- **Banner**: "These changes have already been applied" (persistent Alert)
- **ChangesList**: Displays applied parameters
- **Grids**: Cleared

#### State 3: After Revert

- **Search filter**: Visible (restored)
- **Apply button**: Enabled (can apply again)
- **Revert button**: Disabled
- **Banner**: Hidden
- **Grids**: Cleared

**State Management**:

```typescript
// Check status on load
useEffect(() => {
  if (hasToken) {
    onStatusSearch(); // Fetch profitMasterStatus
    if (updatedTime) {
      setChangesApplied(true);
      dispatch(setProfitEditUpdateChangesAvailable(false));
      dispatch(setProfitEditUpdateRevertChangesAvailable(true));
    } else {
      setChangesApplied(false);
    }
  }
}, [onStatusSearch, hasToken, updatedTime, updatedBy]);
```

### 4. Checksum Validation

**Hook**: `useChecksumValidation`

**Purpose**: Cross-reference current PAY444 values with archived PAY443 values to detect discrepancies

**Implementation**:

```typescript
const { validationData: validationResponse, getFieldValidation } =
  useChecksumValidation({
    profitYear: profitYear || 0,
    autoFetch: true,
    // Pass current values from PAY444 for client-side comparison with PAY443 archived values
    currentValues: profitSharingUpdate?.profitShareUpdateTotals
      ? {
          TotalProfitSharingBalance:
            profitSharingUpdate.profitShareUpdateTotals.beginningBalance,
          DistributionTotals:
            profitSharingUpdate.profitShareUpdateTotals.distributions,
          ForfeitureTotals:
            profitSharingUpdate.profitShareUpdateTotals.forfeiture,
          ContributionTotals:
            profitSharingUpdate.profitShareUpdateTotals.totalContribution,
          EarningsTotals: profitSharingUpdate.profitShareUpdateTotals.earnings,
          IncomingAllocations:
            profitSharingUpdate.profitShareUpdateTotals.allocations,
          OutgoingAllocations:
            profitSharingUpdate.profitShareUpdateTotals.paidAllocations,
          // NetAllocTransfer is calculated: allocations + paidAllocations
          NetAllocTransfer:
            (profitSharingUpdate.profitShareUpdateTotals.allocations || 0) +
            (profitSharingUpdate.profitShareUpdateTotals.paidAllocations || 0),
        }
      : undefined,
  });
```

**Validation Display**: `MasterUpdateSummaryTable` shows validation icons in column headers

- Green checkmark: Values match archived PAY443 values
- Orange warning: Discrepancy detected

**User Action**: Click icon to view validation details popup

### 5. Confirmation Modals

All critical actions use `ProfitShareEditConfirmation` component with different configurations:

#### Save Modal

```typescript
<SmartModal open={openSaveModal} onClose={() => setOpenSaveModal(false)}>
  <ProfitShareEditConfirmation
    performLabel="YES, SAVE"
    closeLabel="NO, CANCEL"
    setOpenModal={setOpenSaveModal}
    actionFunction={() => {
      saveAction();
      setOpenSaveModal(false);
    }}
    messageType="confirmation"
    messageHeadline="You are about to apply the following changes:"
    params={profitSharingEditQueryParams}
    lastWarning="Ready to save? It may take a few minutes to process."
  />
</SmartModal>
```

#### Revert Modal

```typescript
<SmartModal open={openRevertModal} onClose={() => setOpenRevertModal(false)}>
  <ProfitShareEditConfirmation
    performLabel="YES, REVERT"
    closeLabel="NO, CANCEL"
    setOpenModal={setOpenRevertModal}
    actionFunction={() => {
      revertAction();
      setOpenRevertModal(false);
    }}
    messageType="warning"
    messageHeadline="Reverting to the last update will modify the following:"
    params={profitSharingEditQueryParams || profitMasterStatus}
    lastWarning="Do you still wish to revert?"
  />
</SmartModal>
```

#### Empty Form Modal

```typescript
<SmartModal open={openEmptyModal} onClose={() => setOpenEmptyModal(false)}>
  <ProfitShareEditConfirmation
    performLabel="OK"
    closeLabel=""
    setOpenModal={setOpenEmptyModal}
    actionFunction={() => {}}
    messageType="info"
    messageHeadline={
      !minimumFieldsEntered
        ? "You must enter at least contribution, earnings, and max allowed contributions."
        : !adjustedBadgeOneValid
          ? "If you adjust a badge, you must also enter the contribution, earnings, and incoming forfeiture."
          : !adjustedBadgeTwoValid
            ? "If you adjust a secondary badge, you must also enter the earnings amount."
            : ""
    }
    params={profitSharingEditQueryParams}
    lastWarning=""
  />
</SmartModal>
```

### 6. Dual-Grid Tabbed Preview

**Component**: `ProfitShareEditUpdateTabs`

**Two Tabs**:

| Tab             | Report | Purpose                                  | Grid Component          |
| --------------- | ------ | ---------------------------------------- | ----------------------- |
| Preview Updates | PAY444 | Shows updated balances after adjustments | `ProfitShareUpdateGrid` |
| Preview Details | PAY447 | Shows detailed calculations              | `ProfitShareEditGrid`   |

**Implementation**:

```typescript
<TabContext value={value}>
  <TabList onChange={handleChange}>
    <Tab label="Preview Updates" value="1" />
    <Tab label="Preview Details" value="2" />
  </TabList>
  <TabPanel value="1">
    <ProfitShareUpdateGrid {...props} />
  </TabPanel>
  <TabPanel value="2">
    <ProfitShareEditGrid {...props} />
  </TabPanel>
</TabContext>
```

**PAY444 (Preview Updates)**:

- Shows **updated** balances after applying adjustment percentages
- Used for final review before applying changes
- Includes validation icons in summary table

**PAY447 (Preview Details)**:

- Shows **detailed calculations** with breakdowns
- Displays how contribution/earnings percentages were applied
- Provides audit trail of calculation logic

### 7. Form Validation

**Minimum Fields Required**:

```typescript
const wasFormUsed = (
  profitSharingEditQueryParams: ProfitShareEditUpdateQueryParams,
) => {
  return (
    (profitSharingEditQueryParams?.contributionPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams?.earningsPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams?.incomingForfeitPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams?.maxAllowedContributions ?? 0) > 0 ||
    (profitSharingEditQueryParams?.badgeToAdjust ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustContributionAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustEarningsAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustIncomingForfeitAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams?.badgeToAdjust2 ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0) > 0
  );
};
```

**Badge Adjustment Validation**:

- **Badge 1**: If badge entered, must also enter contribution, earnings, and incoming forfeiture amounts
- **Badge 2**: If badge entered, must also enter secondary earnings amount

**Minimum Fields for Save**:

- Contribution %
- Earnings %
- Max Allowed Contributions

**Validation Triggers**:

- Tracked via `setMinimumFieldsEntered`, `setAdjustedBadgeOneValid`, `setAdjustedBadgeTwoValid`
- Save button disabled if validation fails
- Empty form modal shown with specific message

### 8. Total Forfeitures Check

**Critical Validation**: Total Forfeitures must equal 0 before saving

**Implementation**:

```typescript
// In Redux state
totalForfeituresGreaterThanZero: boolean

// In UI
{totalForfeituresGreaterThanZero && (
  <div className="-mt-2 px-[24px] text-sm text-red-600">
    <em>
      * Total Forfeitures value highlighted in red indicates an issue that must be resolved before
      saving.
    </em>
  </div>
)}
```

**Effect**: Save button disabled when `totalForfeituresGreaterThanZero === true`

**Purpose**: Ensures profit-sharing calculations are balanced

---

## Form Parameters

The search filter collects parameters for profit-sharing calculations:

### Core Parameters

| Parameter                  | Type   | Description                                   | Required |
| -------------------------- | ------ | --------------------------------------------- | -------- |
| `contributionPercent`      | number | Percentage for company contributions          | Yes      |
| `earningsPercent`          | number | Percentage for earnings calculations          | Yes      |
| `secondaryEarningsPercent` | number | Secondary earnings percentage                 | No       |
| `incomingForfeitPercent`   | number | Percentage for incoming forfeiture allocation | No       |
| `maxAllowedContributions`  | number | Maximum contribution cap per employee         | Yes      |

### Badge-Specific Adjustments

| Parameter                       | Type   | Description                                      | Conditional |
| ------------------------------- | ------ | ------------------------------------------------ | ----------- |
| `badgeToAdjust`                 | number | Badge number for first adjustment                | Optional    |
| `adjustContributionAmount`      | number | Contribution adjustment amount for badge 1       | If badge 1  |
| `adjustEarningsAmount`          | number | Earnings adjustment amount for badge 1           | If badge 1  |
| `adjustIncomingForfeitAmount`   | number | Incoming forfeit adjustment amount for badge 1   | If badge 1  |
| `badgeToAdjust2`                | number | Badge number for second adjustment               | Optional    |
| `adjustEarningsSecondaryAmount` | number | Secondary earnings adjustment amount for badge 2 | If badge 2  |

**Badge Adjustment Rules**:

1. If `badgeToAdjust` is entered → Must also enter `adjustContributionAmount`, `adjustEarningsAmount`, `adjustIncomingForfeitAmount`
2. If `badgeToAdjust2` is entered → Must also enter `adjustEarningsSecondaryAmount`

---

## Data Flow

### Preview Flow

```
User enters form parameters
    ↓
Click "Preview" button
    ↓
ProfitShareEditUpdateSearchFilter validates
    ↓
Minimum fields entered? Badge adjustments valid?
    ↓
Yes:
  - API request with parameters
  - Response includes:
    - profitSharingUpdate (PAY444 with totals)
    - profitSharingEdit (PAY447 with details)
    - crossReferenceValidation (optional)
  - useChecksumValidation auto-fetches validation data
  - Redux stores profitSharingEditQueryParams
  - Grids display preview data
  - Summary tables show totals
  - MasterUpdateSummaryTable shows validation icons
  - Save button enables (if all validations pass)
    ↓
No:
  - Show empty form modal with specific error message
```

### Apply Flow

```
User clicks "Save Updates" button
    ↓
Prerequisite check passes?
Total Forfeitures === 0?
Year validation passes?
    ↓
Yes:
  - Open save confirmation modal
  - Show parameters being applied
  - User clicks "YES, SAVE"
    ↓
    useSaveAction() called
    ↓
    applyMaster mutation (RTK Query)
    ↓
    API applies changes to profit master (PAY443)
    ↓
    Success:
      - Show success message (employees, beneficiaries, ETVAs affected)
      - Hide search filter
      - Clear grids
      - Enable revert button
      - Show "Changes Applied" banner
      - Display ChangesList
      - useChecksumValidation auto-refreshes
    ↓
    Error:
      - Show error message
      - Keep state unchanged
    ↓
No:
  - Show tooltip explaining why disabled
```

### Revert Flow

```
User clicks "Revert" button
    ↓
Open revert confirmation modal
    ↓
Show parameters being reverted
    ↓
User clicks "YES, REVERT"
    ↓
useRevertAction() called
    ↓
revertMaster lazy query (RTK Query)
    ↓
API reverts profit master to previous state
    ↓
Success:
  - Show success message (employees, beneficiaries, ETVAs reverted)
  - Show search filter
  - Clear grids
  - Disable revert button
  - Hide "Changes Applied" banner
  - Clear ChangesList
  - setChangesApplied(false)
    ↓
Error:
  - Show error message
  - Keep state unchanged
```

---

## Validation Systems

### 1. Prerequisite Validation

**Type**: Blocking validation

**Check**: Frozen data prerequisite must be complete

**Implementation**: `PrerequisiteGuard` component

**Effect**: Save button disabled, tooltip shown

### 2. Year Validation

**Type**: Blocking validation

**Check**: `profitYear === currentYear - 1`

**Effect**: Save button disabled, warning message shown

### 3. Form Validation

**Type**: Blocking validation

**Checks**:

- Minimum fields entered (contribution %, earnings %, max allowed)
- Badge 1 adjustment complete (if badge entered)
- Badge 2 adjustment complete (if badge entered)

**Effect**: Opens empty form modal with specific error

### 4. Total Forfeitures Validation

**Type**: Blocking validation

**Check**: `totalForfeituresGreaterThanZero === false`

**Effect**: Save button disabled, red text warning shown

### 5. Checksum Validation

**Type**: Informational (non-blocking)

**Check**: Cross-reference PAY444 current values with PAY443 archived values

**Implementation**: `useChecksumValidation` hook

**Display**: Validation icons in `MasterUpdateSummaryTable` column headers

- Green: Match
- Orange: Discrepancy

**Effect**: Non-blocking, but provides important audit information

---

## Action Buttons

### Save Button (RenderSaveButton)

**Disabled Conditions**:

1. Changes already applied (`!profitEditUpdateChangesAvailable && status?.updatedTime !== null`)
2. Loading state (`isLoading || profitShareApplyOrRevertLoading`)
3. Total forfeitures > 0 (`totalForfeituresGreaterThanZero`)
4. Invalid year (`invalidProfitShareEditYear`)
5. Prerequisites incomplete (`!prerequisitesComplete`)
6. Read-only mode (`isReadOnly`)

**Tooltips**:

- Read-only: "You are in read-only mode and cannot apply changes."
- Invalid year: "Invalid year for saving changes"
- Total forfeitures: "Total forfeitures is greater than zero."
- Prerequisites: "All prerequisite navigations must be complete before saving."
- Default: "You must have previewed data before saving."

**Click Behavior**:

- If form used and validations pass → Open save modal
- If form not used or validations fail → Open empty modal with error

### Revert Button (RenderRevertButton)

**Disabled Conditions**:

1. No changes to revert (`!profitEditUpdateRevertChangesAvailable`)
2. Loading state (`isLoading || profitShareApplyOrRevertLoading`)
3. Read-only mode (`isReadOnly`)

**Tooltips**:

- Read-only: "You are in read-only mode and cannot revert changes."
- Default: "You must have applied data to revert."

**Click Behavior**:

- Open revert confirmation modal

---

## File Descriptions

### Core Files

1. **`ProfitShareEditUpdate.tsx`** (800 lines)

   - Main page container
   - Custom hooks: `useSaveAction`, `useRevertAction`
   - State management for modals, validation, changes applied
   - Checksum validation integration
   - Prerequisite guard wrapper
   - Action buttons rendering
   - Summary tables composition

2. **`ProfitShareEditUpdateSearchFilter.tsx`**

   - Form with 11 input fields
   - Validation logic for minimum fields and badge adjustments
   - Preview/Reset buttons
   - Redux parameter storage

3. **`ProfitShareEditUpdateTabs.tsx`**

   - Tab context management
   - Two tabs: Preview Updates (PAY444) and Preview Details (PAY447)
   - Renders appropriate grid for each tab

4. **`ProfitShareEditGrid.tsx`**

   - Preview Details grid (PAY447)
   - Shows detailed calculations
   - Column definitions from `ProfitShareEditGridColumns.tsx`
   - Pagination support

5. **`ProfitShareUpdateGrid.tsx`**

   - Preview Updates grid (PAY444)
   - Shows updated balances
   - Column definitions from `ProfitShareUpdateGridColumns.tsx`
   - Pagination support

6. **`MasterUpdateSummaryTable.tsx`**

   - Unified PAY444 summary table
   - Validation icons in column headers
   - Three rows: Total, Allocation, Point
   - Integrates with `useChecksumValidation` hook

7. **`ProfitShareEditConfirmation.tsx`**

   - Reusable confirmation modal
   - Displays parameters being applied/reverted
   - Configurable for save/revert/empty scenarios
   - Shows formatted parameter list

8. **`ChangesList.tsx`**

   - Displays applied parameters after save
   - Shows historical changes
   - Rendered when `profitEditUpdateRevertChangesAvailable === true`

9. **`ProfitShareUpdateSummaryTable.tsx`**
   - Additional summary table for PAY444 grid
   - Shows row-specific totals

### Column Definition Files

10. **`ProfitShareEditGridColumns.tsx`**

    - Column definitions for Preview Details (PAY447) grid

11. **`ProfitShareUpdateGridColumns.tsx`**
    - Column definitions for Preview Updates (PAY444) grid

---

## Common Patterns

### 1. Persistent State Banner

**Pattern**: Use `Alert` (not missive) for persistent state messages

```typescript
{changesApplied && (
  <div className="w-full py-3">
    <Alert severity={Messages.ProfitShareMasterUpdated.message.type}>
      <AlertTitle sx={{ fontWeight: "bold" }}>
        {Messages.ProfitShareMasterUpdated.message.title}
      </AlertTitle>
      {`Updated By: ${updatedBy} | Date: ${updatedTime} `}
    </Alert>
  </div>
)}
```

**Why Alert?** Remains visible (doesn't auto-dismiss) to show persistent state

### 2. Conditional UI Sections

**Pattern**: Hide/show sections based on workflow state

```typescript
// Hide search filter after apply
<Grid width={"100%"} hidden={!profitShareEditUpdateShowSearch}>
  <DSMAccordion title="Filter">
    <ProfitShareEditUpdateSearchFilter />
  </DSMAccordion>
</Grid>

// Show changes list after apply
{profitEditUpdateRevertChangesAvailable && (
  <>
    <Typography variant="h6" sx={{ fontWeight: "bold" }}>
      These changes have already been applied:
    </Typography>
    <ChangesList params={profitSharingEditQueryParams || profitMasterStatus} />
  </>
)}
```

### 3. Loading Indicators

**Pattern**: Inline spinner in button to prevent shrinking

```typescript
{isLoading || profitShareApplyOrRevertLoading ? (
  <div className="spinner">
    <CircularProgress color="inherit" size="20px" />
  </div>
) : (
  "Save Updates"
)}
```

### 4. Tooltip Wrapping

**Pattern**: Wrap disabled buttons with explanatory tooltips

```typescript
if (!profitEditUpdateChangesAvailable || invalidProfitShareEditYear || /* other conditions */) {
  return (
    <Tooltip
      placement="top"
      title={
        isReadOnly
          ? "You are in read-only mode and cannot apply changes."
          : invalidProfitShareEditYear
            ? "Invalid year for saving changes"
            : /* other conditions */
      }>
      <span>{saveButton}</span>
    </Tooltip>
  );
} else {
  return saveButton;
}
```

### 5. Custom Hooks for Actions

**Pattern**: Encapsulate save/revert logic in custom hooks

```typescript
const useSaveAction = () => {
  const [applyMaster] = useGetMasterApplyMutation();
  const dispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();

  const saveAction = async (): Promise<void> => {
    const params = {
      /* build from query params */
    };
    dispatch(setProfitShareApplyOrRevertLoading(true));

    await applyMaster(params)
      .unwrap()
      .then((payload) => {
        // Handle success
        dispatch(setMessage(/* success message */));
        dispatch(setProfitEditUpdateChangesAvailable(false));
        dispatch(setProfitEditUpdateRevertChangesAvailable(true));
        dispatch(setProfitShareEditUpdateShowSearch(false));
        dispatch(clearProfitSharingUpdate());
      })
      .catch((error) => {
        // Handle error
        dispatch(setMessage(/* error message */));
      });

    dispatch(setProfitShareApplyOrRevertLoading(false));
  };

  return saveAction;
};
```
