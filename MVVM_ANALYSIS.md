# MVVM Design Pattern Analysis for Navigation Drawer

## Executive Summary

**Recommendation: Yes, with caveats**

The navigation drawer would benefit from MVVM separation, but given the React ecosystem and existing Redux architecture, a **"React MVVM"** or **"Presentation/Container Pattern"** would be more appropriate than classical MVVM.

## Current Architecture Analysis

### Current Pattern: Mixed Concerns

```
PSDrawer.tsx (625 lines)
├── UI Logic (MUI components, styling)
├── State Management (useState, useSelector, useDispatch)
├── Business Logic (menuLevels transformation, route matching)
├── Side Effects (localStorage, navigation, profit year changes)
└── Event Handlers (drawer toggle, navigation clicks)
```

**Problems:**
- Single 625-line component doing too much
- Business logic mixed with UI rendering
- Hard to test without rendering
- Difficult to reuse logic across components

## MVVM Options for React

### Option 1: React MVVM (Recommended)

**Structure:**
```
Model (Data Layer)
├── NavigationDto (already exists)
├── DrawerConfig (already created)
└── Navigation state in Redux

ViewModel (Logic Layer)
├── useDrawerViewModel() hook
├── useNavigationTree() hook
└── Navigation business logic

View (Presentation Layer)
├── PSDrawer.tsx (pure presentation)
├── RecursiveNavItem.tsx (pure presentation)
└── UI components only
```

**Benefits:**
- Testable logic without rendering
- Reusable ViewModel hooks across components
- Clear separation of concerns
- Fits React/hooks paradigm naturally

### Option 2: Classical MVVM (Not Recommended)

Using MobX or similar for two-way binding:
```typescript
class DrawerViewModel {
  @observable isOpen = false;
  @observable activeSubmenu = null;
  
  @action toggleDrawer() { ... }
  @computed get visibleItems() { ... }
}
```

**Why not:**
- Redux already handles state
- Adds complexity/dependencies
- Against React unidirectional data flow
- Team not familiar with MobX

### Option 3: Presentation/Container Pattern (Alternative)

```
DrawerContainer.tsx
├── Fetches data
├── Handles business logic
├── Manages state
└── Passes props down

DrawerPresentation.tsx
├── Pure UI rendering
├── Receives props
├── Fires callbacks
└── No business logic
```

**Benefits:**
- Simpler than full MVVM
- Still separates concerns
- Easy to understand
- Easy to test

## Recommended Implementation: React MVVM

### Architecture

```
src/components/Drawer/
├── PSDrawer.tsx                    # View (presentation only)
├── RecursiveNavItem.tsx            # View (presentation only)
├── hooks/
│   ├── useDrawerViewModel.ts       # ViewModel logic
│   ├── useNavigationTree.ts        # Navigation business logic
│   ├── useDrawerState.ts           # State management
│   └── useActiveRoute.ts           # Route matching logic
├── models/
│   ├── DrawerConfig.ts             # Model (already created)
│   └── NavigationTypes.ts          # Type definitions
└── utils/
    └── drawerUtils.ts              # Utility functions (already created)
```

### Example: ViewModel Hook

