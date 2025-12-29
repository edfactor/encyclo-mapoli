import { renderHook } from "@testing-library/react";
import { act } from "react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { focusNextElement, onCellKeyDownEventHandler, useNumpadEnterAsTab } from "./useEnterAsTab";

describe("useNumpadEnterAsTab", () => {
  const STORAGE_KEY = "numpad-enter-as-tab-enabled";

  beforeEach(() => {
    // Enable the feature by default
    window.localStorage.setItem(STORAGE_KEY, "true");
    // Clean up DOM
    document.body.innerHTML = "";
  });

  afterEach(() => {
    window.localStorage.clear();
    vi.clearAllMocks();
  });

  describe("Feature Flag Control", () => {
    it("should not intercept NumpadEnter when feature flag is disabled", () => {
      window.localStorage.removeItem(STORAGE_KEY);

      const input = document.createElement("input");
      document.body.appendChild(input);
      input.focus();

      const preventDefaultSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter" });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });

    it("should intercept NumpadEnter when feature flag is enabled", () => {
      window.localStorage.setItem(STORAGE_KEY, "true");

      const input = document.createElement("input");
      document.body.appendChild(input);
      input.focus();

      const preventDefaultSpy = vi.fn();
      const stopPropagationSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter", bubbles: true });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "stopPropagation", { value: stopPropagationSpy });
      Object.defineProperty(event, "target", { value: input, configurable: true });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).toHaveBeenCalled();
      expect(stopPropagationSpy).toHaveBeenCalled();
    });

    it("should use custom storage key when provided", () => {
      const customKey = "custom-enter-key";
      window.localStorage.setItem(customKey, "true");
      window.localStorage.removeItem(STORAGE_KEY);

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: customKey }));

      const input = document.createElement("input");
      document.body.appendChild(input);
      input.focus();

      const preventDefaultSpy = vi.fn();
      const event = new KeyboardEvent("keydown", { code: "NumpadEnter" });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "target", { value: input, configurable: true });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).toHaveBeenCalled();
    });
  });

  describe("Key Code Filtering", () => {
    it("should only intercept NumpadEnter, not regular Enter", () => {
      const input = document.createElement("input");
      document.body.appendChild(input);
      input.focus();

      const preventDefaultSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "Enter", key: "Enter" });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });

    it("should not intercept other key codes", () => {
      const input = document.createElement("input");
      document.body.appendChild(input);
      input.focus();

      const preventDefaultSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const keyCodes = ["Tab", "Space", "Escape", "ArrowDown"];

      keyCodes.forEach((code) => {
        const event = new KeyboardEvent("keydown", { code });
        Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
        window.dispatchEvent(event);
      });

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });
  });

  describe("Special Element Handling", () => {
    it("should not prevent default on textarea elements", () => {
      const textarea = document.createElement("textarea");
      document.body.appendChild(textarea);
      textarea.focus();

      const preventDefaultSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter" });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "target", { value: textarea, configurable: true });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });

    it("should not prevent default on buttons when no modifiers", () => {
      const button = document.createElement("button");
      document.body.appendChild(button);
      button.focus();

      const preventDefaultSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter" });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "target", { value: button, configurable: true });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });

    it("should not prevent default on submit input when no modifiers", () => {
      const submit = document.createElement("input");
      submit.type = "submit";
      document.body.appendChild(submit);
      submit.focus();

      const preventDefaultSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter" });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "target", { value: submit, configurable: true });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });

    it("should not prevent default on anchor links when no modifiers", () => {
      const link = document.createElement("a");
      link.href = "https://example.com";
      document.body.appendChild(link);
      link.focus();

      const preventDefaultSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter" });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "target", { value: link, configurable: true });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });

    it("should not prevent default on AG Grid cells", () => {
      const cell = document.createElement("div");
      cell.className = "ag-cell";
      document.body.appendChild(cell);
      cell.focus();

      const preventDefaultSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter" });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "target", { value: cell, configurable: true });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });

    it("should not prevent default on AG Grid header cells", () => {
      const headerCell = document.createElement("div");
      headerCell.className = "ag-header-cell";
      document.body.appendChild(headerCell);
      headerCell.focus();

      const preventDefaultSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter" });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "target", { value: headerCell, configurable: true });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });
  });

  describe("Modifier Keys", () => {
    it("should handle Shift+NumpadEnter for backward tabbing", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      document.body.appendChild(input1);
      document.body.appendChild(input2);
      input2.focus();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter", shiftKey: true });
      const preventDefaultSpy = vi.fn();
      const stopPropagationSpy = vi.fn();

      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "stopPropagation", { value: stopPropagationSpy });
      Object.defineProperty(event, "target", { value: input2, configurable: true });

      window.dispatchEvent(event);

      expect(preventDefaultSpy).toHaveBeenCalled();
      expect(stopPropagationSpy).toHaveBeenCalled();
    });

    it("should trigger tab behavior with Alt key pressed on inputs", () => {
      const input = document.createElement("input");
      document.body.appendChild(input);
      input.focus();

      const preventDefaultSpy = vi.fn();
      const stopPropagationSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter", altKey: true });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "stopPropagation", { value: stopPropagationSpy });
      Object.defineProperty(event, "target", { value: input, configurable: true });

      window.dispatchEvent(event);

      // Alt key does trigger tab behavior for inputs (special case only applies to buttons/submit/anchors)
      expect(preventDefaultSpy).toHaveBeenCalled();
      expect(stopPropagationSpy).toHaveBeenCalled();
    });

    it("should prevent default on buttons with Alt key", () => {
      const button = document.createElement("button");
      document.body.appendChild(button);
      button.focus();

      const preventDefaultSpy = vi.fn();
      const stopPropagationSpy = vi.fn();

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      const event = new KeyboardEvent("keydown", { code: "NumpadEnter", altKey: true });
      Object.defineProperty(event, "preventDefault", { value: preventDefaultSpy });
      Object.defineProperty(event, "stopPropagation", { value: stopPropagationSpy });
      Object.defineProperty(event, "target", { value: button, configurable: true });

      window.dispatchEvent(event);

      // With Alt key, tab behavior is triggered (special case only when NEITHER Shift NOR Alt)
      expect(preventDefaultSpy).toHaveBeenCalled();
      expect(stopPropagationSpy).toHaveBeenCalled();
    });
  });

  describe("Event Listener Cleanup", () => {
    it("should remove event listener on unmount", () => {
      const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");

      const { unmount } = renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      unmount();

      expect(removeEventListenerSpy).toHaveBeenCalledWith("keydown", expect.any(Function), true);

      removeEventListenerSpy.mockRestore();
    });

    it("should add event listener on mount", () => {
      const addEventListenerSpy = vi.spyOn(window, "addEventListener");

      renderHook(() => useNumpadEnterAsTab({ enabledStorageKey: STORAGE_KEY }));

      expect(addEventListenerSpy).toHaveBeenCalledWith("keydown", expect.any(Function), true);

      addEventListenerSpy.mockRestore();
    });
  });
});

describe("focusNextElement", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
  });

  describe("Basic Focus Navigation", () => {
    it("should focus next element in DOM order", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      document.body.appendChild(input1);
      document.body.appendChild(input2);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1 });
      });

      expect(document.activeElement).toBe(input2);
    });

    it("should wrap to first element when at end", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      document.body.appendChild(input1);
      document.body.appendChild(input2);

      input2.focus();

      act(() => {
        focusNextElement({ activeElement: input2 });
      });

      expect(document.activeElement).toBe(input1);
    });

    it("should focus previous element when backwardTab is true", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      document.body.appendChild(input1);
      document.body.appendChild(input2);

      input2.focus();

      act(() => {
        focusNextElement({ activeElement: input2, backwardTab: true });
      });

      expect(document.activeElement).toBe(input1);
    });

    it("should wrap to last element when backward tabbing from first", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      document.body.appendChild(input1);
      document.body.appendChild(input2);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1, backwardTab: true });
      });

      expect(document.activeElement).toBe(input2);
    });
  });

  describe("Element Filtering", () => {
    it("should skip hidden elements", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      const input3 = document.createElement("input");

      input2.style.display = "none";

      document.body.appendChild(input1);
      document.body.appendChild(input2);
      document.body.appendChild(input3);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1 });
      });

      expect(document.activeElement).toBe(input3);
    });

    it("should skip elements with visibility hidden", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      const input3 = document.createElement("input");

      input2.style.visibility = "hidden";

      document.body.appendChild(input1);
      document.body.appendChild(input2);
      document.body.appendChild(input3);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1 });
      });

      expect(document.activeElement).toBe(input3);
    });

    it("should skip disabled elements", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      const input3 = document.createElement("input");

      input2.disabled = true;

      document.body.appendChild(input1);
      document.body.appendChild(input2);
      document.body.appendChild(input3);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1 });
      });

      expect(document.activeElement).toBe(input3);
    });

    it("should skip readonly elements", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      const input3 = document.createElement("input");

      input2.readOnly = true;

      document.body.appendChild(input1);
      document.body.appendChild(input2);
      document.body.appendChild(input3);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1 });
      });

      expect(document.activeElement).toBe(input3);
    });

    it("should skip elements with negative tabindex", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      const input3 = document.createElement("input");

      input2.tabIndex = -1;

      document.body.appendChild(input1);
      document.body.appendChild(input2);
      document.body.appendChild(input3);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1 });
      });

      expect(document.activeElement).toBe(input3);
    });

    it("should exclude AG Grid cells by default", () => {
      const input1 = document.createElement("input");
      const cell = document.createElement("div");
      cell.className = "ag-cell";
      cell.tabIndex = 0;
      const input2 = document.createElement("input");

      document.body.appendChild(input1);
      document.body.appendChild(cell);
      document.body.appendChild(input2);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1 });
      });

      expect(document.activeElement).toBe(input2);
    });

    it("should include AG Grid cells when excludeGridCells is false", () => {
      const input1 = document.createElement("input");
      const cell = document.createElement("div");
      cell.className = "ag-cell";
      cell.tabIndex = 0;
      const input2 = document.createElement("input");

      document.body.appendChild(input1);
      document.body.appendChild(cell);
      document.body.appendChild(input2);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1, excludeGridCells: false });
      });

      expect(document.activeElement).toBe(cell);
    });

    it("should skip elements with ag-hidden class", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      const input3 = document.createElement("input");

      input2.className = "ag-hidden";

      document.body.appendChild(input1);
      document.body.appendChild(input2);
      document.body.appendChild(input3);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1 });
      });

      expect(document.activeElement).toBe(input3);
    });
  });

  describe("TabIndex Ordering", () => {
    it("should respect positive tabindex ordering", () => {
      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      const input3 = document.createElement("input");

      input1.tabIndex = 3;
      input2.tabIndex = 1;
      input3.tabIndex = 2;

      document.body.appendChild(input1);
      document.body.appendChild(input2);
      document.body.appendChild(input3);

      input2.focus();

      act(() => {
        focusNextElement({ activeElement: input2 });
      });

      expect(document.activeElement).toBe(input3);
    });

    it("should limit to positive tabindex elements when present", () => {
      const input1 = document.createElement("input"); // tabIndex 0 (default)
      const input2 = document.createElement("input");
      const input3 = document.createElement("input");

      input2.tabIndex = 1;
      input3.tabIndex = 2;

      document.body.appendChild(input1);
      document.body.appendChild(input2);
      document.body.appendChild(input3);

      input2.focus();

      act(() => {
        focusNextElement({ activeElement: input2 });
      });

      // Should skip input1 (tabIndex 0) and wrap to input2
      expect(document.activeElement).toBe(input3);
    });
  });

  describe("Scoped Root", () => {
    it("should limit focus to elements within scopeRoot", () => {
      const container1 = document.createElement("div");
      const container2 = document.createElement("div");

      const input1 = document.createElement("input");
      const input2 = document.createElement("input");
      const input3 = document.createElement("input");

      container1.appendChild(input1);
      container1.appendChild(input2);
      container2.appendChild(input3);

      document.body.appendChild(container1);
      document.body.appendChild(container2);

      input1.focus();

      act(() => {
        focusNextElement({ activeElement: input1, scopeRoot: container1 });
      });

      expect(document.activeElement).toBe(input2);
    });
  });

  describe("Edge Cases", () => {
    it("should handle empty document", () => {
      act(() => {
        focusNextElement();
      });

      // Should not throw error
      expect(true).toBe(true);
    });

    it("should handle single element", () => {
      const input = document.createElement("input");
      document.body.appendChild(input);

      input.focus();

      act(() => {
        focusNextElement({ activeElement: input });
      });

      // Should wrap to itself
      expect(document.activeElement).toBe(input);
    });

    it("should handle activeElement not in focusable list", () => {
      const div = document.createElement("div");
      const input = document.createElement("input");

      document.body.appendChild(div);
      document.body.appendChild(input);

      act(() => {
        focusNextElement({ activeElement: div });
      });

      // Should focus first focusable element
      expect(document.activeElement).toBe(input);
    });
  });
});

