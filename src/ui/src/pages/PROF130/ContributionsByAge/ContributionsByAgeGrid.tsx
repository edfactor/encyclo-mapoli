import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetContributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
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
  const columnDefsFullTime = useMemo(() => GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.FullTime), []);
  const columnDefsPartTime = useMemo(() => GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.PartTime), []);

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
    /*
    triggerSearch(
      {
        profitYear: fiscalCloseProfitYear || contributionsByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.FullTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    );
    triggerSearch(
      {
        profitYear: fiscalCloseProfitYear || contributionsByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.PartTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    );
    */
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
            <div className="px-[24px]">
              <h2 className="text-dsm-secondary">Summary</h2>
            </div>
            <div className="sticky top-0 z-10 flex bg-white">
              <TotalsGrid
                displayData={[
                  [contributionsByAgeTotal?.totalEmployees || 0, numberToCurrency(contributionsByAgeTotal?.totalAmount)]
                ]}
                leftColumnHeaders={["All"]}
                topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
              <TotalsGrid
                displayData={[
                  [
                    contributionsByAgeFullTime?.totalEmployees || 0,
                    numberToCurrency(contributionsByAgeFullTime?.totalAmount || 0)
                  ]
                ]}
                leftColumnHeaders={["FullTime"]}
                topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
              <TotalsGrid
                displayData={[
                  [
                    contributionsByAgePartTime?.totalEmployees || 0,
                    numberToCurrency(contributionsByAgePartTime?.totalAmount || 0)
                  ]
                ]}
                leftColumnHeaders={["PartTime"]}
                topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
            </div>
            <Grid
              size={{ xs: 12 }}
              container>
              <Grid size={{ xs: 4 }}>
                <DSMGrid
                  preferenceKey={"AGE_Total"}
                  isLoading={isFetching}
                  handleSortChanged={sortEventHandler}
                  providedOptions={{
                    rowData: contributionsByAgeTotal?.response.results,
                    columnDefs: columnDefsTotal
                  }}
                />
              </Grid>
              <Grid size={{ xs: 4 }}>
                <DSMGrid
                  preferenceKey={"AGE_FullTime"}
                  isLoading={isFetching}
                  handleSortChanged={sortEventHandler}
                  providedOptions={{
                    rowData: contributionsByAgeFullTime?.response.results,
                    theme: "legacy",
                    columnDefs: columnDefsFullTime
                  }}
                />
              </Grid>
              <Grid size={{ xs: 4 }}>
                <DSMGrid
                  preferenceKey={"AGE_PartTime"}
                  isLoading={isFetching}
                  handleSortChanged={sortEventHandler}
                  providedOptions={{
                    rowData: contributionsByAgePartTime?.response.results,
                    columnDefs: columnDefsPartTime
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
