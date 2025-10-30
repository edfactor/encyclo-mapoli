---
applyTo: "src/ui/src/pages/Beneficiaries/**/*.*"
---

# Beneficiaries Page - Technical Architecture

## Overview

The Beneficiaries module provides a comprehensive interface for searching, viewing, and managing beneficiary records in the Demoulas Profit Sharing system. It follows a component-based architecture with clear separation of concerns between search, display, and data management functionality.

**Location**: `src/ui/src/pages/Beneficiaries/`

---

## Architecture Overview

### Component Hierarchy

```
BeneficiaryInquiry (Main Page)
├── MissiveAlertProvider (Context for alerts)
├── BeneficiaryInquirySearchFilter (Search form)
├── MemberResultsGrid (Multiple results - 2+ records)
│   └── MemberResultsGridColumns (Column definitions)
└── IndividualBeneficiaryView (Single member selected)
    ├── CreateBeneficiaryDialog (Add/Edit modal)
    │   └── CreateBeneficiary (Form component)
    ├── MemberDetailsPanel (Member information display)
    ├── BeneficiaryRelationshipsGrids (Relationships)
    │   ├── BeneficiaryOfGridColumns (Column definitions)
    │   ├── BeneficiaryInquiryGridColumns (Column definitions)
    │   └── DeleteBeneficiaryDialog (Delete confirmation)
    └── "Add Beneficiary" Button
```

---

## File Structure & Responsibilities

### 1. **BeneficiaryInquiry.tsx** - Main Page Component

**Purpose**: Top-level orchestration component that manages overall page state and navigation flow.

**Key Responsibilities**:
- Manages search state and results
- Handles member selection logic
- Coordinates between search and detail views
- Manages pagination state
- Clears state on reset and new searches

**Key State**:
```typescript
selectedMember: BeneficiaryDetail | null         // Currently selected member
beneficiarySearchFilterResponse: Paged<BeneficiaryDetail>  // Search results
memberType: number | undefined                    // 0=all, 1=employees, 2=beneficiaries
pageNumber: number                                // Current page
pageSize: number                                  // Results per page
beneficiarySearchFilterRequest: BeneficiarySearchAPIRequest  // Current search params
```

**Search Flow Logic**:
1. User submits search → `onSearch` called
2. `selectedMember` cleared immediately (prevents showing stale data)
3. Search executes via RTK Query
4. If 1 result → Auto-select and load details
5. If 2+ results → Show grid for user selection

**API Integration**:
- `useLazyBeneficiarySearchFilterQuery()` - Search for members
- `useLazyGetBeneficiaryDetailQuery()` - Get detailed member info

---

### 2. **BeneficiaryInquirySearchFilter.tsx** - Search Form

**Purpose**: Search form with mutual exclusivity and auto-detection features.

**Key Features**:
- **Mutual Exclusivity**: SSN, Name, and Badge/PSN fields disable each other
- **Auto Member Type Detection**: Badge length determines member type
  - Empty badge → "All"
  - Badge ≥ 8 digits → "Beneficiaries" (PSN numbers)
  - Badge < 8 digits → "Employees"
- **Loading State**: Disables form and shows spinner during search
- **Helper Text**: Explains why fields are disabled

**Props**:
```typescript
interface BeneficiaryInquirySearchFilterProps {
  onSearch: (params: BeneficiarySearchAPIRequest | undefined) => void;
  onMemberTypeChange: (type: number | undefined) => void;
  onReset: () => void;
  isSearching?: boolean;  // Controls loading state
}
```

**Form Fields**:
- Social Security Number (numeric, max 9 digits)
- Name (text)
- Badge/PSN Number (numeric, max 11 digits)
- Member Type (radio buttons: All, Employees, Beneficiaries)

**Validation**:
- At least one search criterion required
- Form must be valid to enable search
- Fields validate on blur

---

### 3. **MemberResultsGrid.tsx** - Search Results Grid

**Purpose**: Displays grid of 2+ search results for user selection.

**Key Features**:
- Pagination support (default 10 per page)
- Row click to select member
- Responsive column definitions
- Loading state indicator

**Props**:
```typescript
interface MemberResultsGridProps {
  searchResults: Paged<BeneficiaryDetail>;
  isLoading: boolean;
  pageNumber: number;
  pageSize: number;
  onRowClick: (data: BeneficiaryDetail) => void;
  onPageNumberChange: (page: number) => void;
  onPageSizeChange: (size: number) => void;
}
```

**Grid Columns** (defined in `MemberResultsGridColumns.tsx`):
- Badge Number (clickable)
- PSN Suffix
- Name
- SSN
- Date of Birth
- Address (Street, City, State, Zip)

---

### 4. **IndividualBeneficiaryView.tsx** - Selected Member Container

**Purpose**: Container component that displays all information and relationships for a selected member.

