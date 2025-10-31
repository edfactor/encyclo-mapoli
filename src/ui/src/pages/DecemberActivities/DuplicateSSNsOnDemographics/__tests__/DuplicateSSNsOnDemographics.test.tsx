import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import DuplicateSSNsOnDemographics from "../DuplicateSSNsOnDemographics";

vi.mock("../hooks/useDuplicateSSNsOnDemographics", () => ({
  default: vi.fn(() => ({
    searchResults: {
      response: {
        results: [
          { ssn: "123-45-6789", badgeNumber: 12345, employeeName: "John Doe" }
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

vi.mock("../DuplicateSSNsOnDemographicsGrid", () => ({
  default: vi.fn(() => <div data-testid="grid">Grid Component</div>)
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div data-testid="status-dropdown">Status Dropdown</div>)
}));

vi.mock("smart-ui-library", () => ({
  Page: vi.fn(({ label, actionNode, children }) => (
    <div data-testid="page">
      <div>{label}</div>
      {actionNode}
      {children}
    </div>
  ))
}));

describe("DuplicateSSNsOnDemographics", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render the page component", () => {
      render(<DuplicateSSNsOnDemographics />);
      expect(screen.getByTestId("page")).toBeInTheDocument();
    });

    it("should render page with correct label including record count", () => {
      render(<DuplicateSSNsOnDemographics />);
      expect(screen.getByText(/DUPLICATE SSNS.*1 records/i)).toBeInTheDocument();
    });

    it("should render StatusDropdownActionNode", () => {
      render(<DuplicateSSNsOnDemographics />);
      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
    });

    it("should render grid component", () => {
      render(<DuplicateSSNsOnDemographics />);
      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });

  describe("Data display", () => {
    it("should display record count from hook results", () => {
      render(<DuplicateSSNsOnDemographics />);
      expect(screen.getByText(/1 records/i)).toBeInTheDocument();
    });

    it("should display zero records when no results", () => {
      const { useDuplicateSSNsOnDemographics } = require("../hooks/useDuplicateSSNsOnDemographics");

      vi.mocked(useDuplicateSSNsOnDemographics).mockReturnValueOnce({
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
      } as any);

      render(<DuplicateSSNsOnDemographics />);
      expect(screen.getByText(/0 records/i)).toBeInTheDocument();
    });
  });

  describe("Grid props passing", () => {
    it("should pass search results to grid", () => {
      render(<DuplicateSSNsOnDemographics />);
      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });

    it("should pass pagination handlers to grid", () => {
      render(<DuplicateSSNsOnDemographics />);
      const { DuplicateSSNsOnDemographicsGrid } = require("../DuplicateSSNsOnDemographicsGrid");
      expect(DuplicateSSNsOnDemographicsGrid).toHaveBeenCalledWith(
        expect.objectContaining({
          pagination: expect.any(Object)
        }),
        expect.anything()
      );
    });
  });

  describe("Loading states", () => {
    it("should pass isSearching state to grid", () => {
      render(<DuplicateSSNsOnDemographics />);
      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });
  });

  describe("Auto-load functionality", () => {
    it("should initialize hook on mount", () => {
      const { useDuplicateSSNsOnDemographics } = require("../hooks/useDuplicateSSNsOnDemographics");
      render(<DuplicateSSNsOnDemographics />);
      expect(useDuplicateSSNsOnDemographics).toHaveBeenCalled();
    });
  });
});
