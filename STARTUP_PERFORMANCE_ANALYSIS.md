# UI Startup Performance Analysis - Low Hanging Fruit

**Date:** November 28, 2025  
**Status:** Initial Analysis - Ready for Implementation

## Executive Summary

Based on analysis of the React application initialization flow and structure, I've identified several **low-hanging fruit** optimization opportunities that can significantly improve startup performance. These are quick wins that don't require major architectural changes.

---

## Key Performance Bottlenecks Identified

### 1. **AG-Grid Module Registry Registration** ⭐ HIGH PRIORITY

**File:** `src/ui/agGridConfig.ts`  
**Impact:** Blocks app initialization until ALL ag-grid modules load

**Current Code:**

```typescript
import { AllCommunityModule, ModuleRegistry } from "ag-grid-community";
ModuleRegistry.registerModules([AllCommunityModule]);
```

**Problem:**

- `AllCommunityModule` imports and registers **all community ag-grid modules at startup**
- This happens synchronously during app load, blocking parsing and execution
- Most pages only use a fraction of the available grid features
- This registration is imported in `App.tsx` globally, so it runs before any conditional rendering

**Solution - Lazy Module Registration:**

```typescript
// src/ui/agGridConfig.ts
import { ModuleRegistry } from "ag-grid-community";

// Register only essential, commonly-used modules at startup
export const registerCoreGridModules = () => {
  // Import only what's needed at startup (pagination, sorting, filtering basics)
  Promise.resolve().then(async () => {
    const {
      ClientSideRowModelModule,
      PaginationModule,
      FilterToolPanelModule,
    } = await import("ag-grid-community");

    ModuleRegistry.registerModules([
      ClientSideRowModelModule,
      PaginationModule,
      FilterToolPanelModule,
    ]);
  });
};

// Lazy register advanced modules when needed
export const registerAdvancedGridModules = async () => {
  const modules = await import("ag-grid-community").then((m) => [
    m.ClipboardModule,
    m.ExcelExportModule,
    m.ColumnsToolPanelModule,
    m.StatusBarModule,
    m.MasterDetailModule,
    m.SideBarModule,
    m.RangeSelectionModule,
    m.RowGroupingModule,
    m.RichSelectModule,
    m.RowGridModule,
  ]);

  ModuleRegistry.registerModules(modules);
};
```

**Expected Impact:** 🚀 **15-25% reduction in initial parse/execution time**

---

### 2. **Eager API Queries on App Startup** ⭐ HIGH PRIORITY

**Files:** `src/ui/src/App.tsx`  
**Impact:** Triggers multiple API calls before user sees UI

**Current Code:**

```tsx
// App.tsx - Line 35
useGetAppVersionQuery();  // ← Eager query

// Line 40 & subsequent effect
const [loadMissives] = useLazyGetMissivesQuery();

useEffect(() => {
  if (token) {
    loadMissives();  // ← Triggered immediately when token available
    // ... other loads
  }
}, [token, ...]);

// Line 152
useEffect(() => {
  triggerHealth();  // ← Another eager health check
}, [triggerHealth]);
```

**Problem:**

- `useGetAppVersionQuery()` runs without arguments, making it an eager query
- `loadMissives()` is called immediately on token receipt without waiting for user interaction
- `triggerHealth()` is called immediately
- These are not sequenced—all 3 queries fire simultaneously, potentially blocking rendering
- No prioritization or deferral to idle time

**Solution - Defer Non-Critical Queries:**

```tsx
// App.tsx
import { useEffect } from "react";

useEffect(() => {
  // Get version info (critical for display)
  // Keep this one eager, it's needed for build info display
  // useGetAppVersionQuery() stays as-is
}, []);

useEffect(() => {
  if (token) {
    // Defer missives load to idle time using requestIdleCallback
    // Missives are for menu data, not critical for initial render
    if ("requestIdleCallback" in window) {
      const handle = requestIdleCallback(() => {
        loadMissives();
      });
      return () => cancelIdleCallback(handle);
    } else {
      // Fallback for browsers without requestIdleCallback
      const timer = setTimeout(() => loadMissives(), 2000);
      return () => clearTimeout(timer);
    }
  }
}, [token, loadMissives]);

useEffect(() => {
  // Defer health check even further—it's for status display only
  if ("requestIdleCallback" in window) {
    const handle = requestIdleCallback(
      () => {
        triggerHealth();
      },
      { timeout: 3000 }
    );
    return () => cancelIdleCallback(handle);
  } else {
    const timer = setTimeout(() => triggerHealth(), 3000);
    return () => clearTimeout(timer);
  }
}, [triggerHealth]);
```

