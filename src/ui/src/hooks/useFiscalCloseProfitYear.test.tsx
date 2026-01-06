import { configureStore } from "@reduxjs/toolkit";
import { renderHook } from "@testing-library/react";
import { Provider } from "react-redux";
import { describe, expect, it } from "vitest";
import useFiscalCloseProfitYear from "./useFiscalCloseProfitYear";

import { vi } from "vitest";

vi.mock("reduxstore/api/ItOperationsApi", () => ({
  useLazyGetFrozenStateResponseQuery: vi.fn(() => [vi.fn(), {}])
}));

describe("useFiscalCloseProfitYear", () => {
  const createMockStore = (profitYear: number | null) => {
    return configureStore({
      reducer: {
        security: () => ({ token: "" }),
        frozen: () => ({
          frozenStateResponseData: profitYear === null ? null : ({ profitYear } as any),
          frozenStateCollectionData: null,
          error: null
        })
      }
    });
  };

  const wrapper =
    (store: ReturnType<typeof configureStore>) =>
    ({ children }: { children: React.ReactNode }) => <Provider store={store}>{children}</Provider>;

  it("should return the active frozen profit year when available", () => {
    const mockStore = createMockStore(2024);
    const { result } = renderHook(() => useFiscalCloseProfitYear(), { wrapper: wrapper(mockStore) });

    expect(result.current).toBe(2024);
  });

  it("should return different profit years based on frozen state", () => {
    const mockStore2023 = createMockStore(2023);
    const { result: result2023 } = renderHook(() => useFiscalCloseProfitYear(), {
      wrapper: wrapper(mockStore2023)
    });

    expect(result2023.current).toBe(2023);

    const mockStore2025 = createMockStore(2025);
    const { result: result2025 } = renderHook(() => useFiscalCloseProfitYear(), {
      wrapper: wrapper(mockStore2025)
    });

    expect(result2025.current).toBe(2025);
  });

  it("should fall back to current year when frozen state not loaded", () => {
    const mockStore = createMockStore(null);
    const { result } = renderHook(() => useFiscalCloseProfitYear(), { wrapper: wrapper(mockStore) });

    expect(result.current).toBe(new Date().getFullYear());
  });
});
