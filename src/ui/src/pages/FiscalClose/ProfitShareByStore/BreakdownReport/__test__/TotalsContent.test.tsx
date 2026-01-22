import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../../test";
import TotalsContent from "../TotalsContent";

// Hoist mock functions
const {
  mockGetBreakdownByStoreTotals,
  mockUseDecemberFlowProfitYear,
  mockNumberToCurrency
} = vi.hoisted(() => ({
  mockGetBreakdownByStoreTotals: vi.fn(),
  mockUseDecemberFlowProfitYear: vi.fn(() => 2024),
  mockNumberToCurrency: vi.fn((value: number, decimals: number) => 
    `$${value.toLocaleString("en-US", { minimumFractionDigits: decimals, maximumFractionDigits: decimals })}`)
}));

// Mock the API hook and preserve AdhocApi
vi.mock("../../../../../reduxstore/api/AdhocApi", async (importOriginal) => {
  const actual = await importOriginal() as Record<string, unknown>;
  return {
    ...actual,
    useLazyGetBreakdownByStoreTotalsQuery: () => [
      mockGetBreakdownByStoreTotals,
      { isFetching: false }
    ]
  };
});

// Mock hooks
vi.mock("../../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: mockUseDecemberFlowProfitYear
}));

// Mock numberToCurrency
vi.mock("smart-ui-library", () => ({
  numberToCurrency: mockNumberToCurrency
}));

describe("TotalsContent", () => {
  const mockOnLoadingChange = vi.fn();

  const mockTotalsData = {
    totalNumberEmployees: 56,
    totalBeginningBalances: 5544164.89,
    totalEarnings: 384422.28,
    totalContributions: 423855,
    totalForfeitures: 26685.74,
    totalDisbursements: -33.89,
    totalEndBalances: 6379094.02,
    totalVestedBalance: 6258692.5
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should render totals section title for individual store", () => {
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: {
        breakdownByStoreTotals: mockTotalsData,
        breakdownGrandTotals: null
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <TotalsContent
        store={1}
        onLoadingChange={mockOnLoadingChange}
      />,
      { wrapper }
    );

    expect(screen.getByText(/totals for store 1/i)).toBeInTheDocument();
  });

  it("should display total fields when data is available", () => {
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: {
        breakdownByStoreTotals: mockTotalsData,
        breakdownGrandTotals: null
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <TotalsContent
        store={1}
        onLoadingChange={mockOnLoadingChange}
      />,
      { wrapper }
    );

    expect(screen.getByText(/total number of employees/i)).toBeInTheDocument();
    expect(screen.getByText(/total end balances/i)).toBeInTheDocument();
  });
});
