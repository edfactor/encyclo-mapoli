import { renderHook } from "@testing-library/react";
import { act } from "react";
import { describe, expect, it } from "vitest";
import { useRowSelection } from "./useRowSelection";

describe("useRowSelection", () => {
  it("should initialize with empty selection", () => {
    const { result } = renderHook(() => useRowSelection());

    expect(result.current.selectedRowIds).toEqual([]);
    expect(result.current.hasSelectedRows).toBe(false);
  });

  describe("addRowToSelection", () => {
    it("should add a single row to selection", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
      });

      expect(result.current.selectedRowIds).toEqual([1]);
      expect(result.current.hasSelectedRows).toBe(true);
    });

    it("should add multiple rows to selection", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(2);
        result.current.addRowToSelection(3);
      });

      expect(result.current.selectedRowIds).toEqual([1, 2, 3]);
      expect(result.current.hasSelectedRows).toBe(true);
    });

    it("should allow duplicate row IDs", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(1);
      });

      expect(result.current.selectedRowIds).toEqual([1, 1]);
    });
  });

  describe("removeRowFromSelection", () => {
    it("should remove a row from selection", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(2);
        result.current.addRowToSelection(3);
      });

      act(() => {
        result.current.removeRowFromSelection(2);
      });

      expect(result.current.selectedRowIds).toEqual([1, 3]);
    });

    it("should handle removing non-existent row", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
      });

      act(() => {
        result.current.removeRowFromSelection(999);
      });

      expect(result.current.selectedRowIds).toEqual([1]);
    });

    it("should remove all instances of a duplicate row ID", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(2);
      });

      act(() => {
        result.current.removeRowFromSelection(1);
      });

      expect(result.current.selectedRowIds).toEqual([2]);
    });

    it("should update hasSelectedRows when last row is removed", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
      });

      expect(result.current.hasSelectedRows).toBe(true);

      act(() => {
        result.current.removeRowFromSelection(1);
      });

      expect(result.current.hasSelectedRows).toBe(false);
    });
  });

  describe("clearSelection", () => {
    it("should clear all selected rows", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(2);
        result.current.addRowToSelection(3);
      });

      act(() => {
        result.current.clearSelection();
      });

      expect(result.current.selectedRowIds).toEqual([]);
      expect(result.current.hasSelectedRows).toBe(false);
    });

    it("should handle clearing when selection is already empty", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.clearSelection();
      });

      expect(result.current.selectedRowIds).toEqual([]);
      expect(result.current.hasSelectedRows).toBe(false);
    });
  });

  describe("isRowSelected", () => {
    it("should return true when row is selected", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(2);
      });

      expect(result.current.isRowSelected(1)).toBe(true);
      expect(result.current.isRowSelected(2)).toBe(true);
    });

    it("should return false when row is not selected", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
      });

      expect(result.current.isRowSelected(2)).toBe(false);
      expect(result.current.isRowSelected(999)).toBe(false);
    });

    it("should update after removing a row", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
      });

      expect(result.current.isRowSelected(1)).toBe(true);

      act(() => {
        result.current.removeRowFromSelection(1);
      });

      expect(result.current.isRowSelected(1)).toBe(false);
    });
  });

  describe("hasSelectedRows", () => {
    it("should return true when rows are selected", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
      });

      expect(result.current.hasSelectedRows).toBe(true);
    });

    it("should return false when no rows are selected", () => {
      const { result } = renderHook(() => useRowSelection());

      expect(result.current.hasSelectedRows).toBe(false);
    });

    it("should update when transitioning from empty to selected", () => {
      const { result } = renderHook(() => useRowSelection());

      expect(result.current.hasSelectedRows).toBe(false);

      act(() => {
        result.current.addRowToSelection(1);
      });

      expect(result.current.hasSelectedRows).toBe(true);
    });

    it("should update when clearing selection", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(2);
      });

      expect(result.current.hasSelectedRows).toBe(true);

      act(() => {
        result.current.clearSelection();
      });

      expect(result.current.hasSelectedRows).toBe(false);
    });
  });

  describe("complex scenarios", () => {
    it("should handle adding, removing, and clearing in sequence", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(2);
        result.current.addRowToSelection(3);
      });

      expect(result.current.selectedRowIds).toEqual([1, 2, 3]);

      act(() => {
        result.current.removeRowFromSelection(2);
      });

      expect(result.current.selectedRowIds).toEqual([1, 3]);

      act(() => {
        result.current.addRowToSelection(4);
      });

      expect(result.current.selectedRowIds).toEqual([1, 3, 4]);

      act(() => {
        result.current.clearSelection();
      });

      expect(result.current.selectedRowIds).toEqual([]);
      expect(result.current.hasSelectedRows).toBe(false);
    });

    it("should maintain selection order", () => {
      const { result } = renderHook(() => useRowSelection());

      act(() => {
        result.current.addRowToSelection(3);
        result.current.addRowToSelection(1);
        result.current.addRowToSelection(2);
      });

      expect(result.current.selectedRowIds).toEqual([3, 1, 2]);
    });
  });
});
