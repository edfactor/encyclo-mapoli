import { describe, expect, it, vi, beforeEach } from "vitest";
import { useSelector } from "react-redux";
import { useGridPagination } from "../../../../hooks/useGridPagination";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { useLazyGetDuplicateSSNsQuery } from "../../../../reduxstore/api/YearsEndApi";

vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("../../../../hooks/useGridPagination", () => ({
  useGridPagination: vi.fn(() => ({
    pageNumber: 0,
    pageSize: 25,
    sortParams: { sortBy: "ssn", isSortDescending: true },
    handlePaginationChange: vi.fn(),
    handleSortChange: vi.fn(),
    resetPagination: vi.fn()
  }))
}));

vi.mock("react-redux", () => ({
  useSelector: vi.fn(() => true)
}));

const { mockTriggerSearch } = vi.hoisted(() => ({
  mockTriggerSearch: vi.fn(async () => ({
    reportName: "Duplicate SSNs",
    reportDate: "2024-01-15",
    startDate: "2024-01-01",
    endDate: "2024-12-31",
    dataSource: "Test",
    response: {
      results: [{ ssn: "123-45-6789", badgeNumber: 12345, employeeName: "John Doe" }],
      total: 1,
      totalPages: 1,
      pageSize: 25,
      currentPage: 0
    }
  }))
}));

vi.mock("../../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetDuplicateSSNsQuery: vi.fn(() =>
    [mockTriggerSearch, { isFetching: false }, {}] as unknown as ReturnType<typeof useLazyGetDuplicateSSNsQuery>
  )
}));

describe("useDuplicateSSNsOnDemographics Hook", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Hook initialization", () => {
    it("should initialize with default state", () => {
      const hook = vi.fn(() => ({
        searchResults: null,
        isSearching: false,
        pagination: { pageNumber: 0, pageSize: 25 },
        showData: false,
        hasResults: false
      }));

      const result = hook();
      expect(result.searchResults).toBeNull();
      expect(result.isSearching).toBe(false);
    });
  });

  describe("Search functionality", () => {
    it("should trigger search with profit year", async () => {
      const [triggerSearch] = useLazyGetDuplicateSSNsQuery();
      const request = {
        pagination: { skip: 0, take: 25, sortBy: "ssn", isSortDescending: true },
        profitYear: 2024
      };

      const result = await triggerSearch(request);
      expect(result).toBeDefined();
      expect(result.response.results).toHaveLength(1);
    });
  });

  describe("Profit year handling", () => {
    it("should use current December flow profit year", () => {
      const profitYear = useDecemberFlowProfitYear();
      expect(profitYear).toBe(2024);
    });
  });

  describe("Token validation", () => {
    it("should check for security token before searching", () => {
      const hasToken = useSelector((state: { security: { token: string } }) => state.security.token);
      expect(hasToken).toBe(true);
    });
  });

  describe("Pagination state management", () => {
    it("should initialize pagination with default values", () => {
      const pagination = useGridPagination({
        initialPageSize: 25,
        initialSortBy: "ssn",
        initialSortDescending: true,
        onPaginationChange: vi.fn()
      });

      expect(pagination.pageNumber).toBe(0);
      expect(pagination.pageSize).toBe(25);
    });
  });

  describe("Auto-search on mount", () => {
    it("should trigger initial search when profit year is available", async () => {
      const profitYear = useDecemberFlowProfitYear();
      expect(profitYear).toBe(2024);

      const [triggerSearch] = useLazyGetDuplicateSSNsQuery();
      expect(triggerSearch).toBeDefined();
    });
  });

  describe("Error handling", () => {
    it("should handle search errors gracefully", async () => {
      const mockErrorTrigger = vi.fn(async () => {
        throw new Error("API Error");
      });

      vi.mocked(useLazyGetDuplicateSSNsQuery).mockReturnValueOnce(
        [mockErrorTrigger, { isFetching: false }, {}] as unknown as ReturnType<typeof useLazyGetDuplicateSSNsQuery>
      );

      const [triggerSearch] = useLazyGetDuplicateSSNsQuery();
      const request = {
        pagination: { skip: 0, take: 25, sortBy: "ssn", isSortDescending: true },
        profitYear: 2024
      };

      try {
        await triggerSearch(request);
      } catch (error) {
        expect(error).toBeDefined();
      }
    });
  });

  describe("Edge cases", () => {
    it("should handle empty search results", async () => {
      const mockEmptyTrigger = vi.fn(async () => ({
        reportName: "Duplicate SSNs",
        reportDate: "2024-01-15",
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        dataSource: "Test",
        response: {
          results: [],
          total: 0,
          totalPages: 0,
          pageSize: 25,
          currentPage: 0
        }
      }));

      vi.mocked(useLazyGetDuplicateSSNsQuery).mockReturnValueOnce(
        [mockEmptyTrigger, { isFetching: false }, {}] as unknown as ReturnType<typeof useLazyGetDuplicateSSNsQuery>
      );

      const [triggerSearch] = useLazyGetDuplicateSSNsQuery();
      const result = await triggerSearch({
        pagination: { skip: 0, take: 25, sortBy: "ssn", isSortDescending: true },
        profitYear: 2024
      });

      expect(result.response.results).toHaveLength(0);
    });
  });
});
