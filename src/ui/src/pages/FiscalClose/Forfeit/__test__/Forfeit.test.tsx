import { configureStore } from "@reduxjs/toolkit";
import { fireEvent, render, screen } from "@testing-library/react";
import { Provider } from "react-redux";
import { describe, expect, it, vi } from "vitest";
import Forfeit from "../Forfeit";

// Mock the useForfeit hook
const mockExecuteSearch = vi.fn();
const mockHandleStatusChange = vi.fn();
const mockHandleReset = vi.fn();

vi.mock("./hooks/useForfeit", () => ({
  default: vi.fn(() => ({
    searchResults: {
      response: {
        results: [{ badgeNumber: 123, employeeName: "Test User" }],
        total: 1
      }
    },
    isSearching: false,
    showData: true,
    pagination: {
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "badgeNumber", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    },
    executeSearch: mockExecuteSearch,
    handleStatusChange: mockHandleStatusChange,
    handleReset: mockHandleReset
  }))
}));

// Mock the child components
vi.mock("./ForfeitGrid", () => ({
  default: vi.fn(({ searchResults, pagination, isSearching }) => (
    <div data-testid="forfeit-grid">
      <div data-testid="has-results">{searchResults ? "true" : "false"}</div>
      <div data-testid="grid-is-searching">{isSearching.toString()}</div>
      <div data-testid="page-size">{pagination?.pageSize}</div>
    </div>
  ))
}));

vi.mock("./ForfeitSearchFilter", () => ({
  default: vi.fn(({ onSearch, onReset, isSearching }) => (
    <div data-testid="forfeit-search-filter">
      <button
        data-testid="search-button"
        onClick={() => onSearch({ profitYear: 2024 })}>
        Search
      </button>
      <button
        data-testid="reset-button"
        onClick={onReset}>
        Reset
      </button>
      <div data-testid="filter-is-searching">{isSearching.toString()}</div>
    </div>
  ))
}));

vi.mock("../../../components/StatusDropdownActionNode", () => ({
  default: vi.fn(({ onStatusChange }) => (
    <div data-testid="status-dropdown">
      <button
        data-testid="change-to-complete"
        onClick={() => onStatusChange?.("4", "Complete")}>
        Change to Complete
      </button>
      <button
        data-testid="change-to-in-progress"
        onClick={() => onStatusChange?.("2", "In Progress")}>
        Change to In Progress
      </button>
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

  it("should call handleStatusChange when status changes to Complete", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const completeButton = screen.getByTestId("change-to-complete");
    fireEvent.click(completeButton);

    expect(mockHandleStatusChange).toHaveBeenCalledWith("4", "Complete");
  });

  it("should call handleStatusChange when status changes to In Progress", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const inProgressButton = screen.getByTestId("change-to-in-progress");
    fireEvent.click(inProgressButton);

    expect(mockHandleStatusChange).toHaveBeenCalledWith("2", "In Progress");
  });

  it("should call executeSearch when search button is clicked", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const searchButton = screen.getByTestId("search-button");
    fireEvent.click(searchButton);

    expect(mockExecuteSearch).toHaveBeenCalledWith({ profitYear: 2024 });
  });

  it("should call handleReset when reset button is clicked", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const resetButton = screen.getByTestId("reset-button");
    fireEvent.click(resetButton);

    expect(mockHandleReset).toHaveBeenCalled();
  });

  it("should pass correct props to ForfeitGrid", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    expect(screen.getByTestId("has-results")).toHaveTextContent("true");
    expect(screen.getByTestId("grid-is-searching")).toHaveTextContent("false");
    expect(screen.getByTestId("page-size")).toHaveTextContent("25");
  });

  it("should pass correct props to ForfeitSearchFilter", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const searchFilterIsSearching = screen.getByTestId("filter-is-searching");
    expect(searchFilterIsSearching).toHaveTextContent("false");
  });
});
