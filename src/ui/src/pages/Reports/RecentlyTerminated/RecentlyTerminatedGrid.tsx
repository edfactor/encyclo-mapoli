import { Typography } from "@mui/material";
import React, { useMemo } from "react";
import { RecentlyTerminatedResponse } from "reduxstore/types";
import { DSMGrid, formatNumberWithComma, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { GetRecentlyTerminatedColumns } from "./RecentlyTerminatedGridColumns";

interface RecentlyTerminatedGridProps {
  reportData: RecentlyTerminatedResponse | null;
  isLoading: boolean;
  gridPagination: ReturnType<typeof useGridPagination>;
}

const RecentlyTerminatedGrid: React.FC<RecentlyTerminatedGridProps> = ({ reportData, isLoading, gridPagination }) => {
  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = gridPagination;

  const sortEventHandler = (update: ISortParams) => handleSortChange(update);
  const columnDefs = useMemo(() => GetRecentlyTerminatedColumns(), []);

  return (
    <>
      {reportData && reportData.response && (
        <>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
            <Typography
              variant="h6"
              component="h2"
              sx={{ marginLeft: "20px", marginRight: "10px" }}>
              TERMINATED EMPLOYEES ({formatNumberWithComma(reportData.response.total)} Records)
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}
            isLoading={isLoading}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: reportData.response.results,
              columnDefs: columnDefs,
              suppressMultiSort: true
            }}
          />
        </>
      )}
      {reportData && reportData.response && reportData.response.results && reportData.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            handlePaginationChange(value - 1, pageSize);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            handlePaginationChange(0, value);
          }}
          recordCount={reportData.response.total}
        />
      )}
    </>
  );
};

export default RecentlyTerminatedGrid;
