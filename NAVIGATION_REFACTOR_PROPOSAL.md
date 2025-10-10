# Navigation Refactor Proposal: Dynamic Multi-Level Drawer

## Problem Statement

The current drawer implementation (`PSDrawer.tsx`) is hardcoded to:
1. Only display children of "YEAR END" menu item
2. Support exactly 3 levels of nesting (L1, L2, L3)
3. Use custom flattened interfaces instead of the recursive `NavigationDto` structure

This prevents the drawer from being:
- Reusable for other top-level menu items
- Flexible for arbitrary nesting depths
- Consistent with the backend's hierarchical data model

## Current Architecture

### Data Flow
```
NavigationResponseDto (from API)
├── navigation: NavigationDto[]  (recursive structure)
    ├── id, title, url, items: NavigationDto[]
    └── items can nest infinitely
```

### Current Processing
```typescript
// MenuData.ts - Menu bar (GOOD: uses recursive structure)
MenuData(navigationData) → RouteCategory[] (for menu bar)
  - Processes all top-level items
  - Recursively handles children via getRouteData()

// MenuData.ts - Drawer (BAD: hardcoded for "YEAR END")
menuLevels(navigationData) → MenuLevel[] (for drawer)
  - Finds "YEAR END" item specifically
  - Flattens to 3-level structure: MenuLevel → TopPage → SubPages
  - Cannot handle arbitrary depth or other menu sections
```

## Proposed Solution

### 1. Generic Drawer Component

Create a drawer that accepts **any navigation item** and renders its children recursively:

```typescript
export interface PSDrawerProps extends ICommon {
  navigationData?: NavigationResponseDto;
  drawerRootId?: number; // ID of navigation item whose children to display
  drawerTitle?: string;  // Title to show in drawer header
}

const PSDrawer: FC<PSDrawerProps> = ({ 
  navigationData, 
  drawerRootId, 
  drawerTitle = "Navigation" 
}) => {
  // Find the root item by ID (or use first top-level item if not specified)
  const drawerRootItem = drawerRootId 
    ? findNavigationById(navigationData?.navigation, drawerRootId)
    : navigationData?.navigation?.[0];
    
  const drawerItems = drawerRootItem?.items ?? [];
  
  return (
    <Drawer>
      {/* Render recursive navigation tree */}
      {drawerItems.map(item => (
        <RecursiveNavItem 
          key={item.id} 
          item={item} 
          level={0}
          maxExpandedLevel={2} // Control auto-expansion depth
        />
      ))}
    </Drawer>
  );
};
```

### 2. Recursive Navigation Item Component

```typescript
interface RecursiveNavItemProps {
  item: NavigationDto;
  level: number;
  maxExpandedLevel?: number; // Control which levels auto-expand
}

const RecursiveNavItem: FC<RecursiveNavItemProps> = ({ 
  item, 
  level, 
  maxExpandedLevel = 1 
}) => {
  const navigate = useNavigate();
  const location = useLocation();
  const [expanded, setExpanded] = useState(false);
  
  const hasChildren = item.items && item.items.length > 0;
  const isActive = location.pathname === `/${item.url}`;
  const hasActiveChild = hasChildren && containsActivePath(item.items, location.pathname);
  
  // Auto-expand if this level should be expanded or contains active route
  useEffect(() => {
    if (level <= maxExpandedLevel || hasActiveChild) {
      setExpanded(true);
    }
  }, [level, maxExpandedLevel, hasActiveChild]);
  
  const handleClick = () => {
    if (hasChildren) {
      setExpanded(!expanded);
    } else if (item.url) {
      navigate(`/${item.url}`);
    }
  };
  
  return (
    <>
      <ListItemButton
        onClick={handleClick}
        sx={{
          pl: 2 + (level * 2), // Indent based on level
          backgroundColor: isActive ? `${HIGHLIGHT_COLOR}15` : 'transparent',
          borderLeft: isActive ? `4px solid ${HIGHLIGHT_COLOR}` : '4px solid transparent',
        }}>
        <ListItemText primary={item.title} />
        {hasChildren && (expanded ? <ExpandLess /> : <ExpandMore />)}
      </ListItemButton>
      
      {hasChildren && (
        <Collapse in={expanded}>
          <List disablePadding>
            {item.items.map(childItem => (
              <RecursiveNavItem
                key={childItem.id}
                item={childItem}
                level={level + 1}
                maxExpandedLevel={maxExpandedLevel}
              />
            ))}
          </List>
        </Collapse>
      )}
    </>
  );
};

// Helper function
const containsActivePath = (items: NavigationDto[], pathname: string): boolean => {
  for (const item of items) {
    if (`/${item.url}` === pathname) return true;
    if (item.items && containsActivePath(item.items, pathname)) return true;
  }
  return false;
};
```

### 3. Configuration-Driven Approach

Instead of hardcoding "YEAR END", use configuration:

```typescript
// drawerConfig.ts
export interface DrawerConfig {
  rootNavigationId: number;  // Which nav item's children to display
  title: string;             // Drawer header title
  autoExpandDepth: number;   // How many levels to auto-expand
}

export const DRAWER_CONFIGS: Record<string, DrawerConfig> = {
  YEAR_END: {
    rootNavigationId: 55, // "YEAR END" menu ID
    title: "Year End",
    autoExpandDepth: 1
  },
  REPORTS: {
    rootNavigationId: 60, // Example: "REPORTS" menu ID
    title: "Reports",
    autoExpandDepth: 0
  }
  // Add more as needed
};

// Usage in App.tsx or layout component
const activeDrawerConfig = DRAWER_CONFIGS.YEAR_END; // Or dynamically select

<PSDrawer 
  navigationData={navigationData}
  drawerRootId={activeDrawerConfig.rootNavigationId}
  drawerTitle={activeDrawerConfig.title}
/>
```

