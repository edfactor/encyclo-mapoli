import { Grid } from "@mui/material";
import { useMemo } from "react";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import { GridPaginationActions, GridPaginationState } from "../../../hooks/useGridPagination";
import { AccountHistoryReportPaginatedResponse, AccountHistoryReportResponse } from "../../../types/reports/AccountHistoryReportTypes";
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

  const { pageNumber, pageSize, sortParams, handlePageNumberChange, handlePageSizeChange, handleSortChange } = gridPagination;
  const recordCount = data?.response?.total ?? 0;

  return (
    <>
      {showData && data?.response && (
        <Grid
          container
          rowSpacing="24px">
          <Grid width="100%">
            <DSMPaginatedGrid<AccountHistoryReportResponse>
              preferenceKey={GRID_KEYS.ACCOUNT_HISTORY_REPORT}
              data={data.response.results ?? []}
              columnDefs={columnDefs}
              totalRecords={recordCount}
              isLoading={isLoading}
              heightConfig={{ maxHeight: gridMaxHeight }}
              pagination={{
                pageNumber,
                pageSize,
                sortParams: sortParams ?? { sortBy: "", isSortDescending: false },
                handlePageNumberChange: (value: number) => handlePageNumberChange(value - 1),
                handlePageSizeChange,
                handleSortChange: handleSortChange ?? (() => {})
              }}
              showPagination={recordCount > 0}
            />
          </Grid>
        </Grid>
      )}
    </>
  );
};

export default AccountHistoryReportTable;
