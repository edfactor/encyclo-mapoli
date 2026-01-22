import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../../test";
import Under21Content from "../Under21Content";
import BreakdownSummaryGrid from "../BreakdownSummaryGrid";
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

describe("Under21Content", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should render title", () => {
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: {
        breakdownGrandTotals: { rows: [] }
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <Under21Content />,
      { wrapper }
    );

    expect(screen.getByText(/under 21 employees/i)).toBeInTheDocument();
  });

  it("should render component for all stores", () => {
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: {
        breakdownGrandTotals: { rows: [] }
      },
      security: {
        token: "mock-token"
      }
    });

    const { container } = render(
      <Under21Content />,
      { wrapper }
    );

    // Should have some content rendered
    expect(container.firstChild).not.toBeNull();
  });
});

describe("BreakdownSummaryGrid", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should render with custom title", () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    render(
      <BreakdownSummaryGrid
        title="Test Title"
        preferenceKey="test-key"
      />,
      { wrapper }
    );

    expect(screen.getByText("Test Title")).toBeInTheDocument();
  });

  it("should pass under21Participants to hook", () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    render(
      <BreakdownSummaryGrid
        title="Under 21"
        preferenceKey={GRID_KEYS.UNDER_21_BREAKDOWN_REPORT}
        under21Participants
      />,
      { wrapper }
    );

    expect(mockUseBreakdownGrandTotals).toHaveBeenCalledWith(
      expect.objectContaining({ under21Participants: true })
    );
  });

  it("should show loading state", () => {
    mockUseBreakdownGrandTotals.mockReturnValueOnce({
      rowData: [],
      grandTotal: { category: "", ste1: 0, "700": 0, "701": 0, "800": 0, "801": 0, "802": 0, "900": 0, total: 0 },
      isLoading: true,
      error: null
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    render(
      <BreakdownSummaryGrid title="Test" preferenceKey="test" />,
      { wrapper }
    );

    expect(screen.getByRole("progressbar")).toBeInTheDocument();
  });

  it("should show error state", () => {
    mockUseBreakdownGrandTotals.mockReturnValueOnce({
      rowData: [],
      grandTotal: { category: "", ste1: 0, "700": 0, "701": 0, "800": 0, "801": 0, "802": 0, "900": 0, total: 0 },
      isLoading: false,
      error: "Test error message"
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    render(
      <BreakdownSummaryGrid title="Test" preferenceKey="test" />,
      { wrapper }
    );

    expect(screen.getByText("Test error message")).toBeInTheDocument();
  });

  it("should render grid when data is loaded", () => {
    mockUseBreakdownGrandTotals.mockReturnValueOnce({
      rowData: [
        { category: "100% Vested", ste1: 10, "700": 5, "701": 3, "800": 2, "801": 1, "802": 0, "900": 4, total: 25 }
      ],
      grandTotal: { category: "Grand Total", ste1: 10, "700": 5, "701": 3, "800": 2, "801": 1, "802": 0, "900": 4, total: 25 },
      isLoading: false,
      error: null
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    render(
      <BreakdownSummaryGrid title="Test" preferenceKey="test-grid-key" />,
      { wrapper }
    );

    expect(screen.getByTestId("dsm-grid")).toBeInTheDocument();
    expect(screen.getByTestId("dsm-grid")).toHaveAttribute("data-preference-key", "test-grid-key");
  });
});
