import { ColDef, ColGroupDef, GridApi, GridOptions } from "ag-grid-community";
import { ReactNode, RefObject } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useContentAwareGridHeight } from "../../hooks/useContentAwareGridHeight";
import { GridPaginationActions, GridPaginationState, SortParams } from "../../hooks/useGridPagination";
import { ISortParams } from "../DSMGrid/types";

/**
 * Minimal pagination interface required by DSMPaginatedGrid.
 * This allows components that don't use useGridPagination to still use DSMPaginatedGrid
 * by providing only the necessary properties.
 */
export interface MinimalPaginationProps {
  pageNumber: number;
  pageSize: number;
  sortParams: SortParams;
  handlePageNumberChange: (pageNumber: number) => void;
  handlePageSizeChange: (pageSize: number) => void;
  handleSortChange: (sortParams: SortParams) => void;
}

/**
 * Configuration options for content-aware grid height calculation
 */
export interface ContentAwareHeightConfig {
  /** Height of each row in pixels. Default: 41 (AG Grid default) */
  rowHeight?: number;
  /** Height of the header row in pixels. Default: 41 */
  headerHeight?: number;
  /** Additional padding/chrome height. Default: 10 */
  chromeHeight?: number;
  /** Percentage of window height to use as maximum (0-1). Default: 0.4 */
  heightPercentage?: number;
  /** Minimum height in pixels. Default: 100 */
  minHeight?: number;
  /** Maximum height in pixels. Default: 900 */
  maxHeight?: number;
}

/**
 * Configuration for slot wrapper styling
 */
export interface SlotWrapperConfig {
  /** CSS class name for the header wrapper */
  headerClassName?: string;
  /** CSS class name for the beforeGrid wrapper */
  beforeGridClassName?: string;
  /** CSS class name for the afterGrid wrapper */
  afterGridClassName?: string;
}

/**
 * Props for the DSMPaginatedGrid component
 *
 * @template T - The type of row data in the grid
 */
export interface DSMPaginatedGridProps<T = unknown> {
  // ============ Required Props ============

  /**
   * Unique key for localStorage persistence of grid preferences.
   * Must be unique per grid instance. Used for column state, sorting, etc.
   */
  preferenceKey: string;

  /**
   * The row data to display in the grid.
   * Pass null when data hasn't been fetched yet (grid won't render).
   * Pass empty array [] when fetched but no results.
   */
  data: T[] | null;

  /**
   * Column definitions for the grid.
   * Use useMemo to prevent unnecessary re-renders.
   */
  columnDefs: (ColDef | ColGroupDef)[];

  /**
   * Total number of records (for pagination display).
   * This may differ from data.length when using server-side pagination.
   */
  totalRecords: number;

  /**
   * Loading state - shows loading overlay when true
   */
  isLoading: boolean;

  /**
   * Pagination state and handlers from useGridPagination hook or a compatible object.
   * Spread the return value of useGridPagination here, or provide a compatible object
   * with the required properties.
   *
   * @example
   * // Using useGridPagination (recommended)
   * const pagination = useGridPagination({ ... });
   * <DSMPaginatedGrid pagination={pagination} ... />
   *
   * @example
   * // Custom pagination object (when parent manages state)
   * const paginationProps = useMemo(() => ({
   *   pageNumber,
   *   pageSize,
   *   sortParams: { sortBy: "", isSortDescending: false },
   *   handlePageNumberChange: (n: number) => setPageNumber(n),
   *   handlePageSizeChange: (s: number) => setPageSize(s),
   *   handleSortChange: () => {}
   * }), [pageNumber, pageSize]);
   * <DSMPaginatedGrid pagination={paginationProps} ... />
   */
  pagination: MinimalPaginationProps | (GridPaginationState & GridPaginationActions);

  // ============ Optional Render Slots ============

  /**
   * Content to render above the grid (e.g., title, description).
   * Renders inside the wrapper div, before any other content.
   */
  header?: ReactNode;

  /**
   * Actions to render alongside the header (e.g., expand/collapse buttons).
   * Renders in a flex container with the header.
   * Use this for buttons that should appear at the top-right of the grid area.
   *
   * @example
   * headerActions={
   *   <IconButton onClick={onToggleExpand}>
   *     {isExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
   *   </IconButton>
   * }
   */
  headerActions?: ReactNode;

