import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBalanceByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
import { GetBalanceByAgeColumns } from "./BalanceByAgeGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";
import { numberToCurrency } from "smart-ui-library";

const BalanceByAgeGrid = () => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { balanceByAgeTotal, balanceByAgeFullTime, balanceByAgePartTime } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [_discard1, { isLoading }] = useLazyGetBalanceByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  return (
    <>
      {balanceByAgeTotal?.response && (
        <>
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <div className="flex sticky top-0 z-10 bg-white">
            <TotalsGrid
              displayData={[
                [
                  balanceByAgeTotal?.totalBeneficiaries || 0,
                  numberToCurrency(balanceByAgeTotal?.totalBeneficiariesAmount || 0),
                  numberToCurrency(balanceByAgeTotal?.totalBeneficiariesVestedAmount || 0)
                ],
                [
                  balanceByAgeTotal?.totalEmployee || 0,
                  numberToCurrency(balanceByAgeTotal?.totalEmployeeAmount || 0),
                  numberToCurrency(balanceByAgeTotal?.totalEmployeesVestedAmount || 0)
                ],
                [
                  balanceByAgeTotal?.totalMembers || 0,
                  numberToCurrency(balanceByAgeTotal?.balanceTotalAmount || 0),
                  numberToCurrency(balanceByAgeTotal?.vestedTotalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["Beneficiaries", "Employees", "Total"]}
              topRowHeaders={["All", "Count", "Balance", "Vested"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  balanceByAgeFullTime?.totalBeneficiaries || 0,
                  numberToCurrency(balanceByAgeFullTime?.totalBeneficiariesAmount || 0),
                  numberToCurrency(balanceByAgeFullTime?.totalBeneficiariesVestedAmount || 0)
                ],
                [
                  balanceByAgeFullTime?.totalEmployee || 0,
                  numberToCurrency(balanceByAgeFullTime?.totalEmployeeAmount || 0),
                  numberToCurrency(balanceByAgeFullTime?.totalEmployeesVestedAmount || 0)
                ],
                [
                  balanceByAgeFullTime?.totalMembers || 0,
                  numberToCurrency(balanceByAgeFullTime?.balanceTotalAmount || 0),
                  numberToCurrency(balanceByAgeFullTime?.vestedTotalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["Beneficiaries", "Employees", "Total"]}
              topRowHeaders={["FullTime", "Count", "Balance", "Vested"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  balanceByAgePartTime?.totalBeneficiaries || 0,
                  numberToCurrency(balanceByAgePartTime?.totalBeneficiariesAmount || 0),
                  numberToCurrency(balanceByAgePartTime?.totalBeneficiariesVestedAmount || 0)
                ],
                [
                  balanceByAgePartTime?.totalEmployee || 0,
                  numberToCurrency(balanceByAgePartTime?.totalEmployeeAmount || 0),
                  numberToCurrency(balanceByAgePartTime?.totalEmployeesVestedAmount || 0)
                ],
                [
                  balanceByAgePartTime?.totalMembers || 0,
                  numberToCurrency(balanceByAgePartTime?.balanceTotalAmount || 0),
                  numberToCurrency(balanceByAgePartTime?.vestedTotalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["Beneficiaries", "Employees", "Total"]}
              topRowHeaders={["PartTime", "Count", "Balance", "Vested"]}></TotalsGrid>
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
                  rowData: balanceByAgeTotal?.response.results,

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
                  rowData: balanceByAgeFullTime?.response.results,

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
                  rowData: balanceByAgePartTime?.response.results,

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

export default BalanceByAgeGrid;