### 4. Backwards Compatibility

To maintain current functionality during transition:

```typescript
// Keep menuLevels() as a facade that uses the new recursive structure
export const menuLevels = (data: NavigationResponseDto | undefined): MenuLevel[] => {
  // Use YEAR_END_MENU_ID constant
  const yearEndItem = findNavigationById(data?.navigation, YEAR_END_MENU_ID);
  return convertToLegacyMenuLevels(yearEndItem?.items ?? []);
};

// Gradually migrate to direct use of NavigationDto structure
```

## Implementation Steps

### Phase 1: Foundation (1-2 days)
1. Create `RecursiveNavItem` component
2. Create helper utilities:
   - `findNavigationById(items, id)`
   - `containsActivePath(items, pathname)`
   - `flattenNavigationTree(items)` for breadcrumbs
3. Add unit tests for utilities

### Phase 2: Drawer Refactor (2-3 days)
1. Create new `PSDrawerV2` component using recursive approach
2. Add configuration system (`drawerConfig.ts`)
3. Update Redux state to track active drawer config
4. Test thoroughly with existing Year End structure

### Phase 3: Migration (1 day)
1. Replace `PSDrawer` with `PSDrawerV2`
2. Remove `menuLevels()`, `MenuLevel`, `TopPage`, `SubPages` interfaces
3. Update all drawer-related code to use `NavigationDto` directly
4. Remove hardcoded "YEAR END" search

### Phase 4: Enhancement (1-2 days)
1. Add drawer switching capability (e.g., switch between Year End, Reports, etc.)
2. Add keyboard navigation
3. Add accessibility improvements
4. Performance optimization (virtualization for large trees)

## Benefits

### Immediate
- **Reusability**: Drawer works for any navigation section
- **Consistency**: Uses same data structure as backend and menu bar
- **Maintainability**: Single recursive component vs. multiple hardcoded levels

### Long-term
- **Flexibility**: Support arbitrary nesting depth
- **Extensibility**: Easy to add new drawer sections
- **Scalability**: Backend can change navigation structure without frontend changes

## Example: Multiple Drawers

With this approach, you could have:

```typescript
// Year End drawer (current functionality)
<PSDrawer 
  navigationData={navigationData}
  drawerRootId={YEAR_END_MENU_ID}
  drawerTitle="Year End"
/>

// Reports drawer (new capability)
<PSDrawer 
  navigationData={navigationData}
  drawerRootId={REPORTS_MENU_ID}
  drawerTitle="Reports"
/>

// Or even a global drawer showing entire navigation
<PSDrawer 
  navigationData={navigationData}
  drawerRootId={null} // null = show all top-level items
  drawerTitle="All Navigation"
/>
```

## File Structure

```
src/
├── components/
│   └── Drawer/
│       ├── PSDrawer.tsx           # Main drawer container (refactored)
│       ├── RecursiveNavItem.tsx   # Recursive menu item component
│       ├── DrawerConfig.ts        # Configuration for different drawers
│       └── drawerUtils.ts         # Helper functions
├── MenuData.ts                    # Simplified: remove menuLevels eventually
└── types/
    └── navigation.ts              # Already has NavigationDto
```

## Testing Strategy

### Unit Tests
```typescript
describe('RecursiveNavItem', () => {
  it('renders single-level item without children', () => {});
  it('renders multi-level nested structure', () => {});
  it('auto-expands to specified depth', () => {});
  it('highlights active path', () => {});
  it('expands parent when child is active', () => {});
});

describe('drawerUtils', () => {
  it('findNavigationById returns correct item from tree', () => {});
  it('containsActivePath detects nested active routes', () => {});
});
```

### Integration Tests
```typescript
describe('PSDrawer Integration', () => {
  it('displays Year End navigation structure', () => {});
  it('navigates to correct route on item click', () => {});
  it('persists expanded state in localStorage', () => {});
  it('handles navigation data updates', () => {});
});
```

## Risk Mitigation

1. **Breaking Changes**: Keep old `menuLevels()` as facade during transition
2. **Visual Regression**: Capture screenshots before/after for comparison
3. **Performance**: Monitor render performance with large navigation trees
4. **User Training**: Document any UI behavior changes

## Alternatives Considered

### Alternative 1: Keep Current Structure
- **Pros**: No refactor needed, works for current use case
- **Cons**: Not extensible, hardcoded, maintenance burden

### Alternative 2: Hybrid Approach
- **Pros**: Gradually migrate, less risk
- **Cons**: Complexity of maintaining two systems

### Alternative 3: Third-party Tree Component
- **Pros**: Battle-tested, feature-rich
- **Cons**: Learning curve, dependency, less control

**Recommendation**: Proceed with proposed solution (custom recursive component)

## Conclusion

The proposed recursive drawer approach:
- ✅ Removes hardcoded "YEAR END" dependency
- ✅ Supports arbitrary nesting levels (not just 3)
- ✅ Uses consistent data structure throughout app
- ✅ Enables multiple drawer configurations
- ✅ Aligns with backend's hierarchical model
- ✅ Improves maintainability and extensibility

Estimated effort: 5-8 days total (including testing)
Risk level: Medium (requires thorough testing but path is clear)
