import { renderHook, waitFor } from "@testing-library/react";
import { act } from "react";
import { beforeEach, describe, expect, it } from "vitest";
import { useContentAwareGridHeight } from "./useContentAwareGridHeight";

describe("useContentAwareGridHeight", () => {
  beforeEach(() => {
    // Reset window.innerHeight to default
    Object.defineProperty(window, "innerHeight", {
      writable: true,
      configurable: true,
      value: 1000
    });
  });

  describe("with few rows (content smaller than viewport)", () => {
    it("should return height based on content when rows are few", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 3,
          rowHeight: 41,
          headerHeight: 41,
          chromeHeight: 10
        })
      );

      // Expected: headerHeight (41) + 3 rows * 41 + chromeHeight (10) = 174
      // minHeight is 100, so content height (174) is used
      expect(result.current).toBe(174);
    });

    it("should shrink to content height when it exceeds minimum but is less than viewport max", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 6,
          rowHeight: 41,
          headerHeight: 41,
          chromeHeight: 10
        })
      );

      // Expected: 41 + 6 * 41 + 10 = 297 (above minHeight of 100)
      expect(result.current).toBe(297);
    });

    it("should respect minimum height when content is very small", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 1,
          rowHeight: 41,
          headerHeight: 41,
          chromeHeight: 10,
          minHeight: 200
        })
      );

      // Expected: 41 + 1 * 41 + 10 = 92, but minHeight is 200
      expect(result.current).toBe(200);
    });
  });

  describe("with many rows (content larger than viewport)", () => {
    it("should use viewport-based max height when content exceeds it", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 50,
          rowHeight: 41,
          headerHeight: 41,
          chromeHeight: 20,
          heightPercentage: 0.4
        })
      );

      // Content: 41 + 50 * 41 + 20 = 2111
      // Viewport max: 1000 * 0.4 = 400
      // Should use viewport max since content exceeds it
      expect(result.current).toBe(400);
    });

    it("should cap at maxHeight option", () => {
      Object.defineProperty(window, "innerHeight", {
        writable: true,
        configurable: true,
        value: 3000
      });

      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 100,
          rowHeight: 41,
          maxHeight: 600
        })
      );

      // Viewport calculation would be high, but maxHeight caps it
      expect(result.current).toBeLessThanOrEqual(600);
    });
  });

  describe("with zero rows", () => {
    it("should return undefined for auto-height when there are no rows", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 0
        })
      );

      // Zero rows should return undefined to allow "No records found" to display properly
      expect(result.current).toBeUndefined();
    });
  });

  describe("custom configuration", () => {
    it("should respect custom row height", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 5,
          rowHeight: 60,
          headerHeight: 41,
          chromeHeight: 10
        })
      );

      // Expected: 41 + 5 * 60 + 10 = 351
      expect(result.current).toBe(351);
    });

    it("should respect custom header height", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 5,
          rowHeight: 41,
          headerHeight: 60,
          chromeHeight: 10
        })
      );

      // Expected: 60 + 5 * 41 + 10 = 275
      expect(result.current).toBe(275);
    });

    it("should respect custom chrome height", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 5,
          rowHeight: 41,
          headerHeight: 41,
          chromeHeight: 50
        })
      );

      // Expected: 41 + 5 * 41 + 50 = 296
      expect(result.current).toBe(296);
    });

    it("should use default values when not specified", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 5
        })
      );

      // Default: rowHeight=41, headerHeight=41, chromeHeight=10
      // Expected: 41 + 5 * 41 + 10 = 256
      expect(result.current).toBe(256);
    });
  });

  describe("window resize behavior", () => {
    it("should update height when window is resized and content exceeds viewport", async () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 50,
          heightPercentage: 0.4
        })
      );

      // Initial: 1000 * 0.4 = 400
      expect(result.current).toBe(400);

      // Resize window
      act(() => {
        Object.defineProperty(window, "innerHeight", {
          writable: true,
          configurable: true,
          value: 1500
        });
        window.dispatchEvent(new Event("resize"));
      });

      await waitFor(() => {
        // New: 1500 * 0.4 = 600
        expect(result.current).toBe(600);
      });
    });

    it("should not change content-based height on resize when content is small", async () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 5,
          heightPercentage: 0.4
        })
      );

      // Content: 41 + 5 * 41 + 10 = 256
      expect(result.current).toBe(256);

      // Resize window (but content still fits)
      act(() => {
        Object.defineProperty(window, "innerHeight", {
          writable: true,
          configurable: true,
          value: 1500
        });
        window.dispatchEvent(new Event("resize"));
      });

      await waitFor(() => {
        // Content height unchanged since it still fits
        expect(result.current).toBe(256);
      });
    });
  });

  describe("edge cases", () => {
    it("should handle exactly viewport-sized content", () => {
      // Set up so content exactly equals viewport max
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 8, // 41 + 8*41 + 10 = 379
          rowHeight: 41,
          headerHeight: 41,
          chromeHeight: 10,
          heightPercentage: 0.4 // 1000 * 0.4 = 400
        })
      );

      // Content (379) < viewport max (400), so use content height
      expect(result.current).toBe(379);
    });

    it("should handle single row", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 1,
          minHeight: 100
        })
      );

      // 41 + 1 * 41 + 10 = 92, below minHeight of 100
      expect(result.current).toBe(100);
    });

    it("should handle very large row counts gracefully", () => {
      const { result } = renderHook(() =>
        useContentAwareGridHeight({
          rowCount: 10000,
          maxHeight: 500
        })
      );

      // Should be capped by viewport/maxHeight constraints
      expect(result.current).toBeLessThanOrEqual(500);
    });
  });
});
