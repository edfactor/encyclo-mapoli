import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi, beforeEach } from "vitest";

vi.mock("smart-ui-library", () => ({
  DSMGrid: vi.fn(({ isLoading, providedOptions }) => (
    <div data-testid="grid">
      {isLoading && <div data-testid="loading">Loading...</div>}
      <div data-testid="grid-data">{JSON.stringify(providedOptions?.rowData)}</div>
    </div>
  )),
  Pagination: vi.fn(({ pageNumber, pageSize, recordCount }) => (
    <div data-testid="pagination">
      Page {pageNumber} - Size {pageSize} - Total {recordCount}
    </div>
  ))
}));

vi.mock("../DuplicateSSNsOnDemographicsGridColumns", () => ({
  GetDuplicateSSNsOnDemographicsColumns: () => [
    { field: "ssn", headerName: "SSN" },
    { field: "badgeNumber", headerName: "Badge" },
    { field: "employeeName", headerName: "Name" }
  ]
}));

describe("DuplicateSSNsOnDemographicsGrid", () => {
  const mockData = {
    response: {
      results: [
        { ssn: "123-45-6789", badgeNumber: 12345, employeeName: "John Doe" }
      ],
      total: 1
    }
  };

  const mockPagination = {
    pageNumber: 0,
    pageSize: 25,
    sortParams: { sortBy: "ssn", isSortDescending: true },
    handlePaginationChange: vi.fn(),
    handleSortChange: vi.fn()
  };

  const defaultProps = {
    innerRef: { current: null },
    data: mockData,
    isLoading: false,
    showData: true,
    hasResults: true,
    pagination: mockPagination,
    onPaginationChange: vi.fn(),
    onSortChange: vi.fn()
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("Rendering", () => {
    it("should render grid when showData is true", () => {
      const Grid = require("../DuplicateSSNsOnDemographicsGrid").default;
      render(<Grid {...defaultProps} />);
      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });

    it("should not render grid when showData is false", () => {
      const Grid = require("../DuplicateSSNsOnDemographicsGrid").default;
      render(<Grid {...defaultProps} showData={false} />);
      expect(screen.queryByTestId("grid")).not.toBeInTheDocument();
    });

    it("should display loading state", () => {
      const Grid = require("../DuplicateSSNsOnDemographicsGrid").default;
      render(<Grid {...defaultProps} isLoading={true} />);
      expect(screen.getByTestId("loading")).toBeInTheDocument();
    });
  });

  describe("Pagination", () => {
    it("should render pagination when hasResults is true", () => {
      const Grid = require("../DuplicateSSNsOnDemographicsGrid").default;
      render(<Grid {...defaultProps} hasResults={true} />);
      expect(screen.getByTestId("pagination")).toBeInTheDocument();
    });

    it("should not render pagination when hasResults is false", () => {
      const Grid = require("../DuplicateSSNsOnDemographicsGrid").default;
      render(<Grid {...defaultProps} hasResults={false} />);
      expect(screen.queryByTestId("pagination")).not.toBeInTheDocument();
    });
  });

  describe("Data handling", () => {
    it("should handle empty results", () => {
      const Grid = require("../DuplicateSSNsOnDemographicsGrid").default;
      const emptyData = {
        response: {
          results: [],
          total: 0
        }
      };
      render(<Grid {...defaultProps} data={emptyData} hasResults={false} />);
      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });

    it("should handle null data", () => {
      const Grid = require("../DuplicateSSNsOnDemographicsGrid").default;
      render(<Grid {...defaultProps} data={null} showData={false} />);
      expect(screen.queryByTestId("grid")).not.toBeInTheDocument();
    });
  });
});
