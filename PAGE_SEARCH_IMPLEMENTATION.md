# Page Search Feature - Implementation Summary

## Overview

Added a new page search functionality to the MenuBar component that allows users to quickly find and navigate to pages using type-ahead search.

## Features Implemented

### 1. PageSearch Component (`src/ui/src/components/MenuBar/PageSearch.tsx`)

- **Type-ahead search**: Real-time filtering as the user types
- **Searches across**: Page titles and subtitles
- **Contextual hints**: Shows parent navigation path (inspired by VS Code search)
  - Example: "Clean up Reports" shows "in Year End > December Activities"
- **Visual design**: Dark gradient-styled search box matching the menu bar theme
- **Navigation with drawer**:
  - Navigates to the selected page
  - Automatically opens the drawer
  - Sets the correct L0 section as active
  - Page is highlighted in the drawer navigation tree
- **Auto-clear**: Search input clears after navigation

### 2. MenuBar Integration

- **Position**: Search field appears **before** the impersonation control (to the left)
- **Vertical separator**: Added between search and impersonation (when impersonation is visible in dev/QA)
- **Preserved styling**: Original menu bar gradient and styling maintained
- **Responsive layout**: Uses flexbox for proper alignment

### 3. Key Implementation Details

#### Recursive Navigation Flattening

The `flattenNavigation` function recursively processes the navigation tree to create searchable items with full context:

```typescript
interface SearchableNavigationItem {
  id: number;
  title: string;
  subTitle: string;
  url: string;
  parentTitle: string;
  fullPath: string[]; // Complete hierarchy for context
}
```

#### Search Logic

- Case-insensitive search
- Filters on both `title` and `subTitle` fields
- Only includes navigable pages (where `isNavigable === true` and `url` is defined)
- Real-time filtering with debounced dropdown opening

#### Navigation Integration

When a page is selected from the search:

1. **Stores navigation ID** in `localStorage` for drawer active item tracking (`navigationId`)
2. **Pre-expands parent hierarchy**: Gets the parent chain using `getNavigationParentChain` and sets each parent item's expanded state in localStorage (`nav-expanded-{id}`)
3. **Finds L1 submenu**: Extracts the L1 (first-level child) item from the parent chain to determine which submenu section to show
4. Dispatches `setActiveSubMenu` with the **L1 section title** (e.g., "Fiscal Close", not "Year End")
5. Dispatches `openDrawer` to ensure drawer is visible
6. Navigates to the selected page using React Router
7. **Auto-expansion on navigation**: The `RecursiveNavItem` component detects when it contains an active child path and automatically expands to show the full hierarchy

**Understanding the Drawer Structure:**

- **L0** (Menu Bar) - "Year End", "Inquiries & Adjustments", "Distributions", etc.
- **L1** (Drawer Main Menu) - "December Activities", "Fiscal Close", "Print PS Jobs"
- **L2+** (Drawer Submenu) - Nested pages within each L1 section

The drawer operates in two modes:

- **Main Menu View**: Shows L1 items when no submenu is active
- **Submenu View**: Shows children of selected L1 item (triggered by `setActiveSubMenu`)

**Key Features:**

- **Active path detection**: Each navigation item checks if it or any descendant contains the current route
- **Auto-expansion**: Parent items automatically expand when they contain the active page
- **localStorage synchronization**: Expanded states persist and are re-checked when the route changes
- **Persistent expansion**: Once expanded via search, the hierarchy remains expanded until manually collapsed
- **Deep nesting support**: Works correctly for pages nested 4-5+ levels deep (e.g., Year End > Fiscal Close > Prof Share by Store > Under-21 Report > QPAY066TA-UNDR21)

This ensures users can find any page via search and the complete navigation hierarchy is automatically revealed, matching the expected behavior.

#### Contextual Display

Results show:

1. **Title** (bold)
2. **Subtitle** (if available, secondary text)
3. **Parent context** (italicized, shows full path)
   - Example: "in Year End > December Activities > December Activities"

### 4. Styling

Added CSS in `src/ui/src/styles/index.css`:

```css
.navbuttons {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
```

**Note**: Original menubar styling preserved - no changes to background gradient or colors.

Search input styled with:

- Semi-transparent white background
- White text and borders
- Search icon on the left
- Responsive sizing (280px width)

## Testing

### Unit Tests Created

#### PageSearch.test.tsx (18 tests)

