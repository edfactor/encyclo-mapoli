import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";
import { useGridPagination, SortParams } from "./useGridPagination";

describe("useGridPagination", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("initial state", () => {
    it("should initialize with correct default values", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: true
        })
      );

      expect(result.current.pageNumber).toBe(0);
      expect(result.current.pageSize).toBe(25);
      expect(result.current.sortParams.sortBy).toBe("name");
      expect(result.current.sortParams.isSortDescending).toBe(true);
    });

    it("should use default isSortDescending=true when not provided", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 10,
          initialSortBy: "date"
        })
      );

      expect(result.current.sortParams.isSortDescending).toBe(true);
    });

    it("should use initialSortDescending=false when provided", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 10,
          initialSortBy: "date",
          initialSortDescending: false
        })
      );

      expect(result.current.sortParams.isSortDescending).toBe(false);
    });

    it("should handle different initial page sizes", () => {
      const { result: result10 } = renderHook(() =>
        useGridPagination({
          initialPageSize: 10,
          initialSortBy: "name"
        })
      );
      expect(result10.current.pageSize).toBe(10);

      const { result: result50 } = renderHook(() =>
        useGridPagination({
          initialPageSize: 50,
          initialSortBy: "name"
        })
      );
      expect(result50.current.pageSize).toBe(50);

      const { result: result100 } = renderHook(() =>
        useGridPagination({
          initialPageSize: 100,
          initialSortBy: "name"
        })
      );
      expect(result100.current.pageSize).toBe(100);
    });
  });

  describe("handlePaginationChange", () => {
    it("should update pageNumber and pageSize", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name"
        })
      );

      act(() => {
        result.current.handlePaginationChange(2, 50);
      });

      expect(result.current.pageNumber).toBe(2);
      expect(result.current.pageSize).toBe(50);
    });

    it("should preserve sortParams when changing pagination", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "email",
          initialSortDescending: false
        })
      );

      act(() => {
        result.current.handlePaginationChange(1, 50);
      });

      expect(result.current.sortParams.sortBy).toBe("email");
      expect(result.current.sortParams.isSortDescending).toBe(false);
    });

    it("should call onPaginationChange callback when provided", async () => {
      const mockCallback = vi.fn();
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: true,
          onPaginationChange: mockCallback
        })
      );

      act(() => {
        result.current.handlePaginationChange(1, 50);
      });

      await waitFor(() => {
        expect(mockCallback).toHaveBeenCalledWith(1, 50, {
          sortBy: "name",
          isSortDescending: true
        });
      });
    });

    it("should not call onPaginationChange if callback is not provided", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name"
        })
      );

      expect(() => {
        act(() => {
          result.current.handlePaginationChange(1, 50);
        });
      }).not.toThrow();
    });

    it("should handle page 0 correctly", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name"
        })
      );

      act(() => {
        result.current.handlePaginationChange(0, 25);
      });

      expect(result.current.pageNumber).toBe(0);
      expect(result.current.pageSize).toBe(25);
    });

    it("should handle large page numbers", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 10,
          initialSortBy: "name"
        })
      );

      act(() => {
        result.current.handlePaginationChange(999, 10);
      });

      expect(result.current.pageNumber).toBe(999);
    });

    it("should prevent re-entrant calls", async () => {
      const mockCallback = vi.fn();
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          onPaginationChange: mockCallback
        })
      );

      // Try to call twice rapidly
      act(() => {
        result.current.handlePaginationChange(1, 50);
        // This second call should be blocked
        result.current.handlePaginationChange(2, 50);
      });

      await waitFor(() => {
        // Only the first call should succeed
        expect(mockCallback).toHaveBeenCalledTimes(1);
      });
    });
  });

  describe("handleSortChange", () => {
    it("should update sort parameters", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name"
        })
      );

      const newSortParams: SortParams = {
        sortBy: "email",
        isSortDescending: false
      };

      act(() => {
        result.current.handleSortChange(newSortParams);
      });

      expect(result.current.sortParams.sortBy).toBe("email");
      expect(result.current.sortParams.isSortDescending).toBe(false);
    });

    it("should preserve pagination state when changing sort", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 50,
          initialSortBy: "name"
        })
      );

      act(() => {
        result.current.handlePaginationChange(3, 50);
      });

      act(() => {
        result.current.handleSortChange({
          sortBy: "date",
          isSortDescending: true
        });
      });

      expect(result.current.pageNumber).toBe(3);
      expect(result.current.pageSize).toBe(50);
    });

    it("should call onPaginationChange callback with current page/size", async () => {
      const mockCallback = vi.fn();
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          onPaginationChange: mockCallback
        })
      );

      // First, change pagination
      act(() => {
        result.current.handlePaginationChange(2, 50);
      });

      mockCallback.mockClear();

      // Then change sort
      act(() => {
        result.current.handleSortChange({
          sortBy: "email",
          isSortDescending: false
        });
      });

      expect(mockCallback).toHaveBeenCalledWith(2, 50, {
        sortBy: "email",
        isSortDescending: false
      });
    });

    it("should toggle sort direction", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: true
        })
      );

      act(() => {
        result.current.handleSortChange({
          sortBy: "name",
          isSortDescending: false
        });
      });

      expect(result.current.sortParams.isSortDescending).toBe(false);

      act(() => {
        result.current.handleSortChange({
          sortBy: "name",
          isSortDescending: true
        });
      });

      expect(result.current.sortParams.isSortDescending).toBe(true);
    });

    it("should handle changing both sortBy and direction", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: true
        })
      );

      act(() => {
        result.current.handleSortChange({
          sortBy: "createdDate",
          isSortDescending: false
        });
      });

      expect(result.current.sortParams.sortBy).toBe("createdDate");
      expect(result.current.sortParams.isSortDescending).toBe(false);
    });
  });

  describe("resetPagination", () => {
    it("should reset all state to initial values", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: true
        })
      );

      // Change all state
      act(() => {
        result.current.handlePaginationChange(5, 100);
      });

      act(() => {
        result.current.handleSortChange({
          sortBy: "email",
          isSortDescending: false
        });
      });

      // Verify changes
      expect(result.current.pageNumber).toBe(5);
      expect(result.current.pageSize).toBe(100);
      expect(result.current.sortParams.sortBy).toBe("email");
      expect(result.current.sortParams.isSortDescending).toBe(false);

      // Reset
      act(() => {
        result.current.resetPagination();
      });

      // Verify reset to initial values
      expect(result.current.pageNumber).toBe(0);
      expect(result.current.pageSize).toBe(25);
      expect(result.current.sortParams.sortBy).toBe("name");
      expect(result.current.sortParams.isSortDescending).toBe(true);
    });

    it("should reset to different initial values when reconfigured", () => {
      const { rerender, result } = renderHook(
        ({ config }) =>
          useGridPagination({
            initialPageSize: config.initialPageSize,
            initialSortBy: config.initialSortBy,
            initialSortDescending: config.initialSortDescending
          }),
        {
          initialProps: {
            config: {
              initialPageSize: 25,
              initialSortBy: "name",
              initialSortDescending: true
            }
          }
        }
      );

      act(() => {
        result.current.handlePaginationChange(5, 100);
      });

      // Rerender with new config
      rerender({
        config: {
          initialPageSize: 50,
          initialSortBy: "email",
          initialSortDescending: false
        }
      });

      act(() => {
        result.current.resetPagination();
      });

      // Should reset to NEW initial values
      expect(result.current.pageSize).toBe(50);
      expect(result.current.sortParams.sortBy).toBe("email");
      expect(result.current.sortParams.isSortDescending).toBe(false);
    });

    it("should not affect callback when resetting", async () => {
      const mockCallback = vi.fn();
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          onPaginationChange: mockCallback
        })
      );

      mockCallback.mockClear();

      act(() => {
        result.current.resetPagination();
      });

      // Reset does not call the pagination callback
      expect(mockCallback).not.toHaveBeenCalled();
    });
  });

  describe("sortParams memoization", () => {
    it("should return stable sortParams reference when values don't change", () => {
      const { result, rerender } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: true,
          onPaginationChange: () => {}
        })
      );

      const firstSortParams = result.current.sortParams;

      rerender();

      const secondSortParams = result.current.sortParams;

      // Should be the same reference
      expect(firstSortParams).toBe(secondSortParams);
    });

    it("should return new sortParams reference when values change", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: true
        })
      );

      const firstSortParams = result.current.sortParams;

      act(() => {
        result.current.handleSortChange({
          sortBy: "email",
          isSortDescending: false
        });
      });

      const secondSortParams = result.current.sortParams;

      // Should be different reference
      expect(firstSortParams).not.toBe(secondSortParams);
    });
  });

  describe("callback stability", () => {
    it("should update callback without requiring re-render", async () => {
      const callback1 = vi.fn();
      const callback2 = vi.fn();

      const { result, rerender } = renderHook(
        ({ callback }) =>
          useGridPagination({
            initialPageSize: 25,
            initialSortBy: "name",
            onPaginationChange: callback
          }),
        {
          initialProps: { callback: callback1 }
        }
      );

      act(() => {
        result.current.handlePaginationChange(1, 50);
      });

      await waitFor(() => {
        expect(callback1).toHaveBeenCalled();
      });

      // Switch callback
      rerender({ callback: callback2 });

      act(() => {
        result.current.handlePaginationChange(2, 50);
      });

      await waitFor(() => {
        expect(callback2).toHaveBeenCalled();
      });
    });
  });

  describe("edge cases", () => {
    it("should handle empty sortBy string", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: ""
        })
      );

      expect(result.current.sortParams.sortBy).toBe("");
    });

    it("should handle very large pageSize", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 10000,
          initialSortBy: "name"
        })
      );

      expect(result.current.pageSize).toBe(10000);
    });

    it("should handle pageSize of 1", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 1,
          initialSortBy: "name"
        })
      );

      expect(result.current.pageSize).toBe(1);
    });
  });
});