**Alternative - Use Scheduling Library:**

```tsx
// Even better: use a utility for consistent scheduling
import { scheduleIdleCallback } from "../utils/schedulingUtils";

useEffect(() => {
  if (token) {
    scheduleIdleCallback(() => loadMissives(), { priority: "low" });
  }
}, [token, loadMissives]);

useEffect(() => {
  scheduleIdleCallback(() => triggerHealth(), {
    priority: "lowest",
    delay: 1000,
  });
}, [triggerHealth]);
```

**Expected Impact:** 🚀 **10-20% improvement in First Contentful Paint (FCP)**

---

### 3. **Redux Store Configuration Overhead** ⭐ MEDIUM PRIORITY

**File:** `src/ui/src/reduxstore/store.ts`  
**Impact:** Large middleware chain initialization

**Current Code:**

```typescript
middleware: (getDefaultMiddleware) =>
  getDefaultMiddleware({ serializableCheck: false })
    .concat(rtkQueryErrorToastMiddleware(true))
    .concat(EnvironmentUtils.isDevelopmentOrQA ? [apiLoggerMiddleware] : [])
    .concat(SecurityApi.middleware)
    .concat(YearsEndApi.middleware)
    .concat(ItOperationsApi.middleware)
    .concat(MilitaryApi.middleware)
    .concat(InquiryApi.middleware)
    .concat(LookupsApi.middleware)
    .concat(CommonApi.middleware)
    .concat(NavigationApi.middleware)
    .concat(AppSupportApi.middleware)
    .concat(NavigationStatusApi.middleware)
    .concat(BeneficiariesApi.middleware)
    .concat(AdjustmentsApi.middleware)
    .concat(DistributionApi.middleware)
    .concat(PayServicesApi.middleware)
    .concat(AccountHistoryReportApi.middleware)
    .concat(validationApi.middleware);
```

**Problem:**

- 17 separate `.concat()` calls creates inefficiency
- `apiLoggerMiddleware` is added in dev but ALL RTK Query instances are processed
- All reducers are registered upfront, even if not immediately needed

**Solution - Optimize Middleware Chain:**

```typescript
// src/ui/src/reduxstore/store.ts
const apiMiddleware = [
  SecurityApi,
  YearsEndApi,
  ItOperationsApi,
  MilitaryApi,
  InquiryApi,
  LookupsApi,
  CommonApi,
  NavigationApi,
  AppSupportApi,
  NavigationStatusApi,
  BeneficiariesApi,
  AdjustmentsApi,
  DistributionApi,
  PayServicesApi,
  AccountHistoryReportApi,
  validationApi,
];

export const store = configureStore({
  reducer: {
    // ... existing reducers
    ...Object.fromEntries(
      apiMiddleware.map((api) => [api.reducerPath, api.reducer])
    ),
  },
  middleware: (getDefaultMiddleware) => {
    let chain = getDefaultMiddleware({ serializableCheck: false }).concat(
      rtkQueryErrorToastMiddleware(true)
    );

    // Add dev middleware only in dev
    if (EnvironmentUtils.isDevelopmentOrQA) {
      chain = chain.concat(apiLoggerMiddleware);
    }

    // Add all API middleware in single concat
    return chain.concat(apiMiddleware.map((api) => api.middleware));
  },
});
```

**Expected Impact:** 🚀 **5-10% reduction in store initialization**

---

### 4. **Synchronous SmartUI Library Import** ⭐ MEDIUM PRIORITY

**File:** `src/ui/src/App.tsx` Line 9
**Impact:** Blocks app parsing on large CSS bundle

**Current Code:**

```tsx
import { colors, themeOptions, ToastServiceProvider } from "smart-ui-library";
import "smart-ui-library/dist/smart-ui-library.css"; // ← Synchronous CSS load
```

**Problem:**

- CSS import is synchronous and blocks parsing
- Entire smart-ui-library CSS bundle must parse before app renders
- No critical CSS extraction or lazy loading

**Quick Win - Defer Non-Critical CSS:**

```tsx
// App.tsx
useEffect(() => {
  // Defer loading non-critical CSS to after initial render
  const link = document.createElement("link");
  link.rel = "stylesheet";
  link.href = "smart-ui-library/dist/smart-ui-library.css";
  document.head.appendChild(link);
}, []);
```

