import { Grid } from "@mui/material";
import React, { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import ReportSummary from "../../../../components/ReportSummary";
import { under21InactiveColumnDefs } from "./GetUnder21BreakdownGridColumns";

interface Under21InactiveGridProps {
  isLoading?: boolean;
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  pageNumber: number;
  setPageNumber: (value: number) => void;
  pageSize: number;
  setPageSize: (value: number) => void;
  sortParams: ISortParams;
  setSortParams: (value: ISortParams) => void;
}

const Under21InactiveGrid: React.FC<Under21InactiveGridProps> = ({
  isLoading = false,

  setInitialSearchLoaded,
  pageNumber,
  setPageNumber,
  pageSize,
  setPageSize,
  setSortParams
}) => {
  const under21Inactive = useSelector((state: RootState) => state.yearsEnd.under21Inactive);
  //const navigate = useNavigate();

  // Handle navigation for badge clicks
  /*
  const handleNavigation = (path: string) => {
    navigate(path);
  };
  */

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "badgeNumber";
      update.isSortDescending = false;
    }
    setSortParams(update);
    setPageNumber(0);
    setInitialSearchLoaded(true);
  };

  const columnDefs = useMemo(() => under21InactiveColumnDefs(), []);

  return (
    <Grid
      container
      direction="column"
      width="100%">
      {under21Inactive && <ReportSummary report={under21Inactive} />}
      <Grid width="100%">
        <DSMGrid
          preferenceKey="UNDER_21_INACTIVE_REPORT"
          isLoading={isLoading}
          handleSortChanged={sortEventHandler}
          providedOptions={{
            rowData: under21Inactive?.response?.results || [],
            columnDefs
          }}
        />
        {under21Inactive?.response?.results && under21Inactive.response.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              setPageNumber(value - 1);
              setInitialSearchLoaded(true);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              setPageSize(value);
              setPageNumber(1);
              setInitialSearchLoaded(true);
            }}
            recordCount={under21Inactive.response.total || 0}
          />
        )}
      </Grid>
    </Grid>
  );
};

export default Under21InactiveGrid;
