import { configureStore } from "@reduxjs/toolkit";
import { renderHook } from "@testing-library/react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it } from "vitest";
import { NavigationDto } from "../types/navigation/navigation";
import { useIsReadOnlyByStatus } from "./useIsReadOnlyByStatus";

describe("useIsReadOnlyByStatus", () => {
  const createMockNavigation = (
    id: number,
    statusId?: number,
    statusName?: string,
    isReadOnly: boolean = false
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

  const createMockStore = (navigationData: { navigation: NavigationDto[] } | null) => {
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
    const navigation = createMockNavigation(1, 1, "Not Started");
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(false);
  });

  it("should return false when statusId is InProgress (2)", () => {
    localStorage.setItem("navigationId", "1");
    const navigation = createMockNavigation(1, 2, "In Progress");
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(false);
  });

  it("should return true when statusId is NotStarted (1)", () => {
    localStorage.setItem("navigationId", "1");
    const navigation = createMockNavigation(1, 1, "Not Started");
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(true);
  });

  it("should return true when statusId is OnHold (3)", () => {
    localStorage.setItem("navigationId", "1");
    const navigation = createMockNavigation(1, 3, "On Hold");
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(true);
  });

  it("should return true when statusId is Complete (4)", () => {
    localStorage.setItem("navigationId", "1");
    const navigation = createMockNavigation(1, 4, "Complete");
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(true);
  });

  it("should return false when statusId is undefined", () => {
    localStorage.setItem("navigationId", "1");
    const navigation = createMockNavigation(1, undefined, undefined);
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(false);
  });

  it("should ignore isReadOnly flag and only check status", () => {
    localStorage.setItem("navigationId", "1");
    // Even if isReadOnly is true, this hook only checks status
    const navigation = createMockNavigation(1, 2, "In Progress", true);
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    // Should be false because status is InProgress, regardless of isReadOnly flag
    expect(result.current).toBe(false);
  });

  it("should work with nested navigation items", () => {
    localStorage.setItem("navigationId", "3");

    const nestedNav = createMockNavigation(3, 1, "Not Started");
    const parentNav = createMockNavigation(2, 2, "In Progress");
    parentNav.items = [nestedNav];

    const rootNav = createMockNavigation(1, 2, "In Progress");
    rootNav.items = [parentNav];

    const mockStore = createMockStore({ navigation: [rootNav] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(true);
  });

  it("should return false when navigation is not found", () => {
    localStorage.setItem("navigationId", "999");
    const navigation = createMockNavigation(1, 1, "Not Started");
    const mockStore = createMockStore({ navigation: [navigation] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(false);
  });

  it("should handle multiple root navigation items", () => {
    localStorage.setItem("navigationId", "3");

    const nav1 = createMockNavigation(1, 2, "In Progress");
    const nav2 = createMockNavigation(2, 2, "In Progress");
    const nav3 = createMockNavigation(3, 4, "Complete");

    const mockStore = createMockStore({ navigation: [nav1, nav2, nav3] });

    const { result } = renderHook(() => useIsReadOnlyByStatus(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(true);
  });

  describe("edge cases", () => {
    it("should handle empty navigation array", () => {
      localStorage.setItem("navigationId", "1");
      const mockStore = createMockStore({ navigation: [] });

      const { result } = renderHook(() => useIsReadOnlyByStatus(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });

    it("should handle null navigation data", () => {
      localStorage.setItem("navigationId", "1");
      const mockStore = createMockStore(null);

      const { result } = renderHook(() => useIsReadOnlyByStatus(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });

    it("should handle invalid navigationId format", () => {
      localStorage.setItem("navigationId", "not-a-number");
      const navigation = createMockNavigation(1, 1, "Not Started");
      const mockStore = createMockStore({ navigation: [navigation] });

      const { result } = renderHook(() => useIsReadOnlyByStatus(), {
        wrapper: wrapper(mockStore)
      });

      expect(result.current).toBe(false);
    });
  });
});
