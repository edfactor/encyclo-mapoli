import { Grid } from "@mui/material";
import React, { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import ReportSummary from "../../../../components/ReportSummary";
import { useGridPagination } from "../../../../hooks/useGridPagination";
import { GetUnder21BreakdownColumnDefs } from "./GetUnder21BreakdownGridColumns";

interface Under21BreakdownGridProps {
  isLoading?: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumber: number;
  pageSize: number;
  onPageChange: (page: number) => void;
}

const Under21BreakdownGrid: React.FC<Under21BreakdownGridProps> = ({
  isLoading = false,
  setInitialSearchLoaded,
  pageNumber,
  pageSize,
  onPageChange
}) => {
  const under21Breakdown = useSelector((state: RootState) => state.yearsEnd.under21BreakdownByStore);
  //const navigate = useNavigate();

  const { handleSortChange } = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    onPaginationChange: () => {
      // This component doesn't trigger API calls on pagination change
      // The data is already loaded from the parent component
    }
  });

  // Handle navigation for badge clicks
  /*
  const handleNavigation = React.useCallback(
    (path: string) => {
      navigate(path);
    },
    [navigate]
  );
  */

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "badgeNumber";
      update.isSortDescending = false;
    }
    handleSortChange(update);
    setInitialSearchLoaded(true);
  };

  const columnDefs = useMemo(() => GetUnder21BreakdownColumnDefs(), []);

  return (
    <Grid
      container
      direction="column"
      width="100%">
      {under21Breakdown && <ReportSummary report={under21Breakdown} />}
      <Grid width="100%">
        <DSMGrid
          preferenceKey="UNDER_21_BREAKDOWN_REPORT"
          isLoading={isLoading}
          handleSortChanged={sortEventHandler}
          providedOptions={{
            rowData: under21Breakdown?.response?.results || [],
            columnDefs
          }}
        />
        {under21Breakdown?.response?.results && under21Breakdown.response.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              onPageChange(value - 1);
              setInitialSearchLoaded(true);
            }}
            pageSize={pageSize}
            setPageSize={(_value: number) => {
              onPageChange(0);
              setInitialSearchLoaded(true);
            }}
            recordCount={under21Breakdown.response.total || 0}
          />
        )}
      </Grid>
    </Grid>
  );
};

export default Under21BreakdownGrid;
