import { renderHook } from "@testing-library/react";
import { ReactNode } from "react";
import { createMemoryRouter, RouterProvider } from "react-router-dom";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { useUnsavedChangesGuard } from "./useUnsavedChangesGuard";

/**
 * Create a wrapper that provides a data router context for useBlocker.
 * React Router v7's useBlocker hook requires a data router (createMemoryRouter/createBrowserRouter),
 * not the legacy MemoryRouter.
 */
const createDataRouterWrapper = () => {
  return ({ children }: { children: ReactNode }) => {
    const testRouter = createMemoryRouter(
      [
        {
          path: "/",
          element: <>{children}</>
        },
        {
          path: "/other",
          element: <div>Other Page</div>
        }
      ],
      { initialEntries: ["/"] }
    );
    return <RouterProvider router={testRouter} />;
  };
};

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
      const wrapper = createDataRouterWrapper();

      renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      expect(addEventListenerSpy).toHaveBeenCalledWith("beforeunload", expect.any(Function));
    });

    it("should remove beforeunload listener on unmount", () => {
      const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");
      const wrapper = createDataRouterWrapper();

      const { unmount } = renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      unmount();

      expect(removeEventListenerSpy).toHaveBeenCalledWith("beforeunload", expect.any(Function));
    });

    it("should not add beforeunload listener when hasUnsavedChanges is false", () => {
      const addEventListenerSpy = vi.spyOn(window, "addEventListener");
      const wrapper = createDataRouterWrapper();

      renderHook(() => useUnsavedChangesGuard(false), { wrapper });

      // Check that beforeunload was NOT called (it's only added when hasUnsavedChanges is true)
      const beforeunloadCalls = addEventListenerSpy.mock.calls.filter((call) => call[0] === "beforeunload");
      expect(beforeunloadCalls.length).toBe(0);
    });
  });

  describe("styled dialog mode", () => {
    it("should return showDialog as false initially", () => {
      const wrapper = createDataRouterWrapper();
      const { result } = renderHook(() => useUnsavedChangesGuard(true, true), { wrapper });

      expect(result.current.showDialog).toBe(false);
    });

    it("should provide onStay callback", () => {
      const wrapper = createDataRouterWrapper();
      const { result } = renderHook(() => useUnsavedChangesGuard(true, true), { wrapper });

      expect(typeof result.current.onStay).toBe("function");
    });

    it("should provide onLeave callback", () => {
      const wrapper = createDataRouterWrapper();
      const { result } = renderHook(() => useUnsavedChangesGuard(true, true), { wrapper });

      expect(typeof result.current.onLeave).toBe("function");
    });
  });

  describe("hook return values", () => {
    it("should return correct interface when hasUnsavedChanges is false", () => {
      const wrapper = createDataRouterWrapper();
      const { result } = renderHook(() => useUnsavedChangesGuard(false), { wrapper });

      expect(result.current).toHaveProperty("showDialog");
      expect(result.current).toHaveProperty("onStay");
      expect(result.current).toHaveProperty("onLeave");
      expect(result.current.showDialog).toBe(false);
    });

    it("should return correct interface when hasUnsavedChanges is true", () => {
      const wrapper = createDataRouterWrapper();
      const { result } = renderHook(() => useUnsavedChangesGuard(true), { wrapper });

      expect(result.current).toHaveProperty("showDialog");
      expect(result.current).toHaveProperty("onStay");
      expect(result.current).toHaveProperty("onLeave");
    });
  });
});