```typescript
// hooks/useDrawerViewModel.ts
import { useCallback, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { RootState } from "../../../reduxstore/store";
import { openDrawer, closeDrawer, setActiveSubMenu, clearActiveSubMenu } from "../../../reduxstore/slices/generalSlice";
import { findNavigationById, containsActivePath } from "../utils/drawerUtils";
import { DrawerConfig } from "../models/DrawerConfig";

export interface DrawerViewModel {
  // State
  isOpen: boolean;
  activeSubmenu: string | null;
  drawerItems: NavigationDto[];
  currentPath: string;
  
  // Actions
  toggleDrawer: () => void;
  selectMenuItem: (item: NavigationDto) => void;
  navigateToItem: (item: NavigationDto) => void;
  goBack: () => void;
  
  // Computed
  activeTopLevelItem: NavigationDto | null;
  visibleItems: NavigationDto[];
  isItemActive: (item: NavigationDto) => boolean;
  hasActiveChild: (item: NavigationDto) => boolean;
}

export const useDrawerViewModel = (
  navigationData: NavigationResponseDto | undefined,
  config: DrawerConfig
): DrawerViewModel => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  
  // Select state from Redux
  const isOpen = useSelector((state: RootState) => state.general.isDrawerOpen);
  const activeSubmenu = useSelector((state: RootState) => state.general.activeSubmenu);
  
  // Memoized computed values
  const drawerRootItem = useMemo(
    () => findNavigationById(navigationData?.navigation, config.rootNavigationId),
    [navigationData, config.rootNavigationId]
  );
  
  const drawerItems = useMemo(
    () => drawerRootItem?.items ?? [],
    [drawerRootItem]
  );
  
  const activeTopLevelItem = useMemo(
    () => activeSubmenu ? drawerItems.find(item => item.title === activeSubmenu) : null,
    [activeSubmenu, drawerItems]
  );
  
  const visibleItems = useMemo(
    () => activeTopLevelItem?.items ?? drawerItems,
    [activeTopLevelItem, drawerItems]
  );
  
  const currentPath = location.pathname.replace(/^\/+/, '');
  
  // Actions
  const toggleDrawer = useCallback(() => {
    if (isOpen) {
      dispatch(closeDrawer());
      dispatch(clearActiveSubMenu());
    } else {
      dispatch(openDrawer());
    }
  }, [isOpen, dispatch]);
  
  const selectMenuItem = useCallback((item: NavigationDto) => {
    dispatch(setActiveSubMenu(item.title));
  }, [dispatch]);
  
  const navigateToItem = useCallback((item: NavigationDto) => {
    if (item.url) {
      const absolutePath = item.url.startsWith("/") ? item.url : `/${item.url}`;
      
      // Store navigation ID
      if (item.id) {
        localStorage.setItem("navigationId", item.id.toString());
      }
      
      navigate(absolutePath);
    }
  }, [navigate]);
  
  const goBack = useCallback(() => {
    dispatch(clearActiveSubMenu());
  }, [dispatch]);
  
  // Computed helpers
  const isItemActive = useCallback((item: NavigationDto) => {
    const itemPath = item.url?.replace(/^\/+/, '');
    return currentPath === itemPath;
  }, [currentPath]);
  
  const hasActiveChild = useCallback((item: NavigationDto) => {
    return item.items ? containsActivePath(item.items, location.pathname) : false;
  }, [location.pathname]);
  
  return {
    // State
    isOpen,
    activeSubmenu,
    drawerItems,
    currentPath,
    
    // Actions
    toggleDrawer,
    selectMenuItem,
    navigateToItem,
    goBack,
    
    // Computed
    activeTopLevelItem,
    visibleItems,
    isItemActive,
    hasActiveChild
  };
};
```

### Example: Refactored View (Presentation)