**Key Responsibilities**:
- Manages "Add Beneficiary" dialog state
- Handles beneficiary save success (refreshes data)
- Coordinates between member details and relationships

**State Management**:
```typescript
openCreateDialog: boolean                   // Dialog open/closed
selectedBeneficiary: BeneficiaryDto         // Beneficiary being edited (undefined for new)
beneficiaryDialogTitle: string              // "Add Beneficiary" or "Edit Beneficiary"
change: number                              // Increment to trigger refresh
```

**Child Components**:
1. `CreateBeneficiaryDialog` - Modal for add/edit
2. `MemberDetailsPanel` - Member information display
3. "Add Beneficiary" Button
4. `BeneficiaryRelationshipsGrids` - Relationship grids

---

### 5. **MemberDetailsPanel.tsx** - Member Information Display

**Purpose**: Displays basic member information in a clean, organized layout.

**Layout**:
- Two-column responsive grid
- Left column: Name, Address, City, State, Zip
- Right column: SSN, Badge Number, Age

**Props**:
```typescript
interface MemberDetailsPanelProps {
  selectedMember: BeneficiaryDetail;
  memberType: number;  // 1=Employee, 2=Beneficiary
}
```

**Styling**:
- `paddingX="24px"` for horizontal padding
- `marginY="8px"` for vertical spacing
- Consistent with MasterInquiry design patterns

---

### 6. **BeneficiaryRelationshipsGrids.tsx** - Relationships Management

**Purpose**: Displays and manages two types of beneficiary relationships with full CRUD operations.

**Two Grid Sections**:

#### Beneficiary Of Grid
Shows employees/members for whom the selected person is a beneficiary.
- Read-only view
- Columns: Badge, PSN_SUFFIX, Name, SSN, Date of Birth, Address

#### Beneficiaries Grid
Shows beneficiaries of the selected member.
- Full CRUD operations (Create, Read, Update, Delete)
- Editable percentage field with validation
- Actions column: Edit and Delete buttons
- Pagination support (25 per page)
- Sorting support

**Key Features**:
- **Percentage Validation**: Sum of beneficiary percentages must equal 100%
- **Inline Percentage Editing**: Text field with blur-to-save
- **Auto-refresh**: Uses `count` prop to trigger data refresh
- **Delete Confirmation**: Modal dialog before deletion
- **Self-contained State**: Manages own delete/edit state

**Props**:
```typescript
interface BeneficiaryRelationshipsProps {
  selectedMember: BeneficiaryDetail | null;
  count: number;           // External trigger for refresh
  onEditBeneficiary: (beneficiary?: BeneficiaryDto) => void;
}
```

**Internal State**:
```typescript
beneficiaryList: Paged<BeneficiaryDto>         // Beneficiaries list
beneficiaryOfList: Paged<BeneficiaryDto>       // Beneficiary of list
errorPercentage: boolean                        // Percentage validation error
openDeleteConfirmationDialog: boolean           // Delete modal state
deleteBeneficiaryId: number                     // ID to delete
deleteInProgress: boolean                       // Delete loading state
internalChange: number                          // Internal refresh trigger
```

**API Integration**:
- `useLazyGetBeneficiariesQuery()` - Fetch both relationship lists
- `useLazyUpdateBeneficiaryQuery()` - Update beneficiary (percentage)
- `useLazyDeleteBeneficiaryQuery()` - Delete beneficiary

---

### 7. **CreateBeneficiaryDialog.tsx** - Add/Edit Modal Wrapper

**Purpose**: Modal wrapper for the beneficiary creation/edit form.

**Key Features**:
- Material-UI Dialog component
- Close button (X) in top-right
- Dynamic title ("Add Beneficiary" or "Edit Beneficiary")
- Passes through all form props

**Props**:
```typescript
interface CreateBeneficiaryDialogProps {
  open: boolean;
  onClose: () => void;
  title: string;
  selectedBeneficiary?: BeneficiaryDto;
  badgeNumber: number;
  psnSuffix: number;
  onSaveSuccess: () => void;
}
```

---

### 8. **CreateBeneficiary.tsx** - Beneficiary Form

**Purpose**: Form component for creating or editing beneficiary records.

**Key Responsibilities**:
- Manages beneficiary kind lookup data (self-contained)
- Handles form validation with yup
- Creates beneficiary contact and beneficiary records
- Updates existing beneficiary records

**Form Fields**:
- First Name, Last Name (text, required)
- SSN (numeric, required)
- Date of Birth (date picker, required)
- Address fields: Street, City, State, Zipcode (text, required)
- "Address same as employee" checkbox
- Beneficiary Kind (dropdown, required)
- Relationship (text, required)

**State Management**:
```typescript
beneficiaryKind: BeneficiaryKindDto[]  // Self-fetched lookup data
```

