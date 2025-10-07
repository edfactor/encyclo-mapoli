import { renderHook, waitFor } from "@testing-library/react";
import { act } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useGridPagination } from "./useGridPagination";

describe("useGridPagination", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should initialize with default values", () => {
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

  it("should initialize with custom sort direction", () => {
    const { result } = renderHook(() =>
      useGridPagination({
        initialPageSize: 50,
        initialSortBy: "date",
        initialSortDescending: false
      })
    );

    expect(result.current.sortParams.isSortDescending).toBe(false);
  });

  it("should default to descending sort when not specified", () => {
    const { result } = renderHook(() =>
      useGridPagination({
        initialPageSize: 25,
        initialSortBy: "name"
      })
    );

    expect(result.current.sortParams.isSortDescending).toBe(true);
  });

  describe("handlePaginationChange", () => {
    it("should update page number and size", () => {
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

    it("should call onPaginationChange callback with correct parameters", async () => {
      const mockCallback = vi.fn();
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: false,
          onPaginationChange: mockCallback
        })
      );

      act(() => {
        result.current.handlePaginationChange(1, 50);
      });

      await waitFor(
        () => {
          expect(mockCallback).toHaveBeenCalledWith(1, 50, {
            sortBy: "name",
            isSortDescending: false
          });
        },
        { timeout: 1000 }
      );
    });

    it("should prevent re-entrant calls", () => {
      const mockCallback = vi.fn();
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          onPaginationChange: mockCallback
        })
      );

      act(() => {
        result.current.handlePaginationChange(1, 25);
        result.current.handlePaginationChange(2, 25);
      });

      expect(result.current.pageNumber).toBe(1);
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

      act(() => {
        result.current.handleSortChange({
          sortBy: "date",
          isSortDescending: false
        });
      });

      expect(result.current.sortParams.sortBy).toBe("date");
      expect(result.current.sortParams.isSortDescending).toBe(false);
    });

    it("should call onPaginationChange callback when sort changes", () => {
      const mockCallback = vi.fn();
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          onPaginationChange: mockCallback
        })
      );

      act(() => {
        result.current.handleSortChange({
          sortBy: "date",
          isSortDescending: false
        });
      });

      expect(mockCallback).toHaveBeenCalledWith(
        0,
        25,
        expect.objectContaining({
          sortBy: "date",
          isSortDescending: false
        })
      );
    });
  });

  describe("resetPagination", () => {
    it("should reset to initial values", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: true
        })
      );

      act(() => {
        result.current.handlePaginationChange(3, 50);
        result.current.handleSortChange({
          sortBy: "date",
          isSortDescending: false
        });
      });

      act(() => {
        result.current.resetPagination();
      });

      expect(result.current.pageNumber).toBe(0);
      expect(result.current.pageSize).toBe(25);
      expect(result.current.sortParams.sortBy).toBe("name");
      expect(result.current.sortParams.isSortDescending).toBe(true);
    });
  });

  describe("callback stability", () => {
    it("should update callback ref when onPaginationChange changes", async () => {
      const mockCallback1 = vi.fn();
      const mockCallback2 = vi.fn();

      const { result, rerender } = renderHook(
        ({ callback }) =>
          useGridPagination({
            initialPageSize: 25,
            initialSortBy: "name",
            onPaginationChange: callback
          }),
        { initialProps: { callback: mockCallback1 } }
      );

      act(() => {
        result.current.handlePaginationChange(1, 25);
      });

      await waitFor(
        () => {
          expect(mockCallback1).toHaveBeenCalled();
        },
        { timeout: 1000 }
      );

      mockCallback1.mockClear();

      rerender({ callback: mockCallback2 });

      act(() => {
        result.current.handlePaginationChange(2, 25);
      });

      await waitFor(
        () => {
          expect(mockCallback1).not.toHaveBeenCalled();
          expect(mockCallback2).toHaveBeenCalled();
        },
        { timeout: 1000 }
      );
    });
  });

  describe("complex scenarios", () => {
    it("should maintain state through multiple operations", () => {
      const { result } = renderHook(() =>
        useGridPagination({
          initialPageSize: 25,
          initialSortBy: "name",
          initialSortDescending: true
        })
      );

      act(() => {
        result.current.handlePaginationChange(2, 25);
      });

      expect(result.current.pageNumber).toBe(2);

      act(() => {
        result.current.handleSortChange({
          sortBy: "date",
          isSortDescending: false
        });
      });

      expect(result.current.pageNumber).toBe(2);
      expect(result.current.sortParams.sortBy).toBe("date");

      act(() => {
        result.current.handlePaginationChange(0, 50);
      });

      expect(result.current.pageNumber).toBe(0);
      expect(result.current.pageSize).toBe(50);
      expect(result.current.sortParams.sortBy).toBe("date");
    });
  });
});
