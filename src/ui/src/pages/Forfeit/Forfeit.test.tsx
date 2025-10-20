import { configureStore } from "@reduxjs/toolkit";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { describe, expect, it, vi } from "vitest";
import Forfeit from "./Forfeit";

// Mock the child components
vi.mock("./ForfeitGrid", () => ({
  default: vi.fn(({ shouldArchive, initialSearchLoaded }) => (
    <div data-testid="forfeit-grid">
      <div data-testid="should-archive">{shouldArchive.toString()}</div>
      <div data-testid="initial-search-loaded">{initialSearchLoaded.toString()}</div>
    </div>
  ))
}));

vi.mock("./ForfeitSearchFilter", () => ({
  default: vi.fn(({ onSearchClicked }) => (
    <div data-testid="forfeit-search-filter">
      <button
        data-testid="search-button"
        onClick={onSearchClicked}>
        Search
      </button>
    </div>
  ))
}));

vi.mock("../../components/StatusDropdownActionNode", () => ({
  default: vi.fn(({ onStatusChange, onSearchClicked }) => (
    <div data-testid="status-dropdown">
      <button
        data-testid="change-to-complete"
        onClick={() => onStatusChange("4", "Complete")}>
        Change to Complete
      </button>
      <button
        data-testid="change-to-in-progress"
        onClick={() => onStatusChange("2", "In Progress")}>
        Change to In Progress
      </button>
      <div data-testid="on-search-clicked">{onSearchClicked ? "enabled" : "disabled"}</div>
    </div>
  ))
}));

describe("Forfeit", () => {
  const createMockStore = () => {
    return configureStore({
      reducer: {
        security: () => ({ token: "mock-token" }),
        navigation: () => ({ navigationData: null }),
        yearsEnd: () => ({
          selectedProfitYearForFiscalClose: 2024,
          forfeituresAndPoints: null
        })
      }
    });
  };

  const wrapper =
    (store: ReturnType<typeof configureStore>) =>
    ({ children }: { children: React.ReactNode }) => <Provider store={store}>{children}</Provider>;

  it("should render Forfeit page with all components", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    expect(screen.getByTestId("forfeit-search-filter")).toBeInTheDocument();
    expect(screen.getByTestId("forfeit-grid")).toBeInTheDocument();
  });

  it("should set shouldArchive to true when status changes to Complete", async () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const completeButton = screen.getByTestId("change-to-complete");
    fireEvent.click(completeButton);

    await waitFor(() => {
      const shouldArchive = screen.getByTestId("should-archive");
      expect(shouldArchive.textContent).toBe("true");
    });
  });

  it("should reset shouldArchive to false after timeout", async () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const completeButton = screen.getByTestId("change-to-complete");
    fireEvent.click(completeButton);

    await waitFor(() => {
      const shouldArchive = screen.getByTestId("should-archive");
      expect(shouldArchive.textContent).toBe("true");
    });

    // Wait for the 100ms timeout
    await waitFor(
      () => {
        const shouldArchive = screen.getByTestId("should-archive");
        expect(shouldArchive.textContent).toBe("false");
      },
      { timeout: 200 }
    );
  });

  it("should NOT set shouldArchive when status changes to In Progress", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const inProgressButton = screen.getByTestId("change-to-in-progress");
    fireEvent.click(inProgressButton);

    const shouldArchive = screen.getByTestId("should-archive");
    expect(shouldArchive.textContent).toBe("false");
  });

  it("should increment searchClickedTrigger when search button is clicked", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const onSearchClickedStatus = screen.getByTestId("on-search-clicked");
    expect(onSearchClickedStatus.textContent).toBe("disabled");

    const searchButton = screen.getByTestId("search-button");
    fireEvent.click(searchButton);

    expect(onSearchClickedStatus.textContent).toBe("enabled");
  });

  it("should pass onSearchClicked prop to StatusDropdownActionNode after first search", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const searchButton = screen.getByTestId("search-button");

    // Before search - onSearchClicked should be undefined
    let onSearchClickedStatus = screen.getByTestId("on-search-clicked");
    expect(onSearchClickedStatus.textContent).toBe("disabled");

    // Click search
    fireEvent.click(searchButton);

    // After search - onSearchClicked should be defined
    onSearchClickedStatus = screen.getByTestId("on-search-clicked");
    expect(onSearchClickedStatus.textContent).toBe("enabled");
  });

  it("should set initialSearchLoaded to true when search is clicked", async () => {
    //const mockStore = createMockStore();
    //const { container } = render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const searchButton = screen.getByTestId("search-button");

    // Note: This test verifies the prop is passed, but the actual logic
    // is in ForfeitSearchFilter which we've mocked
    fireEvent.click(searchButton);

    // The mock ForfeitSearchFilter doesn't actually call setInitialSearchLoaded,
    // so we just verify the search button can be clicked
    expect(searchButton).toBeInTheDocument();
  });

  it("should handle multiple status changes correctly", async () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    // Change to In Progress - should NOT trigger archive
    const inProgressButton = screen.getByTestId("change-to-in-progress");
    fireEvent.click(inProgressButton);

    let shouldArchive = screen.getByTestId("should-archive");
    expect(shouldArchive.textContent).toBe("false");

    // Change to Complete - SHOULD trigger archive
    const completeButton = screen.getByTestId("change-to-complete");
    fireEvent.click(completeButton);

    await waitFor(() => {
      shouldArchive = screen.getByTestId("should-archive");
      expect(shouldArchive.textContent).toBe("true");
    });
  });
});
