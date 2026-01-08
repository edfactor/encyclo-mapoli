# Frontend Redux State Management Guide

This guide describes how Redux state management works in the Smart Profit Sharing frontend application, including slices and RTK Query API integration.

## Architecture Overview

The application uses **Redux Toolkit (RTK)** for state management with the following structure:

```typescript
src/reduxstore/
 api/                    # RTK Query API definitions
 api.ts             # Base query configuration
 SecurityApi.ts     # Authentication/authorization endpoints
 LookupsApi.ts      # Lookup data endpoints
 YearsEndApi.ts     # Year-end processing endpoints
 BeneficiariesApi.ts # Beneficiary management
...                # Other domain-specific APIs
 slices/                # Redux slices for client state
   securitySlice.ts   # Authentication state
   yearsEndSlice.ts   # Year-end data caching
   lookupsSlice.ts    # Lookup data cache
   ...                # Other state slices
 store.ts               # Root store configuration
 types.ts               # Shared TypeScript types
 healthTypes.ts         # Health check types
```

## Core Concepts

### 1. Store Configuration (`store.ts`)

The Redux store combines:

- **Slices**: Client-side state management
- **RTK Query APIs**: Server data fetching and caching
- **Middleware**: Error handling, logging, API integration

```typescript
export const store = configureStore({
  reducer: {
    // Client state slices
    general: generalSlice,
    security: securitySlice,
    yearsEnd: yearsEndSlice,
    lookups: lookupsSlice,
    // ... other slices

    // RTK Query API reducers
    [SecurityApi.reducerPath]: SecurityApi.reducer,
    [YearsEndApi.reducerPath]: YearsEndApi.reducer,
    [LookupsApi.reducerPath]: LookupsApi.reducer
    // ... other APIs
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({ serializableCheck: false })
      .concat(rtkQueryErrorToastMiddleware(true))
      .concat(EnvironmentUtils.isDevelopmentOrQA ? [apiLoggerMiddleware] : [])
      .concat(SecurityApi.middleware)
      .concat(YearsEndApi.middleware)
  // ... other API middleware
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
```

## Redux Slices

### Adding to an Existing Slice

**Step 1: Add state property to interface**

```typescript
export interface YearsEndState {
  // Existing properties...
  profitMasterStatus: ProfitMasterStatus | null;

  // Add new property
  myNewData: MyNewDataType | null;
  myNewQueryParams: MyNewQueryParams | null;
}
```

**Step 2: Add to initialState**

```typescript
const initialState: YearsEndState = {
  // Existing initial values...
  profitMasterStatus: null,

  // Initialize new properties
  myNewData: null,
  myNewQueryParams: null
};
```

**Step 3: Add reducers**

```typescript
export const yearsEndSlice = createSlice({
  name: "yearsEnd",
  initialState,
  reducers: {
    // Existing reducers...

    // Setter
    setMyNewData: (state, action: PayloadAction<MyNewDataType>) => {
      state.myNewData = action.payload;
    },

    // Clearer
    clearMyNewData: (state) => {
      state.myNewData = null;
    },

    // Query params setter
    setMyNewQueryParams: (state, action: PayloadAction<MyNewQueryParams>) => {
      state.myNewQueryParams = action.payload;
    },

    // Complex update
    updateMyNewDataField: (state, action: PayloadAction<Partial<MyNewDataType>>) => {
      if (state.myNewData) {
        state.myNewData = {
          ...state.myNewData,
          ...action.payload
        };
      }
    }
  }
});
```

**Step 4: Export new actions**

```typescript
export const {
  // Existing exports...
  setMyNewData,
  clearMyNewData,
  setMyNewQueryParams,
  updateMyNewDataField
} = yearsEndSlice.actions;
```

### Common Slice Patterns

**Conditional state updates:**

```typescript
setExecutiveRowsSelected: (state, action: PayloadAction<ExecutiveHoursAndDollars[]>) => {
  if (action.payload.reportType === ReportType.Total) {
    state.executiveRowsTotal = action.payload;
  } else if (action.payload.reportType === ReportType.FullTime) {
    state.executiveRowsFullTime = action.payload;
  }
};
```

**Array manipulation:**

```typescript
// Add items to array
setAdditionalExecutives: (state, action: PayloadAction<Executive[]>) => {
  if (state.additionalExecutives === null) {
    state.additionalExecutives = action.payload;
  } else {
    state.additionalExecutives.push(...action.payload);
  }
},

// Remove item from array
removeExecutive: (state, action: PayloadAction<number>) => {
  if (state.executiveList) {
    state.executiveList = state.executiveList.filter(
      exec => exec.badgeNumber !== action.payload
    );
  }
}
```

