import { Typography } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetContributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
import { GetContributionsByAgeColumns } from "./ContributionsByAgeGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";

const ContributionsByAgeGrid = () => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  function currencyFormat(num : Number) : string {
    return '$' + num.toFixed(2).replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
 }

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
          <div style={{display: 'flex'}}>
          <TotalsGrid 
            displayData = {[[(contributionsByAgeTotal?.totalEmployees || 0), currencyFormat(contributionsByAgeTotal?.totalAmount)], 
              [4, 5],
              [8, 9]]} 
              leftColumnHeaders = {['Regular', 'Hardship', 'DistTotal']}
              topRowHeaders={['All', 'Total', 'Amount']}
          ></TotalsGrid>
          <TotalsGrid 
            displayData = {[[(contributionsByAgeFullTime?.totalEmployees || 0), (contributionsByAgeFullTime?.totalAmount || 0)], 
              [4, 5],
              [8, 9]]} 
              leftColumnHeaders = {['Regular', 'Hardship', 'DistTotal']}
              topRowHeaders={['FullTime', 'Total', 'Amount']}
          ></TotalsGrid>
          <TotalsGrid 
            displayData = {[[(contributionsByAgePartTime?.totalEmployees || 0), (contributionsByAgePartTime?.totalAmount || 0)], 
              [4, 5],
              [8, 9]]} 
              leftColumnHeaders = {['Regular', 'Hardship', 'DistTotal']}
              topRowHeaders={['PartTime', 'Total', 'Amount']}
          ></TotalsGrid>
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
                      age: "CONT TTL",
                      employeeCount: (contributionsByAgeTotal?.totalEmployees || 0),
                      amount: contributionsByAgeTotal?.totalAmount
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
                     age: "CONT TTL",
                      employeeCount: (contributionsByAgeFullTime?.totalEmployees || 0),
                      amount: contributionsByAgeFullTime?.totalAmount
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
                      age: "CONT TTL",
                      employeeCount: (contributionsByAgePartTime?.totalEmployees || 0),
                      amount: contributionsByAgePartTime?.totalAmount
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
