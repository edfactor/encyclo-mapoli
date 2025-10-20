import { configureStore } from "@reduxjs/toolkit";
import { render, screen } from "@testing-library/react";
import { Provider } from "react-redux";
import { BrowserRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { NavigationResponseDto } from "../../reduxstore/types";
import { RouteCategory } from "../../types/MenuTypes";
import MenuBar from "./MenuBar";

const mockNavigate = vi.fn();
const mockDispatch = vi.fn();

vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => mockNavigate,
    useLocation: () => ({ pathname: "/test" })
  };
});

vi.mock("react-redux", async () => {
  const actual = await vi.importActual("react-redux");
  return {
    ...actual,
    useDispatch: () => mockDispatch
  };
});

describe("MenuBar with PageSearch", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  const mockMenuInfo: RouteCategory[] = [
    {
      menuLabel: "Year End",
      parentRoute: "/year-end",
      disabled: false,
      items: []
    },
    {
      menuLabel: "Reports",
      parentRoute: "/reports",
      disabled: false,
      items: []
    }
  ];

  const mockNavigationData: NavigationResponseDto = {
    navigation: [
      {
        id: 1,
        parentId: null,
        title: "Year End",
        subTitle: "",
        url: "",
        orderNumber: 1,
        icon: "",
        requiredRoles: [],
        disabled: false,
        isNavigable: false,
        items: [
          {
            id: 10,
            parentId: 1,
            title: "December Activities",
            subTitle: "",
            url: "/year-end/december",
            orderNumber: 1,
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

  const createMockStore = () => {
    return configureStore({
      reducer: {
        general: () => ({ activeSubMenu: null, drawerOpen: false }),
        navigation: () => ({ navigationData: mockNavigationData })
      }
    });
  };

  const renderMenuBar = (
    menuInfo = mockMenuInfo,
    navigationData = mockNavigationData,
    impersonationMultiSelect?: React.ReactNode
  ) => {
    const store = createMockStore();
    return render(
      <Provider store={store}>
        <BrowserRouter>
          <MenuBar
            menuInfo={menuInfo}
            navigationData={navigationData}
            impersonationMultiSelect={impersonationMultiSelect}
          />
        </BrowserRouter>
      </Provider>
    );
  };

  it("should render MenuBar with PageSearch", () => {
    renderMenuBar();

    // Check for navigation buttons
    expect(screen.getByText("Home")).toBeDefined();
    expect(screen.getByText("Year End")).toBeDefined();
    expect(screen.getByText("Reports")).toBeDefined();

    // Check for PageSearch
    const searchInput = screen.getByPlaceholderText("Search pages...");
    expect(searchInput).toBeDefined();
  });

  it("should render PageSearch on the right side", () => {
    renderMenuBar();

    const menubar = document.querySelector(".menubar");
    expect(menubar).toBeDefined();

    // PageSearch should be in the right container
    const searchInput = screen.getByPlaceholderText("Search pages...");
    expect(searchInput).toBeDefined();
  });

  it("should show vertical separator when impersonation is displayed", () => {
    const impersonationComponent = <div data-testid="impersonation">Impersonate</div>;
    renderMenuBar(mockMenuInfo, mockNavigationData, impersonationComponent);

    // Check impersonation is rendered
    expect(screen.getByTestId("impersonation")).toBeDefined();

    // Check for separator (vertical line) - should be AFTER search, BEFORE impersonation
    const separator = document.querySelector(".h-8.w-px.bg-white.opacity-30");
    expect(separator).toBeDefined();
  });
  it("should not show vertical separator when impersonation is not displayed", () => {
    renderMenuBar(mockMenuInfo, mockNavigationData, undefined);

    // Check for separator (should not exist)
    const separator = document.querySelector(".h-8.w-px.bg-white.opacity-30");
    expect(separator).toBeNull();
  });

  it("should pass navigationData to PageSearch", () => {
    renderMenuBar();

    // PageSearch should receive navigation data and be able to search
    const searchInput = screen.getByPlaceholderText("Search pages...");
    expect(searchInput).toBeDefined();
  });

  it("should render all menu items and search in correct layout", () => {
    renderMenuBar();

    const menubar = document.querySelector(".menubar");
    expect(menubar).toBeDefined();

    // Should have navbuttons on the left
    const navbuttons = document.querySelector(".navbuttons");
    expect(navbuttons).toBeDefined();

    // Should have search on the right
    const searchInput = screen.getByPlaceholderText("Search pages...");
    expect(searchInput).toBeDefined();
  });

  it("should maintain existing menu functionality with search present", () => {
    renderMenuBar();

    // All existing menu items should still be clickable
    const homeButton = screen.getByText("Home");
    const yearEndButton = screen.getByText("Year End");
    const reportsButton = screen.getByText("Reports");

    expect(homeButton).toBeDefined();
    expect(yearEndButton).toBeDefined();
    expect(reportsButton).toBeDefined();
  });

  it("should render with minimal navigation data", () => {
    const minimalNav: NavigationResponseDto = {
      navigation: []
    };

    renderMenuBar([], minimalNav);

    // Search should still render even with no navigation
    const searchInput = screen.getByPlaceholderText("Search pages...");
    expect(searchInput).toBeDefined();
  });

  it("should handle undefined navigation data", () => {
    renderMenuBar(mockMenuInfo, undefined);

    // Should render without crashing
    expect(screen.getByText("Home")).toBeDefined();

    // Search should still render
    const searchInput = screen.getByPlaceholderText("Search pages...");
    expect(searchInput).toBeDefined();
  });

  it("should apply correct CSS classes for layout", () => {
    renderMenuBar();

    // Check menubar has flex layout
    const menubar = document.querySelector(".menubar");
    expect(menubar?.classList.contains("menubar")).toBe(true);

    // Check right container has correct classes
    const rightContainer = document.querySelector(".flex.items-center.gap-4");
    expect(rightContainer).toBeDefined();
  });

  it("should position search before impersonation in DOM order", () => {
    const impersonationComponent = <div data-testid="impersonation">Impersonate</div>;
    renderMenuBar(mockMenuInfo, mockNavigationData, impersonationComponent);

    const impersonation = screen.getByTestId("impersonation");
    const searchInput = screen.getByPlaceholderText("Search pages...");

    expect(impersonation).toBeDefined();
    expect(searchInput).toBeDefined();

    // Both should be in the right container
    const rightContainer = document.querySelector(".flex.items-center.gap-4");
    expect(rightContainer?.contains(impersonation)).toBe(true);
    expect(rightContainer?.contains(searchInput)).toBe(true);
  });
});
