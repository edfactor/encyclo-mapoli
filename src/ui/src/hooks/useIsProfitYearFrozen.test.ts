import { renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useIsProfitYearFrozen } from "./useIsProfitYearFrozen";

// Mock Redux hooks
const mockUseSelector = vi.fn();
const mockUseDispatch = vi.fn();

vi.mock("react-redux", () => ({
  useSelector: (selector: unknown) => mockUseSelector(selector),
  useDispatch: () => mockUseDispatch()
}));

// Mock RTK Query hook
const mockTriggerFrozenStateSearch = vi.fn();
const mockUseLazyGetProfitYearSelectorFrozenDataQuery = vi.fn();

vi.mock("../reduxstore/api/ItOperationsApi", () => ({
  useLazyGetProfitYearSelectorFrozenDataQuery: () => mockUseLazyGetProfitYearSelectorFrozenDataQuery()
}));

describe("useIsProfitYearFrozen", () => {
  beforeEach(() => {
    mockUseSelector.mockClear();
    mockUseDispatch.mockClear();
    mockTriggerFrozenStateSearch.mockClear();
    mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockClear();
  });

  describe("when profitYear is not provided", () => {
    it("should return false when profitYear is undefined", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: { profitYearSelectorData: null },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(undefined));

      expect(result.current).toBe(false);
    });

    it("should return false when profitYear is null", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: { profitYearSelectorData: null },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(undefined));

      expect(result.current).toBe(false);
    });
  });

  describe("when frozen data is not loaded", () => {
    it("should return false when frozenStates is null", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: { profitYearSelectorData: null },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(2024));

      expect(result.current).toBe(false);
    });

    it("should return false when frozenStates.results is null", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: { profitYearSelectorData: { results: null } },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(2024));

      expect(result.current).toBe(false);
    });

    it("should return false when frozenStates.results is undefined", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: { profitYearSelectorData: {} },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(2024));

      expect(result.current).toBe(false);
    });
  });

  describe("when checking frozen state", () => {
    it("should return true when profit year has ACTIVE frozen state", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: [
                { profitYear: 2024, isActive: true },
                { profitYear: 2023, isActive: true }
              ]
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(2024));

      expect(result.current).toBe(true);
    });

    it("should return false when profit year has frozen state but isActive is false", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: [
                { profitYear: 2024, isActive: false },
                { profitYear: 2023, isActive: true }
              ]
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(2024));

      expect(result.current).toBe(false);
    });

    it("should return false when profit year is not in frozen states", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: [
                { profitYear: 2023, isActive: true },
                { profitYear: 2022, isActive: true }
              ]
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(2024));

      expect(result.current).toBe(false);
    });

    it("should return false when frozen states array is empty", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: []
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(2024));

      expect(result.current).toBe(false);
    });
  });

  describe("auto-fetch behavior", () => {
    it("should trigger fetch when profitYear provided, token exists, no frozenStates, and not loading", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: { profitYearSelectorData: null },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      renderHook(() => useIsProfitYearFrozen(2024));

      expect(mockTriggerFrozenStateSearch).toHaveBeenCalledWith({
        skip: 0,
        take: 100,
        sortBy: "createdDateTime",
        isSortDescending: true
      });
    });

    it("should not trigger fetch when profitYear is missing", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: { profitYearSelectorData: null },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      renderHook(() => useIsProfitYearFrozen(undefined));

      expect(mockTriggerFrozenStateSearch).not.toHaveBeenCalled();
    });

    it("should not trigger fetch when token is missing", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: { profitYearSelectorData: null },
          security: { token: null }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      renderHook(() => useIsProfitYearFrozen(2024));

      expect(mockTriggerFrozenStateSearch).not.toHaveBeenCalled();
    });

    it("should not trigger fetch when frozenStates already loaded", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: [{ profitYear: 2024, isActive: true }]
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      renderHook(() => useIsProfitYearFrozen(2024));

      expect(mockTriggerFrozenStateSearch).not.toHaveBeenCalled();
    });

    it("should not trigger fetch when already loading", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: { profitYearSelectorData: null },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: true }
      ]);

      renderHook(() => useIsProfitYearFrozen(2024));

      expect(mockTriggerFrozenStateSearch).not.toHaveBeenCalled();
    });
  });

  describe("multiple profit years", () => {
    it("should correctly identify different profit years", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: [
                { profitYear: 2024, isActive: true },
                { profitYear: 2023, isActive: true },
                { profitYear: 2022, isActive: false }
              ]
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result: result2024 } = renderHook(() => useIsProfitYearFrozen(2024));
      const { result: result2023 } = renderHook(() => useIsProfitYearFrozen(2023));
      const { result: result2022 } = renderHook(() => useIsProfitYearFrozen(2022));
      const { result: result2021 } = renderHook(() => useIsProfitYearFrozen(2021));

      expect(result2024.current).toBe(true);
      expect(result2023.current).toBe(true);
      expect(result2022.current).toBe(false); // isActive is false
      expect(result2021.current).toBe(false); // Not in list
    });
  });

  describe("edge cases", () => {
    it("should handle profit year 0 (returns false due to falsy check)", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: [{ profitYear: 0, isActive: true }]
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(0));

      // The hook checks `if (!profitYear || !frozenStates?.results)`
      // which treats 0 as falsy, so it returns false
      expect(result.current).toBe(false);
    });

    it("should handle negative profit year", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: [{ profitYear: -2024, isActive: true }]
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(-2024));

      expect(result.current).toBe(true);
    });

    it("should handle large profit year numbers", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: [{ profitYear: 999999, isActive: true }]
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(999999));

      expect(result.current).toBe(true);
    });

    it("should handle frozen state with missing isActive property (treat as falsy)", () => {
      mockUseSelector.mockImplementation((selector) => {
        const mockState = {
          frozen: {
            profitYearSelectorData: {
              results: [{ profitYear: 2024 } as { profitYear: number; isActive: boolean }]
            }
          },
          security: { token: "test-token" }
        };
        return selector(mockState);
      });

      mockUseLazyGetProfitYearSelectorFrozenDataQuery.mockReturnValue([
        mockTriggerFrozenStateSearch,
        { isLoading: false }
      ]);

      const { result } = renderHook(() => useIsProfitYearFrozen(2024));

      expect(result.current).toBe(false);
    });
  });
});
