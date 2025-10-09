import { configureStore } from "@reduxjs/toolkit";
import { renderHook } from "@testing-library/react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it } from "vitest";
import { NavigationDto } from "../types/navigation/navigation";
import { useReadOnlyNavigation } from "./useReadOnlyNavigation";

describe("useReadOnlyNavigation", () => {
  const createMockNavigation = (
    id: number,
    isReadOnly: boolean = false,
    statusId?: number,
    statusName?: string
  ): NavigationDto => ({
    id,
    parentId: 0,
    title: `Navigation ${id}`,
    subTitle: "",
    url: `/nav/${id}`,
    orderNumber: id,
    icon: "",
    requiredRoles: [],
    disabled: false,
    isReadOnly,
    statusId,
    statusName,
    items: []
  });

  const createMockStore = (navigationData: any) => {
    return configureStore({
      reducer: {
        navigation: () => ({
          navigationData
        })
      }
    });
  };

  const wrapper =
    (store: ReturnType<typeof configureStore>) =>
    ({ children }: { children: React.ReactNode }) => <Provider store={store}>{children}</Provider>;

  beforeEach(() => {
    localStorage.clear();
  });

  it("should return false when navigationId is not in localStorage", () => {
    const navigation = createMockNavigation(1, false);
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useReadOnlyNavigation(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(false);
  });

  it("should return false when navigation is not read-only", () => {
    localStorage.setItem("navigationId", "1");
    const navigation = createMockNavigation(1, false);
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useReadOnlyNavigation(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(false);
  });

  it("should return true when navigation is read-only", () => {
    localStorage.setItem("navigationId", "1");
    const navigation = createMockNavigation(1, true);
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useReadOnlyNavigation(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(true);
  });

  it("should find nested navigation items", () => {
    localStorage.setItem("navigationId", "3");

    const nestedNav = createMockNavigation(3, true);
    const parentNav = createMockNavigation(2, false);
    parentNav.items = [nestedNav];

    const rootNav = createMockNavigation(1, false);
    rootNav.items = [parentNav];

    const mockStore = createMockStore({ navigation: [rootNav] });

    const { result } = renderHook(() => useReadOnlyNavigation(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(true);
  });

  it("should return false when navigation is not found", () => {
    localStorage.setItem("navigationId", "999");
    const navigation = createMockNavigation(1, true);
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useReadOnlyNavigation(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(false);
  });

  it("should handle deeply nested navigation", () => {
    localStorage.setItem("navigationId", "4");

    const deeplyNested = createMockNavigation(4, true);
    const level3 = createMockNavigation(3, false);
    level3.items = [deeplyNested];

    const level2 = createMockNavigation(2, false);
    level2.items = [level3];

    const level1 = createMockNavigation(1, false);
    level1.items = [level2];

    const mockStore = createMockStore({ navigation: [level1] });

    const { result } = renderHook(() => useReadOnlyNavigation(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(true);
  });

  it("should handle multiple root navigation items", () => {
    localStorage.setItem("navigationId", "3");

    const nav1 = createMockNavigation(1, false);
    const nav2 = createMockNavigation(2, false);
    const nav3 = createMockNavigation(3, true);

    const mockStore = createMockStore({ navigation: [nav1, nav2, nav3] });

    const { result } = renderHook(() => useReadOnlyNavigation(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(true);
  });

  it("should return false when isReadOnly property is undefined", () => {
    localStorage.setItem("navigationId", "1");
    const navigation: NavigationDto = {
      id: 1,
      parentId: 0,
      title: "Navigation 1",
      subTitle: "",
      url: "/nav/1",
      orderNumber: 1,
      icon: "",
      requiredRoles: [],
      disabled: false,
      items: []
    };

    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useReadOnlyNavigation(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(false);
  });

  it("should handle invalid navigationId in localStorage", () => {
    localStorage.setItem("navigationId", "invalid");
    const navigation = createMockNavigation(1, true);
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useReadOnlyNavigation(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(false);
  });

  // Status-based read-only tests
  describe("status-based read-only logic", () => {
    it("should return false when statusId is InProgress (2)", () => {
      localStorage.setItem("navigationId", "1");
      const navigation = createMockNavigation(1, false, 2, "In Progress");
      const mockStore = createMockStore({ navigation: [navigation] });

      const { result } = renderHook(() => useReadOnlyNavigation(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });

    it("should return true when statusId is NotStarted (1)", () => {
      localStorage.setItem("navigationId", "1");
      const navigation = createMockNavigation(1, false, 1, "Not Started");
      const mockStore = createMockStore({ navigation: [navigation] });

      const { result } = renderHook(() => useReadOnlyNavigation(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(true);
    });

    it("should return true when statusId is OnHold (3)", () => {
      localStorage.setItem("navigationId", "1");
      const navigation = createMockNavigation(1, false, 3, "On Hold");
      const mockStore = createMockStore({ navigation: [navigation] });

      const { result } = renderHook(() => useReadOnlyNavigation(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(true);
    });

    it("should return true when statusId is Complete (4)", () => {
      localStorage.setItem("navigationId", "1");
      const navigation = createMockNavigation(1, false, 4, "Complete");
      const mockStore = createMockStore({ navigation: [navigation] });

      const { result } = renderHook(() => useReadOnlyNavigation(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(true);
    });

    it("should return true when both isReadOnly is true and statusId is InProgress", () => {
      localStorage.setItem("navigationId", "1");
      const navigation = createMockNavigation(1, true, 2, "In Progress");
      const mockStore = createMockStore({ navigation: [navigation] });

      const { result } = renderHook(() => useReadOnlyNavigation(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(true);
    });

    it("should return true when both isReadOnly is true and statusId is Complete", () => {
      localStorage.setItem("navigationId", "1");
      const navigation = createMockNavigation(1, true, 4, "Complete");
      const mockStore = createMockStore({ navigation: [navigation] });

      const { result } = renderHook(() => useReadOnlyNavigation(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(true);
    });

    it("should return false when statusId is undefined and isReadOnly is false", () => {
      localStorage.setItem("navigationId", "1");
      const navigation = createMockNavigation(1, false, undefined, undefined);
      const mockStore = createMockStore({ navigation: [navigation] });

      const { result } = renderHook(() => useReadOnlyNavigation(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });
  });
});
