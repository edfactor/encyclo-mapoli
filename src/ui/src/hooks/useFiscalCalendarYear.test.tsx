import { configureStore } from "@reduxjs/toolkit";
import { renderHook } from "@testing-library/react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import type { CalendarResponseDto } from "../types/common/api";
import useFiscalCalendarYear, { useLazyGetAccountingRangeToCurrent } from "./useFiscalCalendarYear";

// Mock the custom hooks
vi.mock("./useDecemberFlowProfitYear", () => ({
  default: vi.fn()
}));

// Mock the RTK Query hooks
vi.mock("reduxstore/api/LookupsApi", () => ({
  useLazyGetAccountingYearQuery: vi.fn(),
  useLazyGetAccountingRangeQuery: vi.fn()
}));

import { useLazyGetAccountingRangeQuery, useLazyGetAccountingYearQuery } from "reduxstore/api/LookupsApi";
import useDecemberFlowProfitYear from "./useDecemberFlowProfitYear";

describe("useFiscalCalendarYear", () => {
  let mockStore: ReturnType<typeof configureStore>;
  let mockFetchAccountingYear: ReturnType<typeof vi.fn>;

  const createMockStore = (hasToken: boolean, accountingYearData: CalendarResponseDto | null = null) => {
    return configureStore({
      reducer: {
        security: () => ({ token: hasToken ? "mock-token" : null }),
        lookups: () => ({ accountingYearData })
      }
    });
  };

  beforeEach(() => {
    vi.clearAllMocks();
    mockFetchAccountingYear = vi.fn();
  });

  const wrapper = ({ children }: { children: React.ReactNode }) => <Provider store={mockStore}>{children}</Provider>;

  it("should return null when no accounting year data is available", () => {
    mockStore = createMockStore(true, null);

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useLazyGetAccountingYearQuery).mockReturnValue([mockFetchAccountingYear, { isLoading: false }] as any);

    const { result } = renderHook(() => useFiscalCalendarYear(), { wrapper });

    expect(result.current).toBeNull();
  });

  it("should return accounting year data from Redux store", () => {
    const mockData: CalendarResponseDto = {
      fiscalBeginDate: "2024-01-01",
      fiscalEndDate: "2024-12-31"
    };
    mockStore = createMockStore(true, mockData);

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useLazyGetAccountingYearQuery).mockReturnValue([mockFetchAccountingYear, { isLoading: false }] as any);

    const { result } = renderHook(() => useFiscalCalendarYear(), { wrapper });

    expect(result.current).toEqual(mockData);
  });

  it("should fetch accounting year when profit year and token are available", () => {
    mockStore = createMockStore(true, null);
    const profitYear = 2024;

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(profitYear);
    vi.mocked(useLazyGetAccountingYearQuery).mockReturnValue([mockFetchAccountingYear, { isLoading: false }] as any);

    renderHook(() => useFiscalCalendarYear(), { wrapper });

    expect(mockFetchAccountingYear).toHaveBeenCalledWith({
      profitYear: profitYear
    });
  });

  it("should not fetch accounting year when token is missing", () => {
    mockStore = createMockStore(false, null);

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useLazyGetAccountingYearQuery).mockReturnValue([mockFetchAccountingYear, { isLoading: false }] as any);

    renderHook(() => useFiscalCalendarYear(), { wrapper });

    expect(mockFetchAccountingYear).not.toHaveBeenCalled();
  });

  it("should not fetch accounting year when profit year is falsy", () => {
    mockStore = createMockStore(true, null);

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(0);
    vi.mocked(useLazyGetAccountingYearQuery).mockReturnValue([mockFetchAccountingYear, { isLoading: false }] as any);

    renderHook(() => useFiscalCalendarYear(), { wrapper });

    expect(mockFetchAccountingYear).not.toHaveBeenCalled();
  });

  it("should refetch when profit year changes", () => {
    mockStore = createMockStore(true, null);

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useLazyGetAccountingYearQuery).mockReturnValue([mockFetchAccountingYear, { isLoading: false }] as any);

    const { rerender } = renderHook(() => useFiscalCalendarYear(), { wrapper });

    expect(mockFetchAccountingYear).toHaveBeenCalledWith({ profitYear: 2024 });

    // Change profit year
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2025);
    rerender();

    expect(mockFetchAccountingYear).toHaveBeenCalledWith({ profitYear: 2025 });
    expect(mockFetchAccountingYear).toHaveBeenCalledTimes(2);
  });
});

