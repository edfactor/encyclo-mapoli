import { renderHook } from "@testing-library/react";
import { ReactNode } from "react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useUnsavedChangesGuard } from "./useUnsavedChangesGuard";

const wrapper = ({ children }: { children: ReactNode }) => <MemoryRouter>{children}</MemoryRouter>;

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

      renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      expect(addEventListenerSpy).toHaveBeenCalledWith("beforeunload", expect.any(Function));
    });

    it("should remove beforeunload listener on unmount", () => {
      const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");

      const { unmount } = renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      unmount();

      expect(removeEventListenerSpy).toHaveBeenCalledWith("beforeunload", expect.any(Function));
    });

    it("should prevent default when hasUnsavedChanges is true", () => {
      renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      const event = new Event("beforeunload", { cancelable: true }) as BeforeUnloadEvent;
      const preventDefaultSpy = vi.spyOn(event, "preventDefault");

      window.dispatchEvent(event);

      expect(preventDefaultSpy).toHaveBeenCalled();
    });

    it("should not prevent default when hasUnsavedChanges is false", () => {
      renderHook(() => useUnsavedChangesGuard(false), { wrapper });

      const event = new Event("beforeunload", { cancelable: true }) as BeforeUnloadEvent;
      const preventDefaultSpy = vi.spyOn(event, "preventDefault");

      window.dispatchEvent(event);

      expect(preventDefaultSpy).not.toHaveBeenCalled();
    });
  });

  describe("popstate event (back/forward navigation)", () => {
    it("should add popstate listener when hasUnsavedChanges is true", () => {
      const addEventListenerSpy = vi.spyOn(window, "addEventListener");

      renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      expect(addEventListenerSpy).toHaveBeenCalledWith("popstate", expect.any(Function));
    });

    it("should remove popstate listener on unmount", () => {
      const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");

      const { unmount } = renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      unmount();

      expect(removeEventListenerSpy).toHaveBeenCalledWith("popstate", expect.any(Function));
    });

    it("should show confirm dialog when user navigates back", () => {
      const confirmSpy = vi.spyOn(window, "confirm").mockReturnValue(false);

      renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      const event = new PopStateEvent("popstate");
      window.dispatchEvent(event);

      expect(confirmSpy).toHaveBeenCalledWith("Please save your changes. Do you want to leave without saving?");
    });

    it("should push state to history when hasUnsavedChanges is true", () => {
      const pushStateSpy = vi.spyOn(window.history, "pushState");

      renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      expect(pushStateSpy).toHaveBeenCalled();
    });
  });

  describe("styled dialog mode", () => {
    it("should return showDialog as false initially", () => {
      const { result } = renderHook(() => useUnsavedChangesGuard(true, true), { wrapper });

      expect(result.current.showDialog).toBe(false);
    });

    it("should provide onStay callback", () => {
      const { result } = renderHook(() => useUnsavedChangesGuard(true, true), { wrapper });

      expect(typeof result.current.onStay).toBe("function");
    });

    it("should provide onLeave callback", () => {
      const { result } = renderHook(() => useUnsavedChangesGuard(true, true), { wrapper });

      expect(typeof result.current.onLeave).toBe("function");
    });
  });

  describe("state transitions", () => {
    it("should add listeners when changing from false to true", () => {
      const addEventListenerSpy = vi.spyOn(window, "addEventListener");

      const { rerender } = renderHook(({ hasChanges }) => useUnsavedChangesGuard(hasChanges), {
        initialProps: { hasChanges: false },
        wrapper
      });

      const initialCallCount = addEventListenerSpy.mock.calls.length;

      rerender({ hasChanges: true });

      expect(addEventListenerSpy.mock.calls.length).toBeGreaterThan(initialCallCount);
    });

    it("should remove listeners when changing from true to false", () => {
      const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");

      const { rerender } = renderHook(({ hasChanges }) => useUnsavedChangesGuard(hasChanges), {
        initialProps: { hasChanges: true },
        wrapper
      });

      rerender({ hasChanges: false });

      expect(removeEventListenerSpy).toHaveBeenCalled();
    });
  });
});
