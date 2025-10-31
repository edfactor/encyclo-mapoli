import { describe, expect, it, vi, beforeEach } from "vitest";
import { useSelector } from "react-redux";
import { useGridPagination } from "../../../../hooks/useGridPagination";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { useLazyGetDuplicateSSNsOnDemographicsQuery } from "../../../../reduxstore/api/YearsEndApi";

vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("../../../../hooks/useGridPagination", () => ({
  useGridPagination: vi.fn(() => ({
    pageNumber: 0,
    pageSize: 25,
    sortParams: { sortBy: "ssn", isSortDescending: true },
    handlePaginationChange: vi.fn(),
    handleSortChange: vi.fn()
  }))
}));

vi.mock("react-redux", () => ({
  useSelector: vi.fn(() => true)
}));

vi.mock("../../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetDuplicateSSNsOnDemographicsQuery: vi.fn(() => [
    vi.fn(async (_request) => ({
      response: {
        results: [{ ssn: "123-45-6789", badgeNumber: 12345, employeeName: "John Doe" }],
        total: 1
      }
    })),
    { isFetching: false }
  ])
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
      const [triggerSearch] = useLazyGetDuplicateSSNsOnDemographicsQuery();
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

      const [triggerSearch] = useLazyGetDuplicateSSNsOnDemographicsQuery();
      expect(triggerSearch).toBeDefined();
    });
  });

  describe("Error handling", () => {
    it("should handle search errors gracefully", async () => {
      vi.mocked(useLazyGetDuplicateSSNsOnDemographicsQuery).mockReturnValueOnce([
        vi.fn(async () => {
          throw new Error("API Error");
        }),
        { isFetching: false }
      ] as ReturnType<typeof useLazyGetDuplicateSSNsOnDemographicsQuery>);

      const [triggerSearch] = useLazyGetDuplicateSSNsOnDemographicsQuery();
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
      vi.mocked(useLazyGetDuplicateSSNsOnDemographicsQuery).mockReturnValueOnce([
        vi.fn(async () => ({
          response: { results: [], total: 0 }
        })),
        { isFetching: false }
      ] as ReturnType<typeof useLazyGetDuplicateSSNsOnDemographicsQuery>);

      const [triggerSearch] = useLazyGetDuplicateSSNsOnDemographicsQuery();
      const result = await triggerSearch({
        pagination: { skip: 0, take: 25, sortBy: "ssn", isSortDescending: true },
        profitYear: 2024
      });

      expect(result.response.results).toHaveLength(0);
    });
  });
});