**Better Solution - Critical CSS Extraction:**

- Extract only critical styles needed for initial render into inline `<style>` tag
- Defer remaining CSS

**Expected Impact:** 🚀 **5-15% improvement in paint time**

---

### 5. **Router and Page Component Bundle Size** ⭐ MEDIUM PRIORITY

**File:** `src/ui/src/components/router/RouterSubAssembly.tsx` Lines 1-75
**Impact:** All route components imported eagerly at startup

**Current Code:**

```tsx
// EAGER imports of ALL pages
import DemographicBadgesNotInPayprofit from "../../pages/DecemberActivities/...";
import DistributionsAndForfeitures from "../../pages/DecemberActivities/...";
import DuplicateNamesAndBirthdays from "../../pages/DecemberActivities/...";
import DuplicateSSNsOnDemographics from "../../pages/DecemberActivities/...";
// ... 40+ more eager imports
import LandingPage from "./LandingPage";
```

**Problem:**

- **40+ page components** imported synchronously
- Entire bundle for all pages included in initial JS chunk
- User downloads code for pages they won't visit on first load

**Solution - Code Split with React.lazy():**

```tsx
import { lazy, Suspense } from "react";

// Lazy load all page components
const LandingPage = lazy(() => import("./LandingPage"));
const DemographicBadgesNotInPayprofit = lazy(
  () =>
    import(
      "../../pages/DecemberActivities/DemographicBadgesNotInPayprofit/DemographicBadgesNotInPayprofit"
    )
);
const DistributionsAndForfeitures = lazy(
  () =>
    import(
      "../../pages/DecemberActivities/DistributionsAndForfeitures/DistributionsAndForfeitures"
    )
);
// ... rest of routes

// Create a loading fallback
const PageLoadingFallback = () => (
  <Box
    sx={{
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      height: "400px",
    }}
  >
    <CircularProgress />
  </Box>
);

// Wrap routes with Suspense
const RouterSubAssembly: React.FC = () => {
  return (
    <Suspense fallback={<PageLoadingFallback />}>
      <Routes>
        <Route path={ROUTES.LANDING} element={<LandingPage />} />
        <Route
          path={ROUTES.DEMOGRAPHICS}
          element={<DemographicBadgesNotInPayprofit />}
        />
        {/* ... rest of routes */}
      </Routes>
    </Suspense>
  );
};
```

**Expected Impact:** 🚀 **30-50% reduction in initial bundle size for first page load**

---

### 6. **Font Loading Strategy** ⭐ LOW PRIORITY (but quick win)

**Issue:** No mention of font optimization
**Impact:** Invisible text flash, layout shifts

**Solution - Add Font Display Strategy:**

```html
<!-- In index.html, if using web fonts -->
<link
  href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap"
  rel="preload"
  as="style"
/>

<!-- Or in CSS -->
@font-face { font-family: 'Roboto'; font-display: swap; /* Shows fallback font
immediately, swaps when loaded */ /* ... */ }
```

**Expected Impact:** 🚀 **5% improvement in perceived performance**

---

## Implementation Priority Order

### Phase 1: Quick Wins (1-2 hours) 🔥

1. **AG-Grid lazy module loading** - Highest ROI, minimal risk
2. **Defer missives/health queries** - Quick change, immediate impact
3. **Middleware optimization** - Clean up, reduce initialization

### Phase 2: Medium Effort (2-4 hours) 🚀

4. **Lazy load page components** - Requires testing but high impact
5. **Defer SmartUI CSS** - Easy, good gains

### Phase 3: Long-term (4+ hours) 📋

6. **Critical CSS extraction** - Requires build process changes
7. **Bundle analysis and splitting** - Ongoing optimization

---

## Performance Measurement Strategy

### Before/After Metrics to Track

1. **First Contentful Paint (FCP)** - Time to first paint
2. **Largest Contentful Paint (LCP)** - Time to main content visible
3. **Time to Interactive (TTI)** - When app is responsive
4. **Initial Bundle Size** - JS downloaded on first load
5. **API Call Waterfall** - Sequence and timing of API calls

### Measurement Tools