```typescript
// PSDrawer.tsx - Pure Presentation
import { FC } from "react";
import { Box, Drawer, IconButton, Typography, List, ListItem, ListItemButton } from "@mui/material";
import { ChevronLeft } from "@mui/icons-material";
import { ICommon } from "smart-ui-library";
import { NavigationResponseDto } from "../../reduxstore/types";
import { DrawerConfig, getDefaultDrawerConfig } from "./models/DrawerConfig";
import { useDrawerViewModel } from "./hooks/useDrawerViewModel";
import RecursiveNavItem from "./RecursiveNavItem";
import { SidebarIcon, SidebarCloseIcon } from "./DrawerIcons";
import { drawerClosedWidth, drawerOpenWidth } from "../../constants";

export interface PSDrawerProps extends ICommon {
  navigationData?: NavigationResponseDto;
  drawerConfig?: DrawerConfig;
}

/**
 * PSDrawer - Pure Presentation Component
 * 
 * Renders the navigation drawer UI. All business logic is in useDrawerViewModel.
 */
const PSDrawer: FC<PSDrawerProps> = ({ 
  navigationData,
  drawerConfig = getDefaultDrawerConfig() 
}) => {
  // ViewModel provides all logic and state
  const vm = useDrawerViewModel(navigationData, drawerConfig);
  
  return (
    <>
      {/* Toggle Button */}
      <Box
        sx={{
          position: "fixed",
          left: vm.isOpen ? "16px" : "12px",
          top: "179px",
          zIndex: (theme) => theme.zIndex.drawer + 100,
          display: "flex",
          alignItems: "center",
          gap: 1,
          justifyContent: "space-between",
          width: vm.isOpen ? "338px" : "auto",
          transition: (theme) =>
            theme.transitions.create("left", {
              easing: theme.transitions.easing.sharp,
              duration: theme.transitions.duration.enteringScreen
            })
        }}>
        {vm.isOpen && (
          <Typography variant="h6" sx={{ fontWeight: "bold" }}>
            {drawerConfig.title}
          </Typography>
        )}
        <IconButton onClick={vm.toggleDrawer}>
          {vm.isOpen ? <SidebarCloseIcon /> : <SidebarIcon />}
        </IconButton>
      </Box>

      {/* Drawer */}
      <Drawer
        variant="permanent"
        sx={{
          width: vm.isOpen ? drawerOpenWidth : drawerClosedWidth,
          display: vm.isOpen ? "block" : "none",
          "& .MuiDrawer-paper": {
            width: vm.isOpen ? drawerOpenWidth : drawerClosedWidth,
            borderRight: "1px solid #BDBDBD",
            boxSizing: "border-box",
            overflowX: "hidden",
            transition: "all 225ms"
          }
        }}>
        <Box sx={{ mt: "215px" }}>
          {vm.activeTopLevelItem ? (
            <SubmenuView
              item={vm.activeTopLevelItem}
              onBack={vm.goBack}
              onNavigate={vm.navigateToItem}
              autoExpandDepth={drawerConfig.autoExpandDepth}
            />
          ) : (
            <MainMenuView
              items={vm.drawerItems}
              onSelectItem={vm.selectMenuItem}
            />
          )}
        </Box>
      </Drawer>
    </>
  );
};

// Sub-components for clarity
const SubmenuView: FC<{...}> = ({ item, onBack, onNavigate, autoExpandDepth }) => (
  <>
    <ListItemButton onClick={onBack}>
      <ChevronLeft />
      <Typography variant="body2" sx={{ fontWeight: "bold" }}>
        {item.title}
      </Typography>
    </ListItemButton>
    <List>
      {item.items?.map((child) => (
        <RecursiveNavItem
          key={child.id}
          item={child}
          level={0}
          maxAutoExpandDepth={autoExpandDepth}
          onNavigate={onNavigate}
        />
      ))}
    </List>
  </>
);

const MainMenuView: FC<{...}> = ({ items, onSelectItem }) => (
  <List>
    {items.map((item) => (
      <ListItem key={item.id} disablePadding>
        <ListItemButton onClick={() => onSelectItem(item)}>
          <Typography variant="body2">{item.title}</Typography>
          <ChevronLeft sx={{ transform: "rotate(180deg)" }} />
        </ListItemButton>
      </ListItem>
    ))}
  </List>
);

export default PSDrawer;
```

### Testing Benefits

**Before (Current):**
```typescript
// Must render entire component to test logic
test('drawer opens on toggle', () => {
  render(<PSDrawer navigationData={mockData} />);
  const button = screen.getByRole('button');
  fireEvent.click(button);
  expect(screen.getByRole('navigation')).toBeVisible();
});
```

**After (MVVM):**
```typescript
// Test ViewModel logic without rendering
test('toggleDrawer dispatches correct actions', () => {
  const { result } = renderHook(() => useDrawerViewModel(mockData, mockConfig));
  
  act(() => {
    result.current.toggleDrawer();
  });
  
  expect(mockDispatch).toHaveBeenCalledWith(openDrawer());
});

// Test View rendering separately
test('PSDrawer renders toggle button', () => {
  const mockVM = {
    isOpen: false,
    toggleDrawer: jest.fn(),
    // ... other VM properties
  };
  
  render(<PSDrawer viewModel={mockVM} />);
  expect(screen.getByRole('button')).toBeInTheDocument();
});
```

