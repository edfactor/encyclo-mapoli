import { Typography } from "@mui/material";
import { useCallback, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { AuditEventDto, NavigationStatusDto } from "../../../types";
import { GetAuditSearchColumns } from "./AuditSearchGridColumns";

interface AuditSearchGridProps {
  data: AuditEventDto[];
  total: number;
  isLoading: boolean;
  onPaginationChange: (pageNumber: number, pageSize: number, sortParams: SortParams) => void;
  navigationStatusList: NavigationStatusDto[];
}

const AuditSearchGrid: React.FC<AuditSearchGridProps> = ({
  data,
  total,
  isLoading,
  onPaginationChange,
  navigationStatusList
}) => {
  const { pageNumber, pageSize, handlePageNumberChange, handlePageSizeChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "createdAt",
    initialSortDescending: true,
    persistenceKey: GRID_KEYS.AUDIT_SEARCH,
    onPaginationChange: useCallback(
      (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        onPaginationChange(pageNum, pageSz, sortPrms);
      },
      [onPaginationChange]
    )
  });

  const columnDefs = useMemo(() => GetAuditSearchColumns(navigationStatusList), [navigationStatusList]);

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Audit Events
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={GRID_KEYS.AUDIT_SEARCH}
        isLoading={isLoading}
        handleSortChanged={handleSortChange}
        providedOptions={{
          rowData: data,
          columnDefs: columnDefs
        }}
      />
      {data && data.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => handlePageNumberChange(value - 1)}
          pageSize={pageSize}
          setPageSize={handlePageSizeChange}
          recordCount={total}
        />
      )}
    </>
  );
};

export default AuditSearchGrid;
