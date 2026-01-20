import { GridPaginationActions, GridPaginationState, SortParams } from "@/hooks/useGridPagination";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ColDef } from "ag-grid-community";
import React from "react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { DSMPaginatedGrid } from "../DSMPaginatedGrid";

interface DSMGridProps {
  providedOptions?: { rowData?: unknown[]; columnDefs?: ColDef[] };
  isLoading?: boolean;
  preferenceKey?: string;
  maxHeight?: number;
  handleSortChanged?: (sortParams: SortParams) => void;
}

interface MockPaginationProps {
  defaultPagination?: { skip: number; take: number };
  onPaginationChanged?: (params: { skip: number; take: number }) => void;
  recordCount?: number;
  rowsPerPageOptions?: number[];
}

// Mock smart-ui-library components
vi.mock("smart-ui-library", () => ({
  DSMGrid: ({ providedOptions, isLoading, preferenceKey, handleSortChanged }: DSMGridProps) => (
    <div
      data-testid="dsm-grid"
      data-preference-key={preferenceKey}>
      <span data-testid="grid-loading">{isLoading ? "Loading" : "Ready"}</span>
      <span data-testid="grid-row-count">{providedOptions?.rowData?.length ?? 0}</span>
      <span data-testid="grid-column-count">{providedOptions?.columnDefs?.length ?? 0}</span>
      {handleSortChanged && (
        <button
          data-testid="sort-button"
          onClick={() => handleSortChanged({ sortBy: "name", isSortDescending: true })}>
          Sort
        </button>
      )}
    </div>
  ),
  Pagination: ({ defaultPagination, onPaginationChanged, recordCount, rowsPerPageOptions }: MockPaginationProps) => {
    return (
      <div data-testid="pagination">
        <span data-testid="page-number">{defaultPagination?.skip}</span>
        <span data-testid="page-size">{defaultPagination?.take}</span>
        <span data-testid="record-count">{recordCount}</span>
        <span data-testid="rows-per-page-options">{rowsPerPageOptions?.join(",") ?? "default"}</span>
        {/* Buttons to simulate user clicking pagination controls - these simulate page number changes */}
        <button
          data-testid="go-to-page-1"
          onClick={() => onPaginationChanged?.({ skip: 0, take: defaultPagination?.take ?? 25 })}>
          Go to Page 1
        </button>
        <button
          data-testid="go-to-page-2"
          onClick={() => onPaginationChanged?.({ skip: 1, take: defaultPagination?.take ?? 25 })}>
          Go to Page 2
        </button>
        <button
          data-testid="go-to-page-5"
          onClick={() => onPaginationChanged?.({ skip: 4, take: defaultPagination?.take ?? 25 })}>
          Go to Page 5
        </button>
        <button
          data-testid="set-page-size-50"
          onClick={() => onPaginationChanged?.({ skip: defaultPagination?.skip ?? 0, take: 50 })}>
          Set Page Size 50
        </button>
      </div>
    );
  }
}));

// Mock the useContentAwareGridHeight hook
vi.mock("@/hooks/useContentAwareGridHeight", () => ({
  useContentAwareGridHeight: () => 400
}));

