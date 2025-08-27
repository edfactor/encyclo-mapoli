import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetContributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams } from "smart-ui-library";
import { TotalsGrid } from "components/TotalsGrid/TotalsGrid";
import { GetContributionsByAgeColumns } from "./ContributionsByAgeGridColumns";
import { Grid } from "@mui/material";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { numberToCurrency } from "smart-ui-library";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

interface ContributionsByAgeGridProps {
  initialSearchLoaded: boolean;
}

const ContributionsByAgeGrid: React.FC<ContributionsByAgeGridProps> = ({ initialSearchLoaded }) => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const {
    contributionsByAgeTotal,
    contributionsByAgeFullTime,
    contributionsByAgePartTime,
    contributionsByAgeQueryParams
  } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetContributionsByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = useMemo(() => GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.Total), []);

  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  const onSearch = useCallback(async () => {
    triggerSearch(
      {
        profitYear: fiscalCloseProfitYear || contributionsByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.Total,
        pagination: { skip: 0, take: 255 }
      },
      false
    );

  }, []);

  useEffect(() => {
    if (hasToken && initialSearchLoaded && contributionsByAgeQueryParams?.profitYear) {
      onSearch();
    }
  }, [contributionsByAgeQueryParams?.profitYear, hasToken, initialSearchLoaded, onSearch]);

  return (
    <>
      {contributionsByAgeTotal?.response?.results &&
        contributionsByAgeFullTime?.response?.results &&
        contributionsByAgePartTime?.response?.results && (
          <>
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
                      contributionsByAgeTotal?.totalEmployees || 0,
                      numberToCurrency(contributionsByAgeTotal?.totalAmount || 0)
                    ]
                  ]}
                  leftColumnHeaders={["Amount"]}
                  topRowHeaders={["Total", "EMPS", "Amount"]}></TotalsGrid>
        <DSMGrid
                  preferenceKey={"CONT_AGE_Total"}
                  isLoading={isFetching}
                  handleSortChanged={sortEventHandler}
                  providedOptions={{
          rowData: contributionsByAgeTotal?.response?.results ?? [],
                    columnDefs: columnDefsTotal ?? []
                  }}
                />
              </Grid>

              <Grid size={{ xs: 4 }}>
                <h2 className="text-dsm-secondary">Full-time</h2>
                <TotalsGrid
                  breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12 }}
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
                  preferenceKey={"CONT_AGE_FullTime"}
                  isLoading={isFetching}
                  handleSortChanged={sortEventHandler}
                  providedOptions={{
          rowData: contributionsByAgeFullTime?.response?.results ?? [],
                    columnDefs: columnDefsTotal ?? []
                  }}
                />
              </Grid>

              <Grid size={{ xs: 4 }}>
                <h2 className="text-dsm-secondary">Part-time</h2>
        <TotalsGrid
                  breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12 }}
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
                  preferenceKey={"CONT_AGE_PartTime"}
                  isLoading={isFetching}
                  handleSortChanged={sortEventHandler}
                  providedOptions={{
        rowData: contributionsByAgePartTime?.response?.results ?? [],
                    columnDefs: columnDefsTotal ?? []
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
