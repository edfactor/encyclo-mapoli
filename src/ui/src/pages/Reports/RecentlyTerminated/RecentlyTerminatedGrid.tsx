import { Typography } from "@mui/material";
import React, { useMemo } from "react";
import { RecentlyTerminatedResponse } from "reduxstore/types";
import { formatNumberWithComma, ISortParams } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { GetRecentlyTerminatedColumns } from "./RecentlyTerminatedGridColumns";

interface RecentlyTerminatedGridProps {
  reportData: RecentlyTerminatedResponse | null;
  isLoading: boolean;
  gridPagination: ReturnType<typeof useGridPagination>;
}

const RecentlyTerminatedGrid: React.FC<RecentlyTerminatedGridProps> = ({ reportData, isLoading, gridPagination }) => {
  const { sortParams, handleSortChange } = gridPagination;

  const sortEventHandler = (update: ISortParams) => handleSortChange(update);
  const columnDefs = useMemo(() => GetRecentlyTerminatedColumns(), []);

  if (!reportData?.response) {
    return null;
  }

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.RECENTLY_TERMINATED}
      data={reportData.response.results}
      columnDefs={columnDefs}
      totalRecords={reportData.response.total}
      isLoading={isLoading}
      pagination={{
        pageNumber: gridPagination.pageNumber,
        pageSize: gridPagination.pageSize,
        sortParams,
        handlePageNumberChange: gridPagination.handlePageNumberChange,
        handlePageSizeChange: gridPagination.handlePageSizeChange,
        handleSortChange
      }}
      onSortChange={sortEventHandler}
      gridOptions={{
        suppressMultiSort: true
      }}
      header={
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
          <Typography
            variant="h6"
            component="h2"
            sx={{ marginLeft: "20px", marginRight: "10px" }}>
            TERMINATED EMPLOYEES ({formatNumberWithComma(reportData.response.total)} Records)
          </Typography>
        </div>
      }
    />
  );
};

export default RecentlyTerminatedGrid;
