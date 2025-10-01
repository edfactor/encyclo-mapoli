import { configureStore } from "@reduxjs/toolkit";
import { renderHook } from "@testing-library/react";
import { Provider } from "react-redux";
import { describe, expect, it } from "vitest";
import useFiscalCloseProfitYear from "./useFiscalCloseProfitYear";

describe("useFiscalCloseProfitYear", () => {
  const createMockStore = (selectedProfitYear: number) => {
    return configureStore({
      reducer: {
        yearsEnd: () => ({
          selectedProfitYearForFiscalClose: selectedProfitYear
        })
      }
    });
  };

  const wrapper =
    (store: ReturnType<typeof configureStore>) =>
    ({ children }: { children: React.ReactNode }) => <Provider store={store}>{children}</Provider>;

  it("should return the selected profit year from Redux store", () => {
    const mockStore = createMockStore(2024);
    const { result } = renderHook(() => useFiscalCloseProfitYear(), { wrapper: wrapper(mockStore) });

    expect(result.current).toBe(2024);
  });

  it("should return different profit years based on store state", () => {
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

  it("should return 0 when profit year is 0", () => {
    const mockStore = createMockStore(0);
    const { result } = renderHook(() => useFiscalCloseProfitYear(), { wrapper: wrapper(mockStore) });

    expect(result.current).toBe(0);
  });
});
