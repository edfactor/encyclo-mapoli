---
applyTo: "src/ui/src/reduxstore/**/*.*"
---

# Redux & RTK Query Architecture Guide

## Overview

This document describes the Redux Toolkit (RTK) and RTK Query implementation patterns used in the Smart Profit Sharing UI application. The architecture follows modern Redux best practices with strong TypeScript typing and a clear separation between API data fetching (RTK Query) and local state management (Redux slices).

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Directory Structure](#directory-structure)
3. [Store Configuration](#store-configuration)
4. [RTK Query API Layer](#rtk-query-api-layer)
5. [Redux Slices](#redux-slices)
6. [Middleware](#middleware)
7. [Common Patterns](#common-patterns)
8. [Naming Conventions](#naming-conventions)
9. [Best Practices](#best-practices)

---

## Architecture Overview

The Redux implementation follows a **dual-reducer pattern**:

- **RTK Query APIs**: Handle all server data fetching, caching, and synchronization
- **Redux Slices**: Manage local UI state and client-side data transformations

### Key Technologies

- **Redux Toolkit (RTK)**: Modern Redux with less boilerplate
- **RTK Query**: Data fetching and caching solution built on top of RTK
- **TypeScript**: Full type safety across the state management layer
- **React-Redux**: React bindings for Redux

---

## Directory Structure

```
src/ui/src/
├── reduxstore/
│   ├── api/                    # RTK Query API definitions
│   │   ├── api.ts              # Base query configuration
│   │   ├── SecurityApi.ts      # Security-related endpoints
│   │   ├── YearsEndApi.ts      # Year-end operations
│   │   ├── LookupsApi.ts       # Lookup data endpoints
│   │   └── ...                 # Other domain APIs
│   ├── slices/                 # Redux Toolkit slices
│   │   ├── securitySlice.ts    # Security state (user, roles, etc.)
│   │   ├── generalSlice.ts     # General UI state
│   │   ├── messageSlice.ts     # Toast/alert messages
│   │   └── ...                 # Other domain slices
│   ├── store.ts                # Store configuration
│   ├── types.ts                # Type re-exports
│   └── healthTypes.ts          # Health check types
├── redux/
│   └── rtkQueryErrorToastMiddleware.ts  # Error handling middleware
└── middleware/
    └── apiLoggerMiddleware.ts  # API logging (dev/QA only)
```

---

## Store Configuration

### Store Setup (`store.ts`)

The store is configured using RTK's `configureStore`:

```typescript
import { configureStore } from "@reduxjs/toolkit";

export const store = configureStore({
  reducer: {
    // Regular Redux slices
    general: generalSlice,
    security: securitySlice,
    yearsEnd: yearsEndSlice,
    // ... other slices

    // RTK Query API reducers (dynamic reducer paths)
    [SecurityApi.reducerPath]: SecurityApi.reducer,
    [YearsEndApi.reducerPath]: YearsEndApi.reducer,
    // ... other API reducers
  },

  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({ serializableCheck: false })
      .concat(rtkQueryErrorToastMiddleware(true))
      .concat(EnvironmentUtils.isDevelopmentOrQA ? [apiLoggerMiddleware] : [])
      .concat(SecurityApi.middleware)
      .concat(YearsEndApi.middleware),
  // ... other API middleware
});

// Infer types from the store
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
```

### Key Configuration Details

- **Serializable Check Disabled**: Set to `false` due to Date objects and other non-serializable data
- **Conditional Middleware**: API logger only runs in development/QA environments
- **Type Inference**: `RootState` and `AppDispatch` types inferred from the store

### Provider Setup (`main.tsx`)

```typescript
import { Provider } from "react-redux";
import { store } from "reduxstore/store";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <Provider store={store}>
      <App />
    </Provider>
  </React.StrictMode>
);
```

---

## RTK Query API Layer

### Base Query Configuration (`api/api.ts`)

All RTK Query APIs share a common base query configuration:

```typescript
export const url = process.env.VITE_REACT_APP_PS_API as string;

export const createDataSourceAwareBaseQuery = (
  timeout?: number,
): BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError> => {
  const rawBaseQuery = fetchBaseQuery({
    baseUrl: `${url}/api/`,
    mode: "cors",
    timeout: timeout ?? 100000, // Default 100 seconds
    prepareHeaders: (headers, { getState }) => {
      const root = getState() as AppState;
      const token = root.security.token;
      const impersonating = root.security.impersonating;

      if (token) {
        headers.set("authorization", `Bearer ${token}`);
      }

      if (impersonating && impersonating.length > 0) {
        headers.set("impersonation", impersonating.join(" | "));
      }

      return headers;
    },
  });

  return async (args, api, extra) => {
    const result = await rawBaseQuery(args, api, extra);

    // Automatically copy x-demographic-data-source header to response
    if (result.data && typeof result.data === "object") {
      const hdr =
        result.meta?.response?.headers?.get("x-demographic-data-source") ??
        "Live";
      (result.data as Record<string, string>).dataSource = hdr;
    }

    return result;
  };
};
```

#### Base Query Features

1. **Automatic Authorization**: Injects JWT token from Redux state
2. **Impersonation Support**: Adds impersonation headers for role switching
3. **Data Source Tracking**: Copies `x-demographic-data-source` header to response data
4. **Configurable Timeout**: Defaults to 100 seconds, can be overridden (e.g., 120s for year-end reports)
5. **CORS Enabled**: Configured for cross-origin requests

### API Definition Pattern

RTK Query APIs follow a consistent structure:

```typescript
import { createApi } from "@reduxjs/toolkit/query/react";
import { createDataSourceAwareBaseQuery } from "./api";

// Optional: Custom timeout for long-running operations
const baseQuery = createDataSourceAwareBaseQuery(120000); // 2 minutes

export const YearsEndApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "yearsEndApi", // Unique identifier
  endpoints: (builder) => ({
    // Query example
    getDuplicateSSNs: builder.query<ResponseType, RequestType>({
      query: (params) => ({
        url: `yearend/duplicate-ssns`,
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          profitYear: params.profitYear,
        },
      }),
      // Optional: Update slice state when query succeeds
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setDuplicateSSNsData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      },
    }),

    // Mutation example
    updateEnrollment: builder.mutation({
      query: () => ({
        url: `yearend/update-enrollment`,
        method: "POST",
        body: {},
      }),
    }),
  }),
});

// Export generated hooks
export const { useLazyGetDuplicateSSNsQuery, useUpdateEnrollmentMutation } =
  YearsEndApi;
```

### API Endpoint Patterns

#### 1. Standard Query

```typescript
getAccountingYear: builder.query<CalendarResponseDto, ProfitYearRequest>({
  query: (params) => ({
    url: "/lookup/calendar/accounting-year",
    method: "GET",
    params: { profitYear: params.profitYear },
  }),
  async onQueryStarted(arg, { dispatch, queryFulfilled }) {
    try {
      const { data } = await queryFulfilled;
      dispatch(setAccountingYearData(data)); // Update slice
    } catch (err) {
      console.log("Err: " + err);
    }
  },
});
```

#### 2. Mutation with Error Suppression

```typescript
updateForfeitureAdjustment: builder.mutation<ResponseType, RequestType>({
  query: (params) => {
    const { suppressAllToastErrors, onlyNetworkToastErrors, ...requestData } =
      params;
    return {
      url: "yearend/forfeiture-adjustments/update",
      method: "PUT",
      body: requestData,
      // Pass metadata to middleware for error handling control
      meta: { suppressAllToastErrors, onlyNetworkToastErrors },
    };
  },
});
```

#### 3. Query with Archive Support

```typescript
getExecutiveHoursAndDollars: builder.query<
  ResponseType,
  RequestType & { archive?: boolean }
>({
  query: (params) => ({
    url: `yearend/executive-hours-and-dollars${
      params.archive ? "?archive=true" : ""
    }`,
    method: "GET",
    params: {
      profitYear: params.profitYear,
      // ... other params
    },
  }),
});
```

#### 4. Response Transformation

```typescript
getMissives: builder.query<MissiveResponse[], void>({
  query: () => ({ url: "/lookup/missives", method: "GET" }),
  transformResponse: (response: {
    items: MissiveResponse[];
    count: number;
  }) => {
    return response.items; // Extract just the items array
  },
});
```

#### 5. Blob/File Download

```typescript
downloadCertificatesFile: builder.query<Blob, CertificateDownloadRequest>({
  query: (params) => ({
    url: "yearend/post-frozen/certificates/download",
    method: "GET",
    params: {
      profitYear: params.profitYear,
      badgeNumbers: params.badgeNumbers,
    },
    responseHandler: (response) => response.blob(), // Handle as blob
  }),
});
```

#### 6. Conditional CSV/JSON Response

```typescript
getEmployeeWagesForYear: builder.query<
  ResponseType,
  RequestType & { acceptHeader: string }
>({
  query: (params) => ({
    url: "yearend/wages-current-year",
    method: "GET",
    params: { profitYear: params.profitYear },
    headers: { Accept: params.acceptHeader },
    responseHandler: async (response) => {
      if (params.acceptHeader === "text/csv") {
        return response.blob();
      }
      return response.json();
    },
  }),
});
```

### Generated Hooks

RTK Query automatically generates hooks for each endpoint:

- **Queries**:

  - `use[EndpointName]Query` - Immediate fetch on mount
  - `useLazy[EndpointName]Query` - Manual trigger

- **Mutations**:
  - `use[EndpointName]Mutation` - Always manual trigger

Example usage in components:

```typescript
// Lazy query (manual trigger)
const [getDuplicateSSNs, { data, isLoading, error }] =
  useLazyGetDuplicateSSNsQuery();

// Trigger the query
useEffect(() => {
  getDuplicateSSNs({ profitYear: 2024, pagination: { skip: 0, take: 50 } });
}, []);

// Mutation
const [updateEnrollment, { isLoading: isUpdating }] =
  useUpdateEnrollmentMutation();

// Trigger mutation
const handleUpdate = async () => {
  await updateEnrollment().unwrap();
};
```

---

## New API Setup Checklist

When creating a new RTK Query API, follow this checklist to avoid common mistakes:

### 1. Create API File

**File**: `src/ui/src/reduxstore/api/myFeatureApi.ts`

```typescript
import { createApi } from "@reduxjs/toolkit/query/react";
import { MyResponse, MyRequest } from "../../pages/MyFeature/types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const myFeatureApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "myFeatureApi", // Must be unique
  tagTypes: ["my-feature-data"], // For cache invalidation
  endpoints: (builder) => ({
    getMyData: builder.query<MyResponse, MyRequest>({
      query: ({
        pageNumber = 1,
        pageSize = 10,
        sortBy = "Created",
        isSortDescending = true,
      }) => ({
        url: "my-endpoint",
        method: "GET",
        params: { pageNumber, pageSize, sortBy, isSortDescending },
      }),
      providesTags: ["my-feature-data"],
    }),
  }),
});

// CRITICAL: Export hooks
export const { useGetMyDataQuery, useLazyGetMyDataQuery } = myFeatureApi;
```

### 2. Register in Store

**File**: `src/ui/src/reduxstore/store.ts`

```typescript
import { myFeatureApi } from "./api/myFeatureApi";

export const store = configureStore({
  reducer: {
    // ... existing reducers
    [myFeatureApi.reducerPath]: myFeatureApi.reducer, // Add reducer
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({ serializableCheck: false })
      .concat(rtkQueryErrorToastMiddleware(true))
      .concat(myFeatureApi.middleware), // Add middleware
});
```

### 3. Export from reduxstore/types.ts (Optional)

```typescript
export * from "./api/myFeatureApi";
```

### 4. Pagination/Sorting Query Parameters

For paginated/sorted endpoints, ensure API accepts these parameters:

```typescript
interface PaginatedSortedRequest {
  pageNumber?: number; // API expects 1-based
  pageSize?: number; // Default 10 or 25
  sortBy?: string; // Column name
  isSortDescending?: boolean; // Sort direction
}

getData: builder.query<ResponseType, PaginatedSortedRequest>({
  query: ({
    pageNumber = 1,
    pageSize = 10,
    sortBy = "Created",
    isSortDescending = true,
  }) => ({
    url: "my-endpoint",
    method: "GET",
    params: { pageNumber, pageSize, sortBy, isSortDescending }, // All four params
  }),
});
```

### Common RTK Query Mistakes

1. **Missing API registration in store**: Middleware not added → API calls fail silently
2. **Missing hook exports**: `useGetMyDataQuery` not exported → component can't import
3. **Wrong `reducerPath`**: Must match key in store reducer object
4. **Missing sort/pagination params**: API definition doesn't accept `sortBy`/`isSortDescending`
5. **Not using `createDataSourceAwareBaseQuery()`**: Won't handle auth/headers correctly
6. **Forgetting tag types**: Cache invalidation won't work on mutations

---

## Redux Slices

Redux slices manage **local UI state** that doesn't come from the server or needs client-side transformations.

### Slice Structure

```typescript
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface SecurityState {
  token: string | null;
  userGroups: string[];
  userRoles: string[];
  userPermissions: string[];
  username: string;
  performLogout: boolean;
  appUser: AppUser | null;
  impersonating: ImpersonationRoles[];
}

const initialState: SecurityState = {
  token: null,
  userRoles: [],
  userPermissions: [],
  userGroups: [],
  username: "",
  performLogout: false,
  appUser: null,
  impersonating: [],
};

export const securitySlice = createSlice({
  name: "security",
  initialState,
  reducers: {
    setUserRoles: (state, action: PayloadAction<string[]>) => {
      state.userRoles = action.payload;
    },
    setToken: (state, action: PayloadAction<string>) => {
      state.token = action.payload;
    },
    clearUserData: (state) => {
      state.token = "";
      state.appUser = null;
      state.userGroups = [];
      state.username = "";
      state.userRoles = [];
      state.userPermissions = [];
      state.performLogout = false;
      state.impersonating = [];
    },
  },
});

export const { setToken, setUserRoles, clearUserData } = securitySlice.actions;
export default securitySlice.reducer;
```

### Common Slice Patterns

#### 1. Simple State Update

```typescript
setBanner: (state, action: PayloadAction<string>) => {
  state.appBanner = action.payload;
};
```

#### 2. LocalStorage Persistence

```typescript
openDrawer: (state) => {
  state.isDrawerOpen = true;
  try {
    localStorage.setItem(
      "drawerState",
      JSON.stringify({
        isDrawerOpen: true,
        activeSubmenu: state.activeSubmenu,
      }),
    );
  } catch (error) {
    console.error("Error saving drawer state to localStorage:", error);
  }
};
```

#### 3. Complex State Reset

```typescript
clearUserData: (state) => {
  state.token = "";
  state.appUser = null;
  state.userGroups = [];
  state.username = "";
  state.userRoles = [];
  state.userPermissions = [];
  state.performLogout = false;
  state.impersonating = [];
};
```

#### 4. Dictionary/Map Pattern

```typescript
export interface MessagesState {
  [key: string]: ApiMessage;
}

const messagesSlice = createSlice({
  name: "messages",
  initialState: {} as MessagesState,
  reducers: {
    setMessage: (state, action: PayloadAction<MessageUpdate>) => {
      state[action.payload.key] = action.payload.message;
    },
    removeMessage: (state, action: PayloadAction<string>) => {
      delete state[action.payload];
    },
    clearMessages: () => ({}),
  },
});
```

### Using Slices in Components

```typescript
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { setToken, clearUserData } from "reduxstore/slices/securitySlice";

function MyComponent() {
  const dispatch = useDispatch();
  const token = useSelector((state: RootState) => state.security.token);
  const username = useSelector((state: RootState) => state.security.username);

  const handleLogin = (newToken: string) => {
    dispatch(setToken(newToken));
  };

  const handleLogout = () => {
    dispatch(clearUserData());
  };

  return (/* ... */);
}
```

---

## Middleware

### 1. RTK Query Error Toast Middleware

Automatically displays toast notifications for API errors.

**Location**: `src/ui/src/redux/rtkQueryErrorToastMiddleware.ts`

**Features**:

- Intercepts rejected RTK Query actions
- Displays appropriate error messages via toast service
- Supports error suppression via metadata flags
- Handles validation errors (400), auth errors (401/403), and network errors

**Error Suppression Options**:

```typescript
// Suppress all toast errors for this request
const [getData] = useLazyGetDataQuery();
getData({ id: 123, suppressAllToastErrors: true });

// Only show network errors (suppress validation/business errors)
const [updateData] = useUpdateDataMutation();
updateData({ data: payload, onlyNetworkToastErrors: true });
```

**Metadata Handling**:

```typescript
query: (params) => {
  const { suppressAllToastErrors, onlyNetworkToastErrors, ...requestData } =
    params;
  return {
    url: "endpoint",
    method: "POST",
    body: requestData,
    meta: { suppressAllToastErrors, onlyNetworkToastErrors },
  };
};
```

### 2. API Logger Middleware

Logs API requests and responses in development/QA environments.

**Location**: `src/ui/src/middleware/apiLoggerMiddleware.ts`

**Configuration**:

```typescript
.concat(EnvironmentUtils.isDevelopmentOrQA ? [apiLoggerMiddleware] : [])
```

Only active in development and QA environments; disabled in production.

---

## Common Patterns

### 1. Pagination

```typescript
export interface PaginationParams {
  skip: number;
  take: number;
  sortBy?: string;
  isSortDescending?: boolean;
}

// In API endpoint
query: (params) => ({
  url: "endpoint",
  params: {
    take: params.pagination.take,
    skip: params.pagination.skip,
    sortBy: params.pagination.sortBy,
    isSortDescending: params.pagination.isSortDescending,
  },
});
```

### 2. Updating Slice State from API

When an API query succeeds, update related slice state:

```typescript
async onQueryStarted(arg, { dispatch, queryFulfilled }) {
  try {
    const { data } = await queryFulfilled;
    dispatch(setAccountingYearData(data));  // Update slice
  } catch (err) {
    console.log("Err: " + err);
  }
}
```

### 3. Clearing State on Error

```typescript
async onQueryStarted(arg, { dispatch, queryFulfilled }) {
  try {
    dispatch(clearProfitSharingUpdate());  // Clear first
    const { data } = await queryFulfilled;
    dispatch(setProfitSharingUpdate(data));
  } catch (err) {
    console.log("Err: " + err);
    dispatch(clearProfitSharingUpdate());  // Clear on error
  }
}
```

### 4. Conditional Data Storage

Store data in different slice locations based on parameters:

```typescript
async onQueryStarted(arg, { dispatch, queryFulfilled }) {
  try {
    const { data } = await queryFulfilled;

    if (arg.storeManagement) {
      dispatch(setBreakdownByStoreManagement(data));
    } else {
      dispatch(setBreakdownByStore(data));
    }
  } catch (err) {
    console.log("Err: " + err);
  }
}
```

### 5. Input Validation in Query

```typescript
query: (arg) => {
  // Validate profit year range
  if (arg.profitYear < 2020 || arg.profitYear > 2100) {
    console.error("Invalid profit year: Must be between 2020 and 2100");
    return { url: "invalid-request", method: "GET" };
  }

  return {
    url: `yearend/frozen/contributions-by-age`,
    params: { profitYear: arg.profitYear },
  };
};
```

---

## Naming Conventions

### Files

- **API Files**: `{Domain}Api.ts` (e.g., `SecurityApi.ts`, `YearsEndApi.ts`)
- **Slice Files**: `{domain}Slice.ts` (e.g., `securitySlice.ts`, `generalSlice.ts`)
- **Middleware**: `{purpose}Middleware.ts`

### Types

- **State Interfaces**: `{Domain}State` (e.g., `SecurityState`, `GeneralState`)
- **Request DTOs**: `{Operation}Request` or `{Operation}RequestDto`
- **Response DTOs**: `{Operation}Response` or `{Operation}ResponseDto`

### Reducers and Actions

- **Slice Name**: `camelCase` matching the domain (e.g., `"security"`, `"yearsEnd"`)
- **Reducer Path**: `"{domain}Api"` (e.g., `"securityApi"`, `"yearsEndApi"`)
- **Action Creators**: `set{Property}`, `clear{Property}`, `update{Property}`
- **Endpoint Names**: `get{Resource}`, `update{Resource}`, `delete{Resource}`

### Hooks

- **Query Hooks**: `useLazy{EndpointName}Query` or `use{EndpointName}Query`
- **Mutation Hooks**: `use{EndpointName}Mutation`

---

## Best Practices

### 1. Type Safety

Always provide full type annotations for queries and mutations:

```typescript
builder.query<ResponseType, RequestType>({...})
builder.mutation<ResponseType, RequestType>({...})
```

### 2. Error Handling

- Log errors in `onQueryStarted` for debugging
- Clear related state on errors
- Use error suppression flags when appropriate
- Let middleware handle toast notifications

### 3. State Management

- **Use RTK Query for server state**: Fetching, caching, and synchronizing with server
- **Use Redux slices for UI state**: Local UI state, derived data, client-only data
- Avoid duplicating server data in slices when RTK Query cache is sufficient

### 4. Performance

- Use `useLazy*Query` hooks when you need manual control over fetching
- Leverage RTK Query's automatic caching and deduplication
- Consider custom timeout values for long-running operations
- Use pagination for large data sets

### 5. Middleware Order

Middleware order matters:

```typescript
getDefaultMiddleware({ serializableCheck: false })
  .concat(rtkQueryErrorToastMiddleware(true)) // First: error handling
  .concat(conditionalMiddleware) // Then: logging/debugging
  .concat(SecurityApi.middleware) // Finally: API middleware
  .concat(YearsEndApi.middleware);
```

### 6. LocalStorage Integration

When persisting state to localStorage:

- Wrap in try/catch blocks
- Handle parse errors gracefully
- Provide sensible defaults
- Clear stale data on logout

### 7. Archive/Historical Data

Use optional `archive` parameter for endpoints that support historical data:

```typescript
const [getData] = useLazyGetDataQuery();
getData({ profitYear: 2023, archive: true });
```

---

## Examples

### Complete Component Example

```typescript
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { useLazyGetDuplicateSSNsQuery } from "reduxstore/api/YearsEndApi";
import { setLoading } from "reduxstore/slices/generalSlice";

export function DuplicateSSNsReport() {
  const dispatch = useDispatch();
  const profitYear = useSelector(
    (state: RootState) => state.yearsEnd.selectedYear
  );
  const duplicateSSNs = useSelector(
    (state: RootState) => state.yearsEnd.duplicateSSNsData
  );

  const [getDuplicateSSNs, { isLoading, error }] =
    useLazyGetDuplicateSSNsQuery();

  useEffect(() => {
    dispatch(setLoading(true));
    getDuplicateSSNs({
      profitYear,
      pagination: { skip: 0, take: 50 },
    }).finally(() => {
      dispatch(setLoading(false));
    });
  }, [profitYear]);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading data</div>;

  return (
    <div>
      {duplicateSSNs?.items.map((item) => (
        <div key={item.id}>{item.name}</div>
      ))}
    </div>
  );
}
```

### Complete API Definition Example

```typescript
import { createApi } from "@reduxjs/toolkit/query/react";
import { setLookupData } from "reduxstore/slices/lookupsSlice";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const LookupsApi = createApi({
  baseQuery,
  reducerPath: "lookupsApi",
  endpoints: (builder) => ({
    getStates: builder.query<StateListResponse[], void>({
      query: () => ({ url: "/lookup/states", method: "GET" }),
      transformResponse: (response: { items: StateListResponse[] }) =>
        response.items,
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setLookupData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      },
    }),

    updateStateTax: builder.mutation<void, StateTaxUpdateRequest>({
      query: (params) => ({
        url: `/lookup/state-taxes/${params.state}`,
        method: "PUT",
        body: params,
      }),
    }),
  }),
});

export const { useGetStatesQuery, useUpdateStateTaxMutation } = LookupsApi;
```

---