**Validation Schema**:
```typescript
const schema = yup.object().shape({
  beneficiarySsn: yup.number().required(),
  relationship: yup.string().required(),
  dateOfBirth: yup.date().required(),
  street: yup.string().required(),
  city: yup.string().required(),
  state: yup.string().required(),
  postalCode: yup.string().required(),
  firstName: yup.string().required(),
  lastName: yup.string().required(),
  addressSameAsBeneficiary: yup.boolean().notRequired(),
  kindId: yup.string().required()
});
```

**API Integration**:
- `useLazyGetBeneficiaryKindQuery()` - Fetch beneficiary kinds
- `useLazyCreateBeneficiaryContactQuery()` - Create contact record
- `useLazyCreateBeneficiariesQuery()` - Create beneficiary record
- `useLazyUpdateBeneficiaryQuery()` - Update beneficiary record

**Create Flow**:
1. User fills form and clicks Submit
2. Create beneficiary contact (personal info)
3. Receive contact ID from response
4. Create beneficiary record (relationship + contact ID)
5. Call `onSaveSuccess()` to close dialog and refresh parent

**Edit Flow**:
1. Form pre-populated with existing beneficiary data
2. User modifies fields and clicks Submit
3. Update beneficiary record (includes contact info)
4. Call `onSaveSuccess()` to close dialog and refresh parent

---

### 9. **DeleteBeneficiaryDialog.tsx** - Delete Confirmation

**Purpose**: Confirmation modal for beneficiary deletion.

**Key Features**:
- Simple yes/no confirmation
- Shows loading spinner during delete
- Red "Delete it!" button for emphasis

**Props**:
```typescript
interface DeleteBeneficiaryDialogProps {
  open: boolean;
  onConfirm: () => void;
  onCancel: () => void;
  isDeleting: boolean;
}
```

---

### 10. **Grid Column Definition Files**

#### BeneficiaryInquiryGridColumns.ts
Defines columns for beneficiaries list grid (used in BeneficiaryRelationshipsGrids).

**Columns**:
- Store
- Badge (clickable link)
- Name
- Address (Street, City, State, Zip)
- SSN
- Date of Birth
- Kind (Beneficiary type)
- Relationship
- Percentage (editable)
- Actions (Edit/Delete buttons)

#### BeneficiaryOfGridColumns.tsx
Defines columns for "Beneficiary Of" grid (read-only).

**Columns**:
- Badge
- PSN_SUFFIX
- Name
- SSN
- Date of Birth
- Address

#### MemberResultsGridColumns.tsx
Defines columns for search results grid.

**Columns**:
- Badge Number
- PSN Suffix
- Name
- SSN
- Date of Birth
- Street, City, State, Zip

---

## State Management Architecture

### Local State (React useState)
Used for UI state and temporary data:
- Dialog open/close states
- Form field values
- Selected items
- Pagination state
- Loading indicators

### RTK Query (Redux Toolkit Query)
Used for server state and API caching:
- Search queries (with automatic caching)
- CRUD operations (mutations)
- Loading and error states
- Automatic refetching

**Key API Hooks**:
```typescript
// Queries
useLazyBeneficiarySearchFilterQuery()
useLazyGetBeneficiaryDetailQuery()
useLazyGetBeneficiariesQuery()
useLazyGetBeneficiaryKindQuery()

// Mutations
useLazyCreateBeneficiariesQuery()
useLazyCreateBeneficiaryContactQuery()
useLazyUpdateBeneficiaryQuery()
useLazyDeleteBeneficiaryQuery()
```

### Context (React Context)
- `MissiveAlertProvider` - Application-wide alert system

---

## Data Flow Patterns

### 1. Search Flow

```
User enters search criteria
    ↓
BeneficiaryInquirySearchFilter validates form
    ↓
onSearch callback with BeneficiarySearchAPIRequest
    ↓
BeneficiaryInquiry sets request state
    ↓
selectedMember cleared (hides detail view)
    ↓
useEffect triggers search
    ↓
RTK Query executes API call
    ↓
Results returned
    ↓
If 1 result: Auto-select and load details
If 2+ results: Show MemberResultsGrid
```

### 2. Member Selection Flow

```
User clicks row in MemberResultsGrid
    ↓
onRowClick callback with BeneficiaryDetail
    ↓
BeneficiaryInquiry.onBadgeClick()
    ↓
Trigger detail query with badge/PSN
    ↓
Set selectedMember state
    ↓
IndividualBeneficiaryView renders
    ↓
Shows MemberDetailsPanel
    ↓
Shows BeneficiaryRelationshipsGrids
```

### 3. Add Beneficiary Flow

