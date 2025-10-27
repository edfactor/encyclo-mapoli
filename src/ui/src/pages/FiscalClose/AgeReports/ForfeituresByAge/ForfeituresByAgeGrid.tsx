import { Grid } from "@mui/material";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, numberToCurrency, TotalsGrid } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../../../reduxstore/types";
import { GetForfeituresByAgeColumns } from "./ForfeituresByAgeGridColumns";

const ForfeituresByAgeGrid: React.FC = () => {
  const { forfeituresByAgeTotal, forfeituresByAgeFullTime, forfeituresByAgePartTime } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const columnDefsTotal = GetForfeituresByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetForfeituresByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetForfeituresByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  // No need for API calls in child component - parent handles data loading

  return (
    <>
      {forfeituresByAgeTotal?.response && (
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
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12 }}
                tablePadding="0px"
                displayData={[
                  [forfeituresByAgeTotal?.totalEmployees || 0, numberToCurrency(forfeituresByAgeTotal?.totalAmount)]
                ]}
                leftColumnHeaders={["All"]}
                topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
            </Grid>
            <Grid size={{ xs: 4 }}>
              <h2 className="text-dsm-secondary">Full-time</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12 }}
                tablePadding="0px"
                displayData={[
                  [
                    forfeituresByAgeFullTime?.totalEmployees || 0,
                    numberToCurrency(forfeituresByAgeFullTime?.totalAmount || 0)
                  ]
                ]}
                leftColumnHeaders={["FullTime"]}
                topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
            </Grid>
            <Grid size={{ xs: 4 }}>
              <h2 className="text-dsm-secondary">Part-time</h2>
              <TotalsGrid
                breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12 }}
                tablePadding="0px"
                displayData={[
                  [
                    forfeituresByAgePartTime?.totalEmployees || 0,
                    numberToCurrency(forfeituresByAgePartTime?.totalAmount || 0)
                  ]
                ]}
                leftColumnHeaders={["PartTime"]}
                topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
            </Grid>
          </Grid>
          <Grid
            size={{ xs: 12 }}
            container
            rowSpacing={0}>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_Total"}
                isLoading={false}
                providedOptions={{
                  rowData: forfeituresByAgeTotal?.response.results ?? [],
                  columnDefs: columnDefsTotal || [],
                  suppressHorizontalScroll: true,
                  suppressColumnVirtualisation: true
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_FullTime"}
                isLoading={false}
                providedOptions={{
                  rowData: forfeituresByAgeFullTime?.response.results ?? [],
                  columnDefs: columnDefsFullTime || [],
                  suppressHorizontalScroll: true,
                  suppressColumnVirtualisation: true
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_PartTime"}
                isLoading={false}
                providedOptions={{
                  rowData: forfeituresByAgePartTime?.response.results ?? [],
                  columnDefs: columnDefsPartTime || [],
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

export default ForfeituresByAgeGrid;
