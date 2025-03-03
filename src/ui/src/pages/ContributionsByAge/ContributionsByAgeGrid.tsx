import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetContributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
import { GetContributionsByAgeColumns } from "./ContributionsByAgeGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";
import { numberToCurrency } from "smart-ui-library";

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
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <div className="flex sticky top-0 z-10 bg-white">
            <TotalsGrid
              displayData={[
                [contributionsByAgeTotal?.totalEmployees || 0, numberToCurrency(contributionsByAgeTotal?.totalAmount)]
              ]}
              leftColumnHeaders={["All"]}
              topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  contributionsByAgeFullTime?.totalEmployees || 0,
                  numberToCurrency(contributionsByAgeFullTime?.totalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["FullTime"]}
              topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  contributionsByAgePartTime?.totalEmployees || 0,
                  numberToCurrency(contributionsByAgePartTime?.totalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["PartTime"]}
              topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
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
