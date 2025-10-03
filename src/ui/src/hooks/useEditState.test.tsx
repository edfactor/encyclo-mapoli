import { renderHook } from "@testing-library/react";
import { act } from "react";
import { describe, expect, it } from "vitest";
import { useEditState } from "./useEditState";

describe("useEditState", () => {
  it("should initialize with empty state", () => {
    const { result } = renderHook(() => useEditState());

    expect(result.current.editedValues).toEqual({});
    expect(result.current.loadingRowIds.size).toBe(0);
    expect(result.current.hasAnyEdits).toBe(false);
  });

  describe("updateEditedValue", () => {
    it("should add a new edited value", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
      });

      expect(result.current.editedValues["row1"]).toEqual({ value: 100, hasError: false });
      expect(result.current.hasAnyEdits).toBe(true);
    });

    it("should update an existing edited value", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
      });

      act(() => {
        result.current.updateEditedValue("row1", 200, true);
      });

      expect(result.current.editedValues["row1"]).toEqual({ value: 200, hasError: true });
    });

    it("should handle multiple edited values", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
        result.current.updateEditedValue("row2", 200, false);
        result.current.updateEditedValue("row3", 300, true);
      });

      expect(Object.keys(result.current.editedValues)).toHaveLength(3);
      expect(result.current.editedValues["row1"]).toEqual({ value: 100, hasError: false });
      expect(result.current.editedValues["row2"]).toEqual({ value: 200, hasError: false });
      expect(result.current.editedValues["row3"]).toEqual({ value: 300, hasError: true });
    });
  });

  describe("removeEditedValue", () => {
    it("should remove a specific edited value", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
        result.current.updateEditedValue("row2", 200, false);
      });

      act(() => {
        result.current.removeEditedValue("row1");
      });

      expect(result.current.editedValues["row1"]).toBeUndefined();
      expect(result.current.editedValues["row2"]).toEqual({ value: 200, hasError: false });
    });

    it("should handle removing non-existent value", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.removeEditedValue("nonexistent");
      });

      expect(result.current.editedValues).toEqual({});
    });
  });

  describe("clearEditedValues", () => {
    it("should clear multiple specific edited values", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
        result.current.updateEditedValue("row2", 200, false);
        result.current.updateEditedValue("row3", 300, false);
      });

      act(() => {
        result.current.clearEditedValues(["row1", "row3"]);
      });

      expect(result.current.editedValues["row1"]).toBeUndefined();
      expect(result.current.editedValues["row3"]).toBeUndefined();
      expect(result.current.editedValues["row2"]).toEqual({ value: 200, hasError: false });
    });

    it("should handle clearing with empty array", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
      });

      act(() => {
        result.current.clearEditedValues([]);
      });

      expect(result.current.editedValues["row1"]).toEqual({ value: 100, hasError: false });
    });
  });

  describe("clearAllEditedValues", () => {
    it("should clear all edited values", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
        result.current.updateEditedValue("row2", 200, false);
        result.current.updateEditedValue("row3", 300, false);
      });

      act(() => {
        result.current.clearAllEditedValues();
      });

      expect(result.current.editedValues).toEqual({});
      expect(result.current.hasAnyEdits).toBe(false);
    });
  });

  describe("getEditedValue", () => {
    it("should return the edited value when it exists", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
      });

      expect(result.current.getEditedValue("row1")).toBe(100);
    });

    it("should return undefined when value does not exist and no fallback provided", () => {
      const { result } = renderHook(() => useEditState());

      expect(result.current.getEditedValue("row1")).toBeUndefined();
    });

    it("should return fallback value when value does not exist", () => {
      const { result } = renderHook(() => useEditState());

      expect(result.current.getEditedValue("row1", 999)).toBe(999);
    });
  });

  describe("hasEditedValue", () => {
    it("should return true when value exists", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
      });

      expect(result.current.hasEditedValue("row1")).toBe(true);
    });

    it("should return false when value does not exist", () => {
      const { result } = renderHook(() => useEditState());

      expect(result.current.hasEditedValue("row1")).toBe(false);
    });
  });

  describe("loading rows", () => {
    it("should add a single loading row", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.addLoadingRow(1);
      });

      expect(result.current.loadingRowIds.has(1)).toBe(true);
      expect(result.current.isRowLoading(1)).toBe(true);
    });

    it("should remove a single loading row", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.addLoadingRow(1);
      });

      act(() => {
        result.current.removeLoadingRow(1);
      });

      expect(result.current.loadingRowIds.has(1)).toBe(false);
      expect(result.current.isRowLoading(1)).toBe(false);
    });

    it("should add multiple loading rows", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.addLoadingRows([1, 2, 3]);
      });

      expect(result.current.loadingRowIds.has(1)).toBe(true);
      expect(result.current.loadingRowIds.has(2)).toBe(true);
      expect(result.current.loadingRowIds.has(3)).toBe(true);
      expect(result.current.loadingRowIds.size).toBe(3);
    });

    it("should remove multiple loading rows", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.addLoadingRows([1, 2, 3, 4]);
      });

      act(() => {
        result.current.removeLoadingRows([1, 3]);
      });

      expect(result.current.loadingRowIds.has(1)).toBe(false);
      expect(result.current.loadingRowIds.has(2)).toBe(true);
      expect(result.current.loadingRowIds.has(3)).toBe(false);
      expect(result.current.loadingRowIds.has(4)).toBe(true);
      expect(result.current.loadingRowIds.size).toBe(2);
    });

    it("should handle adding duplicate loading rows", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.addLoadingRow(1);
        result.current.addLoadingRow(1);
      });

      expect(result.current.loadingRowIds.size).toBe(1);
    });
  });

  describe("isRowLoading", () => {
    it("should return true when row is loading", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.addLoadingRow(1);
      });

      expect(result.current.isRowLoading(1)).toBe(true);
    });

    it("should return false when row is not loading", () => {
      const { result } = renderHook(() => useEditState());

      expect(result.current.isRowLoading(1)).toBe(false);
    });
  });

  describe("hasAnyEdits", () => {
    it("should return true when there are edited values", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
      });

      expect(result.current.hasAnyEdits).toBe(true);
    });

    it("should return false when there are no edited values", () => {
      const { result } = renderHook(() => useEditState());

      expect(result.current.hasAnyEdits).toBe(false);
    });

    it("should update when all values are cleared", () => {
      const { result } = renderHook(() => useEditState());

      act(() => {
        result.current.updateEditedValue("row1", 100, false);
      });

      expect(result.current.hasAnyEdits).toBe(true);

      act(() => {
        result.current.clearAllEditedValues();
      });

      expect(result.current.hasAnyEdits).toBe(false);
    });
  });
});
