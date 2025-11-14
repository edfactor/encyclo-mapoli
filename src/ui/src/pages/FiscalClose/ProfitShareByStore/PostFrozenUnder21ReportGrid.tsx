import { Grid } from "@mui/material";
import React, { useMemo } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { GetPostFrozenUnder21ReportColumnDefs } from "./PostFrozenUnder21ReportGridColumns";

interface PostFrozenUnder21ReportGridProps {
  isLoading?: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  gridPagination: ReturnType<typeof useGridPagination>;
}

const PostFrozenUnder21ReportGrid: React.FC<PostFrozenUnder21ReportGridProps> = ({
  isLoading = false,
  setInitialSearchLoaded,
  gridPagination
}) => {
  const profitSharingUnder21Report = useSelector((state: RootState) => state.yearsEnd.profitSharingUnder21Report);
  const navigate = useNavigate();
  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = gridPagination;

  // Handle navigation for badge clicks
  const handleNavigation = React.useCallback(
    (path: string) => {
      navigate(path);
    },
    [navigate]
  );

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "badgeNumber";
      update.isSortDescending = false;
    }
    handleSortChange(update);
  };

  const columnDefs = useMemo(() => GetPostFrozenUnder21ReportColumnDefs(handleNavigation), [handleNavigation]);

  return (
    <Grid
      container
      direction="column"
      width="100%">
      {profitSharingUnder21Report && <ReportSummary report={profitSharingUnder21Report} />}
      <Grid width="100%">
        <DSMGrid
          preferenceKey="POST_FROZEN_UNDER_21_REPORT"
          isLoading={isLoading}
          handleSortChanged={sortEventHandler}
          providedOptions={{
            rowData: profitSharingUnder21Report?.response?.results || [],
            columnDefs
          }}
        />
        {profitSharingUnder21Report?.response?.results && profitSharingUnder21Report.response.results.length > 0 && (
          <Pagination
            pageNumber={pageNumber}
            setPageNumber={(value: number) => {
              handlePaginationChange(value - 1, pageSize);
              setInitialSearchLoaded(true);
            }}
            pageSize={pageSize}
            setPageSize={(value: number) => {
              handlePaginationChange(0, value);
              setInitialSearchLoaded(true);
            }}
            recordCount={profitSharingUnder21Report.response.total || 0}
          />
        )}
      </Grid>
    </Grid>
  );
};

export default PostFrozenUnder21ReportGrid;
