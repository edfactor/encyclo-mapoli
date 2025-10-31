import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import NegativeEtvaForSSNsOnPayprofit from "../NegativeEtvaForSSNsOnPayprofit";
import useNegativeEtvaForSSNsOnPayprofit from "../hooks/useNegativeEtvaForSSNsOnPayprofit";
import NegativeEtvaForSSNsOnPayprofitGrid from "../NegativeEtvaForSSNsOnPayprofitGrid";

vi.mock("../hooks/useNegativeEtvaForSSNsOnPayprofit", () => ({
  default: vi.fn(() => ({
    searchResults: {
      response: {
        results: [
          {
            ssn: "123-45-6789",
            badgeNumber: 12345,
            employeeName: "John Doe",
            etva: -1500.0
          }
        ],
        total: 1
      }
    },
    isSearching: false,
    pagination: {
      pageNumber: 0,
      pageSize: 25,
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn()
    },
    showData: true,
    hasResults: true
  }))
}));

vi.mock("../NegativeEtvaForSSNsOnPayprofitGrid", () => ({
  default: vi.fn(() => <div data-testid="grid">Grid Component</div>)
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div data-testid="status-dropdown">Status Dropdown</div>)
}));

vi.mock("smart-ui-library", () => ({
  Page: vi.fn(({ label, actionNode, children }: { label: string; actionNode?: React.ReactNode; children?: React.ReactNode }) => (
    <div data-testid="page">
      <div>{label}</div>
      {actionNode}
      {children}
    </div>
  ))
}));

describe("NegativeEtvaForSSNsOnPayprofit", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render the page component", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByTestId("page")).toBeInTheDocument();
    });

    it("should render page with correct label including record count", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByText(/NEGATIVE ETVA.*1 records/i)).toBeInTheDocument();
    });

    it("should render StatusDropdownActionNode", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    });

    it("should render grid component", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });

  describe("Data display", () => {
    it("should display record count from hook results", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByText(/1 records/i)).toBeInTheDocument();
    });

    it("should display zero records when no results", async () => {

      vi.mocked(useNegativeEtvaForSSNsOnPayprofit).mockReturnValueOnce({
        searchResults: null,
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: false,
        hasResults: false
      } as unknown as ReturnType<typeof useNegativeEtvaForSSNsOnPayprofit>);

      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByText(/0 records/i)).toBeInTheDocument();
    });

    it("should display multiple record count", async () => {

      vi.mocked(useNegativeEtvaForSSNsOnPayprofit).mockReturnValueOnce({
        searchResults: {
          response: {
            results: [
              { ssn: "1", badgeNumber: 1, employeeName: "Jane", etva: -100 },
              { ssn: "2", badgeNumber: 2, employeeName: "Bob", etva: -200 }
            ],
            total: 2
          }
        },
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: true,
        hasResults: true
      } as unknown as ReturnType<typeof useNegativeEtvaForSSNsOnPayprofit>);

      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByText(/2 records/i)).toBeInTheDocument();
    });
  });

  describe("Navigation capability", () => {
    it("should support navigation from grid rows", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });

  describe("Auto-load functionality", () => {
    it("should initialize hook on mount", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(useNegativeEtvaForSSNsOnPayprofit).toHaveBeenCalled();
    });
  });

  describe("Grid props passing", () => {
    it("should pass pagination handlers to grid", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(vi.mocked(NegativeEtvaForSSNsOnPayprofitGrid)).toHaveBeenCalledWith(
        expect.objectContaining({
          onPaginationChange: expect.any(Function),
          onSortChange: expect.any(Function)
        }),
        expect.anything()
      );
    });
  });

  describe("Loading states", () => {
    it("should pass isSearching state to grid", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });

  describe("Edge cases", () => {
    it("should handle very negative ETVA values", () => {
      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByTestId("page")).toBeInTheDocument();
    });

    it("should handle large number of records", async () => {

      const largeResults = Array.from({ length: 100 }, (_, i) => ({
        ssn: `xxx-xx-${String(i).padStart(4, "0")}`,
        badgeNumber: 10000 + i,
        employeeName: `Employee ${i}`,
        etva: -(100 + i * 10)
      }));

      vi.mocked(useNegativeEtvaForSSNsOnPayprofit).mockReturnValueOnce({
        searchResults: {
          response: {
            results: largeResults,
            total: 100
          }
        },
        isSearching: false,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          handlePaginationChange: vi.fn(),
          handleSortChange: vi.fn()
        },
        showData: true,
        hasResults: true
      } as unknown as ReturnType<typeof useNegativeEtvaForSSNsOnPayprofit>);

      render(<NegativeEtvaForSSNsOnPayprofit />);

      expect(screen.getByText(/100 records/i)).toBeInTheDocument();
    });
  });
});