**LocalStorage integration:**

```typescript
setSelectedProfitYear: (state, action: PayloadAction<number>) => {
  state.selectedProfitYear = action.payload;
  localStorage.setItem("selectedProfitYear", action.payload.toString());
},

// Initialize from localStorage
const initialState: YearsEndState = {
  selectedProfitYear: localStorage.getItem("selectedProfitYear")
    ? Number(localStorage.getItem("selectedProfitYear"))
    : 2024
};
```

## RTK Query APIs

### Base Query Configuration

All APIs use a shared base query configuration (`api.ts`):

```typescript
export const url = process.env.VITE_REACT_APP_PS_API as string;

export const createDataSourceAwareBaseQuery = () => {
  const rawBaseQuery = fetchBaseQuery({
    baseUrl: `${url}/api/`,
    mode: "cors",
    prepareHeaders: (headers, { getState }) => {
      const root = getState() as RootState;
      const token = root.security.token;
      const impersonating = root.security.impersonating;

      if (token) {
        headers.set("authorization", `Bearer ${token}`);
      }

      if (impersonating && impersonating.length > 0) {
        headers.set("impersonation", impersonating.join(" | "));
      }

      return headers;
    }
  });

  // Custom wrapper to extract x-demographic-data-source header
  return async (args, api, extra) => {
    const result = await rawBaseQuery(args, api, extra);
    if (result.data && typeof result.data === "object") {
      const hdr = result.meta?.response?.headers?.get("x-demographic-data-source") ?? "Live";
      (result.data as Record<string, unknown>).dataSource = hdr;
    }
    return result;
  };
};
```

### API Patterns

There are two main base query patterns used in the application:

#### Pattern 1: Data-Source-Aware Base Query (Recommended)

Most APIs use `createDataSourceAwareBaseQuery()` which:

- Automatically adds authorization headers
- Extracts `x-demographic-data-source` header from responses
- Handles impersonation headers

```typescript
import { createApi } from "@reduxjs/toolkit/query/react";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const LookupsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "lookupsApi",
  endpoints: (builder) => ({
    // ... endpoints
  })
});
```

#### Pattern 2: Standard Fetch Base Query

Some APIs (like SecurityApi) use the standard `fetchBaseQuery`:

```typescript
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { prepareHeaders, url } from "./api";

export const SecurityApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/security/`,
    mode: "cors",
    credentials: "include",
    prepareHeaders
  }),
  reducerPath: "securityApi",
  endpoints: (builder) => ({
    // ... endpoints
  })
});
```

### Query Endpoints (GET requests)

Queries fetch data from the server:

```typescript
export const LookupsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "lookupsApi",
  endpoints: (builder) => ({
    // Simple query with parameters
    getAccountingYear: builder.query<CalendarResponseDto, ProfitYearRequest>({
      query: (params) => ({
        url: "/lookup/calendar/accounting-year",
        method: "GET",
        params: {
          profitYear: params.profitYear
        }
      }),

      // Optional: Dispatch to slice when data is fetched
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setAccountingYearData(data));
        } catch (err) {
          console.error("Error:", err);
        }
      }
    }),

    // Query without parameters
    getMissives: builder.query<MissiveResponse[], void>({
      query: () => ({
        url: "/lookup/missives",
        method: "GET"
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMissivesData(data));
        } catch (err) {
          console.error("Error:", err);
        }
      }
    })
  })
});

// Export hooks
export const { useLazyGetAccountingYearQuery, useLazyGetMissivesQuery } = LookupsApi;
```

### Mutation Endpoints (POST, PUT, DELETE)

Mutations modify server data:

```typescript
export const MilitaryApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "militaryApi",
  endpoints: (builder) => ({
    // POST mutation
    createMilitaryContribution: builder.mutation<
      MasterInquiryDetail,
      CreateMilitaryContributionRequest & {
        suppressAllToastErrors?: boolean;
        onlyNetworkToastErrors?: boolean;
      }
    >({
      query: (request) => {
        const { suppressAllToastErrors, onlyNetworkToastErrors } = request;
        return {
          url: "military",
          method: "POST",
          body: request,
          meta: { suppressAllToastErrors, onlyNetworkToastErrors }
        };
      }
    }),

    // PUT mutation
    updateExecutiveHours: builder.mutation({
      query: (request) => ({
        url: "/executive/hours",
        method: "PUT",
        body: request
      })
    }),

    // DELETE mutation
    deleteBeneficiary: builder.mutation<{ success: boolean; message?: string }, DeleteBeneficiaryRequest>({
      query: (request) => ({
        url: `/beneficiaries/${request.id}`,
        method: "DELETE"
      })
    })
  })
});

