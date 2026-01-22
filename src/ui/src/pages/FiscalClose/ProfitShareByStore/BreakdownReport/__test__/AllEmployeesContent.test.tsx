import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../../test";
import AllEmployeesContent from "../AllEmployeesContent";
import { GRID_KEYS } from "../../../../../constants";

// Hoist mock functions
const {
  mockUseBreakdownGrandTotals
} = vi.hoisted(() => ({
  mockUseBreakdownGrandTotals: vi.fn((): {
    rowData: Array<{ category: string; ste1: number; "700": number; "701": number; "800": number; "801": number; "802": number; "900": number; total: number }>;
    grandTotal: { category: string; ste1: number; "700": number; "701": number; "800": number; "801": number; "802": number; "900": number; total: number };
    isLoading: boolean;
    error: string | null;
  } => ({
    rowData: [],
    grandTotal: { category: "Grand Total", ste1: 0, "700": 0, "701": 0, "800": 0, "801": 0, "802": 0, "900": 0, total: 0 },
    isLoading: false,
    error: null
  }))
}));

// Mock the shared hook
vi.mock("../../../../../hooks/useBreakdownGrandTotals", () => ({
  useBreakdownGrandTotals: mockUseBreakdownGrandTotals,
  default: mockUseBreakdownGrandTotals
}));

// Mock DSMGrid
vi.mock("smart-ui-library", () => ({
  DSMGrid: ({ preferenceKey }: { preferenceKey: string }) => (
    <div data-testid="dsm-grid" data-preference-key={preferenceKey}>
      Mocked Grid
    </div>
  )
}));

describe("AllEmployeesContent", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should render title", () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: {
        token: "mock-token"
      }
    });

    render(<AllEmployeesContent />, { wrapper });

    expect(screen.getByText(/all employees/i)).toBeInTheDocument();
  });

  it("should render with correct grid preference key", () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: {
        token: "mock-token"
      }
    });

    render(<AllEmployeesContent />, { wrapper });

    const grid = screen.getByTestId("dsm-grid");
    expect(grid).toHaveAttribute("data-preference-key", GRID_KEYS.BREAKDOWN_REPORT_SUMMARY);
  });

  it("should not pass under21Participants to hook", () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    render(<AllEmployeesContent />, { wrapper });

    // Should be called without under21Participants or with it set to false/undefined
    expect(mockUseBreakdownGrandTotals).toHaveBeenCalledWith(
      expect.not.objectContaining({ under21Participants: true })
    );
  });

  it("should show loading state when hook returns isLoading", () => {
    mockUseBreakdownGrandTotals.mockReturnValueOnce({
      rowData: [],
      grandTotal: { category: "", ste1: 0, "700": 0, "701": 0, "800": 0, "801": 0, "802": 0, "900": 0, total: 0 },
      isLoading: true,
      error: null
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    render(<AllEmployeesContent />, { wrapper });

    expect(screen.getByRole("progressbar")).toBeInTheDocument();
  });

  it("should show error state when hook returns error", () => {
    mockUseBreakdownGrandTotals.mockReturnValueOnce({
      rowData: [],
      grandTotal: { category: "", ste1: 0, "700": 0, "701": 0, "800": 0, "801": 0, "802": 0, "900": 0, total: 0 },
      isLoading: false,
      error: "Failed to load All Employees data. Please try again."
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    render(<AllEmployeesContent />, { wrapper });

    expect(screen.getByText(/failed to load all employees data/i)).toBeInTheDocument();
  });

  it("should render grid when data is loaded successfully", () => {
    mockUseBreakdownGrandTotals.mockReturnValueOnce({
      rowData: [
        { category: "100% Vested", ste1: 100, "700": 50, "701": 30, "800": 20, "801": 10, "802": 5, "900": 40, total: 255 },
        { category: "Partially Vested", ste1: 50, "700": 25, "701": 15, "800": 10, "801": 5, "802": 2, "900": 20, total: 127 },
        { category: "Not Vested", ste1: 25, "700": 12, "701": 8, "800": 5, "801": 3, "802": 1, "900": 10, total: 64 }
      ],
      grandTotal: { category: "Grand Total", ste1: 175, "700": 87, "701": 53, "800": 35, "801": 18, "802": 8, "900": 70, total: 446 },
      isLoading: false,
      error: null
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    render(<AllEmployeesContent />, { wrapper });

    expect(screen.getByTestId("dsm-grid")).toBeInTheDocument();
  });
});
