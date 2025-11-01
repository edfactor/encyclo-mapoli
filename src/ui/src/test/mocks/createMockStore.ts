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

/**
 * Minimal mock store for testing
 *
 * In real projects, you would import actual reducers:
 *   import securityReducer from "../../reduxstore/slices/securitySlice";
 *   import navigationReducer from "../../reduxstore/slices/navigationSlice";
 *   import yearsEndReducer from "../../reduxstore/slices/yearsEndSlice";
 *
 * For now, we provide a factory that creates reducers on-the-fly
 * This is ideal for test isolation and doesn't require importing all slices
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
  };
  yearsEnd?: {
    selectedProfitYear?: number | null;
    [key: string]: unknown;
  };
  distribution?: {
    [key: string]: unknown;
  };
  inquiry?: {
    masterInquiryMemberDetails?: unknown;
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
      }
    },
    yearsEnd: {
      selectedProfitYear: 2024
    },
    distribution: {},
    inquiry: {
      masterInquiryMemberDetails: null
    }
  };

  // Create simple reducers that just return state
  // In real code, these would be actual slice reducers
  const createSliceReducer =
    (initialState: unknown) =>
    (state = initialState, _action: unknown) =>
      state;

  const baseReducers = {
    security: createSliceReducer(preloadedState?.security ?? defaultState.security),
    navigation: createSliceReducer(preloadedState?.navigation ?? defaultState.navigation),
    yearsEnd: createSliceReducer(preloadedState?.yearsEnd ?? defaultState.yearsEnd),
    distribution: createSliceReducer(preloadedState?.distribution ?? defaultState.distribution),
    inquiry: createSliceReducer(preloadedState?.inquiry ?? defaultState.inquiry)
  };

  // Add lookupsApi reducer and middleware if mocked in tests
  // This allows tests that mock lookupsApi to work properly
  const reducers: { [key: string]: unknown } = baseReducers;
  const lookupsApiReducerPath = "lookupsApi";

  // Provide fallback for lookupsApi that tests can mock
  // When tests mock lookupsApi with vi.mock(), these will be replaced
  const defaultLookupsApiReducer = (state = {}) => state;
  reducers[lookupsApiReducerPath] = defaultLookupsApiReducer;

  return configureStore({
    reducer: reducers,
    middleware: (getDefaultMiddleware) => {
      // Return default middleware - mocked lookupsApi middleware will be added by test mocks
      return getDefaultMiddleware();
    }
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