// Export mutation hooks
export const { useCreateMilitaryContributionMutation, useUpdateExecutiveHoursMutation, useDeleteBeneficiaryMutation } =
  MilitaryApi;
```

### Creating a New API File

**Step 1: Create API file**

Create `src/reduxstore/api/MyNewApi.ts`:

```typescript
import { createApi } from "@reduxjs/toolkit/query/react";
import { createDataSourceAwareBaseQuery } from "./api";
import { setMyData } from "reduxstore/slices/mySlice";
import { MyRequest, MyResponse } from "reduxstore/types";

const baseQuery = createDataSourceAwareBaseQuery();

export const MyNewApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "myNewApi",
  endpoints: (builder) => ({
    getMyData: builder.query<MyResponse, MyRequest>({
      query: (params) => ({
        url: "/my-endpoint",
        method: "GET",
        params
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMyData(data));
        } catch (err) {
          console.error("Failed to fetch data:", err);
        }
      }
    }),

    createMyData: builder.mutation<MyResponse, MyRequest>({
      query: (request) => ({
        url: "/my-endpoint",
        method: "POST",
        body: request
      })
    })
  })
});

export const { useLazyGetMyDataQuery, useCreateMyDataMutation } = MyNewApi;
```

**Step 2: Register API in store**

Add to `store.ts`:

```typescript
import { MyNewApi } from "./api/MyNewApi";

