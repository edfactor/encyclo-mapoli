import { act, renderHook } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { useTooltipState } from "./useTooltipState";

describe("useTooltipState", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("should initialize with isOpen as false", () => {
    const { result } = renderHook(() => useTooltipState());
    expect(result.current.isOpen).toBe(false);
  });

  it("should set isOpen to true when handleOpen is called", () => {
    const { result } = renderHook(() => useTooltipState());

    act(() => {
      result.current.handleOpen();
    });

    expect(result.current.isOpen).toBe(true);
  });

  it("should set isOpen to false after delay when handleClose is called", () => {
    const { result } = renderHook(() => useTooltipState());

    act(() => {
      result.current.handleOpen();
    });
    expect(result.current.isOpen).toBe(true);

    act(() => {
      result.current.handleClose();
    });
    // Still open immediately after calling close
    expect(result.current.isOpen).toBe(true);

    // Advance timers past the default 100ms delay
    act(() => {
      vi.advanceTimersByTime(100);
    });
    expect(result.current.isOpen).toBe(false);
  });

  it("should use custom closeDelay", () => {
    const customDelay = 200;
    const { result } = renderHook(() => useTooltipState(customDelay));

    act(() => {
      result.current.handleOpen();
    });

    act(() => {
      result.current.handleClose();
    });

    // Not closed after 100ms
    act(() => {
      vi.advanceTimersByTime(100);
    });
    expect(result.current.isOpen).toBe(true);

    // Closed after full 200ms
    act(() => {
      vi.advanceTimersByTime(100);
    });
    expect(result.current.isOpen).toBe(false);
  });

  it("should cancel close timeout when handleOpen is called before timeout completes", () => {
    const { result } = renderHook(() => useTooltipState());

    act(() => {
      result.current.handleOpen();
    });

    act(() => {
      result.current.handleClose();
    });

    // Re-open before timeout completes
    act(() => {
      vi.advanceTimersByTime(50);
    });

    act(() => {
      result.current.handleOpen();
    });

    // Advance past original timeout
    act(() => {
      vi.advanceTimersByTime(100);
    });

    // Should still be open because handleOpen cancelled the close
    expect(result.current.isOpen).toBe(true);
  });

  it("should cleanup timeout on unmount", () => {
    const clearTimeoutSpy = vi.spyOn(global, "clearTimeout");
    const { result, unmount } = renderHook(() => useTooltipState());

    act(() => {
      result.current.handleOpen();
    });

    act(() => {
      result.current.handleClose();
    });

    unmount();

    expect(clearTimeoutSpy).toHaveBeenCalled();
    clearTimeoutSpy.mockRestore();
  });
});
