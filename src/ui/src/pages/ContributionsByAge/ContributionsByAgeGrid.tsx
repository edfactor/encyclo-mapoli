import { Typography } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetContributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { GetContributionsByAgeColumns } from "./ContributionsByAgeGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";

const ContributionsByAgeGrid = () => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { contributionsByAgeTotal, contributionsByAgeFullTime, contributionsByAgePartTime } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [_discard1, { isLoading }] = useLazyGetContributionsByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  return (
    <>
      {contributionsByAgeTotal?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${contributionsByAgeTotal.reportName}`}
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
                  rowData: contributionsByAgeTotal?.response.results,
                  pinnedTopRowData: [
                    {
                      age: "Dist Total",
                      employeeCount: (contributionsByAgeTotal?.totalEmployees || 0),
                      amount: contributionsByAgeTotal?.distributionTotalAmount
                    }
                  ],
                  columnDefs: [
                    {
                      headerName: columnDefsTotal.headerName,
                      children: columnDefsTotal.children
                    }
                  ]
                }}
              />
            </Grid2>
            <Grid2 xs={4}>
              <DSMGrid
                preferenceKey={"AGE_FullTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: contributionsByAgeFullTime?.response.results,
                  pinnedTopRowData: [
                    {
                     age: "Dist Total",
                      employeeCount: (contributionsByAgeTotal?.totalEmployees || 0),
                      amount: contributionsByAgeTotal?.distributionTotalAmount
                    }
                  ],
                  columnDefs: [
                    {
                      headerName: columnDefsFullTime.headerName,
                      children: columnDefsFullTime.children
                    }
                  ]
                }}
              />
            </Grid2>
            <Grid2 xs={4}>
              <DSMGrid
                preferenceKey={"AGE_PartTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: contributionsByAgePartTime?.response.results,
                  pinnedTopRowData: [
                    {
                      age: "Dist Total",
                      employeeCount: (contributionsByAgeTotal?.totalEmployees || 0),
                      amount: contributionsByAgeTotal?.distributionTotalAmount
                    }
                  ],
                  columnDefs: [
                    {
                      headerName: columnDefsPartTime.headerName,
                      children: columnDefsPartTime.children
                    }
                  ]
                }}
              />
            </Grid2>
          </Grid2>
        </>
      )}
    </>
  );
};

export default ContributionsByAgeGrid;
