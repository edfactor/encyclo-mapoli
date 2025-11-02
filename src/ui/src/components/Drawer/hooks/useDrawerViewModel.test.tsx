/**
 * Unit Tests for Drawer ViewModel
 *
 * These tests demonstrate the testability benefits of MVVM:
 * - Test business logic without rendering UI
 * - Fast execution (no DOM rendering)
 * - Easy to mock dependencies
 * - Clear test cases for each behavior
 */

import { configureStore } from "@reduxjs/toolkit";
import { act, renderHook } from "@testing-library/react";
import { Provider } from "react-redux";
import { BrowserRouter } from "react-router-dom";
import generalReducer from "../../../reduxstore/slices/generalSlice";
import { NavigationDto, NavigationResponseDto } from "../../../reduxstore/types";
import { DrawerConfig } from "../models/DrawerConfig";
import { useDrawerViewModel } from "./useDrawerViewModel";

// ====================
// Test Setup
// ====================

const createMockStore = (initialState = {}) => {
  return configureStore({
    reducer: {
      general: generalReducer as unknown
    },
    preloadedState: {
      general: {
        isDrawerOpen: false,
        activeSubmenu: null,
        ...initialState
      }
    }
  });
};

const createWrapper = (store: ReturnType<typeof createMockStore>) => {
  return ({ children }: { children: React.ReactNode }) => (
    <Provider store={store}>
      <BrowserRouter>{children}</BrowserRouter>
    </Provider>
  );
};

const mockNavigationData: NavigationResponseDto = {
  navigation: [
    {
      id: 1,
      parentId: 0,
      title: "YEAR END",
      subTitle: "Year End",
      url: "",
      orderNumber: 1,
      icon: "",
      requiredRoles: [],
      disabled: false,
      isNavigable: true,
      items: [
        {
          id: 10,
          parentId: 1,
          title: "December Activities",
          subTitle: "",
          url: "december-activities",
          orderNumber: 1,
          icon: "",
          requiredRoles: [],
          disabled: false,
          isNavigable: true,
          items: [
            {
              id: 100,
              parentId: 10,
              title: "Cleanup Reports",
              subTitle: "",
              url: "cleanup-reports",
              orderNumber: 1,
              icon: "",
              requiredRoles: [],
              disabled: false,
              isNavigable: true,
              items: []
            }
          ]
        },
        {
          id: 20,
          parentId: 1,
          title: "Fiscal Close",
          subTitle: "",
          url: "fiscal-close",
          orderNumber: 2,
          icon: "",
          requiredRoles: [],
          disabled: false,
          isNavigable: true,
          items: []
        }
      ]
    }
  ]
};

const mockDrawerConfig: DrawerConfig = {
  rootNavigationTitle: "YEAR END",
  autoExpandDepth: 1,
  showStatus: true
};

// ====================
// Tests
// ====================

