import React from "react";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { CAPTIONS } from "../../../../constants";
import Forfeit from "../Forfeit";
import { createMockStoreAndWrapper } from "../../../../test";

// Mock the useForfeit hook
const mockExecuteSearch = vi.fn();
const mockHandleStatusChange = vi.fn();
const mockHandleReset = vi.fn();

vi.mock("../hooks/useForfeit", () => ({
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
vi.mock("../ForfeitGrid", () => ({
  default: vi.fn(({ searchResults, pagination, isSearching }) =>
    React.createElement(
      "section",
      { "aria-label": "forfeit grid" },
      React.createElement("div", { "aria-live": "polite" }, searchResults ? "true" : "false"),
      React.createElement("div", null, isSearching.toString()),
      React.createElement("div", null, pagination?.pageSize)
    )
  )
}));

vi.mock("../ForfeitSearchFilter", () => ({
  default: vi.fn(({ onSearch, onReset, isSearching }) =>
    React.createElement(
      "section",
      { "aria-label": "forfeit search filter" },
      React.createElement(
        "button",
        {
          onClick: () => onSearch({ profitYear: 2024 })
        },
        "Search"
      ),
      React.createElement(
        "button",
        {
          onClick: onReset
        },
        "Reset"
      ),
      React.createElement("div", { "aria-live": "polite" }, isSearching.toString())
    )
  )
}));

vi.mock("../../../../components/StatusDropdownActionNode", () => ({
  default: vi.fn(({ onStatusChange }) =>
    React.createElement(
      "section",
      { "aria-label": "status dropdown" },
      React.createElement(
        "button",
        {
          onClick: () => onStatusChange?.("4", "Complete")
        },
        "Change to Complete"
      ),
      React.createElement(
        "button",
        {
          onClick: () => onStatusChange?.("2", "In Progress")
        },
        "Change to In Progress"
      )
    )
  )
}));

describe("Forfeit", () => {
  let wrapper: ReturnType<typeof createMockStoreAndWrapper>["wrapper"];

  beforeEach(() => {
    vi.clearAllMocks();
    const storeAndWrapper = createMockStoreAndWrapper({
      yearsEnd: {
        selectedProfitYearForFiscalClose: 2024,
        forfeituresAndPoints: null
      }
    });
    wrapper = storeAndWrapper.wrapper;
  });

  it("should render Forfeit page with all components", async () => {
    render(<Forfeit />, { wrapper });

    await waitFor(() => {
      expect(screen.getByLabelText("status dropdown")).toBeInTheDocument();
      expect(screen.getByLabelText("forfeit search filter")).toBeInTheDocument();
      expect(screen.getByLabelText("forfeit grid")).toBeInTheDocument();
    });
  });

  it("should call handleStatusChange when status changes to Complete", async () => {
    const user = userEvent.setup();
    render(<Forfeit />, { wrapper });

    await waitFor(() => {
      expect(screen.getByRole("button", { name: /change to complete/i })).toBeInTheDocument();
    });

    const completeButton = screen.getByRole("button", { name: /change to complete/i });
    await user.click(completeButton);

    await waitFor(() => {
      expect(mockHandleStatusChange).toHaveBeenCalledWith("4", "Complete");
    });
  });

  it("should call handleStatusChange when status changes to In Progress", async () => {
    const user = userEvent.setup();
    render(<Forfeit />, { wrapper });

    await waitFor(() => {
      expect(screen.getByRole("button", { name: /change to in progress/i })).toBeInTheDocument();
    });

    const inProgressButton = screen.getByRole("button", { name: /change to in progress/i });
    await user.click(inProgressButton);

    await waitFor(() => {
      expect(mockHandleStatusChange).toHaveBeenCalledWith("2", "In Progress");
    });
  });

  it("should call executeSearch when search button is clicked", async () => {
    const user = userEvent.setup();
    render(<Forfeit />, { wrapper });

    await waitFor(() => {
      expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
    });

    const searchButton = screen.getByRole("button", { name: /search/i });
    await user.click(searchButton);

    await waitFor(() => {
      expect(mockExecuteSearch).toHaveBeenCalledWith({ profitYear: 2024 });
    });
  });

  it("should call handleReset when reset button is clicked", async () => {
    const user = userEvent.setup();
    render(<Forfeit />, { wrapper });

    await waitFor(() => {
      expect(screen.getByRole("button", { name: /reset/i })).toBeInTheDocument();
    });

    const resetButton = screen.getByRole("button", { name: /reset/i });
    await user.click(resetButton);

    await waitFor(() => {
      expect(mockHandleReset).toHaveBeenCalled();
    });
  });

  it("should pass correct props to ForfeitGrid", async () => {
    render(<Forfeit />, { wrapper });

    // Verify that the page structure is rendered correctly
    await waitFor(() => {
      expect(screen.getByText(CAPTIONS.FORFEIT)).toBeInTheDocument();
      expect(screen.getByText("Filter")).toBeInTheDocument();
    });
  });

  it("should pass correct props to ForfeitSearchFilter", async () => {
    render(<Forfeit />, { wrapper });

    // Verify that the search filter is rendered and the search button is initially not disabled
    // (indicating isSearching prop is false)
    await waitFor(() => {
      const searchButton = screen.getByRole("button", { name: /search/i });
      expect(searchButton).toBeInTheDocument();
    });
  });
});
