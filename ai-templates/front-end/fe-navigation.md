# Frontend Navigation System Guide

This guide describes how navigation works in the Smart Profit Sharing frontend, including menu structure, routing, and how to add new pages to the application.

## Navigation Architecture Overview

The navigation system has two main display areas:

1. **Top Navigation Bar (MenuBar)** - Horizontal menu across the top
2. **Left Drawer (PSDrawer)** - Collapsible sidebar for Year End workflows

## Data Flow

### Backend to Frontend

Navigation structure is fetched from the backend API:

```typescript
// API call in RouterSubAssembly.tsx
const { data, isSuccess } = useGetNavigationQuery(
  { navigationId: undefined },
  { skip: !token }
);

// Returns NavigationResponseDto
interface NavigationResponseDto {
  navigation: NavigationDto[];
}
```

### Navigation Data Structure

```typescript
interface NavigationDto {
  id: number;                    // Unique navigation item ID
  parentId: number | null;       // Parent ID (null for top-level)
  title: string;                 // Display name
  subTitle: string;              // Optional subtitle (legacy name)
  url: string;                   // Route path
  statusId?: number;             // Status tracking ID
  statusName?: string;           // "Complete", "In Progress", "On Hold"
  orderNumber: number;           // Sort order
  icon: string;                  // Icon identifier
  requiredRoles: string[];       // Required roles to view
  disabled: boolean;             // Is menu item disabled
  isNavigable?: boolean;         // Should appear in menus
  isReadOnly?: boolean;          // Read-only role restrictions
  items: NavigationDto[];        // Child menu items
}
```


## Top Navigation Bar (MenuBar)

### Location
`./src/ui/src/components/MenuBar/MenuBar.tsx`

### Menu Structure

The top navigation consists of:
- **Home** button (always visible)
- **Category buttons** with dropdowns (from `MenuData()`)
- **Impersonation selector** (development/QA only)

### How MenuData Works

`MenuData()` in `src/MenuData.ts` transforms backend navigation into top menu structure:

```typescript
export const MenuData = (data: NavigationResponseDto | undefined): RouteCategory[] => {
  if (!data || !data.navigation) {
    return [];
  }

  // Get top-level items (parentId === null)
  const topLevelItems = data.navigation
    .filter((m) => m.parentId === null && (m.isNavigable ?? true))
    .sort((a, b) => a.orderNumber - b.orderNumber);

  // Process each top-level item
  topLevelItems.forEach((values: NavigationDto) => {
    // Check role permissions
    const hasRequiredRole = values.requiredRoles.length > 0 &&
      values.requiredRoles.some((role) => localStorageImpersonating.includes(role));
    const noRolesRequired = values.requiredRoles.length === 0;

    if ((hasRequiredRole || noRolesRequired) && (values.isNavigable ?? true)) {
      finalData.push(createRouteCategory(values));
    }
  });

  return finalData;
};
```

### RouteCategory Type

```typescript
export type RouteCategory = {
  menuLabel: string;        // Display text in menu bar
  parentRoute: string;      // URL path
  items?: RouteData[];      // Dropdown items (if any)
  underlined?: boolean;     // Visual indicator
  disabled?: boolean;       // Is menu disabled
  roles?: string[];         // Required roles
};

export type RouteData = {
  caption: string;          // Display text in dropdown
  route: string;            // URL path
  divider?: boolean;        // Show divider after item
  disabled?: boolean;       // Is item disabled
  requiredPermission?: string;
};
```

### Example Menu Structure

```typescript
// Top-level category with dropdown
{
  menuLabel: "Inquiries",
  parentRoute: "inquiries",
  items: [
    { caption: "Master Inquiry", route: "master-inquiry" },
    { caption: "Beneficiary Inquiry", route: "beneficiary" }
  ]
}

// Top-level category without dropdown (direct link)
{
  menuLabel: "Reports",
  parentRoute: "/reports"
}
```

## Left Drawer (PSDrawer)

### Location
`./src/ui/src/components/Drawer/PSDrawer.tsx`

### Purpose
The drawer provides hierarchical navigation for **Year End** workflows, specifically:
- **December Activities** (with profit year selector)
- **Fiscal Close** (with profit year selector)

### Drawer Structure

The drawer has a 3-level hierarchy:

```
Year End (Drawer Title)

   December Activities (Main Level)
      Clean Up Reports (Top Page)
         Duplicate SSNs (Sub Page)
         Negative ETVA (Sub Page)
      Termination (Top Page - No Sub Pages)
      Distributions and Forfeitures (Top Page)

   Fiscal Close (Main Level)
       Eligible Employees (Top Page)
       Manage Executive Hours (Top Page)
```

