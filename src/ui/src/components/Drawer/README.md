# PSDrawer MVVM Implementation

## Overview

The refactored drawer implements MVVM (Model-View-ViewModel) pattern with **zero hardcoded IDs**. All navigation logic is based on the data structure from the API where:
- **L0 (Menu Bar)**: Items where `parentId === null`
- **L1+ (Drawer)**: All nested descendants

This provides better testability, maintainability, and a fully dynamic navigation system.

## Navigation Structure

The API returns a multi-dimensional structure:

```json
{
  "navigation": [
    {
      "parentId": null,        // ← L0: Menu bar (YEAR END, INQUIRIES, etc.)
      "title": "YEAR END",
      "id": 6,
      "items": [               // ← L1: First level in drawer
        {
          "parentId": 6,
          "title": "December Activities",
          "items": [           // ← L2: Second level in drawer
            { "parentId": 9, "title": "Clean up Reports", "items": [] }
          ]
        }
      ]
    }
  ]
}
```

**Key Insight**: Configuration uses **titles** (not IDs) to reference L0 items from the API.

See `DYNAMIC_NAVIGATION.md` for complete documentation.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         VIEW LAYER                          │
│  PSDrawer.refactored.tsx - Pure presentation component      │
│  RecursiveNavItem.tsx - Recursive navigation rendering      │
└─────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────┐
│                      VIEWMODEL LAYER                        │
│  hooks/useDrawerViewModel.ts - Business logic & state       │
└─────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────┐
│                        MODEL LAYER                          │
│  models/DrawerConfig.ts - Configuration                     │
│  NavigationDto - Data structure (from backend)              │
│  Redux State - Application state                            │
└─────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────┐
│                       UTILITIES                             │
│  utils/drawerUtils.ts - Pure helper functions               │
└─────────────────────────────────────────────────────────────┘
```

## File Structure

```
src/components/Drawer/
├── PSDrawer.refactored.tsx        # New MVVM-based drawer (150 lines)
├── PSDrawer.original.tsx          # Backup of original (625 lines)
├── PSDrawer.tsx                   # Current implementation (to be replaced)
├── RecursiveNavItem.tsx           # Recursive item renderer
│
├── hooks/
│   ├── index.ts                   # Barrel export
│   └── useDrawerViewModel.ts     # ViewModel hook (business logic)
│
├── models/
│   ├── index.ts                   # Barrel export
│   └── DrawerConfig.ts            # Configuration types and presets
│
└── utils/
    ├── index.ts                   # Barrel export
    └── drawerUtils.ts             # Helper functions
```

## Key Improvements

### 1. Recursive Navigation (No More 3-Level Limit)

**Before:**
```typescript
// Hardcoded 3 levels: MenuLevel → TopPage → SubPages
interface MenuLevel {
  mainTitle: string;
  topPage: TopPage[];
}
interface TopPage {
  topTitle: string;
  subPages: SubPages[];  // Only 1 level of nesting
}
```

**After:**
```typescript
// Unlimited nesting via recursive NavigationDto
<RecursiveNavItem 
  item={navigationItem}  // Can nest infinitely
  level={0}
/>
```

### 2. No More "YEAR END" Hardcoding

**Before:**
```typescript
// Hardcoded to find "YEAR END" specifically
const yearEndList = data.navigation.find((m) => m.title === "YEAR END");
```

**After:**
```typescript
// Configurable for any navigation section
const config = DRAWER_CONFIGS.YEAR_END;  // Or REPORTS, BENEFICIARIES, etc.
<PSDrawer navigationData={data} drawerConfig={config} />
```

### 3. Testable Business Logic

**Before:**
```typescript
// Must render entire component to test
render(<PSDrawer navigationData={mockData} />);
fireEvent.click(screen.getByRole('button'));
```

**After:**
```typescript
// Test ViewModel logic without rendering
const { result } = renderHook(() => useDrawerViewModel(mockData, mockConfig));
act(() => result.current.toggleDrawer());
expect(mockDispatch).toHaveBeenCalledWith(openDrawer());
```

### 4. Clear Separation of Concerns

**Before (PSDrawer.tsx):**
- 625 lines mixing UI, logic, state, and side effects

**After:**
- **View (150 lines)**: Pure presentation, no logic
- **ViewModel (200 lines)**: All business logic, testable
- **Models (100 lines)**: Configuration and types
- **Utils (200 lines)**: Reusable helper functions

## Usage

### Basic Usage (Backwards Compatible)

```typescript
// Defaults to Year End drawer
<PSDrawer navigationData={navigationData} />
```

### With Explicit Configuration

```typescript
import { DRAWER_CONFIGS } from "./components/Drawer/models";

