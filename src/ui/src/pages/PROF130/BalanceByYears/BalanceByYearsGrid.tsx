import { useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, numberToCurrency } from "smart-ui-library";
import { TotalsGrid } from "components/TotalsGrid/TotalsGrid";
import { GetBalanceByYearsGridColumns } from "./BalanceByYearsGridColumns";
import { Grid } from "@mui/material";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
// numberToCurrency imported above

interface BalanceByYearsGridProps {
  initialSearchLoaded: boolean;
}

const BalanceByYearsGrid: React.FC<BalanceByYearsGridProps> = ({ initialSearchLoaded }) => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { balanceByYearsTotal, balanceByYearsFullTime, balanceByYearsPartTime } =
    useSelector((state: RootState) => state.yearsEnd);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetBalanceByYearsGridColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetBalanceByYearsGridColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetBalanceByYearsGridColumns(FrozenReportsByAgeRequestType.PartTime);

  // No need for API calls in child component - parent handles data loading

  return (
    <>
      {balanceByYearsTotal?.response && (
        <>
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <Grid
            size={{ xs: 12 }}
            container
            columnSpacing={2}
            rowSpacing={0}>
            <Grid size={{ xs: 4 }}>
              <h2 className="text-dsm-secondary">Total</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12 }}
                tablePadding="0px"
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
            </Grid>
            <Grid size={{ xs: 4 }}>
              <h2 className="text-dsm-secondary">Full-time</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12 }}
                tablePadding="0px"
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
            </Grid>
            <Grid size={{ xs: 4 }}>
              <h2 className="text-dsm-secondary">Part-time</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12 }}
                tablePadding="0px"
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
            </Grid>
          </Grid>
          <Grid
            size={{ xs: 12 }}
            container
            columnSpacing={2}
            rowSpacing={0}>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_Total"}
                isLoading={false}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: balanceByYearsTotal?.response.results ?? [],
                  columnDefs: columnDefsTotal ?? []
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_FullTime"}
                isLoading={false}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: balanceByYearsFullTime?.response.results ?? [],
                  columnDefs: columnDefsFullTime ?? []
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_PartTime"}
                isLoading={false}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: balanceByYearsPartTime?.response.results ?? [],
                  columnDefs: columnDefsPartTime ?? []
                }}
              />
            </Grid>
          </Grid>
        </>
      )}
    </>
  );
};

export default BalanceByYearsGrid;
