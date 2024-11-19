import { Typography } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDistributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { GetDistributionsByAgeColumns } from "./DistributionByAgeGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";

const DistributionByAgeGrid = () => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { distributionsByAgeTotal, distributionsByAgeFullTime, distributionsByAgePartTime } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [_discard1, { isLoading }] = useLazyGetDistributionsByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = distributionsByAgeTotal
    ? GetDistributionsByAgeColumns(distributionsByAgeTotal)
    : {
        headerName: "",
        children: []
      };
  const columnDefsFullTime = distributionsByAgeFullTime
    ? GetDistributionsByAgeColumns(distributionsByAgeFullTime)
    : {
        headerName: "",
        children: []
      };
  const columnDefsPartTime = distributionsByAgePartTime
    ? GetDistributionsByAgeColumns(distributionsByAgePartTime)
    : {
        headerName: "",
        children: []
      };

  return (
    <>
      {distributionsByAgeTotal?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${distributionsByAgeTotal.reportName} (${distributionsByAgeTotal?.response.total || 0})`}
            </Typography>
          </div>
          <Grid2
            container
            xs={12}>
            <Grid2 xs={4}>
              <DSMGrid
                preferenceKey={"AGE_Total"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: distributionsByAgeTotal?.response.results,
                  pinnedTopRowData: columnDefsTotal.pinnedRowDataTotal,
                  columnDefs: columnDefsTotal.children
                }}
              />
            </Grid2>
            <Grid2 xs={4}>
              <DSMGrid
                preferenceKey={"AGE_FullTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: distributionsByAgeFullTime?.response.results,
                  pinnedTopRowData: columnDefsFullTime.pinnedRowDataTotal,
                  columnDefs: columnDefsFullTime.children
                }}
              />
            </Grid2>
            <Grid2 xs={4}>
              <DSMGrid
                preferenceKey={"AGE_PartTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: distributionsByAgePartTime?.response.results,
                  pinnedTopRowData: columnDefsPartTime.pinnedRowDataTotal,
                  columnDefs: columnDefsPartTime.children
                }}
              />
            </Grid2>
          </Grid2>
        </>
      )}
    </>
  );
};

export default DistributionByAgeGrid;
