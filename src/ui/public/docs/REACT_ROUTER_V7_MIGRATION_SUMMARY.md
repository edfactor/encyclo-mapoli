# React Router v7 Migration Summary

## Overview

Successfully migrated the Profit Sharing application from legacy BrowserRouter pattern to React Router v7 data router API.

**Migration Date:** January 2025  
**React Router Version:** 7.12.0  
**Reason:** CVE security vulnerability + modernization to v7 best practices

## Problem Statement

After upgrading React Router to v7.12.0 (CVE-driven), the UI entered an infinite render loop. The legacy BrowserRouter pattern was incompatible with v7's expectations around data loading and navigation state management.

## Solution Architecture

### Core Pattern: Authentication-First Loader Architecture

React Router v7 introduces **loaders** - functions that run before route rendering to fetch required data. Our implementation enforces a critical security requirement: **authentication token MUST exist before making navigation API calls**.

```
┌─────────────────────────────────────────────────────┐
│                  Router Initialization               │
│  createBrowserRouter(createRoutes(store))           │
└──────────────────┬──────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────┐
│              Navigation Loader (Root)                │
│  1. Check token exists in Redux                     │
│  2. If no token → throw 401 Response                │
│  3. If token → dispatch RTK Query for navigation    │
│  4. Return navigation data                          │
└──────────────────┬──────────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
        ▼                     ▼
┌──────────────┐    ┌────────────────────┐
│  Token ✓     │    │  Token ✗ (401)     │
│  RootLayout  │    │  LoaderErrorBoundary│
│  renders     │    │  watches token     │
│  with data   │    │  auto-retries      │
└──────────────┘    └────────────────────┘
```

### Key Components

#### 1. **loaders.ts** - Authentication-Gated Navigation Loader

```typescript
export async function navigationLoader(store: AppStore): Promise<NavigationResponseDto> {
  const token = store.getState().security.token;

  if (!token) {
    throw new Response("Unauthorized", { status: 401 });
  }

  const queryAction = (store.dispatch as any)(
    NavigationApi.endpoints.getNavigation.initiate(...)
  );

  return await queryAction.unwrap();
}
```

**Critical**: This loader **throws 401** if token is missing, preventing unauthorized API calls. LoaderErrorBoundary handles retry when token appears.

#### 2. **routeConfig.tsx** - 48+ Routes with v7 Patterns

- **Root route** with loader and error boundary
- **Route-level Suspense** for each lazy component (v7 best practice)
- **Individual protected route wrapping** for role-based access
- **Conditional Okta routes** (login, callback) based on environment

```typescript
export function createRoutes(store: AppStore): RouteObject[] {
  return [
    {
      path: "/",
      loader: () => navigationLoader(store),
      element: <RootLayout />,
      errorElement: <LoaderErrorBoundary />,
      children: [
        {
          path: ROUTES.MASTER_INQUIRY,
          element: (
            <Suspense fallback={<PageLoadingFallback />}>
              <MasterInquiry />
            </Suspense>
          ),
        },
        {
          path: ROUTES.DEMOGRAPHIC_FREEZE,
          element: (
            <ProtectedRoute requiredRoles={[ImpersonationRoles.ItOperations]}>
              <Suspense fallback={<PageLoadingFallback />}>
                <DemographicFreeze />
              </Suspense>
            </ProtectedRoute>
          ),
        },
        // ... 46 more routes
      ],
    },
  ];
}
```

#### 3. **LoaderErrorBoundary.tsx** - Auto-Retry on Auth

Handles loader errors with automatic retry when authentication completes:

```typescript
const LoaderErrorBoundary: React.FC = () => {
  const error = useRouteError();
  const token = useSelector((state: RootState) => state.security.token);
  const navigate = useNavigate();

  const is401 = isRouteErrorResponse(error) && error.status === 401;

  // CRITICAL: Auto-retry when token appears
  useEffect(() => {
    if (token && is401) {
      navigate(0); // Revalidate all loaders
    }
  }, [token, is401, navigate]);

  // Show "Authentication in Progress" for 401 errors
  // Show "Server Error" with retry button for 500+ errors
  // Show "Unexpected Error" for other errors
};
```

#### 4. **RootLayout.tsx** - Replaces RouterSubAssembly

Uses `useLoaderData()` instead of `useGetNavigationQuery()`:

```typescript
const RootLayout: React.FC = () => {
  const navigationData = useLoaderData() as NavigationResponseDto;

  // Impersonation logic, MenuBar, Drawer, Breadcrumbs...

  return (
    <>
      {!isFullscreen && <MenuBar navigationData={navigationData} />}
      <Box>
        {!isFullscreen && <DSMDynamicBreadcrumbs />}
        {!isFullscreen && <SmartPSDrawer navigationData={navigationData} />}
        <Outlet /> {/* Child routes render here */}
      </Box>
    </>
  );
};
```

