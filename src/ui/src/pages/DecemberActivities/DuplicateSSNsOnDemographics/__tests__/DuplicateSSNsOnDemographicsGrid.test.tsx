import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import DuplicateSSNsOnDemographicsGrid from "../DuplicateSSNsOnDemographicsGrid";

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
    reportName: "Duplicate SSNs on Demographics",
    reportDate: "2024-01-15",
    startDate: "2024-01-01",
    endDate: "2024-12-31",
    dataSource: "Test Data",
    response: {
      results: [
        {
          ssn: "123-45-6789",
          badgeNumber: 12345,
          name: "John Doe",
          address: {
            street: "123 Main St",
            street2: null,
            city: "Boston",
            state: "MA",
            postalCode: "02101",
            countryIso: "US"
          },
          hireDate: "2020-01-01",
          terminationDate: null,
          rehireDate: null,
          status: "A",
          employmentStatusName: "Active",
          storeNumber: 100,
          profitSharingRecords: 5,
          hoursCurrentYear: 2080,
          hoursLastYear: 2080,
          incomeCurrentYear: 50000
        }
      ],
      total: 1,
      totalPages: 1,
      pageSize: 25,
      currentPage: 0
    }
  };

  const mockPagination = {
    pageNumber: 0,
    pageSize: 25,
    sortParams: { sortBy: "ssn", isSortDescending: true },
    handlePaginationChange: vi.fn(),
    handleSortChange: vi.fn(),
    resetPagination: vi.fn()
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
      render(<DuplicateSSNsOnDemographicsGrid {...defaultProps} />);
      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });

    it("should not render grid when showData is false", () => {
      render(
        <DuplicateSSNsOnDemographicsGrid
          {...defaultProps}
          showData={false}
        />
      );
      expect(screen.queryByTestId("grid")).not.toBeInTheDocument();
    });

    it("should display loading state", () => {
      render(
        <DuplicateSSNsOnDemographicsGrid
          {...defaultProps}
          isLoading={true}
        />
      );
      expect(screen.getByTestId("loading")).toBeInTheDocument();
    });
  });

  describe("Pagination", () => {
    it("should render pagination when hasResults is true", () => {
      render(
        <DuplicateSSNsOnDemographicsGrid
          {...defaultProps}
          hasResults={true}
        />
      );
      expect(screen.getByTestId("pagination")).toBeInTheDocument();
    });

    it("should not render pagination when hasResults is false", () => {
      render(
        <DuplicateSSNsOnDemographicsGrid
          {...defaultProps}
          hasResults={false}
        />
      );
      expect(screen.queryByTestId("pagination")).not.toBeInTheDocument();
    });
  });

  describe("Data handling", () => {
    it("should handle empty results", () => {
      const emptyData = {
        reportName: "Duplicate SSNs on Demographics",
        reportDate: "2024-01-15",
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        dataSource: "Test Data",
        response: {
          results: [],
          total: 0,
          totalPages: 0,
          pageSize: 25,
          currentPage: 0
        }
      };
      render(
        <DuplicateSSNsOnDemographicsGrid
          {...defaultProps}
          data={emptyData}
          hasResults={false}
        />
      );
      expect(screen.getByTestId("grid")).toBeInTheDocument();
    });

    it("should handle null data", () => {
      render(
        <DuplicateSSNsOnDemographicsGrid
          {...defaultProps}
          data={null}
          showData={false}
        />
      );
      expect(screen.queryByTestId("grid")).not.toBeInTheDocument();
    });
  });
});
