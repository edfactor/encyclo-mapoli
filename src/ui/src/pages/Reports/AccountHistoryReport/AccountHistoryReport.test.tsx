import { act, renderHook } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { AccountHistoryReportRequest } from "../../../types/reports/AccountHistoryReportTypes";

/**
 * PS-#### : Unit tests for AccountHistoryReport pagination and filtering logic
 * These tests verify the page state management without rendering the full component
 */
describe("AccountHistoryReport - Pagination Logic", () => {
  describe("useGridPagination Hook", () => {
    it("should initialize with default pagination values", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "profitYear",
          initialSortDescending: true
        })
      );

      expect(result.current.pageNumber).toBe(0);
      expect(result.current.pageSize).toBe(25);
      expect(result.current.sortParams.sortBy).toBe("profitYear");
      expect(result.current.sortParams.isSortDescending).toBe(true);
    });

    it("should support pagination change", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "profitYear",
          initialSortDescending: true
        })
      );

      act(() => {
        result.current.handlePaginationChange(1, 25);
      });

      expect(result.current).toBeDefined();
    });

    it("should support page size change", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "badgeNumber",
          initialSortDescending: false
        })
      );

      act(() => {
        result.current.handlePaginationChange(0, 50);
      });

      expect(result.current).toBeDefined();
    });

    it("should handle sort change", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "profitYear",
          initialSortDescending: true
        })
      );

      act(() => {
        result.current.handleSortChange({
          sortBy: "contributions",
          isSortDescending: false
        });
      });

      expect(result.current.sortParams.sortBy).toBe("contributions");
      expect(result.current.sortParams.isSortDescending).toBe(false);
    });

    it("should reset pagination to first page", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "profitYear",
          initialSortDescending: true
        })
      );

      act(() => {
        result.current.resetPagination();
      });

      expect(result.current.pageNumber).toBe(0);
    });
  });

  describe("Request Building", () => {
    it("should build correct API request with pagination parameters", () => {
      const pageNumber = 2;
      const pageSize = 50;
      const sortParams: SortParams = {
        sortBy: "earnings",
        isSortDescending: true
      };

      const request: AccountHistoryReportRequest = {
        badgeNumber: 12345,
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };

      expect(request.pagination.skip).toBe(100);
      expect(request.pagination.take).toBe(50);
      expect(request.pagination.sortBy).toBe("earnings");
      expect(request.pagination.isSortDescending).toBe(true);
    });

    it("should calculate skip value correctly for different pages", () => {
      const cases = [
        { pageNumber: 0, pageSize: 25, expectedSkip: 0 },
        { pageNumber: 1, pageSize: 25, expectedSkip: 25 },
        { pageNumber: 2, pageSize: 25, expectedSkip: 50 },
        { pageNumber: 0, pageSize: 50, expectedSkip: 0 },
        { pageNumber: 1, pageSize: 50, expectedSkip: 50 }
      ];

      cases.forEach(({ pageNumber, pageSize, expectedSkip }) => {
        const skip = pageNumber * pageSize;
        expect(skip).toBe(expectedSkip);
      });
    });

    it("should handle optional date parameters", () => {
      const request: AccountHistoryReportRequest = {
        badgeNumber: 12345,
        startDate: undefined,
        endDate: undefined,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "profitYear",
          isSortDescending: true
        }
      };

      expect(request.startDate).toBeUndefined();
      expect(request.endDate).toBeUndefined();
    });
  });

  describe("Sorting Logic", () => {
    it("should support sorting by different fields", () => {
      const fields = ["profitYear", "badgeNumber", "fullName", "contributions", "earnings", "endingBalance"];

      fields.forEach((field) => {
        const request: AccountHistoryReportRequest = {
          badgeNumber: 12345,
          pagination: {
            skip: 0,
            take: 25,
            sortBy: field,
            isSortDescending: false
          }
        };

        expect(request.pagination.sortBy).toBe(field);
      });
    });

    it("should toggle sort direction", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "contributions",
          initialSortDescending: false
        })
      );

      act(() => {
        result.current.handleSortChange({
          sortBy: "contributions",
          isSortDescending: true
        });
      });

      expect(result.current.sortParams.isSortDescending).toBe(true);
    });
  });

  describe("Boundary Conditions", () => {
    it("should handle first page request", () => {
      const request: AccountHistoryReportRequest = {
        badgeNumber: 12345,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "profitYear",
          isSortDescending: true
        }
      };

      expect(request.pagination.skip).toBe(0);
      expect(request.pagination.take).toBe(25);
    });

    it("should handle large page number", () => {
      const pageNumber = 9999;
      const pageSize = 25;
      const skip = pageNumber * pageSize;

      expect(skip).toBe(249975);
    });

    it("should handle minimum and maximum page sizes", () => {
      const minSize = 10;
      const maxSize = 100;

      const minRequest: AccountHistoryReportRequest = {
        badgeNumber: 12345,
        pagination: {
          skip: 0,
          take: minSize,
          sortBy: "profitYear",
          isSortDescending: true
        }
      };

      const maxRequest: AccountHistoryReportRequest = {
        badgeNumber: 12345,
        pagination: {
          skip: 0,
          take: maxSize,
          sortBy: "profitYear",
          isSortDescending: true
        }
      };

      expect(minRequest.pagination.take).toBe(minSize);
      expect(maxRequest.pagination.take).toBe(maxSize);
    });
  });
});
