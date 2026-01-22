import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../../test";
import StoreContent from "../StoreContent";

// Hoist mock functions
const { mockUseDecemberFlowProfitYear } = vi.hoisted(() => ({
  mockUseDecemberFlowProfitYear: vi.fn(() => 2024)
}));

// Mock child components
vi.mock("../StoreManagementGrid", () => ({
  default: vi.fn(({ isGridExpanded, onToggleExpand }) => (
    <div data-testid="store-management-grid">
      Store Management Grid
      {isGridExpanded !== undefined && (
        <button data-testid="management-expand-btn" onClick={onToggleExpand}>
          {isGridExpanded ? "Collapse" : "Expand"}
        </button>
      )}
    </div>
  ))
}));

vi.mock("../AssociatesGrid", () => ({
  default: vi.fn(({ isGridExpanded, onToggleExpand }) => (
    <div data-testid="associates-grid">
      Associates Grid
      {isGridExpanded !== undefined && (
        <button data-testid="associates-expand-btn" onClick={onToggleExpand}>
          {isGridExpanded ? "Collapse" : "Expand"}
        </button>
      )}
    </div>
  ))
}));

// Mock hooks
vi.mock("../../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: mockUseDecemberFlowProfitYear
}));

vi.mock("../../../../../hooks/useGridExpansion", () => ({
  useGridExpansion: () => ({
    isGridExpanded: false,
    handleToggleGridExpand: vi.fn()
  })
}));

describe("StoreContent", () => {
  const mockOnGridExpandChange = vi.fn();
  const mockOnLoadingChange = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should render store title when provided", () => {
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: {
        breakdownByStoreManagement: {},
        breakdownByStore: {},
        breakdownByStoreQueryParams: null
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <StoreContent
        store={1}
        refetchTrigger={0}
        onGridExpandChange={mockOnGridExpandChange}
        onLoadingChange={mockOnLoadingChange}
      />,
      { wrapper }
    );

    expect(screen.getByText("Store 1")).toBeInTheDocument();
  });

  it("should render both grids", () => {
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: {
        breakdownByStoreManagement: {},
        breakdownByStore: {},
        breakdownByStoreQueryParams: null
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <StoreContent
        store={1}
        refetchTrigger={0}
        onGridExpandChange={mockOnGridExpandChange}
        onLoadingChange={mockOnLoadingChange}
      />,
      { wrapper }
    );

    expect(screen.getByTestId("store-management-grid")).toBeInTheDocument();
    expect(screen.getByTestId("associates-grid")).toBeInTheDocument();
  });

  it("should show no store selected message when store is null", () => {
    const { wrapper } = createMockStoreAndWrapper({
      yearsEnd: {
        breakdownByStoreManagement: {},
        breakdownByStore: {},
        breakdownByStoreQueryParams: null
      },
      security: {
        token: "mock-token"
      }
    });

    render(
      <StoreContent
        store={null}
        refetchTrigger={0}
        onGridExpandChange={mockOnGridExpandChange}
        onLoadingChange={mockOnLoadingChange}
      />,
      { wrapper }
    );

    expect(screen.getByText("No Store Selected")).toBeInTheDocument();
  });
});