### How menuLevels Works

`menuLevels()` in `src/MenuData.ts` creates drawer structure:

```typescript
export const menuLevels = (data: NavigationResponseDto | undefined): MenuLevel[] => {
  if (!data || !data.navigation) {
    return [];
  }

  // Find the Year End navigation item (ID 55)
  const YEAR_END_MENU_ID = 55;
  const yearEndList = data.navigation.find((m) => m.id === YEAR_END_MENU_ID);

  if (!yearEndList || !yearEndList.items) {
    return [];
  }

  // Transform items into MenuLevel structure
  return yearEndList.items
    .filter((v) => v.isNavigable ?? true)
    .map((value) => ({
      navigationId: value.id,
      statusId: value.statusId,
      statusName: value.statusName,
      mainTitle: value.title + addSubTitle(value.subTitle),
      topPage: value.items && value.items.length > 0 ? populateTopPage(value.items) : []
    }));
};
```

### MenuLevel Type

```typescript
interface MenuLevel {
  navigationId?: number;
  mainTitle: string;          // "December Activities", "Fiscal Close"
  statusId?: number;
  statusName?: string;
  topPage: TopPage[];         // Second-level items
}

interface TopPage {
  navigationId?: number;
  topTitle: string;           // "Clean Up Reports", "Termination"
  topRoute?: string;          // URL for this page
  statusId?: number;
  statusName?: string;        // "Complete", "In Progress", "On Hold"
  disabled?: boolean;
  subPages: SubPages[];       // Third-level items
}

interface SubPages {
  navigationId?: number;
  subTitle?: string;          // "Duplicate SSNs", "Negative ETVA"
  subRoute?: string;          // URL for this page
  statusId?: number;
  statusName?: string;
  disabled?: boolean;
}
```

### Drawer Features

**Profit Year Selector:**
- December Activities and Fiscal Close each have their own profit year selector
- Changes sync with Redux state and clear cached data

**Status Tracking:**
- Each page shows completion status ("Complete", "In Progress", "On Hold")
- Main levels show completion count (e.g., "5 of 10 completed")

**Active Route Highlighting:**
- Current page is highlighted in blue
- Parent sections auto-expand when navigating to child pages

## Routing System

### Route Definition

Routes are defined in `./src/ui/src/constants.ts`:

```typescript
export const ROUTES = {
  FISCAL_CLOSE: "fiscal-close",
  MASTER_INQUIRY: "master-inquiry",
  DISTRIBUTIONS_AND_FORFEITURES: "distributions-and-forfeitures",
  MANAGE_EXECUTIVE_HOURS: "manage-executive-hours-and-dollars",
  // ... more routes
} as const;
```

### Route Registration

Routes are mapped to components in `RouterSubAssembly.tsx`:

```typescript
<Routes>
  <Route
    path={ROUTES.MASTER_INQUIRY + "/:badgeNumber?"}
    element={<MasterInquiry />}
  />
  <Route
    path={ROUTES.DISTRIBUTIONS_AND_FORFEITURES}
    element={<DistributionsAndForfeitures />}
  />
  <Route
    path={ROUTES.MANAGE_EXECUTIVE_HOURS}
    element={<ManageExecutiveHoursAndDollars />}
  />
  {/* ... more routes */}
</Routes>
```



### Route Access Control

`RouterSubAssembly.tsx` validates navigation access:

```typescript
useEffect(() => {
  if (isSuccess && data?.navigation && token &&
      location.pathname !== "/unauthorized" &&
      location.pathname !== "/dev-debug" &&
      location.pathname !== "/documentation") {

    const currentPath = location.pathname;
    const isAllowed = isPathAllowedInNavigation(currentPath, data.navigation);

    if (!isAllowed) {
      const queryParams = createUnauthorizedParams(currentPath);
      navigate(`/unauthorized?${queryParams}`, { replace: true });
    }
  }
}, [isSuccess, data, location.pathname, navigate, token]);
```


### Legacy Name Display

Many pages show both new and legacy names:

```typescript
// Format: "New Report Name (Legacy Name)"
const getNewReportName = (caption: string): string => {
  if (!caption.includes("(")) {
    return caption;
  }
  return caption.split(" (")[0];
};

const getLegacyReportName = (caption: string): string => {
  const legacyReportName = caption.split(/[()]/)[1];
  if (legacyReportName === caption) {
    return "";
  }
  return legacyReportName;
};
```

Display in drawer:

```typescript
<ListItemText
  primary={getNewReportName(page.topTitle)}
  secondary={getLegacyReportName(page.topTitle)}
/>
```