## Implementation Phases

### Phase 1: Extract ViewModels (1-2 days)
1. Create `useDrawerViewModel` hook
2. Create `useNavigationTree` hook
3. Create `useActiveRoute` hook
4. Write comprehensive unit tests for hooks

### Phase 2: Refactor Views (2-3 days)
1. Update `PSDrawer` to use ViewModel hooks
2. Update `RecursiveNavItem` to be pure presentation
3. Extract sub-components (SubmenuView, MainMenuView)
4. Update integration tests

### Phase 3: Cleanup (1 day)
1. Remove business logic from view components
2. Consolidate state management
3. Update documentation
4. Code review and refinement

## Comparison: Current vs MVVM

| Aspect | Current (PSDrawer.tsx) | MVVM Approach |
|--------|------------------------|---------------|
| **Lines of code** | 625 lines | View: ~150, VM: ~200 |
| **Testability** | Must render UI | Test logic independently |
| **Reusability** | Logic tied to component | ViewModel hooks reusable |
| **Complexity** | High (all in one file) | Medium (distributed) |
| **Maintainability** | Low (hard to find logic) | High (clear separation) |
| **Learning curve** | Low | Medium |
| **Performance** | Same | Same (memoization in VM) |

## Alternative: Presentation/Container (Simpler)

If full MVVM is too complex, consider simpler Container pattern:

```
DrawerContainer.tsx (150 lines)
├── All business logic
├── State management
├── Event handlers
└── Passes props to presentation

DrawerPresentation.tsx (100 lines)
├── Pure UI rendering
├── Receives all props
└── No logic, just rendering
```

**Pros:** Simpler, easier to understand, still separates concerns
**Cons:** Less reusable than ViewModel hooks

## Recommendation

**Use React MVVM (ViewModel Hooks) because:**

1. ✅ **Already moving toward better architecture** with refactor
2. ✅ **Testability is critical** for complex navigation logic
3. ✅ **Reusability** - Other components need similar navigation logic
4. ✅ **Fits React paradigm** - Hooks are natural ViewModels
5. ✅ **Team can learn incrementally** - Start with one ViewModel
6. ✅ **Scales well** - Can apply pattern to other components

**Implementation order:**
1. Start with new `PSDrawerV2` using MVVM from the beginning
2. Extract ViewModels as hooks
3. Keep views as pure presentation components
4. Migrate old `PSDrawer` only after new one is stable

## Code Structure

```
src/components/Drawer/
├── PSDrawer.tsx                    # View (100 lines)
├── RecursiveNavItem.tsx            # View (150 lines)
│
├── hooks/                          # ViewModels
│   ├── useDrawerViewModel.ts       # Main drawer logic (200 lines)
│   ├── useNavigationTree.ts        # Tree operations (100 lines)
│   ├── useActiveRoute.ts           # Route matching (50 lines)
│   └── index.ts                    # Barrel export
│
├── models/                         # Models
│   ├── DrawerConfig.ts             # Already created
│   ├── DrawerState.ts              # State types
│   └── index.ts
│
├── utils/                          # Pure functions
│   └── drawerUtils.ts              # Already created
│
└── __tests__/
    ├── useDrawerViewModel.test.ts  # ViewModel tests
    ├── PSDrawer.test.tsx           # View tests
    └── integration.test.tsx        # E2E tests
```

## Conclusion

**Yes, adopt MVVM (React style with hooks)** for the drawer refactor because:
- Dramatically improves testability
- Enables logic reuse across components
- Separates concerns clearly
- Fits naturally with React hooks
- Makes codebase more maintainable

Start with `PSDrawerV2` as a clean slate, implement MVVM from the start, then migrate old drawer once proven.

**Estimated effort:** 4-5 days (includes testing)
**Risk level:** Low-Medium (incremental adoption, old code stays working)
**ROI:** High (better tests, maintainability, reusability)
