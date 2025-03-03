import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBalanceByYearsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
import { GetBalanceByYearsColumns } from "./BalanceByYearsGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";
import { numberToCurrency } from "smart-ui-library";

const BalanceByYearsGrid = () => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { balanceByYearsTotal, balanceByYearsFullTime, balanceByYearsPartTime } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [_discard1, { isLoading }] = useLazyGetBalanceByYearsQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetBalanceByYearsColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetBalanceByYearsColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetBalanceByYearsColumns(FrozenReportsByAgeRequestType.PartTime);

  return (
    <>
      {balanceByYearsTotal?.response && (
        <>
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <div className="flex sticky top-0 z-10 bg-white">
            <TotalsGrid
              displayData={[
                [
                  balanceByYearsTotal?.totalBeneficiaries || 0,
                  numberToCurrency(balanceByYearsTotal?.totalBeneficiariesAmount || 0),
                  numberToCurrency(balanceByYearsTotal?.totalBeneficiariesVestedAmount || 0)
                ],
                [
                  balanceByYearsTotal?.totalEmployee || 0,
                  numberToCurrency(balanceByYearsTotal?.totalEmployeeAmount || 0),
                  numberToCurrency(balanceByYearsTotal?.totalEmployeesVestedAmount || 0)
                ],
                [
                  balanceByYearsTotal?.totalMembers || 0,
                  numberToCurrency(balanceByYearsTotal?.balanceTotalAmount || 0),
                  numberToCurrency(balanceByYearsTotal?.vestedTotalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["Beneficiaries", "Employees", "Total"]}
              topRowHeaders={["All", "Count", "Balance", "Vested"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  balanceByYearsFullTime?.totalBeneficiaries || 0,
                  numberToCurrency(balanceByYearsFullTime?.totalBeneficiariesAmount || 0),
                  numberToCurrency(balanceByYearsFullTime?.totalBeneficiariesVestedAmount || 0)
                ],
                [
                  balanceByYearsFullTime?.totalEmployee || 0,
                  numberToCurrency(balanceByYearsFullTime?.totalEmployeeAmount || 0),
                  numberToCurrency(balanceByYearsFullTime?.totalEmployeesVestedAmount || 0)
                ],
                [
                  balanceByYearsFullTime?.totalMembers || 0,
                  numberToCurrency(balanceByYearsFullTime?.balanceTotalAmount || 0),
                  numberToCurrency(balanceByYearsFullTime?.vestedTotalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["Beneficiaries", "Employees", "Total"]}
              topRowHeaders={["FullTime", "Count", "Balance", "Vested"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  balanceByYearsPartTime?.totalBeneficiaries || 0,
                  numberToCurrency(balanceByYearsPartTime?.totalBeneficiariesAmount || 0),
                  numberToCurrency(balanceByYearsPartTime?.totalBeneficiariesVestedAmount || 0)
                ],
                [
                  balanceByYearsPartTime?.totalEmployee || 0,
                  numberToCurrency(balanceByYearsPartTime?.totalEmployeeAmount || 0),
                  numberToCurrency(balanceByYearsPartTime?.totalEmployeesVestedAmount || 0)
                ],
                [
                  balanceByYearsPartTime?.totalMembers || 0,
                  numberToCurrency(balanceByYearsPartTime?.balanceTotalAmount || 0),
                  numberToCurrency(balanceByYearsPartTime?.vestedTotalAmount || 0)
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
                  rowData: balanceByYearsTotal?.response.results,
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
                  rowData: balanceByYearsFullTime?.response.results,
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
                  rowData: balanceByYearsPartTime?.response.results,
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

export default BalanceByYearsGrid;