describe("useLazyGetAccountingRangeToCurrent", () => {
  let mockStore: ReturnType<typeof configureStore>;
  let mockTrigger: ReturnType<typeof vi.fn>;

  const createMockStore = (hasToken: boolean) => {
    return configureStore({
      reducer: {
        security: () => ({ token: hasToken ? "mock-token" : null }),
        yearsEnd: () => ({ selectedProfitYearForDecemberActivities: 2024 })
      }
    });
  };

  beforeEach(() => {
    vi.clearAllMocks();
    mockTrigger = vi.fn().mockResolvedValue(undefined);
  });

  const wrapper = ({ children }: { children: React.ReactNode }) => <Provider store={mockStore}>{children}</Provider>;

  it("should return trigger function and result", () => {
    mockStore = createMockStore(true);
    const mockResult = { data: [], isLoading: false };

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useLazyGetAccountingRangeQuery).mockReturnValue([mockTrigger, mockResult] as any);

    const { result } = renderHook(() => useLazyGetAccountingRangeToCurrent(5), { wrapper });

    expect(result.current).toHaveLength(2);
    expect(typeof result.current[0]).toBe("function");
    expect(result.current[1]).toBe(mockResult);
  });

  it("should call trigger with correct date range when hasToken is true", async () => {
    mockStore = createMockStore(true);
    const currentYear = 2024;
    const yearsBack = 5;

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(currentYear);
    vi.mocked(useLazyGetAccountingRangeQuery).mockReturnValue([mockTrigger, { data: [], isLoading: false }] as any);

    const { result } = renderHook(() => useLazyGetAccountingRangeToCurrent(yearsBack), {
      wrapper
    });

    await result.current[0]();

    expect(mockTrigger).toHaveBeenCalledWith({
      beginProfitYear: currentYear - yearsBack,
      endProfitYear: currentYear
    });
  });

  it("should not call trigger when hasToken is false", async () => {
    mockStore = createMockStore(false);

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useLazyGetAccountingRangeQuery).mockReturnValue([mockTrigger, { data: [], isLoading: false }] as any);

    const { result } = renderHook(() => useLazyGetAccountingRangeToCurrent(5), { wrapper });

    const triggerResult = await result.current[0]();

    expect(mockTrigger).not.toHaveBeenCalled();
    expect(triggerResult).toBeUndefined();
  });

  it("should calculate correct date range for different yearsBack values", async () => {
    mockStore = createMockStore(true);
    const currentYear = 2024;

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(currentYear);

    const testCases = [
      { yearsBack: 1, expectedBegin: 2023 },
      { yearsBack: 5, expectedBegin: 2019 },
      { yearsBack: 10, expectedBegin: 2014 }
    ];

    for (const { yearsBack, expectedBegin } of testCases) {
      mockTrigger.mockClear();
      vi.mocked(useLazyGetAccountingRangeQuery).mockReturnValue([mockTrigger, { data: [], isLoading: false }] as any);

      const { result } = renderHook(() => useLazyGetAccountingRangeToCurrent(yearsBack), {
        wrapper
      });

      await result.current[0]();

      expect(mockTrigger).toHaveBeenCalledWith({
        beginProfitYear: expectedBegin,
        endProfitYear: currentYear
      });
    }
  });

  it("should memoize wrapped trigger function", () => {
    mockStore = createMockStore(true);

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useLazyGetAccountingRangeQuery).mockReturnValue([mockTrigger, { data: [], isLoading: false }] as any);

    const { result, rerender } = renderHook(() => useLazyGetAccountingRangeToCurrent(5), {
      wrapper
    });

    const firstTrigger = result.current[0];
    rerender();
    const secondTrigger = result.current[0];

    expect(firstTrigger).toBe(secondTrigger);
  });

  it("should update wrapped trigger when dependencies change", () => {
    mockStore = createMockStore(true);

    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useLazyGetAccountingRangeQuery).mockReturnValue([mockTrigger, { data: [], isLoading: false }] as any);

    const { result, rerender } = renderHook(({ yearsBack }) => useLazyGetAccountingRangeToCurrent(yearsBack), {
      wrapper,
      initialProps: { yearsBack: 5 }
    });

    const firstTrigger = result.current[0];

    // Change yearsBack prop
    rerender({ yearsBack: 10 });
    const secondTrigger = result.current[0];

    expect(firstTrigger).not.toBe(secondTrigger);
  });
});
