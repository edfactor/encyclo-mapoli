import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDistributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
import { GetDistributionsByAgeColumns } from "./DistributionByAgeGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";
import { currencyFormat } from "utils/numberUtils"; // Import utility function

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

  const columnDefsTotal = GetDistributionsByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetDistributionsByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetDistributionsByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  return (
    <>
      {distributionsByAgeTotal?.response && (
        <>
          <div className="px-[24px]">
          <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <div className="flex sticky top-0 z-10 bg-white">
              <TotalsGrid 
                displayData = {[
                  [(distributionsByAgeTotal?.regularTotalEmployees || 0), 
                    currencyFormat(distributionsByAgeTotal?.regularTotalAmount || 0)], 
                  [(distributionsByAgeTotal?.hardshipTotalEmployees || 0), 
                    currencyFormat(distributionsByAgeTotal?.hardshipTotalAmount || 0)],
                  [(distributionsByAgeTotal?.bothHardshipAndRegularEmployees || 0),
                    currencyFormat(distributionsByAgeTotal?.bothHardshipAndRegularAmount || 0)],
                  [(distributionsByAgeTotal?.totalEmployees || 0), 
                    currencyFormat(distributionsByAgeTotal?.distributionTotalAmount || 0)]
                  ]} 
                  leftColumnHeaders = {['Regular', 'Hardship', 'Both', 'Dist Total']}
                  topRowHeaders={['Total', 'EMPS', 'Amount']}

              ></TotalsGrid>
              <TotalsGrid 
                displayData = {[
                  [(distributionsByAgeFullTime?.regularTotalEmployees || 0), 
                    currencyFormat(distributionsByAgeFullTime?.regularTotalAmount || 0)], 
                  [(distributionsByAgeFullTime?.hardshipTotalEmployees || 0), 
                    currencyFormat(distributionsByAgeFullTime?.hardshipTotalAmount || 0)],
                  [(distributionsByAgeFullTime?.bothHardshipAndRegularEmployees || 0),
                    currencyFormat(distributionsByAgeFullTime?.bothHardshipAndRegularAmount || 0)],
                  [(distributionsByAgeFullTime?.totalEmployees || 0), 
                    currencyFormat(distributionsByAgeFullTime?.distributionTotalAmount || 0)]
                  ]}
                  leftColumnHeaders = {['Regular', 'Hardship', 'Both', 'Dist Total']}
                  topRowHeaders={['FullTime', 'EMPS', 'Amount']}
              ></TotalsGrid>
              <TotalsGrid 
                displayData = {[
                  [(distributionsByAgePartTime?.regularTotalEmployees || 0), 
                    currencyFormat(distributionsByAgePartTime?.regularTotalAmount || 0)], 
                  [(distributionsByAgePartTime?.hardshipTotalEmployees || 0), 
                    currencyFormat(distributionsByAgePartTime?.hardshipTotalAmount || 0)],
                  [(distributionsByAgePartTime?.bothHardshipAndRegularEmployees || 0),
                    currencyFormat(distributionsByAgePartTime?.bothHardshipAndRegularAmount || 0)],
                  [(distributionsByAgePartTime?.totalEmployees || 0), 
                    currencyFormat(distributionsByAgePartTime?.distributionTotalAmount || 0)]
                  ]}
                  leftColumnHeaders = {['Regular', 'Hardship', 'Both', 'Dist Total']}
                  topRowHeaders={['PartTime', 'EMPS', 'Amount']}
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
                  rowData: distributionsByAgeTotal?.response.results,
                  
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
                  rowData: distributionsByAgeFullTime?.response.results,
                  
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
                  rowData: distributionsByAgePartTime?.response.results,
                  
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

export default DistributionByAgeGrid;