export const store = configureStore({
  reducer: {
    // ... existing reducers
    [MyNewApi.reducerPath]: MyNewApi.reducer
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({ serializableCheck: false })
      // ... existing middleware
      .concat(MyNewApi.middleware)
});
```

## Using Redux in Components

### Accessing State (useSelector)

```typescript
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

function MyComponent() {
  // Access slice state
  const token = useSelector((state: RootState) => state.security.token);
  const profitYear = useSelector((state: RootState) =>
    state.yearsEnd.selectedProfitYearForDecemberActivities
  );

  // Access complex state
  const executiveData = useSelector((state: RootState) =>
    state.yearsEnd.executiveHoursAndDollars
  );

  return <div>{/* Use state */}</div>;
}
```

### Dispatching Actions (useDispatch)

```typescript
import { useDispatch } from "react-redux";
import { setToken, clearUserData } from "reduxstore/slices/securitySlice";
import { AppDispatch } from "reduxstore/store";

function MyComponent() {
  const dispatch = useDispatch<AppDispatch>();

  const handleLogin = (token: string) => {
    dispatch(setToken(token));
  };

  const handleLogout = () => {
    dispatch(clearUserData());
  };

  return <div>{/* Component JSX */}</div>;
}
```

### Using Query Hooks

```typescript
import { useLazyGetAccountingYearQuery } from "reduxstore/api/LookupsApi";

function MyComponent() {
  const [trigger, { data, isLoading, error }] = useLazyGetAccountingYearQuery();

  const handleFetch = () => {
    trigger({ profitYear: 2024 });
  };

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading data</div>;

  return (
    <div>
      <button onClick={handleFetch}>Fetch Data</button>
      {data && <div>{JSON.stringify(data)}</div>}
    </div>
  );
}
```

### Using Mutation Hooks

```typescript
import { useCreateMilitaryContributionMutation } from "reduxstore/api/MilitaryApi";

function MyComponent() {
  const [createContribution, { isLoading, error }] = useCreateMilitaryContributionMutation();

  const handleSubmit = async (formData: CreateMilitaryContributionRequest) => {
    try {
      const result = await createContribution(formData).unwrap();
      console.log("Success:", result);
    } catch (err) {
      console.error("Failed:", err);
    }
  };

  return (
    <form onSubmit={(e) => {
      e.preventDefault();
      handleSubmit(/* form data */);
    }}>
      <button type="submit" disabled={isLoading}>
        {isLoading ? "Creating..." : "Create"}
      </button>
      {error && <div>Error: {error.message}</div>}
    </form>
  );
}
```

### Combined Pattern (Query + Slice)

Common pattern: Fetch data via API, cache in slice, read from slice:

```typescript
import { useSelector, useDispatch } from "react-redux";
import { useLazyGetAccountingYearQuery } from "reduxstore/api/LookupsApi";
import { RootState } from "reduxstore/store";

function MyComponent() {
  const dispatch = useDispatch();

  // API hook
  const [fetchYear] = useLazyGetAccountingYearQuery();

  // Read from slice (cached data)
  const accountingYear = useSelector((state: RootState) =>
    state.lookups.accountingYearData
  );

  useEffect(() => {
    // Fetch if not cached
    if (!accountingYear) {
      fetchYear({ profitYear: 2024 });
      // onQueryStarted will dispatch to slice automatically
    }
  }, [accountingYear, fetchYear]);

  return <div>{/* Use accountingYear */}</div>;
}
```

## Advanced Patterns

### Extra Reducers (Responding to API Fulfillment)

Listen to API endpoint fulfillment in slices:

```typescript
export const yearsEndSlice = createSlice({
  name: "yearsEnd",
  initialState,
  reducers: {
    // ... regular reducers
  },
  extraReducers: (builder) => {
    // Respond to API endpoint completion
    builder.addMatcher(YearsEndApi.endpoints.getBreakdownByStore.matchFulfilled, (state, action) => {
      if (action.meta.arg.originalArgs.storeManagement) {
        state.storeManagementBreakdown = action.payload;
      } else {
        state.breakdownByStore = action.payload;
      }
    });
  }
});
```

### Error Handling with Meta

Suppress toast notifications for specific API calls:

```typescript
createMilitaryContribution: builder.mutation<
  Response,
  Request & {
    suppressAllToastErrors?: boolean;
    onlyNetworkToastErrors?: boolean;
  }
>({
  query: (request) => {
    const { suppressAllToastErrors, onlyNetworkToastErrors } = request;
    return {
      url: "military",
      method: "POST",
      body: request,
      meta: { suppressAllToastErrors, onlyNetworkToastErrors }
    };
  }
});
```

Usage:

```typescript
const [create] = useCreateMilitaryContributionMutation();

await create({
  // Request data
  badgeNumber: 12345,
  profitYear: 2024,
  // Error handling flags
  suppressAllToastErrors: true,
  onlyNetworkToastErrors: false
});
```

### Conditional Data Updates

Update different state properties based on response data:

```typescript
setDistributionsByAge: (state, action: PayloadAction<DistributionsByAge>) => {
  if (action.payload.reportType === ReportType.Total) {
    state.distributionsByAgeTotal = action.payload;
  }
  if (action.payload.reportType === ReportType.FullTime) {
    state.distributionsByAgeFullTime = action.payload;
  }
  if (action.payload.reportType === ReportType.PartTime) {
    state.distributionsByAgePartTime = action.payload;
  }
};
```

## Best Practices

### 2. Naming Conventions

**Slices:**

- State interface: `[Name]State` (e.g., `SecurityState`)
- Slice variable: `[name]Slice` (e.g., `securitySlice`)
- Actions: Verb + noun (e.g., `setToken`, `clearUserData`)

**APIs:**

- API variable: `[Name]Api` (e.g., `LookupsApi`)
- Reducer path: `[name]Api` (e.g., `"lookupsApi"`)
- Endpoints: Verb + noun (e.g., `getAccountingYear`, `createMilitaryContribution`)
- Hooks: `useLazy[Endpoint]Query` or `use[Endpoint]Mutation`

### 7. Error Handling

Handle errors in `onQueryStarted`:

```typescript
async onQueryStarted(arg, { dispatch, queryFulfilled }) {
  try {
    const { data } = await queryFulfilled;
    dispatch(setData(data));
  } catch (err) {
    console.error("Failed to fetch:", err);
    dispatch(setError("Failed to load data"));
  }
}
```

## Middleware

### API Logger Middleware

Logs API requests/responses in development/QA:

```typescript
.concat(EnvironmentUtils.isDevelopmentOrQA ? [apiLoggerMiddleware] : [])
```

### RTK Query Error Toast Middleware

Automatically shows toast notifications for API errors:

```typescript
.concat(rtkQueryErrorToastMiddleware(true))
```

## Quick Reference

### Import Statements

```typescript
// Slice
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

// API
import { createApi } from "@reduxjs/toolkit/query/react";
import { createDataSourceAwareBaseQuery } from "./api";

// Component usage
import { useSelector, useDispatch } from "react-redux";
import { RootState, AppDispatch } from "reduxstore/store";
```

### File Checklist

When adding new Redux functionality:

- [ ] Define TypeScript types in appropriate file
- [ ] Create/update slice in `slices/` directory
- [ ] Create/update API in `api/` directory
- [ ] Register reducer in `store.ts`
- [ ] Register middleware in `store.ts`
- [ ] Export actions from slice
- [ ] Export hooks from API
- [ ] Use in components via hooks