- ✅ Render tests (search input, icon)
- ✅ Empty state handling
- ✅ Filter by title and subtitle
- ✅ Parent context display
- ✅ Full path for nested pages
- ✅ Case-insensitive search
- ✅ Handle undefined/empty navigation data
- ✅ Multiple results
- ✅ No results message
- ✅ Only includes navigable pages
- ✅ Display subtitles when available
- ✅ Navigation data handling
- ✅ Input value updates

#### MenuBar.test.tsx (11 tests)

- ✅ Renders MenuBar with PageSearch
- ✅ PageSearch positioned before impersonation
- ✅ Vertical separator with impersonation
- ✅ No separator without impersonation
- ✅ Passes navigationData to PageSearch
- ✅ Correct layout with menu items
- ✅ Maintains existing menu functionality
- ✅ Handles minimal navigation data
- ✅ Handles undefined navigation data
- ✅ Correct CSS classes applied
- ✅ DOM ordering (search before impersonation)

**All 29 tests passing** ✅

## Files Modified

1. **Created**: `src/ui/src/components/MenuBar/PageSearch.tsx` (215 lines)
2. **Created**: `src/ui/src/components/MenuBar/PageSearch.test.tsx` (393 lines)
3. **Created**: `src/ui/src/components/MenuBar/MenuBar.test.tsx` (241 lines)
4. **Modified**: `src/ui/src/components/MenuBar/MenuBar.tsx`
   - Added PageSearch import
   - Updated layout with flex containers
   - Added vertical separator for impersonation
   - **PageSearch positioned before impersonation**
5. **Modified**: `src/ui/src/styles/index.css`
   - Added `.navbuttons` styles
   - **Preserved original menubar styling** (no background changes)

## Usage Example

```tsx
<MenuBar
  menuInfo={menuData}
  navigationData={navigationResponse}
  impersonationMultiSelect={<ImpersonationMultiSelect ... />}
/>
```

The PageSearch component automatically:

- Receives navigation data from MenuBar props
- Flattens the navigation tree
- Provides search functionality
- Handles navigation on selection

## User Experience

1. User types in the search box (e.g., "terminated letters")
2. Dropdown shows matching pages with context:
   ```
   Terminated Letters
   QPROF003-1
   in Inquiries & Adjustments > Adhoc Reports
   ```
3. User clicks a result
4. App navigates to the selected page
5. **Drawer automatically opens** with the correct L0 section (e.g., "Inquiries & Adjustments")
6. **All parent levels expand** to show the full path (e.g., "Adhoc Reports" > "Terminated Letters")
7. **Selected page is highlighted** in the navigation tree with active styling
8. Search input clears automatically

The result is a seamless navigation experience where users can quickly jump to any page and see exactly where it lives in the navigation hierarchy.

## Technical Notes

- Uses MUI `Autocomplete` component with `freeSolo` mode
- `disablePortal` keeps dropdown in DOM hierarchy for testing
- Manual filtering (`filterOptions={(x) => x}`) for custom search logic
- Memoized search results for performance
- Dropdown only opens when input has content

### Auto-Expansion Implementation

The drawer's `RecursiveNavItem` component includes intelligent auto-expansion logic:

```typescript
// Check if this item or any descendant contains the active path
const containsActivePath = useCallback(
  (navItem: NavigationDto): boolean => {
    const navItemPath = navItem.url?.replace(/^\/+/, "");
    if (currentPath === navItemPath) return true;

    if (navItem.items && navItem.items.length > 0) {
      return navItem.items.some((child) => containsActivePath(child));
    }

    return false;
  },
  [currentPath]
);

// Auto-expand if this item contains the active path
useEffect(() => {
  if (hasActiveChild && !isActive) {
    setExpanded(true);
  }
}, [hasActiveChild, isActive]);

// Re-check localStorage when location changes
useEffect(() => {
  const storedExpanded = getStoredExpanded();
  if (storedExpanded && !expanded) {
    setExpanded(true);
  }
}, [location.pathname]);
```

This ensures that when navigating from the page search:

1. The search sets localStorage expanded states for all parents
2. React Router navigates to the new page
3. The drawer detects the route change and re-checks localStorage
4. Parent items auto-expand because they contain the active path
5. The full hierarchy becomes visible

## Future Enhancements (Optional)

- Keyboard shortcuts (e.g., Ctrl+K to focus search)
- Recent searches
- Search result highlighting
- Fuzzy matching
- Search by navigation ID or other metadata
- Analytics tracking for popular searches