  /**
   * Content to render between header and grid.
   * Useful for TotalsGrid, ReportSummary, filter chips, etc.
   *
   * For sticky TotalsGrid positioning, use `beforeGridClassName="sticky top-0 z-10 bg-white"`
   *
   * @example
   * beforeGrid={
   *   <div className="sticky top-0 z-10 flex bg-white">
   *     <TotalsGrid ... />
   *     <TotalsGrid ... />
   *   </div>
   * }
   */
  beforeGrid?: ReactNode;

  /**
   * Content to render after the pagination.
   * Useful for action buttons, dialogs, modals, etc.
   */
  afterGrid?: ReactNode;

  // ============ Slot Styling ============

  /**
   * Configuration for slot wrapper styling.
   * Allows adding CSS classes to slot wrapper divs.
   *
   * @example
   * // For sticky TotalsGrid
   * slotClassNames={{ beforeGridClassName: "sticky top-0 z-10 bg-white" }}
   */
  slotClassNames?: SlotWrapperConfig;

  // ============ Optional Configuration ============

  /**
   * Configuration for content-aware height calculation.
   * The grid automatically shrinks for small data sets and
   * uses viewport-based max height for large data sets.
   */
  heightConfig?: ContentAwareHeightConfig;

  /**
   * Custom options for rows per page dropdown.
   * Default: [25, 50, 100] from Pagination component
   */
  rowsPerPageOptions?: number[];

  /**
   * Additional AG Grid options to merge with defaults.
   * Use for rowSelection, pinnedTopRowData, custom cell renderers, context, etc.
   *
   * Common use cases:
   * - `pinnedTopRowData`: Display totals row pinned at top of grid
   * - `rowSelection`: Enable row selection modes
   * - `context`: Pass data to cell renderers
   * - `getRowClass`: Custom row styling
   *
   * @example
   * // Pinned totals row
   * gridOptions={{ pinnedTopRowData: [{ total: 1000 }] }}
   *
   * @example
   * // Row selection with context
   * gridOptions={{
   *   rowSelection: { mode: "multiRow", checkboxes: false },
   *   context: { isReadOnly }
   * }}
   */
  gridOptions?: Partial<GridOptions>;

  /**
   * Custom sort change handler.
   * If not provided, uses pagination.handleSortChange.
   * Use this for custom sort logic (e.g., compound sort keys).
   */
  onSortChange?: (sortParams: SortParams) => void;

  /**
   * Whether to show the pagination control.
   * Default: true when data exists and has records
   */
  showPagination?: boolean;

  /**
   * Ref to the wrapper div for external access.
   * Useful for scrolling, printing, or measuring.
   */
  innerRef?: RefObject<HTMLDivElement | null>;

  /**
   * Custom controls to display in the grid header.
   * Passed directly to DSMGrid.
   */
  controls?: ReactNode[];

  /**
   * Whether to show the column visibility control dropdown.
   * Default: true
   */
  showColumnControl?: boolean;

  /**
   * Callback to receive the AG Grid API instance.
   * Useful for programmatic grid control.
   */
  onGridApiReady?: (gridApi: GridApi) => void;

  /**
   * CSS class name for the wrapper div
   */
  className?: string;
}