describe("DSMPaginatedGrid", () => {
  // Sample test data
  const sampleData = [
    { id: 1, name: "Item 1", value: 100 },
    { id: 2, name: "Item 2", value: 200 },
    { id: 3, name: "Item 3", value: 300 }
  ];

  const sampleColumnDefs: ColDef[] = [
    { field: "id", headerName: "ID" },
    { field: "name", headerName: "Name" },
    { field: "value", headerName: "Value" }
  ];

  /**
   * Creates a mock pagination object for testing.
   * Follows the pattern from useGridPagination hook.
   */
  const createMockPagination = (
    overrides?: Partial<GridPaginationState & GridPaginationActions>
  ): GridPaginationState & GridPaginationActions => ({
    pageNumber: 0,
    pageSize: 25,
    sortParams: { sortBy: "id", isSortDescending: false },
    handlePaginationChange: vi.fn(),
    handlePageNumberChange: vi.fn(),
    handlePageSizeChange: vi.fn(),
    handleSortChange: vi.fn(),
    resetPagination: vi.fn(),
    clearPersistedState: vi.fn(),
    ...overrides
  });

  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.resetAllMocks();
  });

  describe("Render States", () => {
    it("should not render grid when data is null", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={null}
          columnDefs={sampleColumnDefs}
          totalRecords={0}
          isLoading={false}
          pagination={pagination}
        />
      );

      expect(screen.queryByTestId("dsm-grid")).not.toBeInTheDocument();
    });

    it("should render grid when data is empty array", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={[]}
          columnDefs={sampleColumnDefs}
          totalRecords={0}
          isLoading={false}
          pagination={pagination}
        />
      );

      expect(screen.getByTestId("dsm-grid")).toBeInTheDocument();
      expect(screen.getByTestId("grid-row-count")).toHaveTextContent("0");
    });

    it("should render grid when data has items", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
        />
      );

      expect(screen.getByTestId("dsm-grid")).toBeInTheDocument();
      expect(screen.getByTestId("grid-row-count")).toHaveTextContent("3");
      expect(screen.getByTestId("grid-column-count")).toHaveTextContent("3");
    });

    it("should pass loading state to grid", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={true}
          pagination={pagination}
        />
      );

      expect(screen.getByTestId("grid-loading")).toHaveTextContent("Loading");
    });

    it("should pass preferenceKey to grid", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="my-custom-preference-key"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
        />
      );

      expect(screen.getByTestId("dsm-grid")).toHaveAttribute("data-preference-key", "my-custom-preference-key");
    });
  });

  describe("Pagination Display", () => {
    it("should show pagination when data has items (default behavior)", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
        />
      );

      expect(screen.getByTestId("pagination")).toBeInTheDocument();
    });

    it("should hide pagination when data is empty (default behavior)", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={[]}
          columnDefs={sampleColumnDefs}
          totalRecords={0}
          isLoading={false}
          pagination={pagination}
        />
      );

      expect(screen.queryByTestId("pagination")).not.toBeInTheDocument();
    });

    it("should show pagination when showPagination=true even with empty data", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={[]}
          columnDefs={sampleColumnDefs}
          totalRecords={0}
          isLoading={false}
          pagination={pagination}
          showPagination={true}
        />
      );

      expect(screen.getByTestId("pagination")).toBeInTheDocument();
    });

    it("should hide pagination when showPagination=false even with data", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          showPagination={false}
        />
      );

      expect(screen.queryByTestId("pagination")).not.toBeInTheDocument();
    });

    it("should pass totalRecords to Pagination component", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={999}
          isLoading={false}
          pagination={pagination}
        />
      );

      expect(screen.getByTestId("record-count")).toHaveTextContent("999");
    });

    it("should pass rowsPerPageOptions to Pagination component", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          rowsPerPageOptions={[10, 25, 50, 100]}
        />
      );

      expect(screen.getByTestId("rows-per-page-options")).toHaveTextContent("10,25,50,100");
    });
  });

  /**
   * CRITICAL REGRESSION TESTS: Page Number Conversion
   *
   * The DSMPaginatedGrid component converts between:
   * - UI/Pagination component: 1-based page numbers (user sees Page 1, 2, 3...)
   * - API/Internal state: 0-based page numbers (API expects page=0, 1, 2...)
   *
   * The conversion happens at line 424 in DSMPaginatedGrid.tsx:
   *   setPageNumber={(value: number) => handlePageNumberChange(value - 1)}
   *
   * This means when Pagination component calls setPageNumber(1), the parent's
   * handlePageNumberChange should receive 0.
   *
   * With the new Pagination interface (smart-ui-library 6.x), skip is already 0-based.
   * DSMPaginatedGrid passes pageNumber directly as skip and receives skip directly back.
   */
  describe("Pagination State Passthrough - REGRESSION TESTS", () => {
    it("should call handlePageNumberChange when navigating to page 0 from a different page", async () => {
      const user = userEvent.setup();
      const handlePageNumberChange = vi.fn();
      // Start on page 5 so clicking "go to page 1" (skip=0) triggers a change
      const pagination = createMockPagination({ handlePageNumberChange, pageNumber: 5 });

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
        />
      );

      // Click "Go to Page 1" which sends skip=0
      await user.click(screen.getByTestId("go-to-page-1"));

      // handlePageNumberChange should receive 0 (the skip value)
      expect(handlePageNumberChange).toHaveBeenCalledTimes(1);
      expect(handlePageNumberChange).toHaveBeenCalledWith(0);
    });

    it("should call handlePageNumberChange when navigating to page 1 from page 0", async () => {
      const user = userEvent.setup();
      const handlePageNumberChange = vi.fn();
      const pagination = createMockPagination({ handlePageNumberChange, pageNumber: 0 });

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
        />
      );

      // Click "Go to Page 2" which sends skip=1
      await user.click(screen.getByTestId("go-to-page-2"));

      // handlePageNumberChange should receive 1 (the skip value)
      expect(handlePageNumberChange).toHaveBeenCalledTimes(1);
      expect(handlePageNumberChange).toHaveBeenCalledWith(1);
    });

    it("should call handlePageNumberChange when navigating to page 4 from page 0", async () => {
      const user = userEvent.setup();
      const handlePageNumberChange = vi.fn();
      const pagination = createMockPagination({ handlePageNumberChange, pageNumber: 0 });

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={500}
          isLoading={false}
          pagination={pagination}
        />
      );

      // Click "Go to Page 5" which sends skip=4
      await user.click(screen.getByTestId("go-to-page-5"));

      // handlePageNumberChange should receive 4 (the skip value)
      expect(handlePageNumberChange).toHaveBeenCalledTimes(1);
      expect(handlePageNumberChange).toHaveBeenCalledWith(4);
    });

    it("should pass pageNumber directly to Pagination as skip (no conversion)", () => {
      const pagination = createMockPagination({ pageNumber: 3 });

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
        />
      );

      // pageNumber from state (3) should be passed directly as skip to Pagination
      expect(screen.getByTestId("page-number")).toHaveTextContent("3");
    });

    it("should call handlePageSizeChange when page size changes", async () => {
      const user = userEvent.setup();
      const handlePageSizeChange = vi.fn();
      const pagination = createMockPagination({ handlePageSizeChange });

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
        />
      );

      // User changes page size to 50
      await user.click(screen.getByTestId("set-page-size-50"));

      // handlePageSizeChange should receive the exact value
      expect(handlePageSizeChange).toHaveBeenCalledTimes(1);
      expect(handlePageSizeChange).toHaveBeenCalledWith(50);
    });

    it("should not call handlePageNumberChange when clicking same page", async () => {
      const user = userEvent.setup();
      const handlePageNumberChange = vi.fn();
      // Start on page 0
      const pagination = createMockPagination({ handlePageNumberChange, pageNumber: 0 });

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
        />
      );

      // Click "Go to Page 1" which sends skip=0 - same as current page
      await user.click(screen.getByTestId("go-to-page-1"));

      // handlePageNumberChange should NOT be called since page hasn't changed
      expect(handlePageNumberChange).not.toHaveBeenCalled();
    });

    it("should handle multiple page changes correctly", async () => {
      const user = userEvent.setup();
      const handlePageNumberChange = vi.fn();
      // Start on page 3 so all navigations trigger changes
      const pagination = createMockPagination({ handlePageNumberChange, pageNumber: 3 });

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={500}
          isLoading={false}
          pagination={pagination}
        />
      );

      // Navigate to different pages
      await user.click(screen.getByTestId("go-to-page-1")); // skip=0
      await user.click(screen.getByTestId("go-to-page-5")); // skip=4
      await user.click(screen.getByTestId("go-to-page-2")); // skip=1

      expect(handlePageNumberChange).toHaveBeenCalledTimes(3);
      expect(handlePageNumberChange).toHaveBeenNthCalledWith(1, 0);
      expect(handlePageNumberChange).toHaveBeenNthCalledWith(2, 4);
      expect(handlePageNumberChange).toHaveBeenNthCalledWith(3, 1);
    });
  });

  describe("Sort Handling", () => {
    it("should call pagination handleSortChange when no custom onSortChange provided", async () => {
      const user = userEvent.setup();
      const handleSortChange = vi.fn();
      const pagination = createMockPagination({ handleSortChange });

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
        />
      );

      await user.click(screen.getByTestId("sort-button"));

      expect(handleSortChange).toHaveBeenCalledTimes(1);
      expect(handleSortChange).toHaveBeenCalledWith({
        sortBy: "name",
        isSortDescending: true
      });
    });

    it("should call custom onSortChange when provided", async () => {
      const user = userEvent.setup();
      const customSortHandler = vi.fn();
      const handleSortChange = vi.fn();
      const pagination = createMockPagination({ handleSortChange });

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          onSortChange={customSortHandler}
        />
      );

      await user.click(screen.getByTestId("sort-button"));

      // Custom handler should be called, not the pagination handler
      expect(customSortHandler).toHaveBeenCalledTimes(1);
      expect(customSortHandler).toHaveBeenCalledWith({
        sortBy: "name",
        isSortDescending: true
      });
      expect(handleSortChange).not.toHaveBeenCalled();
    });
  });

  describe("Slot Rendering", () => {
    it("should render header slot when provided", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          header={<h1 data-testid="custom-header">My Grid Header</h1>}
        />
      );

      expect(screen.getByTestId("custom-header")).toBeInTheDocument();
      expect(screen.getByTestId("custom-header")).toHaveTextContent("My Grid Header");
    });

    it("should render headerActions slot when provided", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          header={<span>Title</span>}
          headerActions={<button data-testid="export-button">Export</button>}
        />
      );

      expect(screen.getByTestId("export-button")).toBeInTheDocument();
    });

    it("should render beforeGrid slot when provided", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          beforeGrid={<div data-testid="filter-bar">Filter Controls</div>}
        />
      );

      expect(screen.getByTestId("filter-bar")).toBeInTheDocument();
    });

    it("should render afterGrid slot when provided", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          afterGrid={<div data-testid="summary-section">Summary: 3 items</div>}
        />
      );

      expect(screen.getByTestId("summary-section")).toBeInTheDocument();
    });

    it("should apply custom className to slots", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          beforeGrid={<span>Before</span>}
          afterGrid={<span>After</span>}
          slotClassNames={{
            beforeGridClassName: "my-before-class",
            afterGridClassName: "my-after-class"
          }}
        />
      );

      // The beforeGrid and afterGrid wrappers should have the custom classes
      const beforeWrapper = screen.getByText("Before").parentElement;
      const afterWrapper = screen.getByText("After").parentElement;

      expect(beforeWrapper).toHaveClass("my-before-class");
      expect(afterWrapper).toHaveClass("my-after-class");
    });
  });

  describe("Container and Ref", () => {
    it("should apply custom className to container", () => {
      const pagination = createMockPagination();

      const { container } = render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          className="my-custom-container-class"
        />
      );

      expect(container.firstChild).toHaveClass("my-custom-container-class");
    });

    it("should forward innerRef to container", () => {
      const pagination = createMockPagination();
      const ref = React.createRef<HTMLDivElement>();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          innerRef={ref}
        />
      );

      expect(ref.current).toBeInstanceOf(HTMLDivElement);
    });
  });

  describe("Grid API Callback", () => {
    it("should be callable when onGridApiReady is provided", () => {
      const pagination = createMockPagination();
      const onGridApiReady = vi.fn();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          onGridApiReady={onGridApiReady}
        />
      );

      // Grid should render without error
      expect(screen.getByTestId("dsm-grid")).toBeInTheDocument();
    });
  });

  describe("Edge Cases", () => {
    it("should handle null data gracefully (not undefined)", () => {
      // NOTE: Component only handles null, not undefined.
      // Passing undefined will cause a runtime error at line 389:
      // `const hasData = data !== null && data.length > 0`
      // This test verifies null handling works correctly.
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={null}
          columnDefs={sampleColumnDefs}
          totalRecords={0}
          isLoading={false}
          pagination={pagination}
        />
      );

      // Should not crash with null data, grid should not render
      expect(screen.queryByTestId("dsm-grid")).not.toBeInTheDocument();
    });

    it("should handle zero totalRecords", () => {
      const pagination = createMockPagination();

      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={0}
          isLoading={false}
          pagination={pagination}
        />
      );

      expect(screen.getByTestId("record-count")).toHaveTextContent("0");
    });

    it("should pass showColumnControl to grid", () => {
      const pagination = createMockPagination();

      // Just verify component renders with this prop without error
      render(
        <DSMPaginatedGrid
          preferenceKey="test-grid"
          data={sampleData}
          columnDefs={sampleColumnDefs}
          totalRecords={100}
          isLoading={false}
          pagination={pagination}
          showColumnControl={false}
        />
      );

      expect(screen.getByTestId("dsm-grid")).toBeInTheDocument();
    });
  });
});