**Key difference**: Navigation data is pre-loaded by loader, no `skip` logic needed.

#### 5. **Router.tsx** - Data Router Initialization

```typescript
const Router = () => {
  const router = useMemo(() => createBrowserRouter(createRoutes(store)), []);
  return <RouterProvider router={router} />;
};

export default withOktaSecurity(Router);
```

#### 6. **RouteSecurity.tsx** - HOC Pattern

Refactored from component wrapper to higher-order component:

```typescript
export function withOktaSecurity<P>(RouterComponent: ComponentType<P>) {
  return (props: P) => {
    const oktaEnabled = EnvironmentUtils.isOktaEnabled;

    if (!oktaEnabled) {
      dispatch(setToken("dev-token"));
      return <RouterComponent {...props} />;
    }

    return (
      <OktaSecurityWrapper>
        <RouterComponent {...props} />
      </OktaSecurityWrapper>
    );
  };
}
```

**Fixed**: RestoreOriginalUriHandler infinite loop (added `useState` flag to track restoration).

#### 7. **useUnsavedChangesGuard.ts** - useBlocker API

Replaced deprecated `UNSAFE_NavigationContext` with v7's `useBlocker`:

```typescript
export const useUnsavedChangesGuard = (hasUnsavedChanges: boolean) => {
  const blocker = useBlocker(
    ({ currentLocation, nextLocation }) =>
      hasUnsavedChanges && currentLocation.pathname !== nextLocation.pathname
  );

  useEffect(() => {
    if (blocker.state === "blocked") {
      const confirmed = window.confirm("Leave without saving?");
      confirmed ? blocker.proceed() : blocker.reset();
    }
  }, [blocker]);

  return { showDialog: blocker.state === "blocked", ... };
};
```

## Files Modified/Created

### Created Files

- [components/router/loaders.ts](src/ui/src/components/router/loaders.ts) - Authentication-gated navigation loader
- [components/router/routeConfig.tsx](src/ui/src/components/router/routeConfig.tsx) - Route configuration with 48+ routes
- [components/router/LoaderErrorBoundary.tsx](src/ui/src/components/router/LoaderErrorBoundary.tsx) - Error boundary with auto-retry
- [components/router/RootLayout.tsx](src/ui/src/components/router/RootLayout.tsx) - Layout component using useLoaderData

### Modified Files

- [package.json](src/ui/package.json) - Updated @types/react-router-dom to ^7.0.0
- [reduxstore/store.ts](src/ui/src/reduxstore/store.ts) - Added `export type AppStore`
- [components/router/Router.tsx](src/ui/src/components/router/Router.tsx) - Switched to createBrowserRouter
- [components/router/RouteSecurity.tsx](src/ui/src/components/router/RouteSecurity.tsx) - Converted to HOC, fixed infinite loop
- [hooks/useUnsavedChangesGuard.ts](src/ui/src/hooks/useUnsavedChangesGuard.ts) - Replaced UNSAFE_NavigationContext with useBlocker
- 7 grid component files - Replaced `from "react-router"` with `from "react-router-dom"`

### Preserved Files (Not Modified)

- [components/ProtectedRoute/ProtectedRoute.tsx](src/ui/src/components/ProtectedRoute/ProtectedRoute.tsx) - Already compatible with Outlet pattern
- [components/router/RouterSubAssembly.tsx](src/ui/src/components/router/RouterSubAssembly.tsx) - Legacy file (can be deleted after verification)

## Migration Checklist (All ✅ Complete)

- [x] **Step 1**: Update TypeScript definitions (@types/react-router-dom ^7.0.0)
- [x] **Step 2**: Create authentication-gated navigation loader with token validation
- [x] **Step 3**: Create route configuration with all 48+ routes, Suspense, protected routes
- [x] **Step 4**: Create RootLayout component using useLoaderData
- [x] **Step 5**: Update Router.tsx to data router (createBrowserRouter + RouterProvider)
- [x] **Step 6**: Refactor RouteSecurity to HOC, fix RestoreOriginalUriHandler infinite loop
- [x] **Step 7**: Replace UNSAFE_NavigationContext with useBlocker API
- [x] **Step 8**: Verify ProtectedRoute Outlet compatibility (already compatible)
- [x] **Step 9**: Create LoaderErrorBoundary with auto-retry on auth completion
- [x] **Step 10**: Standardize imports (react-router → react-router-dom), verify tsc

## Testing Status

### Build Verification

