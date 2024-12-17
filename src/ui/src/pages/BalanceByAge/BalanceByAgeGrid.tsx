import { Typography } from "@mui/material";
import { useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetBalanceByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { GetBalanceByAgeColumns } from "./BalanceByAgeGridColumns";
import Grid2 from "@mui/material/Unstable_Grid2";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";

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
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${balanceByAgeTotal.reportName}`}
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
                  rowData: balanceByAgeTotal?.response.results,
                  pinnedTopRowData: [
                    {
                      age: "BEN",
                      employeeCount: balanceByAgeTotal?.totalBeneficiaries || 0,
                      currentBalance: balanceByAgeTotal?.totalBeneficiariesAmount,
                      vestedBalance: balanceByAgeTotal?.totalBeneficiariesVestedAmount
                    },
                    {
                      age: "EMPLOYEE",
                      employeeCount: balanceByAgeTotal?.totalEmployee || 0,
                      currentBalance: balanceByAgeTotal?.totalEmployeeAmount,
                      vestedBalance: balanceByAgeTotal?.totalEmployeesVestedAmount
                    },
                    {
                      age: "TOTAL",
                      employeeCount: balanceByAgeTotal?.totalMembers || 0,
                      currentBalance: balanceByAgeTotal?.balanceTotalAmount,
                      vestedBalance: balanceByAgeTotal?.vestedTotalAmount
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
                  rowData: balanceByAgeFullTime?.response.results,
                  pinnedTopRowData: [
                    {
                      age: "BEN",
                      employeeCount: balanceByAgeFullTime?.totalBeneficiaries || 0,
                      currentBalance: balanceByAgeFullTime?.totalBeneficiariesAmount,
                      vestedBalance: balanceByAgeFullTime?.totalBeneficiariesVestedAmount
                    },
                    {
                      age: "EMPLOYEE",
                      employeeCount: balanceByAgeFullTime?.totalEmployee || 0,
                       currentBalance: balanceByAgeFullTime?.totalEmployeeAmount,
                      vestedBalance: balanceByAgeFullTime?.totalEmployeesVestedAmount
                    },
                    {
                      age: "TOTAL",
                      employeeCount: balanceByAgeFullTime?.totalMembers || 0,
                      currentBalance: balanceByAgeFullTime?.balanceTotalAmount,
                      vestedBalance: balanceByAgeFullTime?.vestedTotalAmount
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
                  rowData: balanceByAgePartTime?.response.results,
                  pinnedTopRowData: [
                    {
                      age: "BEN",
                      employeeCount: balanceByAgePartTime?.totalBeneficiaries || 0,
                      currentBalance: balanceByAgePartTime?.totalBeneficiariesAmount,
                      vestedBalance: balanceByAgePartTime?.totalBeneficiariesVestedAmount
                    },
                    {
                      age: "EMPLOYEE",
                      employeeCount: balanceByAgePartTime?.totalEmployee || 0,
                      currentBalance: balanceByAgePartTime?.totalEmployeeAmount,
                      vestedBalance: balanceByAgePartTime?.totalEmployeesVestedAmount
                    },
                    {
                      age: "TOTAL",
                      employeeCount: balanceByAgePartTime?.totalMembers || 0,
                      currentBalance: balanceByAgePartTime?.balanceTotalAmount,
                      vestedBalance: balanceByAgePartTime?.vestedTotalAmount
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

export default BalanceByAgeGrid;
