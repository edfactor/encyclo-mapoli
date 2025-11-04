/**
 * Mock Redux Store Factory
 *
 * Creates properly-configured Redux stores for testing with all required slices
 * and sensible defaults. This ensures tests have access to all necessary state
 * and don't fail due to missing reducers or undefined state properties.
 *
 * Usage:
 *   const store = createMockStore();
 *   const wrapper = ({ children }) => <Provider store={store}>{children}</Provider>;
 *   render(<Component />, { wrapper });
 *
 * With custom preloaded state:
 *   const store = createMockStore({
 *     yearsEnd: { selectedProfitYear: 2024 }
 *   });
 */

import { configureStore, PreloadedState } from "@reduxjs/toolkit";
import React, { PropsWithChildren, ReactNode } from "react";
import { Provider } from "react-redux";
import { AccountHistoryReportApi } from "../../reduxstore/api/AccountHistoryReportApi";
import { AdjustmentsApi } from "../../reduxstore/api/AdjustmentsApi";
import { AppSupportApi } from "../../reduxstore/api/AppSupportApi";
import { BeneficiariesApi } from "../../reduxstore/api/BeneficiariesApi";
import { CommonApi } from "../../reduxstore/api/CommonApi";
import { DistributionApi } from "../../reduxstore/api/DistributionApi";
import { InquiryApi } from "../../reduxstore/api/InquiryApi";
import { ItOperationsApi } from "../../reduxstore/api/ItOperationsApi";
import { LookupsApi } from "../../reduxstore/api/LookupsApi";
import { MilitaryApi } from "../../reduxstore/api/MilitaryApi";
import { NavigationApi } from "../../reduxstore/api/NavigationApi";
import { NavigationStatusApi } from "../../reduxstore/api/NavigationStatusApi";
import { PayServicesApi } from "../../reduxstore/api/PayServicesApi";
import { SecurityApi } from "../../reduxstore/api/SecurityApi";
import { validationApi } from "../../reduxstore/api/ValidationApi";
import { YearsEndApi } from "../../reduxstore/api/YearsEndApi";

/**
 * Mock store for testing with RTK Query support
 *
 * Includes all RTK Query API middleware to prevent middleware warnings
 * in tests that use RTK Query hooks.
 *
 * For test isolation, we use simple reducers that return initial state.
 */

interface MockRootState {
  security?: {
    token?: string | null;
    user?: { id: string; name: string } | null;
  };
  navigation?: {
    navigationData?: {
      navigation?: Array<{
        id?: number;
        statusId?: number;
        isReadOnly?: boolean;
      }>;
    };
    profitYearSelectorData?: {
      profitYears?: Array<{
        profitYear: number;
        fiscalBeginDate: string;
        fiscalEndDate: string;
        isCurrent?: boolean;
      }>;
      currentProfitYear?: number;
    };
  };
  frozen?: {
    profitYearSelectorData?: Array<{
      profitYear: number;
      isFrozen?: boolean;
    }> | null;
    [key: string]: unknown;
  };
  yearsEnd?: {
    selectedProfitYear?: number | null;
    selectedProfitYearForDecemberActivities?: number | null;
    profitYearSelectorData?: unknown[];
    yearsEndData?: unknown | null;
    [key: string]: unknown;
  };
  distribution?: {
    [key: string]: unknown;
  };
  inquiry?: {
    masterInquiryMemberDetails?: unknown;
    [key: string]: unknown;
  };
  lookups?: {
    missives?: unknown[];
    [key: string]: unknown;
  };
  [key: string]: unknown;
}

/**
 * Creates a Redux store for testing with all required reducers
 *
 * @param preloadedState - Custom initial state (merged with defaults)
 * @returns Configured Redux store with mock reducers
 *
 * @example
 * // Basic usage
 * const store = createMockStore();
 *
 * // Custom state
 * const store = createMockStore({
 *   yearsEnd: { selectedProfitYear: 2024 }
 * });
 */
