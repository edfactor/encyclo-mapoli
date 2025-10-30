import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook } from "@testing-library/react";

// Mock dependencies
vi.mock("../../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetDuplicateNamesAndBirthdaysQuery: vi.fn()
}));

vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn()
}));

vi.mock("../../../../hooks/useGridPagination", () => ({
  useGridPagination: vi.fn()
}));

vi.mock("react-redux", () => ({
  useSelector: vi.fn(),
  useDispatch: vi.fn()
}));

import useDuplicateNamesAndBirthdays from "./useDuplicateNamesAndBirthdays";
import * as YearsEndApi from "../../../../reduxstore/api/YearsEndApi";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { useGridPagination } from "../../../../hooks/useGridPagination";
import { useSelector } from "react-redux";

const mockDuplicateData = {
  results: [
    {
      id: 1,
      firstName: "John",
      lastName: "Smith",
      dateOfBirth: "1990-01-15",
      badgeNumber: 123456
    }
  ],
  total: 1
};

describe("useDuplicateNamesAndBirthdays", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should not fetch when token is missing", () => {
    const mockTriggerSearch = vi.fn();
    vi.mocked(YearsEndApi.useLazyGetDuplicateNamesAndBirthdaysQuery).mockReturnValue([
      mockTriggerSearch,
      { isFetching: false }
    ] as unknown as ReturnType<typeof vi.fn>);

    vi.mocked(useSelector).mockReturnValue(null);
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useGridPagination).mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "name", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    } as unknown as ReturnType<typeof useGridPagination>);

    const { result } = renderHook(() => useDuplicateNamesAndBirthdays());

    expect(result.current.isSearching).toBe(false);
  });

  it("should not fetch when profitYear is missing", () => {
    const mockTriggerSearch = vi.fn();
    vi.mocked(YearsEndApi.useLazyGetDuplicateNamesAndBirthdaysQuery).mockReturnValue([
      mockTriggerSearch,
      { isFetching: false }
    ] as unknown as ReturnType<typeof vi.fn>);

    vi.mocked(useSelector).mockReturnValue("mock-token");
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(null);
    vi.mocked(useGridPagination).mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "name", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    } as unknown as ReturnType<typeof useGridPagination>);

    const { result } = renderHook(() => useDuplicateNamesAndBirthdays());

    expect(result.current.isSearching).toBe(false);
  });

  it("should expose executeSearch function", () => {
    const mockTriggerSearch = vi.fn().mockReturnValue({
      unwrap: vi.fn().mockResolvedValue(mockDuplicateData)
    });

    vi.mocked(YearsEndApi.useLazyGetDuplicateNamesAndBirthdaysQuery).mockReturnValue([
      mockTriggerSearch,
      { isFetching: false }
    ] as unknown as ReturnType<typeof vi.fn>);

    vi.mocked(useSelector).mockReturnValue("mock-token");
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useGridPagination).mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "name", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    } as unknown as ReturnType<typeof useGridPagination>);

    const { result } = renderHook(() => useDuplicateNamesAndBirthdays());

    expect(typeof result.current.executeSearch).toBe("function");
  });

  it("should expose pagination object", () => {
    const mockTriggerSearch = vi.fn();
    vi.mocked(YearsEndApi.useLazyGetDuplicateNamesAndBirthdaysQuery).mockReturnValue([
      mockTriggerSearch,
      { isFetching: false }
    ] as unknown as ReturnType<typeof vi.fn>);

    vi.mocked(useSelector).mockReturnValue("mock-token");
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useGridPagination).mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "name", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    } as unknown as ReturnType<typeof useGridPagination>);

    const { result } = renderHook(() => useDuplicateNamesAndBirthdays());

    expect(result.current.pagination).toBeDefined();
    expect(result.current.pagination.pageNumber).toBe(0);
    expect(result.current.pagination.pageSize).toBe(25);
  });

  it("should expose showData selector", () => {
    const mockTriggerSearch = vi.fn();
    vi.mocked(YearsEndApi.useLazyGetDuplicateNamesAndBirthdaysQuery).mockReturnValue([
      mockTriggerSearch,
      { isFetching: false }
    ] as unknown as ReturnType<typeof vi.fn>);

    vi.mocked(useSelector).mockReturnValue("mock-token");
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useGridPagination).mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "name", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    } as unknown as ReturnType<typeof useGridPagination>);

    const { result } = renderHook(() => useDuplicateNamesAndBirthdays());

    expect(typeof result.current.showData).toBe("boolean");
  });

  it("should expose hasResults selector", () => {
    const mockDuplicateData = {
      response: {
        results: [
          {
            id: 1,
            firstName: "John",
            lastName: "Smith",
            dateOfBirth: "1990-01-15",
            badgeNumber: 123456
          }
        ]
      },
      pageInfo: { pageNumber: 0, pageSize: 25, totalPages: 1, totalCount: 1 }
    };

    const mockTriggerSearch = vi.fn().mockReturnValue({
      unwrap: vi.fn().mockResolvedValue(mockDuplicateData)
    });
    vi.mocked(YearsEndApi.useLazyGetDuplicateNamesAndBirthdaysQuery).mockReturnValue([
      mockTriggerSearch,
      { isFetching: false }
    ] as unknown as ReturnType<typeof vi.fn>);

    vi.mocked(useSelector).mockReturnValue("mock-token");
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useGridPagination).mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "name", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    } as unknown as ReturnType<typeof useGridPagination>);

    const { result } = renderHook(() => useDuplicateNamesAndBirthdays());

    expect(typeof result.current.hasResults).toBe("boolean");
  });

  it("should return null searchParams initially", () => {
    const mockTriggerSearch = vi.fn();
    vi.mocked(YearsEndApi.useLazyGetDuplicateNamesAndBirthdaysQuery).mockReturnValue([
      mockTriggerSearch,
      { isFetching: false }
    ] as unknown as ReturnType<typeof vi.fn>);

    vi.mocked(useSelector).mockReturnValue("mock-token");
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(null);
    vi.mocked(useGridPagination).mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "name", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    } as unknown as ReturnType<typeof useGridPagination>);

    const { result } = renderHook(() => useDuplicateNamesAndBirthdays());

    expect(result.current.searchParams).toBeNull();
  });

  it("should expose isSearching state", () => {
    const mockTriggerSearch = vi.fn();
    vi.mocked(YearsEndApi.useLazyGetDuplicateNamesAndBirthdaysQuery).mockReturnValue([
      mockTriggerSearch,
      { isFetching: true }
    ] as unknown as ReturnType<typeof vi.fn>);

    vi.mocked(useSelector).mockReturnValue("mock-token");
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
    vi.mocked(useGridPagination).mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "name", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    } as unknown as ReturnType<typeof useGridPagination>);

    const { result } = renderHook(() => useDuplicateNamesAndBirthdays());

    expect(typeof result.current.isSearching).toBe("boolean");
  });
});
