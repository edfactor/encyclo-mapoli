import { renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useUnsavedChangesGuard } from "./useUnsavedChangesGuard";

describe("useUnsavedChangesGuard", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Mock window.confirm
    vi.spyOn(window, "confirm").mockReturnValue(true);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  describe("beforeunload event", () => {
    it("should add beforeunload listener when hasUnsavedChanges is true", () => {
      const addEventListenerSpy = vi.spyOn(window, "addEventListener");

      renderHook(() => useUnsavedChangesGuard(true));

      expect(addEventListenerSpy).toHaveBeenCalledWith("beforeunload", expect.any(Function));
    });

    it("should remove beforeunload listener on unmount", () => {
      const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");

      const { unmount } = renderHook(() => useUnsavedChangesGuard(true));

      unmount();

      expect(removeEventListenerSpy).toHaveBeenCalledWith("beforeunload", expect.any(Function));
    });

    it("should prevent default when hasUnsavedChanges is true", () => {
      renderHook(() => useUnsavedChangesGuard(true));

      const event = new Event("beforeunload", { cancelable: true }) as BeforeUnloadEvent;
      const preventDefaultSpy = vi.spyOn(event, "preventDefault");

      window.dispatchEvent(event);

      expect(preventDefaultSpy).toHaveBeenCalled();
    });

    it("should not prevent default when hasUnsavedChanges is false", () => {
      renderHook(() => useUnsavedChangesGuard(false));

      const event = new Event("beforeunload", { cancelable: true }) as BeforeUnloadEvent;
      const preventDefaultSpy = vi.spyOn(event, "preventDefault");

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });
  });

  describe("popstate event (back/forward navigation)", () => {
    it("should add popstate listener when hasUnsavedChanges is true", () => {
      const addEventListenerSpy = vi.spyOn(window, "addEventListener");

      renderHook(() => useUnsavedChangesGuard(true));

      expect(addEventListenerSpy).toHaveBeenCalledWith("popstate", expect.any(Function));
    });

    it("should remove popstate listener on unmount", () => {
      const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");

      const { unmount } = renderHook(() => useUnsavedChangesGuard(true));

      unmount();

      expect(removeEventListenerSpy).toHaveBeenCalledWith("popstate", expect.any(Function));
    });

    it("should show confirm dialog when user navigates back", () => {
      const confirmSpy = vi.spyOn(window, "confirm").mockReturnValue(false);

      renderHook(() => useUnsavedChangesGuard(true));

      const event = new PopStateEvent("popstate");
      window.dispatchEvent(event);

      expect(confirmSpy).toHaveBeenCalledWith(
        "Please save your changes. Do you want to leave without saving?"
      );
    });

    it("should push state to history when hasUnsavedChanges is true", () => {
      const pushStateSpy = vi.spyOn(window.history, "pushState");

      renderHook(() => useUnsavedChangesGuard(true));

      expect(pushStateSpy).toHaveBeenCalled();
    });
  });

  describe("click event (link navigation)", () => {
    it("should add click listener when hasUnsavedChanges is true", () => {
      const addEventListenerSpy = vi.spyOn(document, "addEventListener");

      renderHook(() => useUnsavedChangesGuard(true));

      expect(addEventListenerSpy).toHaveBeenCalledWith("click", expect.any(Function), true);
    });

    it("should remove click listener on unmount", () => {
      const removeEventListenerSpy = vi.spyOn(document, "removeEventListener");

      const { unmount } = renderHook(() => useUnsavedChangesGuard(true));

      unmount();

      expect(removeEventListenerSpy).toHaveBeenCalledWith("click", expect.any(Function), true);
    });

    it("should show confirm dialog when clicking on link with different href", () => {
      const confirmSpy = vi.spyOn(window, "confirm").mockReturnValue(false);

      renderHook(() => useUnsavedChangesGuard(true));

      const link = document.createElement("a");
      link.href = "/different-page";
      document.body.appendChild(link);

      const event = new MouseEvent("click", { bubbles: true, cancelable: true });
      Object.defineProperty(event, "target", { value: link, writable: false });

      link.dispatchEvent(event);

      expect(confirmSpy).toHaveBeenCalled();

      document.body.removeChild(link);
    });

    it("should not show confirm dialog when clicking on link with same href", () => {
      const confirmSpy = vi.spyOn(window, "confirm");

      // Set the current pathname
      Object.defineProperty(window, "location", {
        value: { pathname: "/current-page" },
        writable: true
      });

      renderHook(() => useUnsavedChangesGuard(true));

      const link = document.createElement("a");
      link.href = "/current-page";
      document.body.appendChild(link);

      const event = new MouseEvent("click", { bubbles: true, cancelable: true });
      Object.defineProperty(event, "target", { value: link, writable: false });

      link.dispatchEvent(event);

      expect(confirmSpy).not.toHaveBeenCalled();

      document.body.removeChild(link);
    });

    it("should not show confirm dialog when clicking on hash link", () => {
      const confirmSpy = vi.spyOn(window, "confirm");

      renderHook(() => useUnsavedChangesGuard(true));

      const link = document.createElement("a");
      link.href = "#";
      document.body.appendChild(link);

      const event = new MouseEvent("click", { bubbles: true, cancelable: true });
      Object.defineProperty(event, "target", { value: link, writable: false });

      link.dispatchEvent(event);

      expect(confirmSpy).not.toHaveBeenCalled();

      document.body.removeChild(link);
    });

    it("should prevent navigation when user cancels confirm dialog", () => {
      vi.spyOn(window, "confirm").mockReturnValue(false);

      renderHook(() => useUnsavedChangesGuard(true));

      const link = document.createElement("a");
      link.href = "/different-page";
      document.body.appendChild(link);

      const event = new MouseEvent("click", { bubbles: true, cancelable: true });
      const preventDefaultSpy = vi.spyOn(event, "preventDefault");
      const stopPropagationSpy = vi.spyOn(event, "stopPropagation");
      Object.defineProperty(event, "target", { value: link, writable: false });

      link.dispatchEvent(event);

      expect(preventDefaultSpy).toHaveBeenCalled();
      expect(stopPropagationSpy).toHaveBeenCalled();

      document.body.removeChild(link);
    });
  });

  describe("state transitions", () => {
    it("should add listeners when changing from false to true", () => {
      const addEventListenerSpy = vi.spyOn(window, "addEventListener");

      const { rerender } = renderHook(({ hasChanges }) => useUnsavedChangesGuard(hasChanges), {
        initialProps: { hasChanges: false }
      });

      const initialCallCount = addEventListenerSpy.mock.calls.length;

      rerender({ hasChanges: true });

      expect(addEventListenerSpy.mock.calls.length).toBeGreaterThan(initialCallCount);
    });

    it("should remove listeners when changing from true to false", () => {
      const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");

      const { rerender } = renderHook(({ hasChanges }) => useUnsavedChangesGuard(hasChanges), {
        initialProps: { hasChanges: true }
      });

      rerender({ hasChanges: false });

      expect(removeEventListenerSpy).toHaveBeenCalled();
    });
  });

  describe("button clicks", () => {
    it("should handle clicks on buttons with role attribute", () => {
      const confirmSpy = vi.spyOn(window, "confirm").mockReturnValue(false);

      renderHook(() => useUnsavedChangesGuard(true));

      const button = document.createElement("div");
      button.setAttribute("role", "button");
      button.setAttribute("href", "/different-page");
      document.body.appendChild(button);

      const event = new MouseEvent("click", { bubbles: true, cancelable: true });
      Object.defineProperty(event, "target", { value: button, writable: false });

      button.dispatchEvent(event);

      expect(confirmSpy).toHaveBeenCalled();

      document.body.removeChild(button);
    });

    it("should handle clicks on actual button elements", () => {
      const confirmSpy = vi.spyOn(window, "confirm").mockReturnValue(false);

      renderHook(() => useUnsavedChangesGuard(true));

      const button = document.createElement("button");
      button.setAttribute("href", "/different-page");
      document.body.appendChild(button);

      const event = new MouseEvent("click", { bubbles: true, cancelable: true });
      Object.defineProperty(event, "target", { value: button, writable: false });

      button.dispatchEvent(event);

      expect(confirmSpy).toHaveBeenCalled();

      document.body.removeChild(button);
    });
  });
});