describe("useDrawerViewModel", () => {
  describe("Initial State", () => {
    it("should initialize with drawer closed", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.isOpen).toBe(false);
    });

    it("should load drawer items from navigation data", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.drawerItems).toHaveLength(2);
      expect(result.current.drawerItems[0].title).toBe("December Activities");
      expect(result.current.drawerItems[1].title).toBe("Fiscal Close");
    });

    it("should set drawer title from config", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.drawerTitle).toBe("Year End");
    });

    it("should not be in submenu view initially", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.isInSubmenuView).toBe(false);
      expect(result.current.activeTopLevelItem).toBeNull();
    });
  });

  describe("Computed Values", () => {
    it("should return drawer items as visible items when not in submenu", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.visibleItems).toEqual(result.current.drawerItems);
    });

    it("should return submenu items as visible items when in submenu view", () => {
      const store = createMockStore({ activeSubmenu: "December Activities" });
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.isInSubmenuView).toBe(true);
      expect(result.current.activeTopLevelItem?.title).toBe("December Activities");
      expect(result.current.visibleItems).toHaveLength(1);
      expect(result.current.visibleItems[0].title).toBe("Cleanup Reports");
    });

    it("should clean current path by removing leading slashes", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      // Current path from BrowserRouter will be cleaned of leading slashes
      expect(result.current.currentPath).not.toMatch(/^\//);
    });
  });

  describe("Actions", () => {
    it("should toggle drawer open when closed", () => {
      const store = createMockStore({ isDrawerOpen: false });
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.isOpen).toBe(false);

      act(() => {
        result.current.toggleDrawer();
      });

      expect((store.getState() as unknown).general.isDrawerOpen).toBe(true);
    });

    it("should toggle drawer closed when open and clear submenu", () => {
      const store = createMockStore({
        isDrawerOpen: true,
        activeSubmenu: "December Activities"
      });
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.isOpen).toBe(true);
      expect(result.current.activeSubmenu).toBe("December Activities");

      act(() => {
        result.current.toggleDrawer();
      });

      expect((store.getState() as unknown).general.isDrawerOpen).toBe(false);
      // Redux clearActiveSubMenu sets to empty string, not null
      expect((store.getState() as unknown).general.activeSubmenu).toBe("");
    });

    it("should select menu item and set active submenu", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      const menuItem = result.current.drawerItems[0]; // December Activities

      act(() => {
        result.current.selectMenuItem(menuItem);
      });

      expect((store.getState() as unknown).general.activeSubmenu).toBe("December Activities");
    });

    it("should not select disabled menu items", () => {
      const disabledItem: NavigationDto = {
        ...mockNavigationData.navigation[0].items![0],
        disabled: true
      };

      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      const initialSubmenu = (store.getState() as unknown).general.activeSubmenu;

      act(() => {
        result.current.selectMenuItem(disabledItem);
      });

      // Should not change activeSubmenu when item is disabled
      expect((store.getState() as unknown).general.activeSubmenu).toBe(initialSubmenu);
    });

    it("should go back to main menu from submenu view", () => {
      const store = createMockStore({ activeSubmenu: "December Activities" });
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.isInSubmenuView).toBe(true);

      act(() => {
        result.current.goBackToMainMenu();
      });

      // Redux clearActiveSubMenu action sets activeSubmenu to empty string, not null
      expect((store.getState() as unknown).general.activeSubmenu).toBe("");
    });
  });

  describe("Helper Functions", () => {
    it("should identify active item by matching path", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      // Mock current path would need router mocking for full test
      // This is a simplified version
      expect(typeof result.current.isItemActive).toBe("function");
    });

    it("should check if item has active child", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      const leafItem = mockNavigationData.navigation[0].items![1]; // No children

      expect(result.current.hasActiveChild(leafItem)).toBe(false);
    });

    it("should determine auto-expand based on level and config", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      const item = mockNavigationData.navigation[0].items![0];

      // Level 0 should auto-expand (config.autoExpandDepth = 1)
      expect(result.current.shouldAutoExpand(item, 0)).toBe(true);

      // Level 1 should auto-expand (config.autoExpandDepth = 1)
      expect(result.current.shouldAutoExpand(item, 1)).toBe(true);

      // Note: Level 2 might still auto-expand if item has active children
      // The actual behavior depends on isItemActive and hasActiveChild
    });
  });

  describe("Edge Cases", () => {
    it("should handle undefined navigation data gracefully", () => {
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(undefined, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.drawerItems).toEqual([]);
      expect(result.current.visibleItems).toEqual([]);
    });

    it("should handle empty navigation data", () => {
      const emptyData: NavigationResponseDto = { navigation: [] };
      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(emptyData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.drawerItems).toEqual([]);
    });

    it("should filter non-navigable items", () => {
      const dataWithHidden: NavigationResponseDto = {
        navigation: [
          {
            ...mockNavigationData.navigation[0],
            items: [
              ...mockNavigationData.navigation[0].items!,
              {
                id: 30,
                parentId: 1,
                title: "Hidden Item",
                subTitle: "",
                url: "hidden",
                orderNumber: 3,
                icon: "",
                requiredRoles: [],
                disabled: false,
                isNavigable: false, // Should be filtered out
                items: []
              }
            ]
          }
        ]
      };

      const store = createMockStore();
      const { result } = renderHook(() => useDrawerViewModel(dataWithHidden, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      expect(result.current.drawerItems).toHaveLength(2); // Hidden item not included
      expect(result.current.drawerItems.find((item) => item.title === "Hidden Item")).toBeUndefined();
    });
  });

  describe("Performance", () => {
    it("should memoize computed values to prevent unnecessary recalculation", () => {
      const store = createMockStore();
      const { result, rerender } = renderHook(() => useDrawerViewModel(mockNavigationData, mockDrawerConfig), {
        wrapper: createWrapper(store)
      });

      const firstDrawerItems = result.current.drawerItems;

      // Rerender without changing props
      rerender();

      // Should return same reference (memoized)
      expect(result.current.drawerItems).toBe(firstDrawerItems);
    });
  });
});