export const createMockStore = (preloadedState?: PreloadedState<MockRootState>) => {
  // Default state for each slice
  const defaultState: MockRootState = {
    security: {
      token: "mock-token",
      user: null
    },
    navigation: {
      navigationData: {
        navigation: []
      },
      profitYearSelectorData: {
        profitYears: [
          {
            profitYear: 2024,
            fiscalBeginDate: "2024-01-01",
            fiscalEndDate: "2024-12-31",
            isCurrent: true
          }
        ],
        currentProfitYear: 2024
      }
    },
    frozen: {
      profitYearSelectorData: null
    },
    yearsEnd: {
      selectedProfitYear: 2024,
      selectedProfitYearForDecemberActivities: 2024,
      profitYearSelectorData: [],
      yearsEndData: null
    },
    distribution: {},
    inquiry: {
      masterInquiryMemberDetails: null
    },
    lookups: {
      missives: []
    }
  };

  // Create simple reducers that just return state
  // In real code, these would be actual slice reducers
  const createSliceReducer =
    (initialState: unknown) =>
    (state = initialState, _action: unknown) =>
      state;

  // Deep merge preloaded state with defaults to preserve nested properties
  const mergedState: MockRootState = {
    security: { ...defaultState.security, ...preloadedState?.security },
    navigation: {
      ...defaultState.navigation,
      ...preloadedState?.navigation,
      profitYearSelectorData: {
        ...defaultState.navigation.profitYearSelectorData,
        ...preloadedState?.navigation?.profitYearSelectorData
      }
    },
    frozen: { ...defaultState.frozen, ...preloadedState?.frozen },
    yearsEnd: { ...defaultState.yearsEnd, ...preloadedState?.yearsEnd },
    distribution: { ...defaultState.distribution, ...preloadedState?.distribution },
    inquiry: { ...defaultState.inquiry, ...preloadedState?.inquiry },
    lookups: { ...defaultState.lookups, ...preloadedState?.lookups }
  };

  const baseReducers = {
    security: createSliceReducer(mergedState.security),
    navigation: createSliceReducer(mergedState.navigation),
    frozen: createSliceReducer(mergedState.frozen),
    yearsEnd: createSliceReducer(mergedState.yearsEnd),
    distribution: createSliceReducer(mergedState.distribution),
    inquiry: createSliceReducer(mergedState.inquiry),
    lookups: createSliceReducer(mergedState.lookups)
  };

  // Add RTK Query API reducers that tests commonly mock
  // When tests mock these APIs with vi.mock(), these fallback reducers prevent errors
  const reducers: { [key: string]: unknown } = baseReducers;

  // Add RTK Query API reducers - must match real store configuration
  reducers[SecurityApi.reducerPath] = SecurityApi.reducer;
  reducers[YearsEndApi.reducerPath] = YearsEndApi.reducer;
  reducers[ItOperationsApi.reducerPath] = ItOperationsApi.reducer;
  reducers[MilitaryApi.reducerPath] = MilitaryApi.reducer;
  reducers[InquiryApi.reducerPath] = InquiryApi.reducer;
  reducers[LookupsApi.reducerPath] = LookupsApi.reducer;
  reducers[CommonApi.reducerPath] = CommonApi.reducer;
  reducers[NavigationApi.reducerPath] = NavigationApi.reducer;
  reducers[AppSupportApi.reducerPath] = AppSupportApi.reducer;
  reducers[NavigationStatusApi.reducerPath] = NavigationStatusApi.reducer;
  reducers[BeneficiariesApi.reducerPath] = BeneficiariesApi.reducer;
  reducers[AdjustmentsApi.reducerPath] = AdjustmentsApi.reducer;
  reducers[DistributionApi.reducerPath] = DistributionApi.reducer;
  reducers[PayServicesApi.reducerPath] = PayServicesApi.reducer;
  reducers[AccountHistoryReportApi.reducerPath] = AccountHistoryReportApi.reducer;
  reducers[validationApi.reducerPath] = validationApi.reducer;

  return configureStore({
    reducer: reducers,
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware({ serializableCheck: false })
        // Add all RTK Query API middleware - critical to prevent warnings
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
        .concat(validationApi.middleware)
  });
};

type MockStore = ReturnType<typeof createMockStore>;

/**
 * Creates a wrapper component that provides Redux store
 *
 * @param store - Redux store to provide
 * @returns Wrapper component for test rendering
 *
 * @example
 * const store = createMockStore();
 * const wrapper = createProviderWrapper(store);
 * render(<Component />, { wrapper });
 */
type ProviderProps = PropsWithChildren<Record<string, unknown>>;

export const createProviderWrapper = (store: MockStore) => {
  const Wrapper = ({ children }: ProviderProps) => {
    return React.createElement(Provider, { store }, children);
  };
  return Wrapper;
};

/**
 * Combined helper: creates store and wrapper in one call
 *
 * @param preloadedState - Custom initial state
 * @returns Object with store and wrapper
 *
 * @example
 * const { store, wrapper } = createMockStoreAndWrapper({
 *   yearsEnd: { selectedProfitYear: 2025 }
 * });
 * render(<Component />, { wrapper });
 */
export const createMockStoreAndWrapper = (preloadedState?: PreloadedState<MockRootState>) => {
  const store = createMockStore(preloadedState);
  const wrapper = createProviderWrapper(store);

  return { store, wrapper };
};

/**
 * Utility for common test wrapper patterns
 *
 * Combines Redux Provider with any additional providers (Alerts, Router, etc.)
 *
 * @example
 * const wrapper = createTestWrapper(store, [
 *   MissiveAlertProvider,
 *   BrowserRouter
 * ]);
 * render(<Component />, { wrapper });
 */
export const createTestWrapper = (
  store: MockStore,
  additionalProviders?: Array<(props: { children: ReactNode }) => ReactNode>
) => {
  return ({ children }: ProviderProps) => {
    let content: ReactNode = React.createElement(Provider, { store }, children);

    // Wrap with additional providers in order
    if (additionalProviders) {
      for (const ProviderComponent of additionalProviders) {
        content = React.createElement(ProviderComponent, undefined, content);
      }
    }

    return content;
  };
};

/**
 * Creates a selector mock that returns specific state
 *
 * Useful for testing components that use useSelector
 *
 * @example
 * const selector = createSelectorMock({
 *   yearsEnd: { selectedProfitYear: 2024 }
 * });
 * vi.mocked(useSelector).mockImplementation(selector);
 */
export const createSelectorMock = (state: MockRootState) => {
  return (selector: (state: MockRootState) => unknown) => {
    return selector(state);
  };
};
