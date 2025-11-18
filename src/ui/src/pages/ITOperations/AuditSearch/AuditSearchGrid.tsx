import { Typography } from "@mui/material";
import { useCallback, useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { AuditEventDto } from "../../../types";
import { GetAuditSearchColumns } from "./AuditSearchGridColumns";

interface AuditSearchGridProps {
  data: AuditEventDto[];
  total: number;
  isLoading: boolean;
  onPaginationChange: (pageNumber: number, pageSize: number, sortParams: SortParams) => void;
}

const AuditSearchGrid: React.FC<AuditSearchGridProps> = ({ data, total, isLoading, onPaginationChange }) => {
  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "createdAt",
    initialSortDescending: true,
    onPaginationChange: useCallback(
      (pageNum: number, pageSz: number, sortPrms: SortParams) => {
        onPaginationChange(pageNum, pageSz, sortPrms);
      },
      [onPaginationChange]
    )
  });

  const columnDefs = useMemo(() => GetAuditSearchColumns(), []);

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
        preferenceKey={"AUDIT_SEARCH"}
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
          setPageNumber={(value: number) => {
            handlePaginationChange(value - 1, pageSize);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            handlePaginationChange(0, value);
          }}
          recordCount={total}
        />
      )}
    </>
  );
};

export default AuditSearchGrid;
