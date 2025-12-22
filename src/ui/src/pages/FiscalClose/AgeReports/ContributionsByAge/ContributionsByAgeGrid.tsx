import { Grid } from "@mui/material";
import { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, numberToCurrency, TotalsGrid } from "smart-ui-library";
import { GRID_KEYS } from "../../../../constants";
import { FrozenReportsByAgeRequestType } from "../../../../reduxstore/types";
import { GetContributionsByAgeColumns } from "./ContributionsByAgeGridColumns";

const ContributionsByAgeGrid: React.FC = () => {
  const { contributionsByAgeTotal, contributionsByAgeFullTime, contributionsByAgePartTime } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const columnDefsTotal = useMemo(() => GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.Total), []);

  // No need for API calls in child component - parent handles data loading

  return (
    <>
      {contributionsByAgeTotal?.response?.results &&
        contributionsByAgeFullTime?.response?.results &&
        contributionsByAgePartTime?.response?.results && (
          <>
            <Grid
              size={{ xs: 12 }}
              container
              rowSpacing={0}
              columnSpacing={2}>
              <Grid size={{ xs: 4 }}>
                <h2 className="px-[24px] text-dsm-secondary">Total</h2>
                <TotalsGrid
                  breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                  tablePadding="0px"
                  displayData={[
                    [
                      contributionsByAgeTotal?.totalEmployees || 0,
                      numberToCurrency(contributionsByAgeTotal?.totalAmount || 0)
                    ]
                  ]}
                  leftColumnHeaders={["Amount"]}
                  topRowHeaders={["Total", "EMPS", "Amount"]}></TotalsGrid>
                <DSMGrid
                  preferenceKey={GRID_KEYS.CONTRIBUTIONS_AGE_TOTAL}
                  isLoading={false}
                  providedOptions={{
                    rowData: contributionsByAgeTotal?.response?.results ?? [],
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
                      contributionsByAgeFullTime?.totalEmployees || 0,
                      numberToCurrency(contributionsByAgeFullTime?.totalAmount || 0)
                    ]
                  ]}
                  leftColumnHeaders={["Amount"]}
                  topRowHeaders={["FullTime", "EMPS", "Amount"]}></TotalsGrid>
                <DSMGrid
                  preferenceKey={GRID_KEYS.CONTRIBUTIONS_AGE_FULLTIME}
                  isLoading={false}
                  providedOptions={{
                    rowData: contributionsByAgeFullTime?.response?.results ?? [],
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
                      contributionsByAgePartTime?.totalEmployees || 0,
                      numberToCurrency(contributionsByAgePartTime?.totalAmount || 0)
                    ]
                  ]}
                  leftColumnHeaders={["Amount"]}
                  topRowHeaders={["Total", "EMPS", "Amount"]}></TotalsGrid>
                <DSMGrid
                  preferenceKey={GRID_KEYS.CONTRIBUTIONS_AGE_PARTTIME}
                  isLoading={false}
                  providedOptions={{
                    rowData: contributionsByAgePartTime?.response?.results ?? [],
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

export default ContributionsByAgeGrid;