```
User clicks "Add Beneficiary" button
    ↓
IndividualBeneficiaryView sets dialog state
    ↓
CreateBeneficiaryDialog opens
    ↓
CreateBeneficiary form renders
    ↓
User fills form and submits
    ↓
Create beneficiary contact (API)
    ↓
Create beneficiary record (API)
    ↓
onSaveSuccess callback
    ↓
Dialog closes
    ↓
Increment change counter
    ↓
BeneficiaryRelationshipsGrids refreshes
```

### 4. Edit Beneficiary Flow

```
User clicks Edit button in grid
    ↓
BeneficiaryRelationshipsGrids calls onEditBeneficiary
    ↓
IndividualBeneficiaryView opens dialog with data
    ↓
CreateBeneficiary form pre-populated
    ↓
User modifies and submits
    ↓
Update beneficiary (API)
    ↓
onSaveSuccess callback
    ↓
Dialog closes and data refreshes
```

### 5. Delete Beneficiary Flow

```
User clicks Delete button
    ↓
BeneficiaryRelationshipsGrids shows confirmation
    ↓
DeleteBeneficiaryDialog opens
    ↓
User confirms
    ↓
Delete beneficiary (API)
    ↓
Increment internalChange counter
    ↓
Grid refreshes
    ↓
Dialog closes
```

---

## Key Design Patterns

### 1. Progressive Disclosure
- Search → Results → Details
- Only show relevant UI based on current state
- Clear state on new searches to prevent confusion

### 2. Component Composition
- Small, focused components
- Clear props interfaces
- Separation of concerns (display vs. logic)

### 3. Self-Contained State
- Components manage their own internal state
- Use callbacks for parent communication
- Minimize prop drilling

### 4. Mutual Exclusivity (Search Form)
- SSN, Name, and Badge fields disable each other
- Helper text explains why fields are disabled
- Reset button clears all fields

### 5. Auto-Detection (Member Type)
- Badge length automatically sets member type
- Radio buttons disabled when badge has value
- Clear badge to manually select member type

### 6. Refresh Patterns
- Parent passes `count` prop to trigger child refresh
- Child manages `internalChange` for self-triggered refresh
- Both patterns work with useEffect dependencies

### 7. Loading States
- Search button shows spinner during search
- Delete button shows spinner during delete
- Form disabled during submission

---

## API Integration

### Request/Response Types

All types defined in `src/ui/src/types/beneficiary/beneficiary.ts`

**Key Types**:
```typescript
// Search
BeneficiarySearchForm           // Form shape (0|1|2 for memberType)
BeneficiarySearchAPIRequest     // API request (includes pagination)
BeneficiaryDetail               // Search result item

// Details
BeneficiaryDto                  // Full beneficiary record
BeneficiaryKindDto             // Beneficiary kind lookup

// CRUD
CreateBeneficiaryRequest       // Create beneficiary
CreateBeneficiaryContactRequest // Create contact
UpdateBeneficiaryRequest       // Update beneficiary
DeleteBeneficiaryRequest       // Delete beneficiary

// Grid
BeneficiariesGetAPIResponse    // Contains beneficiaries + beneficiaryOf
```

### Badge/PSN Handling

**Badge Numbers**:
- Employee badges: ≤ 7 digits
- PSN (Profit Sharing Number): 8+ digits
- PSN = Badge + Suffix

**Parsing Logic**:
```typescript
if (badgeStr.length <= MAX_EMPLOYEE_BADGE_LENGTH) {
  badge = Number(badgeNumber);
} else {
  badge = parseInt(badgeStr.slice(0, MAX_EMPLOYEE_BADGE_LENGTH - 1));
  psn = parseInt(badgeStr.slice(MAX_EMPLOYEE_BADGE_LENGTH - 1));
}
```

---

## Styling & Layout

### Grid System
- Uses Material-UI Grid v2 with `size` prop
- Responsive breakpoints: `xs`, `sm`, `md`
- Consistent spacing: `spacing={3}` for grids

### Typography
- `variant="h2"` for section headers
- Color: `#0258A5` (blue) for headers
- Padding: `paddingX="24px"` for consistent left/right padding
- Margin: `marginY="8px"` for vertical spacing

### Form Layout
- Two-column responsive layout on desktop
- Full-width on mobile
- Consistent field sizing (`size="small"`)
- Helper text for validation and exclusions

### Disabled States
- Grey background (`backgroundColor: "#f5f5f5"`)
- Disabled attribute on form controls
- Info-colored helper text (`color: "info.main"`)

---

## Validation & Error Handling

### Form Validation
- yup schemas for all forms
- Validation on blur
- Field-level error messages
- Form-level disabled state

### Percentage Validation
- Beneficiary percentages must sum to 100%
- Alert banner shown if validation fails
- Previous value restored on error
- Inline validation on blur

### API Error Handling
- Errors logged to console
- User-friendly error messages (when available)
- Loading states prevent duplicate submissions
- Graceful degradation on failures

---