/**
 * A wrapper component that combines DSMGrid with Pagination for a consistent
 * paginated grid experience across the application.
 *
 * Features:
 * - Automatic content-aware height calculation (shrinks for small data sets)
 * - Consistent page number handling (0-based internal, 1-based UI)
 * - Render slots for flexible composition (header, headerActions, beforeGrid, afterGrid)
 * - Slot wrapper styling for sticky positioning (e.g., TotalsGrid)
 * - Full TypeScript generics support for row data types
 *
 * @template T - The type of row data in the grid
 *
 * @example
 * ```tsx
 * // Basic usage
 * const pagination = useGridPagination({
 *   initialPageSize: 50,
 *   initialSortBy: "name",
 *   persistenceKey: GRID_KEYS.MY_GRID
 * });
 *
 * <DSMPaginatedGrid
 *   preferenceKey={GRID_KEYS.MY_GRID}
 *   data={results}
 *   columnDefs={columnDefs}
 *   totalRecords={totalCount}
 *   isLoading={isFetching}
 *   pagination={pagination}
 * />
 *
 * // With render slots
 * <DSMPaginatedGrid
 *   preferenceKey={GRID_KEYS.MY_GRID}
 *   data={results}
 *   columnDefs={columnDefs}
 *   totalRecords={totalCount}
 *   isLoading={isFetching}
 *   pagination={pagination}
 *   header={<Typography variant="h6">My Grid Title</Typography>}
 *   beforeGrid={<ReportSummary report={data} />}
 *   afterGrid={<MyActionButtons />}
 * />
 *
 * // With header actions (expand/collapse buttons)
 * <DSMPaginatedGrid
 *   preferenceKey={GRID_KEYS.MY_GRID}
 *   data={results}
 *   columnDefs={columnDefs}
 *   totalRecords={totalCount}
 *   isLoading={isFetching}
 *   pagination={pagination}
 *   header={<ReportSummary report={data} />}
 *   headerActions={
 *     <IconButton onClick={onToggleExpand}>
 *       {isExpanded ? <FullscreenExitIcon /> : <FullscreenIcon />}
 *     </IconButton>
 *   }
 * />
 *
 * // With sticky TotalsGrid (using slotClassNames)
 * <DSMPaginatedGrid
 *   preferenceKey={GRID_KEYS.MY_GRID}
 *   data={results}
 *   columnDefs={columnDefs}
 *   totalRecords={totalCount}
 *   isLoading={isFetching}
 *   pagination={pagination}
 *   beforeGrid={
 *     <>
 *       <TotalsGrid displayData={[[total1]]} leftColumnHeaders={["Total 1"]} />
 *       <TotalsGrid displayData={[[total2]]} leftColumnHeaders={["Total 2"]} />
 *     </>
 *   }
 *   slotClassNames={{ beforeGridClassName: "sticky top-0 z-10 flex bg-white" }}
 * />
 *
 * // With pinned totals row in grid
 * <DSMPaginatedGrid
 *   preferenceKey={GRID_KEYS.MY_GRID}
 *   data={results}
 *   columnDefs={columnDefs}
 *   totalRecords={totalCount}
 *   isLoading={isFetching}
 *   pagination={pagination}
 *   gridOptions={{
 *     pinnedTopRowData: [{ columnA: "Total", columnB: sumTotal }]
 *   }}
 * />
 * ```
 */
export function DSMPaginatedGrid<T = unknown>({
  // Required
  preferenceKey,
  data,
  columnDefs,
  totalRecords,
  isLoading,
  pagination,
  // Render slots
  header,
  headerActions,
  beforeGrid,
  afterGrid,
  // Slot styling
  slotClassNames,
  // Configuration
  heightConfig,
  rowsPerPageOptions,
  gridOptions,
  onSortChange,
  showPagination: showPaginationProp,
  innerRef,
  controls,
  showColumnControl = true,
  onGridApiReady,
  className
}: DSMPaginatedGridProps<T>) {
  const { pageNumber, pageSize, handlePageNumberChange, handlePageSizeChange, handleSortChange } = pagination;

  // Calculate content-aware grid height
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: data?.length ?? 0,
    ...heightConfig
  });

  // Handle sort change - use custom handler if provided, otherwise use pagination's handler
  const handleSort = (sortParams: ISortParams) => {
    if (onSortChange) {
      onSortChange(sortParams);
    } else {
      handleSortChange(sortParams);
    }
  };

  // Determine if we should show pagination
  const hasData = data !== null && data.length > 0;
  const showPagination = showPaginationProp ?? hasData;

  // Render header with optional actions
  const renderHeader = () => {
    if (!header && !headerActions) return null;

    if (headerActions) {
      return (
        <div className={`flex items-center justify-between ${slotClassNames?.headerClassName ?? ""}`}>
          <div className="flex-1">{header}</div>
          <div className="flex items-center gap-2">{headerActions}</div>
        </div>
      );
    }

    return slotClassNames?.headerClassName ? (
      <div className={slotClassNames.headerClassName}>{header}</div>
    ) : (
      header
    );
  };

  return (
    <div ref={innerRef} className={className}>
      {renderHeader()}

      {beforeGrid && (
        <div className={slotClassNames?.beforeGridClassName}>{beforeGrid}</div>
      )}

      {data && (
        <DSMGrid
          preferenceKey={preferenceKey}
          isLoading={isLoading}
          maxHeight={gridMaxHeight}
          handleSortChanged={handleSort}
          controls={controls}
          showColumnControl={showColumnControl}
          setGridApi={onGridApiReady}
          providedOptions={{
            rowData: data,
            columnDefs,
            suppressMultiSort: true,
            ...gridOptions
          }}
        />
      )}

      {showPagination && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => handlePageNumberChange(value - 1)}
          pageSize={pageSize}
          setPageSize={handlePageSizeChange}
          recordCount={totalRecords}
          {...(rowsPerPageOptions && { rowsPerPageOptions })}
        />
      )}

      {afterGrid && (
        <div className={slotClassNames?.afterGridClassName}>{afterGrid}</div>
      )}
    </div>
  );
}

export default DSMPaginatedGrid;
