import { renderHook, waitFor } from "@testing-library/react";
import { act } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useDynamicGridHeight } from "./useDynamicGridHeight";

describe("useDynamicGridHeight", () => {
  beforeEach(() => {
    // Reset window.innerHeight to default
    Object.defineProperty(window, "innerHeight", {
      writable: true,
      configurable: true,
      value: 1000
    });
  });

  it("should return default height based on 40% of window height", () => {
    const { result } = renderHook(() => useDynamicGridHeight());

    // 1000 * 0.4 = 400
    expect(result.current).toBe(400);
  });

  it("should respect custom height percentage", () => {
    const { result } = renderHook(() => useDynamicGridHeight({ heightPercentage: 0.5 }));

    // 1000 * 0.5 = 500
    expect(result.current).toBe(500);
  });

  it("should enforce minimum height constraint", () => {
    Object.defineProperty(window, "innerHeight", {
      writable: true,
      configurable: true,
      value: 500
    });

    const { result } = renderHook(() =>
      useDynamicGridHeight({
        heightPercentage: 0.4,
        minHeight: 300,
        maxHeight: 800
      })
    );

    // 500 * 0.4 = 200, but minHeight is 300
    expect(result.current).toBe(300);
  });

  it("should enforce maximum height constraint", () => {
    Object.defineProperty(window, "innerHeight", {
      writable: true,
      configurable: true,
      value: 3000
    });

    const { result } = renderHook(() =>
      useDynamicGridHeight({
        heightPercentage: 0.4,
        minHeight: 300,
        maxHeight: 800
      })
    );

    // 3000 * 0.4 = 1200, but maxHeight is 800
    expect(result.current).toBe(800);
  });

  it("should update height when window is resized", async () => {
    const { result } = renderHook(() => useDynamicGridHeight({ heightPercentage: 0.4 }));

    expect(result.current).toBe(400);

    // Simulate window resize
    act(() => {
      Object.defineProperty(window, "innerHeight", {
        writable: true,
        configurable: true,
        value: 1500
      });
      window.dispatchEvent(new Event("resize"));
    });

    await waitFor(() => {
      // 1500 * 0.4 = 600
      expect(result.current).toBe(600);
    });
  });

  it("should apply custom min and max bounds", () => {
    Object.defineProperty(window, "innerHeight", {
      writable: true,
      configurable: true,
      value: 2000
    });

    const { result } = renderHook(() =>
      useDynamicGridHeight({
        heightPercentage: 0.6,
        minHeight: 400,
        maxHeight: 1000
      })
    );

    // 2000 * 0.6 = 1200, but maxHeight is 1000
    expect(result.current).toBe(1000);
  });

  it("should handle very small window heights", () => {
    Object.defineProperty(window, "innerHeight", {
      writable: true,
      configurable: true,
      value: 200
    });

    const { result } = renderHook(() =>
      useDynamicGridHeight({
        heightPercentage: 0.4,
        minHeight: 300,
        maxHeight: 800
      })
    );

    // 200 * 0.4 = 80, but minHeight is 300
    expect(result.current).toBe(300);
  });

  it("should clean up resize listener on unmount", () => {
    const removeEventListenerSpy = vi.spyOn(window, "removeEventListener");

    const { unmount } = renderHook(() => useDynamicGridHeight());

    unmount();

    expect(removeEventListenerSpy).toHaveBeenCalledWith("resize", expect.any(Function));
  });

  it("should handle multiple resize events", async () => {
    const { result } = renderHook(() => useDynamicGridHeight({ heightPercentage: 0.4 }));

    expect(result.current).toBe(400);

    // First resize
    act(() => {
      Object.defineProperty(window, "innerHeight", {
        writable: true,
        configurable: true,
        value: 1200
      });
      window.dispatchEvent(new Event("resize"));
    });

    await waitFor(() => {
      expect(result.current).toBe(480);
    });

    // Second resize
    act(() => {
      Object.defineProperty(window, "innerHeight", {
        writable: true,
        configurable: true,
        value: 800
      });
      window.dispatchEvent(new Event("resize"));
    });

    await waitFor(() => {
      expect(result.current).toBe(320);
    });
  });

  it("should use default values when no options provided", () => {
    Object.defineProperty(window, "innerHeight", {
      writable: true,
      configurable: true,
      value: 1000
    });

    const { result } = renderHook(() => useDynamicGridHeight());

    // 1000 * 0.4 = 400 (within default bounds of 300-800)
    expect(result.current).toBe(400);
  });
});
