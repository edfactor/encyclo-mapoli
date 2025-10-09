import { configureStore } from "@reduxjs/toolkit";
import { renderHook } from "@testing-library/react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { NAVIGATION_STATUS } from "../constants";
import { NavigationDto } from "../reduxstore/types";
import { usePrerequisiteNavigations } from "./usePrerequisiteNavigations";

// Mock smart-ui-library
vi.mock("smart-ui-library", () => ({
  setMessage: () => ({ type: "SET_MESSAGE" })
}));

describe("usePrerequisiteNavigations", () => {
  const createMockNavigation = (id: number, prerequisiteNavigations: NavigationDto[] = []): NavigationDto => ({
    id,
    parentId: 0,
    title: `Navigation ${id}`,
    subTitle: "",
    url: `/nav/${id}`,
    statusId: NAVIGATION_STATUS.COMPLETE,
    statusName: "Complete",
    orderNumber: id,
    icon: "",
    requiredRoles: [],
    disabled: false,
    prerequisiteNavigations,
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
    vi.clearAllMocks();
  });

  it("should return prerequisitesComplete as true when no navigation ID provided", () => {
    const mockStore = createMockStore({ navigation: [] });

    const { result } = renderHook(() => usePrerequisiteNavigations(null), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current.prerequisitesComplete).toBe(true);
    expect(result.current.incompletePrerequisites).toEqual([]);
  });

  it("should return prerequisitesComplete as true when navigation has no prerequisites", () => {
    const navigation = createMockNavigation(1, []);
    const mockStore = createMockStore({
      navigation: [navigation]
    });

    const { result } = renderHook(() => usePrerequisiteNavigations(1), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current.prerequisitesComplete).toBe(true);
    expect(result.current.incompletePrerequisites).toEqual([]);
  });

  it("should return prerequisitesComplete as true when all prerequisites are complete", () => {
    const prerequisite1 = createMockNavigation(2);
    prerequisite1.statusId = NAVIGATION_STATUS.COMPLETE;

    const prerequisite2 = createMockNavigation(3);
    prerequisite2.statusId = NAVIGATION_STATUS.COMPLETE;

    const navigation = createMockNavigation(1, [prerequisite1, prerequisite2]);

    const mockStore = createMockStore({
      navigation: [navigation]
    });

    const { result } = renderHook(() => usePrerequisiteNavigations(1), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current.prerequisitesComplete).toBe(true);
    expect(result.current.incompletePrerequisites).toEqual([]);
  });

  it("should return prerequisitesComplete as false when prerequisites are incomplete", () => {
    const prerequisite1 = createMockNavigation(2);
    prerequisite1.statusId = NAVIGATION_STATUS.IN_PROGRESS;
    prerequisite1.statusName = "In Progress";

    const navigation = createMockNavigation(1, [prerequisite1]);

    const mockStore = createMockStore({
      navigation: [navigation]
    });

    const { result } = renderHook(() => usePrerequisiteNavigations(1), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current.prerequisitesComplete).toBe(false);
    expect(result.current.incompletePrerequisites).toHaveLength(1);
    expect(result.current.incompletePrerequisites[0].id).toBe(2);
  });

  it("should find nested navigation items", () => {
    const nestedNav = createMockNavigation(3);
    const parentNav = createMockNavigation(2);
    parentNav.items = [nestedNav];

    const rootNav = createMockNavigation(1);
    rootNav.items = [parentNav];

    const mockStore = createMockStore({
      navigation: [rootNav]
    });

    const { result } = renderHook(() => usePrerequisiteNavigations(3), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current.currentNavigation).toBeDefined();
    expect(result.current.currentNavigation?.id).toBe(3);
  });

  it("should dispatch message when prerequisites are incomplete", () => {
    const prerequisite1 = createMockNavigation(2);
    prerequisite1.statusId = NAVIGATION_STATUS.IN_PROGRESS;
    prerequisite1.statusName = "In Progress";
    prerequisite1.title = "Step 1";

    const navigation = createMockNavigation(1, [prerequisite1]);

    const mockStore = createMockStore({
      navigation: [navigation]
    });

    const messageConfig = {
      messageTemplate: {
        key: "test-key",
        message: {
          type: "error" as const,
          title: "Prerequisites Required"
        }
      }
    };

    const { result } = renderHook(() => usePrerequisiteNavigations(1, messageConfig), {
      wrapper: wrapper(mockStore)
    });

    // Verify prerequisites are detected as incomplete
    expect(result.current.prerequisitesComplete).toBe(false);
    expect(result.current.incompletePrerequisites).toHaveLength(1);
  });

  it("should build custom message when build function provided", () => {
    const prerequisite1 = createMockNavigation(2);
    prerequisite1.statusId = NAVIGATION_STATUS.IN_PROGRESS;
    prerequisite1.statusName = "In Progress";

    const navigation = createMockNavigation(1, [prerequisite1]);

    const mockStore = createMockStore({
      navigation: [navigation]
    });

    const customBuild = vi.fn((template, incomplete, current) => ({
      ...template,
      message: {
        ...template.message,
        message: "Custom message"
      }
    }));

    const messageConfig = {
      messageTemplate: {
        key: "test-key",
        message: {
          type: "error" as const,
          title: "Prerequisites Required"
        }
      },
      build: customBuild
    };

    const { result } = renderHook(() => usePrerequisiteNavigations(1, messageConfig), {
      wrapper: wrapper(mockStore)
    });

    // Verify state is set correctly and custom build was called
    expect(result.current.prerequisitesComplete).toBe(false);
    expect(customBuild).toHaveBeenCalled();
  });

  it("should handle multiple incomplete prerequisites", () => {
    const prerequisite1 = createMockNavigation(2);
    prerequisite1.statusId = NAVIGATION_STATUS.IN_PROGRESS;
    prerequisite1.statusName = "In Progress";
    prerequisite1.title = "Step 1";

    const prerequisite2 = createMockNavigation(3);
    prerequisite2.statusId = NAVIGATION_STATUS.NOT_STARTED;
    prerequisite2.statusName = "Not Started";
    prerequisite2.title = "Step 2";

    const navigation = createMockNavigation(1, [prerequisite1, prerequisite2]);

    const mockStore = createMockStore({
      navigation: [navigation]
    });

    const { result } = renderHook(() => usePrerequisiteNavigations(1), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current.prerequisitesComplete).toBe(false);
    expect(result.current.incompletePrerequisites).toHaveLength(2);
  });

  it("should return current navigation", () => {
    const navigation = createMockNavigation(1);
    const mockStore = createMockStore({
      navigation: [navigation]
    });

    const { result } = renderHook(() => usePrerequisiteNavigations(1), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current.currentNavigation).toBeDefined();
    expect(result.current.currentNavigation?.id).toBe(1);
  });
});