// Year End drawer (same as default)
<PSDrawer 
  navigationData={navigationData}
  drawerConfig={DRAWER_CONFIGS.YEAR_END}
/>

// Reports drawer (new capability!)
<PSDrawer 
  navigationData={navigationData}
  drawerConfig={DRAWER_CONFIGS.REPORTS}
/>
```

### Custom Configuration

```typescript
const customConfig: DrawerConfig = {
  rootNavigationId: 55,           // Navigation item ID
  title: "My Custom Section",
  autoExpandDepth: 2,             // Auto-expand 2 levels
  showStatus: true,
  itemFilter: (item) => !item.disabled
};

<PSDrawer 
  navigationData={navigationData}
  drawerConfig={customConfig}
/>
```

## Configuration Options

### Predefined Configs (in `DRAWER_CONFIGS`)

```typescript
export const DRAWER_CONFIGS = {
  YEAR_END: {
    rootNavigationId: 55,
    title: "Year End",
    autoExpandDepth: 1,    // Expand first level by default
    showStatus: true       // Show status chips
  },
  
  REPORTS: {
    rootNavigationId: 60,
    title: "Reports",
    autoExpandDepth: 0,    // Don't auto-expand
    showStatus: false
  },
  
  // Add more as needed...
};
```

### Auto-Expand Depth

- `0`: No auto-expansion (all collapsed initially)
- `1`: Expand first level only
- `2`: Expand first two levels
- `999`: Expand everything

Items containing the active route always auto-expand regardless of this setting.

## ViewModel API

The `useDrawerViewModel` hook provides:

### State
```typescript
{
  isOpen: boolean;              // Is drawer open?
  activeSubmenu: string | null; // Current submenu (if any)
  drawerItems: NavigationDto[]; // Top-level items
  currentPath: string;          // Current route path
  drawerTitle: string;          // Title from config
}
```

### Actions
```typescript
{
  toggleDrawer: () => void;                     // Open/close drawer
  selectMenuItem: (item: NavigationDto) => void; // Select top-level item
  navigateToItem: (item: NavigationDto) => void; // Navigate to route
  goBackToMainMenu: () => void;                  // Exit submenu view
}
```

### Computed Values
```typescript
{
  activeTopLevelItem: NavigationDto | null;  // Selected top-level item
  visibleItems: NavigationDto[];             // Items currently shown
  isInSubmenuView: boolean;                  // In submenu mode?
}
```

### Helper Functions
```typescript
{
  isItemActive: (item: NavigationDto) => boolean;    // Is route active?
  hasActiveChild: (item: NavigationDto) => boolean;  // Has active descendant?
  shouldAutoExpand: (item: NavigationDto, level: number) => boolean;
}
```

## Migration Path

### Phase 1: Side-by-Side (Current)
- `PSDrawer.refactored.tsx` - New MVVM implementation
- `PSDrawer.tsx` - Original implementation (still active)
- Both coexist for testing and comparison

### Phase 2: Testing (Next Step)
1. Test new drawer in development
2. Verify all features work (navigation, state persistence, etc.)
3. Run E2E tests
4. Compare visual appearance

### Phase 3: Gradual Rollout
1. Use feature flag to control which drawer is shown
2. Enable for subset of users
3. Monitor for issues
4. Gradually increase rollout

### Phase 4: Complete Migration
```powershell
# Replace old drawer with new one
Remove-Item src/ui/src/components/Drawer/PSDrawer.tsx
Rename-Item src/ui/src/components/Drawer/PSDrawer.refactored.tsx PSDrawer.tsx