describe("onCellKeyDownEventHandler", () => {
  const STORAGE_KEY = "numpad-enter-as-tab-enabled";

  beforeEach(() => {
    window.localStorage.setItem(STORAGE_KEY, "true");
  });

  afterEach(() => {
    window.localStorage.clear();
  });

  describe("Feature Flag Control", () => {
    it("should not handle event when feature flag is disabled", () => {
      window.localStorage.removeItem(STORAGE_KEY);

      const mockEvent = {
        event: new KeyboardEvent("keydown", { code: "NumpadEnter" }),
        api: {
          stopEditing: vi.fn(),
          tabToNextCell: vi.fn(),
          tabToPreviousCell: vi.fn()
        }
      };

      const preventDefaultSpy = vi.spyOn(mockEvent.event, "preventDefault");

      onCellKeyDownEventHandler(mockEvent as never);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
      expect(mockEvent.api.stopEditing).not.toHaveBeenCalled();
    });
  });

  describe("Key Handling", () => {
    it("should not handle non-NumpadEnter keys", () => {
      const mockEvent = {
        event: new KeyboardEvent("keydown", { code: "Enter" }),
        api: {
          stopEditing: vi.fn(),
          tabToNextCell: vi.fn()
        }
      };

      const preventDefaultSpy = vi.spyOn(mockEvent.event, "preventDefault");

      onCellKeyDownEventHandler(mockEvent as never);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });

    it("should stop editing on NumpadEnter", () => {
      const mockEvent = {
        event: new KeyboardEvent("keydown", { code: "NumpadEnter" }),
        api: {
          stopEditing: vi.fn(),
          tabToNextCell: vi.fn(),
          getPinnedTopRowCount: vi.fn().mockReturnValue(0),
          getPinnedBottomRowCount: vi.fn().mockReturnValue(0),
          getFocusedCell: vi.fn().mockReturnValue({ rowIndex: 0, column: { getColId: () => "col1" } }),
          getDisplayedRowCount: vi.fn().mockReturnValue(5),
          getAllDisplayedColumns: vi.fn().mockReturnValue([{ getColId: () => "col1" }, { getColId: () => "col2" }])
        },
        rowPinned: null
      };

      onCellKeyDownEventHandler(mockEvent as never);

      expect(mockEvent.api.stopEditing).toHaveBeenCalled();
    });

    it("should call tabToNextCell for forward navigation", () => {
      const mockEvent = {
        event: new KeyboardEvent("keydown", { code: "NumpadEnter" }),
        api: {
          stopEditing: vi.fn(),
          tabToNextCell: vi.fn(),
          getPinnedTopRowCount: vi.fn().mockReturnValue(0),
          getPinnedBottomRowCount: vi.fn().mockReturnValue(0),
          getFocusedCell: vi.fn().mockReturnValue({ rowIndex: 1, column: { getColId: () => "col1" } }),
          getDisplayedRowCount: vi.fn().mockReturnValue(5),
          getAllDisplayedColumns: vi.fn().mockReturnValue([{ getColId: () => "col1" }, { getColId: () => "col2" }])
        },
        rowPinned: null
      };

      onCellKeyDownEventHandler(mockEvent as never);

      expect(mockEvent.api.tabToNextCell).toHaveBeenCalled();
    });

    it("should call tabToPreviousCell for backward navigation with Shift", () => {
      const mockEvent = {
        event: new KeyboardEvent("keydown", { code: "NumpadEnter", shiftKey: true }),
        api: {
          stopEditing: vi.fn(),
          tabToPreviousCell: vi.fn(),
          getPinnedTopRowCount: vi.fn().mockReturnValue(0),
          getPinnedBottomRowCount: vi.fn().mockReturnValue(0),
          getFocusedCell: vi.fn().mockReturnValue({ rowIndex: 1, column: { getColId: () => "col1" } }),
          getDisplayedRowCount: vi.fn().mockReturnValue(5),
          getAllDisplayedColumns: vi.fn().mockReturnValue([{ getColId: () => "col1" }, { getColId: () => "col2" }])
        },
        rowPinned: null
      };

      onCellKeyDownEventHandler(mockEvent as never);

      expect(mockEvent.api.tabToPreviousCell).toHaveBeenCalled();
    });
  });
});
