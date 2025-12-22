# SmartPSDrawer - Auto-Detecting Navigation Drawer

## Problem

The original PSDrawer defaults to "YEAR END" section only:

```tsx
// RouterSubAssembly.tsx
<PSDrawer navigationData={data} /> // ❌ Always shows Year End
```

This means:

- ❌ Going to `/master-inquiry` (under INQUIRIES) → Shows Year End drawer
- ❌ Going to `/distributions-inquiry` (under DISTRIBUTIONS) → Shows Year End drawer
- ❌ Only Year End routes show correct drawer

## Solution

**SmartPSDrawer** automatically detects which L0 navigation section contains the current route and displays the appropriate drawer.

```tsx
// RouterSubAssembly.tsx
<SmartPSDrawer navigationData={data} /> // ✅ Auto-detects correct section
```

Now:

- ✅ At `/master-inquiry` → Shows INQUIRIES drawer automatically
- ✅ At `/distributions-inquiry` → Shows DISTRIBUTIONS drawer automatically
- ✅ At `/december-activities` → Shows YEAR END drawer automatically
- ✅ At `/beneficiaries` → Shows BENEFICIARIES drawer automatically

## How It Works

```typescript
// SmartPSDrawer.tsx
const currentL0 = getL0NavigationForRoute(navigationData, location.pathname);
// Finds: "Which L0 section (INQUIRIES, YEAR END, etc.) contains this route?"

const drawerConfig = createDrawerConfig(currentL0.title);
// Creates: Config for that specific section dynamically

return <PSDrawer drawerConfig={drawerConfig} />;
// Shows: The correct drawer for the current section
```

## Route Detection Logic

The system searches the navigation tree to find which L0 item contains the current route:

```
Current Path: /master-inquiry

Search Navigation Tree:
  ├─ INQUIRIES (parentId: null) ← L0
  │  └─ MASTER INQUIRY (url: master-inquiry) ← FOUND!
  │
  ├─ DISTRIBUTIONS (parentId: null) ← L0
  │  └─ Distribution Inquiry (url: distributions-inquiry)
  │
  └─ YEAR END (parentId: null) ← L0
     ├─ December Activities
     └─ Fiscal Close

Result: Show INQUIRIES drawer
```

## Usage

### Automatic (Recommended)

```tsx
import SmartPSDrawer from "./components/Drawer/SmartPSDrawer";

<SmartPSDrawer navigationData={navigationData} />;
```

That's it! The drawer automatically:

1. Detects current route
2. Finds which L0 section contains it
3. Shows that section's drawer
4. Updates when route changes

### Manual (Advanced)

If you need explicit control:

```tsx
import PSDrawer from "./components/Drawer/PSDrawer";
import { createDrawerConfig } from "./components/Drawer/models";

// Explicitly specify which section
<PSDrawer
  navigationData={navigationData}
  drawerConfig={createDrawerConfig("INQUIRIES")}
/>;
```

## Example Scenarios

### Scenario 1: User navigates between sections

```
User clicks "Master Inquiry" in menu bar
  ↓
Route changes to /master-inquiry
  ↓
SmartPSDrawer detects route is under INQUIRIES
  ↓
Drawer shows: MASTER INQUIRY, ADJUSTMENTS
  ↓
User clicks "December Activities" in Year End menu
  ↓
Route changes to /december-activities
  ↓
SmartPSDrawer detects route is under YEAR END
  ↓
Drawer shows: December Activities, Fiscal Close, Print PS Jobs
```

### Scenario 2: Direct URL navigation

```
User types /distributions-inquiry in browser
  ↓
SmartPSDrawer detects route is under DISTRIBUTIONS
  ↓
Drawer shows: Distribution Inquiry (and other DISTRIBUTIONS items)
```

### Scenario 3: Unknown route

```
User navigates to /some-new-route not in navigation yet
  ↓
SmartPSDrawer can't find containing L0 section
  ↓
Falls back to default (YEAR END)
```

## Implementation Details

### Files Modified

1. **Created**: `SmartPSDrawer.tsx` (60 lines)

   - Wrapper component
   - Auto-detects current L0 section
   - Passes config to PSDrawer

2. **Modified**: `RouterSubAssembly.tsx`
   - Changed: `<PSDrawer />` → `<SmartPSDrawer />`
   - Import updated

### Key Functions Used

```typescript
// From utils/navigationStructureUtils.ts
getL0NavigationForRoute(navigationData, pathname);
// Returns: NavigationDto | undefined (the L0 item containing route)

// From models/DrawerConfig.ts
createDrawerConfig(l0Title);
// Returns: DrawerConfig for any L0 navigation title
```

## Benefits

### 1. Zero Configuration

No need to specify which drawer to show - it's automatic.

### 2. Consistent User Experience

Users always see navigation relevant to their current section.

### 3. Future-Proof

Backend adds new L0 sections → drawer automatically works for them.

### 4. Reduces Confusion

Users in "INQUIRIES" section see INQUIRIES navigation, not Year End.

## Testing

### Manual Testing

1. Navigate to different sections:

   ```
   /master-inquiry → Should show INQUIRIES drawer
   /distributions-inquiry → Should show DISTRIBUTIONS drawer
   /december-activities → Should show YEAR END drawer
   ```

2. Check drawer content matches section:

   - In INQUIRIES: Should see Master Inquiry, Adjustments
   - In DISTRIBUTIONS: Should see Distribution Inquiry items
   - In YEAR END: Should see December Activities, Fiscal Close

3. Verify drawer updates on navigation:
   - Click different menu bar items
   - Drawer should switch to match new section

### Automated Testing

```typescript
describe('SmartPSDrawer', () => {
  it('should show INQUIRIES drawer for /master-inquiry route', () => {
    render(<SmartPSDrawer navigationData={mockData} />, {
      initialRoute: '/master-inquiry'
    });

    expect(screen.getByText('MASTER INQUIRY')).toBeInTheDocument();
    expect(screen.queryByText('December Activities')).not.toBeInTheDocument();
  });

  it('should show YEAR END drawer for /december-activities route', () => {
    render(<SmartPSDrawer navigationData={mockData} />, {
      initialRoute: '/december-activities'
    });

    expect(screen.getByText('December Activities')).toBeInTheDocument();
    expect(screen.queryByText('MASTER INQUIRY')).not.toBeInTheDocument();
  });
});
```

## Rollback

If SmartPSDrawer causes issues, revert to explicit config:

```tsx
// RouterSubAssembly.tsx
import PSDrawer from "./components/Drawer/PSDrawer";
import { getDefaultDrawerConfig } from "./components/Drawer/models";

<PSDrawer
  navigationData={data}
  drawerConfig={getDefaultDrawerConfig()} // Explicit Year End
/>;
```

## Summary

**Before**: Drawer always showed Year End section regardless of current route  
**After**: Drawer automatically shows the section containing the current route

This makes the navigation **context-aware** and provides a better user experience across all sections of the application.

---

**Created**: October 10, 2025  
**Status**: ✅ Implemented and ready for testing