# Clean up old code
# Remove menuLevels() function from MenuData.ts
# Remove MenuLevel, TopPage, SubPages interfaces
```

## Testing

### Unit Tests (ViewModel)

```typescript
// Test business logic without rendering
describe('useDrawerViewModel', () => {
  it('should toggle drawer open', () => {
    const { result } = renderHook(() => 
      useDrawerViewModel(mockData, mockConfig)
    );
    
    act(() => result.current.toggleDrawer());
    
    expect(result.current.isOpen).toBe(true);
  });
  
  it('should filter non-navigable items', () => {
    const { result } = renderHook(() => 
      useDrawerViewModel(dataWithHidden, mockConfig)
    );
    
    expect(result.current.drawerItems).toHaveLength(2);
  });
});
```

### Integration Tests (View + ViewModel)

```typescript
describe('PSDrawer Integration', () => {
  it('should navigate on item click', async () => {
    render(<PSDrawer navigationData={mockData} />);
    
    const item = screen.getByText('December Activities');
    await userEvent.click(item);
    
    expect(mockNavigate).toHaveBeenCalledWith('/december-activities');
  });
});
```

## Troubleshooting

### Import Errors

If you see import errors, ensure barrel exports are in place:

```typescript
// hooks/index.ts
export { useDrawerViewModel } from './useDrawerViewModel';

// models/index.ts
export { DRAWER_CONFIGS, getDefaultDrawerConfig } from './DrawerConfig';

// utils/index.ts
export { findNavigationById, containsActivePath } from './drawerUtils';
```

### Type Errors

If NavigationDto types aren't found:

```typescript
// Ensure correct import path
import { NavigationDto } from "../../reduxstore/types";
// Not from "../../../../reduxstore/types"
```

### Drawer Not Showing Items

Check that:
1. `navigationData` is loaded and not undefined
2. `rootNavigationId` in config matches actual navigation ID
3. Items have `isNavigable !== false`

Debug with:
```typescript
console.log('Navigation data:', navigationData);
console.log('Drawer items:', vm.drawerItems);
console.log('Config:', drawerConfig);
```

## Performance Considerations

### Memoization

The ViewModel uses `useMemo` and `useCallback` to prevent unnecessary re-renders:

```typescript
const drawerItems = useMemo(
  () => drawerRootItem?.items ?? [],
  [drawerRootItem]
);
```

### Lazy Rendering

`RecursiveNavItem` only renders visible items. Collapsed sections aren't rendered:

```typescript
<Collapse in={expanded}>
  {/* Children only rendered when expanded */}
</Collapse>
```

### localStorage

Expanded state is persisted to localStorage to maintain user preferences across sessions.

## Future Enhancements

### Multiple Drawers

Support switching between different sections:

```typescript
const [activeConfig, setActiveConfig] = useState(DRAWER_CONFIGS.YEAR_END);

<PSDrawer 
  navigationData={navigationData}
  drawerConfig={activeConfig}
/>
```

### Search/Filter

Add search capability to ViewModel:

```typescript
const [searchQuery, setSearchQuery] = useState('');

const filteredItems = useMemo(() => 
  filterNavigationTree(drawerItems, item => 
    item.title.toLowerCase().includes(searchQuery.toLowerCase())
  ),
  [drawerItems, searchQuery]
);
```

### Keyboard Navigation

Add keyboard shortcuts (already supported by MUI but can be enhanced).

### Drag & Drop Reordering

If needed, add drag-drop capability for reordering menu items.

## Benefits Summary

| Metric | Before | After | Improvement |
|--------|--------|-------|-------------|
| Component Size | 625 lines | 150 lines | 76% reduction |
| Testability | Requires rendering | Unit test hooks | Much easier |
| Reusability | Hardcoded | Configurable | Fully reusable |
| Nesting Depth | 3 levels max | Unlimited | Scalable |
| Configuration | Hardcoded "YEAR END" | Any section | Flexible |
| Maintainability | Mixed concerns | Clear separation | Much better |

## Documentation

- **MVVM_ANALYSIS.md** - Complete architectural analysis
- **NAVIGATION_REFACTOR_PROPOSAL.md** - Original proposal document
- **DRAWER_MIGRATION_GUIDE.md** - Step-by-step migration guide
- **This README** - Implementation reference

## Support

For questions or issues:
1. Check existing documentation files
2. Review `useDrawerViewModel.ts` comments
3. Compare with `PSDrawer.original.tsx` for reference
4. Reach out to team for assistance

---

**Status**: ✅ Implementation complete, ready for testing
**Next Step**: Test in development environment and verify all features work correctly
