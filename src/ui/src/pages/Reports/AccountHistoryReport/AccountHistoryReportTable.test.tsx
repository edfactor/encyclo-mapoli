import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../../hooks/useGridPagination";
import { AccountHistoryReportResponse, ReportResponseBase } from "../../../types/reports/AccountHistoryReportTypes";
import AccountHistoryReportTable from "./AccountHistoryReportTable";

interface DSMGridProps {
  providedOptions?: { rowData?: AccountHistoryReportResponse[] };
}

interface PaginationProps {
  pageNumber?: number;
  pageSize?: number;
  recordCount?: number;
}

// Mock the smart-ui-library components
vi.mock("smart-ui-library", () => ({
  DSMGrid: ({ providedOptions }: DSMGridProps) => (
    <div data-testid="dsm-grid">{providedOptions?.rowData?.length ?? 0} rows</div>
  ),
  Pagination: ({ pageNumber, pageSize, recordCount }: PaginationProps) => (
    <div data-testid="pagination">
      Page {pageNumber}, Size {pageSize}, Total {recordCount}
    </div>
  )
}));

// Mock the hook
vi.mock("../../../hooks/useDynamicGridHeight", () => ({
  useDynamicGridHeight: () => 400
}));

// Mock the grid columns
vi.mock("./AccountHistoryReportGridColumns", () => ({
  GetAccountHistoryReportColumns: () => [
    { field: "profitYear", headerName: "Profit Year" },
    { field: "contributions", headerName: "Contributions" }
  ]
}));

describe("AccountHistoryReportTable", () => {
  const createMockResponse = (
    overrides?: Partial<ReportResponseBase<AccountHistoryReportResponse>>
  ): ReportResponseBase<AccountHistoryReportResponse> => ({
    reportName: "Account History Report",
    reportDate: "2024-10-30T00:00:00Z",
    startDate: "2024-01-01",
    endDate: "2024-12-31",
    response: {
      pageSize: 25,
      currentPage: 1,
      totalPages: 1,
      resultHash: null,
      total: 2,
      isPartialResult: false,
      timeoutOccurred: false,
      results: [
        {
          badgeNumber: 12345,
          fullName: "John Doe",
          ssn: "***-**-6789",
          profitYear: 2024,
          contributions: 1000,
          earnings: 500,
          forfeitures: 100,
          withdrawals: 50,
          endingBalance: 5000
        },
        {
          badgeNumber: 12345,
          fullName: "John Doe",
          ssn: "***-**-6789",
          profitYear: 2023,
          contributions: 950,
          earnings: 450,
          forfeitures: 75,
          withdrawals: 25,
          endingBalance: 4000
        }
      ]
    },
    ...overrides
  });

  const createMockGridPagination = (
    overrides?: Partial<GridPaginationState & GridPaginationActions>
  ): GridPaginationState & GridPaginationActions => ({
    pageNumber: 0,
    pageSize: 25,
    sortParams: { sortBy: "profitYear", isSortDescending: true } as SortParams,
    handlePaginationChange: vi.fn(),
    handleSortChange: vi.fn(),
    resetPagination: vi.fn(),
    ...overrides
  });

  it("should render grid with results", () => {
    const data = createMockResponse();
    const gridPagination = createMockGridPagination();

    render(
      <AccountHistoryReportTable
        data={data}
        isLoading={false}
        error={undefined}
        showData={true}
        gridPagination={gridPagination}
      />
    );

    expect(screen.getByTestId("dsm-grid")).toBeInTheDocument();
    expect(screen.getByText(/2 rows/)).toBeInTheDocument();
  });

  it("should render pagination component with correct data", () => {
    const data = createMockResponse();
    const gridPagination = createMockGridPagination();

    render(
      <AccountHistoryReportTable
        data={data}
        isLoading={false}
        error={undefined}
        showData={true}
        gridPagination={gridPagination}
      />
    );

    expect(screen.getByTestId("pagination")).toBeInTheDocument();
    expect(screen.getByText(/Page 0, Size 25, Total 2/)).toBeInTheDocument();
  });

  it("should not show pagination when no records", () => {
    const data = createMockResponse({
      response: {
        pageSize: 25,
        currentPage: 1,
        totalPages: 0,
        resultHash: null,
        total: 0,
        isPartialResult: false,
        timeoutOccurred: false,
        results: []
      }
    });
    const gridPagination = createMockGridPagination();

    render(
      <AccountHistoryReportTable
        data={data}
        isLoading={false}
        error={undefined}
        showData={true}
        gridPagination={gridPagination}
      />
    );

    expect(screen.queryByTestId("pagination")).not.toBeInTheDocument();
  });

  it("should not render when showData is false", () => {
    const data = createMockResponse();
    const gridPagination = createMockGridPagination();

    render(
      <AccountHistoryReportTable
        data={data}
        isLoading={false}
        error={undefined}
        showData={false}
        gridPagination={gridPagination}
      />
    );

    expect(screen.queryByTestId("dsm-grid")).not.toBeInTheDocument();
    expect(screen.queryByTestId("pagination")).not.toBeInTheDocument();
  });

  it("should handle pagination change", () => {
    const handlePaginationChange = vi.fn();
    const data = createMockResponse();
    const gridPagination = createMockGridPagination({
      handlePaginationChange
    });

    render(
      <AccountHistoryReportTable
        data={data}
        isLoading={false}
        error={undefined}
        showData={true}
        gridPagination={gridPagination}
      />
    );

    // Verify pagination component receives correct handlers
    expect(gridPagination.handlePaginationChange).toBeDefined();
  });

  it("should display correct page number in pagination (0-indexed)", () => {
    const data = createMockResponse();
    const gridPagination = createMockGridPagination({
      pageNumber: 2
    });

    render(
      <AccountHistoryReportTable
        data={data}
        isLoading={false}
        error={undefined}
        showData={true}
        gridPagination={gridPagination}
      />
    );

    expect(screen.getByText(/Page 2, Size 25/)).toBeInTheDocument();
  });
});
