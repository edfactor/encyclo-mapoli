import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../../test";
import QPAY066TABreakdownParameters from "../QPAY066TABreakdownParameters";

// Hoist mock functions
const { mockUseDecemberFlowProfitYear } = vi.hoisted(() => ({
  mockUseDecemberFlowProfitYear: vi.fn(() => 2024)
}));

// Mock SearchAndReset component
vi.mock("smart-ui-library", () => ({
  SearchAndReset: vi.fn(({ handleSearch, handleReset, disabled }) => (
    <div data-testid="search-and-reset">
      <button
        data-testid="search-btn"
        onClick={handleSearch}
        disabled={disabled}>
        Search
      </button>
      <button
        data-testid="reset-btn"
        onClick={handleReset}>
        Reset
      </button>
    </div>
  ))
}));

// Mock DuplicateSsnGuard
vi.mock("../../../../../components/DuplicateSsnGuard", () => ({
  default: ({ children }: { children: ({ prerequisitesComplete }: { prerequisitesComplete: boolean }) => React.ReactElement }) =>
    children({ prerequisitesComplete: true })
}));

// Mock hooks
vi.mock("../../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: mockUseDecemberFlowProfitYear
}));

describe("QPAY066TABreakdownParameters", () => {
  const mockOnStoreChange = vi.fn();
  const mockOnReset = vi.fn();
  const mockOnSearch = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should render form fields", () => {
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: { 
        selectedProfitYear: 2024,
        breakdownByStoreQueryParams: null
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <QPAY066TABreakdownParameters
        onStoreChange={mockOnStoreChange}
        onReset={mockOnReset}
        isLoading={false}
        onSearch={mockOnSearch}
      />,
      { wrapper }
    );

    expect(screen.getByLabelText(/badge number/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/employee name/i)).toBeInTheDocument();
  });

  it("should render search and reset buttons", () => {
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: { 
        selectedProfitYear: 2024,
        breakdownByStoreQueryParams: null
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <QPAY066TABreakdownParameters
        onStoreChange={mockOnStoreChange}
        onReset={mockOnReset}
        isLoading={false}
        onSearch={mockOnSearch}
      />,
      { wrapper }
    );

    expect(screen.getByTestId("search-btn")).toBeInTheDocument();
    expect(screen.getByTestId("reset-btn")).toBeInTheDocument();
  });

  it("should call onReset when reset button is clicked", async () => {
    const user = userEvent.setup();
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: { 
        selectedProfitYear: 2024,
        breakdownByStoreQueryParams: null
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <QPAY066TABreakdownParameters
        onStoreChange={mockOnStoreChange}
        onReset={mockOnReset}
        isLoading={false}
        onSearch={mockOnSearch}
      />,
      { wrapper }
    );

    const resetBtn = screen.getByTestId("reset-btn");
    await user.click(resetBtn);

    await waitFor(() => {
      expect(mockOnReset).toHaveBeenCalled();
    });
  });

  it("should call onSearch when search button is clicked", async () => {
    const user = userEvent.setup();
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: { 
        selectedProfitYear: 2024,
        breakdownByStoreQueryParams: null
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <QPAY066TABreakdownParameters
        onStoreChange={mockOnStoreChange}
        onReset={mockOnReset}
        isLoading={false}
        onSearch={mockOnSearch}
      />,
      { wrapper }
    );

    const searchBtn = screen.getByTestId("search-btn");
    await user.click(searchBtn);

    await waitFor(() => {
      expect(mockOnSearch).toHaveBeenCalled();
    });
  });
});
