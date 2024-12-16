import { Typography } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBalanceByYearsQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { GetBalanceByYearsColumns } from "./BalanceByYearsGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";

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
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${balanceByYearsTotal.reportName}`}
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
                  rowData: balanceByYearsTotal?.response.results,
                  pinnedTopRowData: [
                    {
                      years: "BEN",
                      employeeCount: balanceByYearsTotal?.totalBeneficiaries || 0,
                      currentBalance: balanceByYearsTotal?.totalBeneficiariesAmount,
                      vestedBalance: balanceByYearsTotal?.totalBeneficiariesVestedAmount
                    },
                    {
                      years: "EMPLOYEE",
                      employeeCount: balanceByYearsTotal?.totalEmployee || 0,
                      currentBalance: balanceByYearsTotal?.totalEmployeeAmount,
                      vestedBalance: balanceByYearsTotal?.totalEmployeesVestedAmount
                    },
                    {
                      years: "TOTAL",
                      employeeCount: balanceByYearsTotal?.totalMembers || 0,
                      currentBalance: balanceByYearsTotal?.balanceTotalAmount,
                      vestedBalance: balanceByYearsTotal?.vestedTotalAmount
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
                  rowData: balanceByYearsFullTime?.response.results,
                  pinnedTopRowData: [
                    {
                      years: "BEN",
                      employeeCount: balanceByYearsFullTime?.totalBeneficiaries || 0,
                      currentBalance: balanceByYearsFullTime?.totalBeneficiariesAmount,
                      vestedBalance: balanceByYearsFullTime?.totalBeneficiariesVestedAmount
                    },
                    {
                      years: "EMPLOYEE",
                      employeeCount: balanceByYearsFullTime?.totalEmployee || 0,
                       currentBalance: balanceByYearsFullTime?.totalEmployeeAmount,
                      vestedBalance: balanceByYearsFullTime?.totalEmployeesVestedAmount
                    },
                    {
                      years: "TOTAL",
                      employeeCount: balanceByYearsFullTime?.totalMembers || 0,
                      currentBalance: balanceByYearsFullTime?.balanceTotalAmount,
                      vestedBalance: balanceByYearsFullTime?.vestedTotalAmount
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
                  rowData: balanceByYearsPartTime?.response.results,
                  pinnedTopRowData: [
                    {
                      years: "BEN",
                      employeeCount: balanceByYearsPartTime?.totalBeneficiaries || 0,
                      currentBalance: balanceByYearsPartTime?.totalBeneficiariesAmount,
                      vestedBalance: balanceByYearsPartTime?.totalBeneficiariesVestedAmount
                    },
                    {
                      years: "EMPLOYEE",
                      employeeCount: balanceByYearsPartTime?.totalEmployee || 0,
                      currentBalance: balanceByYearsPartTime?.totalEmployeeAmount,
                      vestedBalance: balanceByYearsPartTime?.totalEmployeesVestedAmount
                    },
                    {
                      years: "TOTAL",
                      employeeCount: balanceByYearsPartTime?.totalMembers || 0,
                      currentBalance: balanceByYearsPartTime?.balanceTotalAmount,
                      vestedBalance: balanceByYearsPartTime?.vestedTotalAmount
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

export default BalanceByYearsGrid;
