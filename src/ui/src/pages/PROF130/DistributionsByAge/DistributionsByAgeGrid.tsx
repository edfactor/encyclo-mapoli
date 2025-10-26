import { Grid } from "@mui/material";
import { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, TotalsGrid, numberToCurrency } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { GetDistributionsByAgeColumns } from "./DistributionsByAgeGridColumns";

const DistributionByAgeGrid: React.FC = () => {
  const { distributionsByAgeTotal, distributionsByAgeFullTime, distributionsByAgePartTime } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const columnDefsTotal = useMemo(() => GetDistributionsByAgeColumns(FrozenReportsByAgeRequestType.Total), []);

  // No need for API calls in child component - parent handles data loading

  return (
    <>
      {distributionsByAgeTotal?.response && (
        <>
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
                    distributionsByAgeTotal?.regularTotalEmployees || 0,
                    numberToCurrency(distributionsByAgeTotal?.regularTotalAmount || 0)
                  ],
                  [
                    distributionsByAgeTotal?.hardshipTotalEmployees || 0,
                    numberToCurrency(distributionsByAgeTotal?.hardshipTotalAmount || 0)
                  ],
                  [
                    distributionsByAgeTotal?.totalEmployees || 0,
                    numberToCurrency(distributionsByAgeTotal?.distributionTotalAmount || 0)
                  ]
                ]}
                leftColumnHeaders={["Regular", "Hardship", "Dist Total"]}
                topRowHeaders={["Total", "EMPS", "Amount"]}></TotalsGrid>
              <DSMGrid
                preferenceKey={"DIST_AGE_Total"}
                isLoading={false}
                providedOptions={{
                  rowData: distributionsByAgeTotal?.response?.results ?? [],
                  columnDefs: columnDefsTotal ?? [],
                  suppressHorizontalScroll: true,
                  suppressColumnVirtualisation: true
                }}
              />
            </Grid>

            <Grid size={{ xs: 4 }}>
              <h2 className="text-dsm-secondary">Full-time</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                tablePadding="0px"
                displayData={[
                  [
                    distributionsByAgeFullTime?.regularTotalEmployees || 0,
                    numberToCurrency(distributionsByAgeFullTime?.regularTotalAmount || 0)
                  ],
                  [
                    distributionsByAgeFullTime?.hardshipTotalEmployees || 0,
                    numberToCurrency(distributionsByAgeFullTime?.hardshipTotalAmount || 0)
                  ],
                  [
                    distributionsByAgeFullTime?.totalEmployees || 0,
                    numberToCurrency(distributionsByAgeFullTime?.distributionTotalAmount || 0)
                  ]
                ]}
                leftColumnHeaders={["Regular", "Hardship", "Dist Total"]}
                topRowHeaders={["FullTime", "EMPS", "Amount"]}></TotalsGrid>
              <DSMGrid
                preferenceKey={"DIST_AGE_FullTime"}
                isLoading={false}
                providedOptions={{
                  rowData: distributionsByAgeFullTime?.response?.results ?? [],
                  columnDefs: columnDefsTotal ?? [],
                  suppressHorizontalScroll: true,
                  suppressColumnVirtualisation: true
                }}
              />
            </Grid>

            <Grid size={{ xs: 4 }}>
              <h2 className="text-dsm-secondary">Part-time</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                tablePadding="0px"
                displayData={[
                  [
                    distributionsByAgePartTime?.regularTotalEmployees || 0,
                    numberToCurrency(distributionsByAgePartTime?.regularTotalAmount || 0)
                  ],
                  [
                    distributionsByAgePartTime?.hardshipTotalEmployees || 0,
                    numberToCurrency(distributionsByAgePartTime?.hardshipTotalAmount || 0)
                  ],
                  [
                    distributionsByAgePartTime?.totalEmployees || 0,
                    numberToCurrency(distributionsByAgePartTime?.distributionTotalAmount || 0)
                  ]
                ]}
                leftColumnHeaders={["Regular", "Hardship", "Dist Total"]}
                topRowHeaders={["Total", "EMPS", "Amount"]}></TotalsGrid>
              <DSMGrid
                preferenceKey={"DIST_AGE_PartTime"}
                isLoading={false}
                providedOptions={{
                  rowData: distributionsByAgePartTime?.response?.results ?? [],
                  columnDefs: columnDefsTotal ?? [],
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

export default DistributionByAgeGrid;
