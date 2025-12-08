import { Grid } from "@mui/material";
import { useMemo } from "react";
import { DSMGrid, Pagination } from "smart-ui-library";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { GridPaginationActions, GridPaginationState } from "../../../hooks/useGridPagination";
import { AccountHistoryReportPaginatedResponse } from "../../../types/reports/AccountHistoryReportTypes";
import { GetAccountHistoryReportColumns } from "./AccountHistoryReportGridColumns";

interface AccountHistoryReportTableProps {
  data: AccountHistoryReportPaginatedResponse | undefined;
  isLoading: boolean;
  error: unknown;
  showData: boolean;
  gridPagination: GridPaginationState & GridPaginationActions;
}

const AccountHistoryReportTable: React.FC<AccountHistoryReportTableProps> = ({
  data,
  isLoading,
  showData,
  gridPagination
}) => {
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: data?.response?.results?.length ?? 0
  });
  const columnDefs = useMemo(() => GetAccountHistoryReportColumns(), []);

  const { pageNumber, pageSize, handlePaginationChange } = gridPagination;
  const recordCount = data?.response?.total ?? 0;

  return (
    <>
      {showData && data?.response && (
        <Grid
          container
          rowSpacing="24px">
          <Grid width="100%">
            <DSMGrid
              preferenceKey="Account_History_Report"
              isLoading={isLoading}
              maxHeight={gridMaxHeight}
              providedOptions={{
                rowData: data.response.results ?? [],
                columnDefs: columnDefs
              }}
            />
          </Grid>
          {recordCount > 0 && (
            <Grid width="100%">
              <Pagination
                pageNumber={pageNumber}
                setPageNumber={(value: number) => handlePaginationChange(value - 1, pageSize)}
                pageSize={pageSize}
                setPageSize={(value: number) => {
                  handlePaginationChange(0, value);
                }}
                recordCount={recordCount}
              />
            </Grid>
          )}
        </Grid>
      )}
    </>
  );
};

export default AccountHistoryReportTable;
