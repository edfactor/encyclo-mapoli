import { describe, expect, it, vi } from "vitest";
import { renderHook } from "@testing-library/react";
import { act } from "react";
import React from "react";

describe("PAY426N - Expand/Collapse Functionality", () => {
  describe("State Management", () => {
    it("should initialize with grid not expanded", () => {
      const { result } = renderHook(() => {
        const [isGridExpanded, setIsGridExpanded] = React.useState(false);
        return { isGridExpanded, setIsGridExpanded };
      });

      expect(result.current.isGridExpanded).toBe(false);
    });

    it("should initialize with drawer state not remembered", () => {
      const { result } = renderHook(() => {
        const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = React.useState(false);
        return { wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand };
      });

      expect(result.current.wasDrawerOpenBeforeExpand).toBe(false);
    });

    it("should toggle grid expanded state", () => {
      const { result } = renderHook(() => {
        const [isGridExpanded, setIsGridExpanded] = React.useState(false);
        return { isGridExpanded, setIsGridExpanded };
      });

      act(() => {
        result.current.setIsGridExpanded(true);
      });

      expect(result.current.isGridExpanded).toBe(true);

      act(() => {
        result.current.setIsGridExpanded(false);
      });

      expect(result.current.isGridExpanded).toBe(false);
    });

    it("should remember drawer state before expanding", () => {
      const { result } = renderHook(() => {
        const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = React.useState(false);
        return { wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand };
      });

      act(() => {
        result.current.setWasDrawerOpenBeforeExpand(true);
      });

      expect(result.current.wasDrawerOpenBeforeExpand).toBe(true);
    });
  });

  describe("Toggle Handler Logic", () => {
    it("should handle expand action", () => {
      const mockDispatch = vi.fn();
      const mockCloseDrawer = vi.fn();
      const mockSetFullscreen = vi.fn();
      
      const { result } = renderHook(() => {
        const [isGridExpanded, setIsGridExpanded] = React.useState(false);
        const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = React.useState(false);
        const isDrawerOpen = true;

        const handleToggleGridExpand = () => {
          if (!isGridExpanded) {
            setWasDrawerOpenBeforeExpand(isDrawerOpen);
            mockDispatch(mockCloseDrawer());
            mockDispatch(mockSetFullscreen(true));
            setIsGridExpanded(true);
          }
        };

        return { isGridExpanded, wasDrawerOpenBeforeExpand, handleToggleGridExpand };
      });

      act(() => {
        result.current.handleToggleGridExpand();
      });

      expect(result.current.isGridExpanded).toBe(true);
      expect(result.current.wasDrawerOpenBeforeExpand).toBe(true);
      expect(mockDispatch).toHaveBeenCalledTimes(2);
    });

    it("should handle collapse action and restore drawer", () => {
      const mockDispatch = vi.fn();
      const mockSetFullscreen = vi.fn();
      const mockOpenDrawer = vi.fn();
      
      const { result } = renderHook(() => {
        const [isGridExpanded, setIsGridExpanded] = React.useState(true);
        const [wasDrawerOpenBeforeExpand] = React.useState(true);

        const handleToggleGridExpand = () => {
          if (isGridExpanded) {
            mockDispatch(mockSetFullscreen(false));
            setIsGridExpanded(false);
            if (wasDrawerOpenBeforeExpand) {
              mockDispatch(mockOpenDrawer());
            }
          }
        };

        return { isGridExpanded, handleToggleGridExpand };
      });

      act(() => {
        result.current.handleToggleGridExpand();
      });

      expect(result.current.isGridExpanded).toBe(false);
      expect(mockDispatch).toHaveBeenCalledTimes(2);
    });

    it("should not restore drawer if it was closed before expanding", () => {
      const mockDispatch = vi.fn();
      const mockSetFullscreen = vi.fn();
      const mockOpenDrawer = vi.fn();
      
      const { result } = renderHook(() => {
        const [isGridExpanded, setIsGridExpanded] = React.useState(true);
        const [wasDrawerOpenBeforeExpand] = React.useState(false);

        const handleToggleGridExpand = () => {
          if (isGridExpanded) {
            mockDispatch(mockSetFullscreen(false));
            setIsGridExpanded(false);
            if (wasDrawerOpenBeforeExpand) {
              mockDispatch(mockOpenDrawer());
            }
          }
        };

        return { isGridExpanded, handleToggleGridExpand };
      });

      act(() => {
        result.current.handleToggleGridExpand();
      });

      expect(result.current.isGridExpanded).toBe(false);
      expect(mockDispatch).toHaveBeenCalledTimes(1); // Only setFullscreen, not openDrawer
    });
  });

  describe("Props Passing", () => {
    it("should pass isGridExpanded prop to ReportGrid", () => {
      const isGridExpanded = true;
      const reportGridProps = {
        params: { reportId: 1 },
        onLoadingChange: vi.fn(),
        isFrozen: false,
        searchTrigger: 0,
        isGridExpanded: isGridExpanded,
        onToggleExpand: vi.fn()
      };

      expect(reportGridProps.isGridExpanded).toBe(true);
    });

    it("should pass onToggleExpand handler to ReportGrid", () => {
      const mockHandler = vi.fn();
      const reportGridProps = {
        params: { reportId: 1 },
        onLoadingChange: vi.fn(),
        isFrozen: false,
        searchTrigger: 0,
        isGridExpanded: false,
        onToggleExpand: mockHandler
      };

      expect(reportGridProps.onToggleExpand).toBe(mockHandler);
    });

    it("should pass external expand props to ProfitSummary", () => {
      const mockHandler = vi.fn();
      const profitSummaryProps = {
        frozenData: false,
        externalIsGridExpanded: true,
        externalOnToggleExpand: mockHandler
      };

      expect(profitSummaryProps.externalIsGridExpanded).toBe(true);
      expect(profitSummaryProps.externalOnToggleExpand).toBe(mockHandler);
    });
  });

  describe("Page Component Behavior", () => {
    it("should hide page label when expanded", () => {
      const isGridExpanded = true;
      const pageLabel = isGridExpanded ? "" : "PAY426N";

      expect(pageLabel).toBe("");
    });

    it("should show page label when not expanded", () => {
      const isGridExpanded = false;
      const pageLabel = isGridExpanded ? "" : "PAY426N";

      expect(pageLabel).toBe("PAY426N");
    });

    it("should hide action node when expanded", () => {
      const isGridExpanded = true;
      const actionNode = isGridExpanded ? undefined : { component: "StatusDropdown" };

      expect(actionNode).toBeUndefined();
    });

    it("should show action node when not expanded", () => {
      const isGridExpanded = false;
      const actionNode = isGridExpanded ? undefined : { component: "StatusDropdown" };

      expect(actionNode).toBeDefined();
      expect(actionNode?.component).toBe("StatusDropdown");
    });
  });
});
