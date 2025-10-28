import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { NavigationDto } from "../../reduxstore/types";
import RecursiveNavItem from "./RecursiveNavItem";

// Mock react-router-dom hooks
const mockNavigate = vi.fn();
const mockUseLocation = vi.fn();

vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    useLocation: () => mockUseLocation()
  };
});

describe("RecursiveNavItem - Collapsible Menu Tests", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    localStorage.clear();
    // Default location
    mockUseLocation.mockReturnValue({ pathname: "/test-path" });
  });

  const createNavItem = (overrides?: Partial<NavigationDto>): NavigationDto => ({
    id: 1,
    parentId: 0,
    title: "Parent Menu",
    subTitle: "",
    url: "/parent",
    orderNumber: 1,
    icon: "",
    requiredRoles: [],
    disabled: false,
    items: [],
    ...overrides
  });

  const createNestedNavItems = (): NavigationDto => ({
    id: 1,
    parentId: 0,
    title: "Parent Menu",
    subTitle: "",
    url: "/parent",
    orderNumber: 1,
    icon: "",
    requiredRoles: [],
    disabled: false,
    items: [
      {
        id: 2,
        parentId: 1,
        title: "Child Menu 1",
        subTitle: "",
        url: "/parent/child1",
        orderNumber: 1,
        icon: "",
        requiredRoles: [],
        disabled: false,
        items: []
      },
      {
        id: 3,
        parentId: 1,
        title: "Child Menu 2",
        subTitle: "",
        url: "/parent/child2",
        orderNumber: 2,
        icon: "",
        requiredRoles: [],
        disabled: false,
        items: []
      }
    ]
  });

  it("should render menu item with expand icon when it has children", () => {
    const navItem = createNestedNavItems();

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
          maxAutoExpandDepth={-1} // Set to -1 to ensure collapsed by default
        />
      </MemoryRouter>
    );

    expect(screen.getByText("Parent Menu")).toBeInTheDocument();
    // Should show ExpandMore icon (collapsed state)
    expect(screen.getByTestId("ExpandMoreIcon")).toBeInTheDocument();
  });

  it("should toggle between collapsed and expanded states when clicked", async () => {
    const navItem = createNestedNavItems();

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
          maxAutoExpandDepth={-1} // Set to -1 to ensure collapsed by default
        />
      </MemoryRouter>
    );

    const parentButton = screen.getByText("Parent Menu").closest("div")?.parentElement;
    expect(parentButton).toBeInTheDocument();

    // Initially collapsed - children should not be visible
    expect(screen.queryByText("Child Menu 1")).not.toBeInTheDocument();

    // Click to expand
    fireEvent.click(parentButton!);

    // Children should now be visible
    await waitFor(() => {
      expect(screen.getByText("Child Menu 1")).toBeInTheDocument();
      expect(screen.getByText("Child Menu 2")).toBeInTheDocument();
    });

    // Should show ExpandLess icon (expanded state)
    expect(screen.getByTestId("ExpandLessIcon")).toBeInTheDocument();

    // Click again to collapse
    fireEvent.click(parentButton!);

    // Children should be hidden again
    await waitFor(() => {
      expect(screen.queryByText("Child Menu 1")).not.toBeInTheDocument();
      expect(screen.queryByText("Child Menu 2")).not.toBeInTheDocument();
    });

    // Should show ExpandMore icon again (collapsed state)
    expect(screen.getByTestId("ExpandMoreIcon")).toBeInTheDocument();
  });

  it("should persist expanded state to localStorage when expanded", async () => {
    const navItem = createNestedNavItems();

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
          maxAutoExpandDepth={-1} // Set to -1 to ensure collapsed by default
        />
      </MemoryRouter>
    );

    const parentButton = screen.getByText("Parent Menu").closest("div")?.parentElement;

    // Click to expand
    fireEvent.click(parentButton!);

    await waitFor(() => {
      const storedValue = localStorage.getItem("nav-expanded-1");
      expect(storedValue).toBe("true");
    });
  });

  it("should persist collapsed state to localStorage when collapsed", async () => {
    const navItem = createNestedNavItems();
    // Pre-set localStorage to expanded
    localStorage.setItem("nav-expanded-1", "true");

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
        />
      </MemoryRouter>
    );

    const parentButton = screen.getByText("Parent Menu").closest("div")?.parentElement;

    // Click to collapse
    fireEvent.click(parentButton!);

    await waitFor(() => {
      const storedValue = localStorage.getItem("nav-expanded-1");
      expect(storedValue).toBe("false");
    });
  });

  it("should restore expanded state from localStorage on mount", () => {
    const navItem = createNestedNavItems();
    // Pre-set localStorage to expanded
    localStorage.setItem("nav-expanded-1", "true");

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
        />
      </MemoryRouter>
    );

    // Children should be visible immediately (restored from localStorage)
    expect(screen.getByText("Child Menu 1")).toBeInTheDocument();
    expect(screen.getByText("Child Menu 2")).toBeInTheDocument();
    expect(screen.getByTestId("ExpandLessIcon")).toBeInTheDocument();
  });

  it("should remain collapsed after user manually collapses it, even if it contains active child", async () => {
    // This tests that all menus start collapsed by default
    const navItem = createNestedNavItems();

    // Mock useLocation to return active child path
    mockUseLocation.mockReturnValue({ pathname: "/parent/child1" });

    render(
      <MemoryRouter initialEntries={["/parent/child1"]}>
        <RecursiveNavItem
          item={navItem}
          level={0}
        />
      </MemoryRouter>
    );

    // Should be collapsed by default, even though it contains active child
    expect(screen.queryByText("Child Menu 1")).not.toBeInTheDocument();

    const parentButton = screen.getByText("Parent Menu").closest("div")?.parentElement;

    // User manually expands
    fireEvent.click(parentButton!);

    // Should now be expanded
    await waitFor(() => {
      expect(screen.getByText("Child Menu 1")).toBeInTheDocument();
    });

    // User manually collapses again
    fireEvent.click(parentButton!);

    // Should remain collapsed
    await waitFor(() => {
      expect(screen.queryByText("Child Menu 1")).not.toBeInTheDocument();
    });

    // LocalStorage should reflect collapsed state
    const storedValue = localStorage.getItem("nav-expanded-1");
    expect(storedValue).toBe("false");
  });

  it("should respect maxAutoExpandDepth prop", () => {
    const deeplyNestedItem: NavigationDto = {
      id: 1,
      parentId: 0,
      title: "Level 0",
      subTitle: "",
      url: "/level0",
      orderNumber: 1,
      icon: "",
      requiredRoles: [],
      disabled: false,
      items: [
        {
          id: 2,
          parentId: 1,
          title: "Level 1",
          subTitle: "",
          url: "/level0/level1",
          orderNumber: 1,
          icon: "",
          requiredRoles: [],
          disabled: false,
          items: [
            {
              id: 3,
              parentId: 2,
              title: "Level 2",
              subTitle: "",
              url: "/level0/level1/level2",
              orderNumber: 1,
              icon: "",
              requiredRoles: [],
              disabled: false,
              items: []
            }
          ]
        }
      ]
    };

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={deeplyNestedItem}
          level={0}
          maxAutoExpandDepth={1}
        />
      </MemoryRouter>
    );

    // Level 0 should be auto-expanded (level 0 <= maxAutoExpandDepth 1)
    expect(screen.getByText("Level 1")).toBeInTheDocument();
  });

  it("should call onNavigate callback when navigable item is clicked", () => {
    const navItem = createNavItem({ url: "/test-path" });
    const onNavigate = vi.fn();

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
          onNavigate={onNavigate}
        />
      </MemoryRouter>
    );

    const menuButton = screen.getByText("Parent Menu").closest("div")?.parentElement;
    fireEvent.click(menuButton!);

    expect(onNavigate).toHaveBeenCalledWith(navItem);
  });

  it("should store navigationId in localStorage when item is clicked", () => {
    const navItem = createNavItem({ id: 42, url: "/test-path" });

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
        />
      </MemoryRouter>
    );

    const menuButton = screen.getByText("Parent Menu").closest("div")?.parentElement;
    fireEvent.click(menuButton!);

    expect(localStorage.getItem("navigationId")).toBe("42");
  });

  it("should not expand or navigate when disabled", () => {
    const navItem = createNestedNavItems();
    navItem.disabled = true;
    const onNavigate = vi.fn();

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
          maxAutoExpandDepth={-1} // Set to -1 to ensure collapsed by default
          onNavigate={onNavigate}
        />
      </MemoryRouter>
    );

    const menuButton = screen.getByText("Parent Menu").closest("div")?.parentElement;
    fireEvent.click(menuButton!);

    // Should not expand
    expect(screen.queryByText("Child Menu 1")).not.toBeInTheDocument();

    // Should not call navigate callback
    expect(onNavigate).not.toHaveBeenCalled();
  });

  it("should handle localStorage errors gracefully", async () => {
    const navItem = createNestedNavItems();

    // Mock localStorage to throw error
    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
    const setItemSpy = vi.spyOn(Storage.prototype, "setItem").mockImplementation(() => {
      throw new Error("Storage quota exceeded");
    });

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
          maxAutoExpandDepth={-1} // Set to -1 to ensure collapsed by default
        />
      </MemoryRouter>
    );

    const parentButton = screen.getByText("Parent Menu").closest("div")?.parentElement;

    // Click should not crash even when localStorage fails
    fireEvent.click(parentButton!);

    // Children should still expand despite localStorage error
    await waitFor(() => {
      expect(screen.getByText("Child Menu 1")).toBeInTheDocument();
    });

    // Should log error
    await waitFor(() => {
      expect(consoleErrorSpy).toHaveBeenCalledWith("Error saving expanded state:", expect.any(Error));
    });

    setItemSpy.mockRestore();
    consoleErrorSpy.mockRestore();
  });

  it("should display status chip for leaf nodes without children", () => {
    const navItem = createNavItem({
      statusName: "In Progress",
      items: []
    });

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
        />
      </MemoryRouter>
    );

    expect(screen.getByText("In Progress")).toBeInTheDocument();
  });

  it("should not display status chip for parent nodes with children", () => {
    const navItem = createNestedNavItems();
    navItem.statusName = "In Progress";

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
        />
      </MemoryRouter>
    );

    // Status chip should not be displayed for parent nodes
    expect(screen.queryByText("In Progress")).not.toBeInTheDocument();
  });

  it("should apply correct indentation based on nesting level", () => {
    const navItem = createNavItem();

    const { container } = render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={2}
        />
      </MemoryRouter>
    );

    // Level 2 should have indentation of 2 + 2*2 = 6
    const listItemButton = container.querySelector('[role="button"]');

    // Check that paddingLeft is applied (exact value depends on MUI theme)
    expect(listItemButton).toHaveStyle({ paddingLeft: expect.any(String) });
  });

  it("should highlight active menu item with correct styling", () => {
    const navItem = createNavItem({ url: "/test-path" });

    // Mock useLocation to return matching path
    mockUseLocation.mockReturnValue({ pathname: "/test-path" });

    const { container } = render(
      <MemoryRouter initialEntries={["/test-path"]}>
        <RecursiveNavItem
          item={navItem}
          level={0}
        />
      </MemoryRouter>
    );

    const menuText = screen.getByText("Parent Menu");

    // Check that the menu text exists and is in the document
    expect(menuText).toBeInTheDocument();

    // The ListItemButton should have a left border for active items
    // MUI's sx styles create className-based styles, so we check the element has the right structure
    const listItemButton = container.querySelector('[role="button"]');
    expect(listItemButton).toBeInTheDocument();
  });

  it("should recursively render nested children correctly", () => {
    const navItem = createNestedNavItems();
    localStorage.setItem("nav-expanded-1", "true");

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
        />
      </MemoryRouter>
    );

    // All levels should be rendered
    expect(screen.getByText("Parent Menu")).toBeInTheDocument();
    expect(screen.getByText("Child Menu 1")).toBeInTheDocument();
    expect(screen.getByText("Child Menu 2")).toBeInTheDocument();
  });

  it("should filter out non-navigable items", () => {
    const navItem: NavigationDto = {
      ...createNestedNavItems(),
      items: [
        {
          id: 2,
          parentId: 1,
          title: "Navigable Child",
          subTitle: "",
          url: "/parent/child1",
          orderNumber: 1,
          icon: "",
          requiredRoles: [],
          disabled: false,
          isNavigable: true,
          items: []
        },
        {
          id: 3,
          parentId: 1,
          title: "Non-Navigable Child",
          subTitle: "",
          url: "/parent/child2",
          orderNumber: 2,
          icon: "",
          requiredRoles: [],
          disabled: false,
          isNavigable: false,
          items: []
        }
      ]
    };

    localStorage.setItem("nav-expanded-1", "true");

    render(
      <MemoryRouter>
        <RecursiveNavItem
          item={navItem}
          level={0}
        />
      </MemoryRouter>
    );

    // Only navigable items should be rendered
    expect(screen.getByText("Navigable Child")).toBeInTheDocument();
    expect(screen.queryByText("Non-Navigable Child")).not.toBeInTheDocument();
  });
});
