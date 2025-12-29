import { configureStore } from "@reduxjs/toolkit";
import { renderHook, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useIsProfitYearFrozen } from "./useIsProfitYearFrozen";

// Mock the RTK Query API
const { mockTriggerFetchActiveFrozenState } = vi.hoisted(() => ({
  mockTriggerFetchActiveFrozenState: vi.fn()
}));

vi.mock("../reduxstore/api/ItOperationsApi", () => ({
  useLazyGetFrozenStateResponseQuery: vi.fn(() => [mockTriggerFetchActiveFrozenState, { isLoading: false }])
}));

describe("useIsProfitYearFrozen", () => {
  const createMockStore = (
    frozenStateData: {
      profitYear: number;
      isActive: boolean;
    } | null,
    token: string | null = "test-token"
  ) => {
    return configureStore({
      reducer: {
        frozen: () => ({
          frozenStateResponseData: frozenStateData
        }),
        security: () => ({
          token
        })
      }
    });
  };

  const wrapper =
    (store: ReturnType<typeof configureStore>) =>
    ({ children }: { children: React.ReactNode }) => <Provider store={store}>{children}</Provider>;

  beforeEach(() => {
    vi.clearAllMocks();

    mockTriggerFetchActiveFrozenState.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({
        profitYear: 2024,
        isActive: true
      })
    });
  });

  describe("Initial State and Return Value", () => {
    it("should return false when profitYear is not provided", () => {
      const mockStore = createMockStore(null);
      const { result } = renderHook(() => useIsProfitYearFrozen(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });

    it("should return false when profitYear is undefined", () => {
      const mockStore = createMockStore(null);
      const { result } = renderHook(() => useIsProfitYearFrozen(undefined), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });

    it("should return false when frozenStateResponseData is null", () => {
      const mockStore = createMockStore(null);
      const { result } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });
  });

  describe("Frozen State Detection", () => {
    it("should return true when profit year matches and is active", () => {
      const mockStore = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      const { result } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(true);
    });

    it("should return false when profit year matches but is not active", () => {
      const mockStore = createMockStore({
        profitYear: 2024,
        isActive: false
      });

      const { result } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });

    it("should return false when profit year does not match", () => {
      const mockStore = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      const { result } = renderHook(() => useIsProfitYearFrozen(2023), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });

    it("should return false when isActive is explicitly false", () => {
      const mockStore = createMockStore({
        profitYear: 2024,
        isActive: false
      });

      const { result } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });
  });

  describe("Auto-Fetch Behavior", () => {
    it("should trigger fetch when profitYear provided, token exists, and no frozen state data", async () => {
      const mockStore = createMockStore(null, "test-token");

      renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore)
      });

      await waitFor(() => {
        expect(mockTriggerFetchActiveFrozenState).toHaveBeenCalled();
      });
    });

    it("should not trigger fetch when profitYear is not provided", () => {
      const mockStore = createMockStore(null, "test-token");

      renderHook(() => useIsProfitYearFrozen(), {
        wrapper: wrapper(mockStore)
      });

      expect(mockTriggerFetchActiveFrozenState).not.toHaveBeenCalled();
    });

    it("should not trigger fetch when token is null", () => {
      const mockStore = createMockStore(null, null);

      renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore)
      });

      expect(mockTriggerFetchActiveFrozenState).not.toHaveBeenCalled();
    });

    it("should not trigger fetch when frozen state data already exists", () => {
      const mockStore = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore)
      });

      expect(mockTriggerFetchActiveFrozenState).not.toHaveBeenCalled();
    });

    it("should not trigger fetch when profitYear is undefined", () => {
      const mockStore = createMockStore(null, "test-token");

      renderHook(() => useIsProfitYearFrozen(undefined), {
        wrapper: wrapper(mockStore)
      });

      expect(mockTriggerFetchActiveFrozenState).not.toHaveBeenCalled();
    });
  });

  describe("Multiple Profit Years", () => {
    it("should correctly identify different frozen profit years", () => {
      const mockStore2023 = createMockStore({
        profitYear: 2023,
        isActive: true
      });

      const { result: result2023 } = renderHook(() => useIsProfitYearFrozen(2023), {
        wrapper: wrapper(mockStore2023)
      });

      expect(result2023.current).toBe(true);

      const mockStore2024 = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      const { result: result2024 } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore2024)
      });

      expect(result2024.current).toBe(true);
    });

    it("should return false for non-frozen years when another year is frozen", () => {
      const mockStore = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      const { result } = renderHook(() => useIsProfitYearFrozen(2025), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });
  });

  describe("Edge Cases", () => {
    it("should handle profitYear of 0 (treated as falsy)", () => {
      const mockStore = createMockStore({
        profitYear: 0,
        isActive: true
      });

      const { result } = renderHook(() => useIsProfitYearFrozen(0), {
        wrapper: wrapper(mockStore)
      });

      // The hook treats 0 as falsy (no profitYear provided), so returns false
      expect(result.current).toBe(false);
    });

    it("should handle negative profit years", () => {
      const mockStore = createMockStore({
        profitYear: -1,
        isActive: true
      });

      const { result } = renderHook(() => useIsProfitYearFrozen(-1), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(true);
    });

    it("should handle very large profit years", () => {
      const mockStore = createMockStore({
        profitYear: 9999,
        isActive: true
      });

      const { result } = renderHook(() => useIsProfitYearFrozen(9999), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(true);
    });
  });

  describe("State Changes", () => {
    it("should update when profitYear prop changes", () => {
      const mockStore = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      const { result, rerender } = renderHook(({ year }) => useIsProfitYearFrozen(year), {
        wrapper: wrapper(mockStore),
        initialProps: { year: 2024 }
      });

      expect(result.current).toBe(true);

      rerender({ year: 2025 });

      expect(result.current).toBe(false);
    });

    it("should return false when profitYear changes from valid to undefined", () => {
      const mockStore = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      const { result, rerender } = renderHook(({ year }) => useIsProfitYearFrozen(year), {
        wrapper: wrapper(mockStore),
        initialProps: { year: 2024 }
      });

      expect(result.current).toBe(true);

      rerender({ year: undefined });

      expect(result.current).toBe(false);
    });
  });

  describe("Realistic Scenarios", () => {
    it("should handle typical frozen demographics workflow", () => {
      // Start with no frozen state
      const mockStore = createMockStore(null, "test-token");

      const { result } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore)
      });

      // Initially false (no data)
      expect(result.current).toBe(false);

      // After data loads, frozen state exists
      const mockStoreWithData = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      const { result: resultWithData } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStoreWithData)
      });

      expect(resultWithData.current).toBe(true);
    });

    it("should handle unfreezing scenario", () => {
      // Start with frozen state
      const mockStoreFrozen = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      const { result: resultFrozen } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStoreFrozen)
      });

      expect(resultFrozen.current).toBe(true);

      // After unfreezing
      const mockStoreUnfrozen = createMockStore({
        profitYear: 2024,
        isActive: false
      });

      const { result: resultUnfrozen } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStoreUnfrozen)
      });

      expect(resultUnfrozen.current).toBe(false);
    });

    it("should handle checking multiple years simultaneously", () => {
      const mockStore = createMockStore({
        profitYear: 2024,
        isActive: true
      });

      const { result: result2023 } = renderHook(() => useIsProfitYearFrozen(2023), {
        wrapper: wrapper(mockStore)
      });

      const { result: result2024 } = renderHook(() => useIsProfitYearFrozen(2024), {
        wrapper: wrapper(mockStore)
      });

      const { result: result2025 } = renderHook(() => useIsProfitYearFrozen(2025), {
        wrapper: wrapper(mockStore)
      });

      expect(result2023.current).toBe(false);
      expect(result2024.current).toBe(true);
      expect(result2025.current).toBe(false);
    });
  });
});
