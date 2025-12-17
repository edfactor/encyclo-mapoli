import { Grid } from "@mui/material";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, numberToCurrency, TotalsGrid } from "smart-ui-library";
import { GRID_KEYS } from "../../../../constants";
import { FrozenReportsByAgeRequestType } from "../../../../reduxstore/types";
import { GetBalanceByAgeColumns } from "./BalanceByAgeGridColumns";

const BalanceByAgeGrid: React.FC = () => {
  const { balanceByAgeTotal, balanceByAgeFullTime, balanceByAgePartTime } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const columnDefsTotal = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetBalanceByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  // No need for API calls in child component - parent handles data loading

  return (
    <>
      {balanceByAgeTotal?.response && (
        <>
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <Grid
            size={{ xs: 12 }}
            container
            rowSpacing={0}>
            <Grid size={{ xs: 4 }}>
              <h2 className="px-[24px] text-dsm-secondary">Total</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                tablePadding="0px"
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
            </Grid>
            <Grid size={{ xs: 4 }}>
              <h2 className="text-dsm-secondary">Full-time</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                tablePadding="0px"
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
            </Grid>
            <Grid size={{ xs: 4 }}>
              <h2 className="text-dsm-secondary">Part-time</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                tablePadding="0px"
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
            </Grid>
          </Grid>
          <Grid
            size={{ xs: 12 }}
            container
            rowSpacing={0}>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={GRID_KEYS.BALANCE_AGE_TOTAL}
                isLoading={false}
                providedOptions={{
                  rowData: balanceByAgeTotal?.response.results ?? [],
                  columnDefs: columnDefsTotal ?? [],
                  suppressHorizontalScroll: true,
                  suppressColumnVirtualisation: true
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={GRID_KEYS.BALANCE_AGE_FULLTIME}
                isLoading={false}
                providedOptions={{
                  rowData: balanceByAgeFullTime?.response.results ?? [],
                  columnDefs: columnDefsFullTime ?? [],
                  suppressHorizontalScroll: true,
                  suppressColumnVirtualisation: true
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={GRID_KEYS.BALANCE_AGE_PARTTIME}
                isLoading={false}
                providedOptions={{
                  rowData: balanceByAgePartTime?.response.results ?? [],
                  columnDefs: columnDefsPartTime ?? [],
                  suppressHorizontalScroll: true,
                  suppressColumnVirtualisation: true
                }}
              />
            </Grid>
          </Grid>
        </>
      )}
    </>
  );
};

export default BalanceByAgeGrid;