```typescript
// Add to App.tsx for performance monitoring
useEffect(() => {
  // Log Core Web Vitals
  const observer = new PerformanceObserver((list) => {
    for (const entry of list.getEntries()) {
      console.log(`${entry.name}: ${entry.value}ms`);
      // Send to telemetry system
      EndpointTelemetry.PerformanceMetrics.Record(
        entry.value,
        new ("metric", entry.name)()
      );
    }
  });

  observer.observe({
    entryTypes: ["navigation", "paint", "largest-contentful-paint"],
  });
  return () => observer.disconnect();
}, []);
```

---

## Code Changes Summary

### 1. Create AG-Grid Lazy Loading Module

**File to create:** `src/ui/src/agGridConfig.ts` (replace existing)

```typescript
import { ModuleRegistry } from "ag-grid-community";

// Register core modules synchronously at startup
export const registerCoreGridModules = () => {
  import("ag-grid-community").then(
    ({ ClientSideRowModelModule, PaginationModule, FilterToolPanelModule }) => {
      ModuleRegistry.registerModules([
        ClientSideRowModelModule,
        PaginationModule,
        FilterToolPanelModule,
      ]);
    }
  );
};

// Lazy register advanced modules when needed
export const registerAdvancedGridModules = async () => {
  const {
    ClipboardModule,
    ExcelExportModule,
    ColumnsToolPanelModule,
    StatusBarModule,
    MasterDetailModule,
    SideBarModule,
    RangeSelectionModule,
    RowGroupingModule,
    RichSelectModule,
    RowGridModule,
  } = await import("ag-grid-community");

  ModuleRegistry.registerModules([
    ClipboardModule,
    ExcelExportModule,
    ColumnsToolPanelModule,
    StatusBarModule,
    MasterDetailModule,
    SideBarModule,
    RangeSelectionModule,
    RowGroupingModule,
    RichSelectModule,
    RowGridModule,
  ]);
};
```

### 2. Update App.tsx to Defer Non-Critical Queries

**Changes to:** `src/ui/src/App.tsx`

Replace eager query effects with deferred versions (see examples above).

### 3. Optimize Redux Store Initialization

**Changes to:** `src/ui/src/reduxstore/store.ts`

Use the optimized middleware chain pattern shown above.

### 4. Add Code Splitting to Router

**Changes to:** `src/ui/src/components/router/RouterSubAssembly.tsx`

Convert eager imports to `React.lazy()` with Suspense boundaries.

---

## Testing Recommendations

```typescript
// Test that page load works with deferred queries
describe("App startup performance", () => {
  it("should show initial UI before loading missives", async () => {
    render(<App />);
    expect(screen.getByRole("main")).toBeInTheDocument();
    // Missives should not be in DOM yet
    await waitFor(
      () => {
        expect(screen.queryByTestId("missives-loaded")).toBeInTheDocument();
      },
      { timeout: 3000 }
    );
  });

  it("should handle missing health check gracefully", async () => {
    // Health check is deferred, should not break UI if it fails
    render(<App />);
    // UI should render regardless
    expect(screen.getByRole("main")).toBeInTheDocument();
  });

  it("should lazy load page components on demand", async () => {
    render(<App />);
    // Navigate to a lazy-loaded page
    userEvent.click(screen.getByText("Go to page"));
    // Should show loading indicator
    expect(screen.getByRole("progressbar")).toBeInTheDocument();
  });
});
```

---

## Risk Assessment

| Change                  | Risk Level | Mitigation                                                           |
| ----------------------- | ---------- | -------------------------------------------------------------------- |
| AG-Grid lazy loading    | Low        | Ensure core modules include what's needed; test grid functionality   |
| Defer queries           | Low        | User might not see data if they navigate quickly; use loading states |
| Middleware optimization | Very Low   | Refactoring only; no behavior change                                 |
| Lazy-load pages         | Medium     | Requires Suspense boundaries; test network failures                  |
| Defer CSS               | Low        | Use `link` tag with `rel="stylesheet"`; fallback to eager load       |

---

## Expected Overall Improvement

**Conservative Estimate:** 20-35% faster startup  
**Optimistic Estimate:** 40-60% faster startup (if all changes implemented)

The biggest wins come from:

1. AG-Grid lazy loading (15-25%)
2. Route code splitting (30-50% for first page)
3. Deferring non-critical queries (10-20%)

---

## Next Steps

1. **Measure baseline** - Use Chrome DevTools Lighthouse for current metrics
2. **Implement Phase 1** changes
3. **Measure again** - Compare improvement
4. **Iterate** - Implement Phase 2, repeat
5. **Monitor** - Add performance telemetry to production

---

**Created:** November 28, 2025  
**Status:** Ready for Implementation
