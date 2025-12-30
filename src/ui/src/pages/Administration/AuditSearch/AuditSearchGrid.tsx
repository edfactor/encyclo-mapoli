import { Typography } from "@mui/material";
import { useCallback, useMemo } from "react";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
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
  const pagination = useGridPagination({
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
    <DSMPaginatedGrid<AuditEventDto>
      preferenceKey={GRID_KEYS.AUDIT_SEARCH}
      data={data}
      columnDefs={columnDefs}
      totalRecords={total}
      isLoading={isLoading}
      pagination={pagination}
      onSortChange={pagination.handleSortChange}
      showPagination={data && data.length > 0}
      header={
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Audit Events
        </Typography>
      }
      slotClassNames={{ headerClassName: "px-6" }}
    />
  );
};

export default AuditSearchGrid;