- ✅ **TypeScript compilation**: `npm run tsc` - 0 errors
- ✅ **QA build**: `npm run build:qa` - Successful with all code-split chunks created
- ✅ **Chunk files**: All 48+ lazy routes generated separate chunk files (ProfitShareEditUpdate-CyRMF6_9.js, MasterInquiry-CcnfbY6T.js, etc.)

### Manual Testing Required

- [ ] **Authentication flow**: Verify Okta login → token → navigation loader → render
- [ ] **Auto-retry**: Verify 401 error → wait for token → auto-navigate(0) → success
- [ ] **Protected routes**: Verify role-based access control for IT Operations routes
- [ ] **Impersonation**: Verify localStorage persistence in dev/QA environments
- [ ] **Navigation guard**: Verify unsaved changes dialog with useBlocker
- [ ] **Drawer/MenuBar**: Verify layout rendering with useLoaderData
- [ ] **Code splitting**: Verify lazy routes load on demand (check Network tab)

## Breaking Changes

### ✅ Resolved in Migration

1. **BrowserRouter removed**: Replaced with createBrowserRouter (data router API)
2. **UNSAFE_NavigationContext removed**: Replaced with useBlocker
3. **Component-based routing**: Replaced with RouteObject configuration
4. **useGetNavigationQuery in layout**: Replaced with useLoaderData
5. **RouteSecurity wrapper**: Converted to withOktaSecurity HOC

### No Breaking Changes For

- **Page components**: No changes required (still lazy-loaded)
- **ProtectedRoute**: Already compatible with Outlet pattern
- **Redux state**: No changes to security/navigation slices
- **API calls**: RTK Query endpoints unchanged
- **Styling/UI**: MenuBar, Drawer, Breadcrumbs rendering preserved

## Performance Benefits

### Before (Legacy BrowserRouter)

- Navigation data fetched on every component render
- Skip logic created render loops when token missing
- No code splitting for routes
- Loading states scattered across components

### After (Data Router)

- Navigation data pre-loaded by loader (fetched once)
- Token validation before render (fail fast with 401)
- Auto code splitting with lazy() and route-level Suspense
- Centralized error handling with LoaderErrorBoundary
- No render loops (loader blocks until data ready)

## Security Improvements

1. **Token validation before API calls**: Loader throws 401 if token missing, preventing unauthorized requests
2. **Auto-retry on auth**: LoaderErrorBoundary watches token state, revalidates when ready
3. **Protected routes**: Individual wrapping allows fine-grained role-based access control
4. **Okta integration preserved**: withOktaSecurity HOC maintains authentication flow
5. **Impersonation persistence**: localStorage logic preserved for dev/QA environments

## Future Enhancements

### Recommended (Not Required)

1. **Delete RouterSubAssembly.tsx**: Legacy file no longer used (verify first)
2. **Add loader for protected routes**: Pre-validate roles in loader instead of component
3. **Add deferred data**: Use React Router `defer()` for non-critical data
4. **Add prefetch**: Use `<Link prefetch="intent">` for faster navigation
5. **Add revalidation**: Configure when loaders should re-run (on focus, interval, etc.)

### Optional (Advanced)

1. **Split loaders**: Separate navigation loader from page-specific loaders
2. **Cache strategy**: Implement loader-level caching for expensive queries
3. **Optimistic UI**: Use React Router actions for mutations with instant feedback
4. **Progressive enhancement**: Add forms with native submission fallback

## Troubleshooting

### If UI is stuck loading:

1. Check browser console for loader errors
2. Verify token exists in Redux: `store.getState().security.token`
3. Check Network tab for `/navigation` API call (should have Authorization header)
4. Verify LoaderErrorBoundary is rendering (should show "Authentication in Progress" if 401)

### If navigation not working:

1. Verify routes in routeConfig.tsx match ROUTES constants
2. Check for missing Suspense wrappers (will cause hydration errors)
3. Verify Outlet is present in RootLayout (child routes won't render without it)

### If authentication fails:

1. Check Okta configuration in .env files (VITE_REACT_APP_OKTA_CLIENT_ID, etc.)
2. Verify withOktaSecurity HOC is wrapping Router
3. Check OktaTokenSync is dispatching setToken action
4. Verify RestoreOriginalUriHandler restored flag prevents infinite loop

## References

- [React Router v7 Documentation](https://reactrouter.com/en/main)
- [Data Router API](https://reactrouter.com/en/main/routers/create-browser-router)
- [Loaders](https://reactrouter.com/en/main/route/loader)
- [Error Boundaries](https://reactrouter.com/en/main/route/error-element)
- [useBlocker Hook](https://reactrouter.com/en/main/hooks/use-blocker)

## Contributors

- AI Assistant (implementation)
- User (requirements, architecture decisions)

---

**Status**: ✅ Migration Complete - Ready for Testing  
**Next Steps**: Manual testing in dev/QA environments, then UAT/Production deployment
