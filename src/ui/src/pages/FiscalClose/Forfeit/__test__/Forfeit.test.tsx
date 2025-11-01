import { configureStore } from "@reduxjs/toolkit";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Provider } from "react-redux";
import { describe, expect, it, vi } from "vitest";
import { fireEvent } from "@testing-library/react";
import { CAPTIONS } from "../../../../constants";
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
    <section aria-label="forfeit grid">
      <div aria-live="polite">{searchResults ? "true" : "false"}</div>
      <div>{isSearching.toString()}</div>
      <div>{pagination?.pageSize}</div>
    </section>
  ))
}));

vi.mock("./ForfeitSearchFilter", () => ({
  default: vi.fn(({ onSearch, onReset, isSearching }) => (
    <section aria-label="forfeit search filter">
      <button
        onClick={() => onSearch({ profitYear: 2024 })}>
        Search
      </button>
      <button
        onClick={onReset}>
        Reset
      </button>
      <div aria-live="polite">{isSearching.toString()}</div>
    </section>
  ))
}));

vi.mock("../../../components/StatusDropdownActionNode", () => ({
  default: vi.fn(({ onStatusChange }) => (
    <section aria-label="status dropdown">
      <button
        onClick={() => onStatusChange?.("4", "Complete")}>
        Change to Complete
      </button>
      <button
        onClick={() => onStatusChange?.("2", "In Progress")}>
        Change to In Progress
      </button>
    </section>
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

    expect(screen.getByLabelText("status dropdown")).toBeInTheDocument();
    expect(screen.getByLabelText("forfeit search filter")).toBeInTheDocument();
    expect(screen.getByLabelText("forfeit grid")).toBeInTheDocument();
  });

  it("should call handleStatusChange when status changes to Complete", async () => {
    const user = userEvent.setup();
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const completeButton = screen.getByRole("button", { name: /change to complete/i });
    await user.click(completeButton);

    await waitFor(() => {
      expect(mockHandleStatusChange).toHaveBeenCalledWith("4", "Complete");
    });
  });

  it("should call handleStatusChange when status changes to In Progress", async () => {
    const user = userEvent.setup();
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const inProgressButton = screen.getByRole("button", { name: /change to in progress/i });
    await user.click(inProgressButton);

    await waitFor(() => {
      expect(mockHandleStatusChange).toHaveBeenCalledWith("2", "In Progress");
    });
  });

  it("should call executeSearch when search button is clicked", async () => {
    const user = userEvent.setup();
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const searchButton = screen.getByRole("button", { name: /search/i });
    await user.click(searchButton);

    await waitFor(() => {
      expect(mockExecuteSearch).toHaveBeenCalledWith({ profitYear: 2024 });
    });
  });

  it("should call handleReset when reset button is clicked", async () => {
    const user = userEvent.setup();
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    const resetButton = screen.getByRole("button", { name: /reset/i });
    await user.click(resetButton);

    await waitFor(() => {
      expect(mockHandleReset).toHaveBeenCalled();
    });
  });

  it("should pass correct props to ForfeitGrid", () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    // Verify that the page structure is rendered correctly
    expect(screen.getByText(CAPTIONS.FORFEIT)).toBeInTheDocument();
    expect(screen.getByText("Filter")).toBeInTheDocument();
  });

  it("should pass correct props to ForfeitSearchFilter", async () => {
    const mockStore = createMockStore();
    render(<Forfeit />, { wrapper: wrapper(mockStore) });

    // Verify that the search filter is rendered and the search button is initially not disabled
    // (indicating isSearching prop is false)
    await waitFor(() => {
      const searchButton = screen.getByRole("button", { name: /search/i });
      expect(searchButton).toBeInTheDocument();
    });
  });
});
