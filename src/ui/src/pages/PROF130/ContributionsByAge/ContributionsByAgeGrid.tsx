import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetContributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
import { GetContributionsByAgeColumns } from "./ContributionsByAgeGridColumns";
import Grid2 from "@mui/material/Grid2";
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
  const [triggerSearch, { isLoading }] = useLazyGetContributionsByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetContributionsByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const onSearch = useCallback(async () => {
    triggerSearch(
      {
        profitYear: fiscalCloseProfitYear || contributionsByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.Total,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
    triggerSearch(
      {
        profitYear: fiscalCloseProfitYear || contributionsByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.FullTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
    triggerSearch(
      {
        profitYear: fiscalCloseProfitYear || contributionsByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.PartTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
  }, [contributionsByAgeQueryParams?.profitYear, triggerSearch, fiscalCloseProfitYear]);

  useEffect(() => {
    if (initialSearchLoaded && contributionsByAgeQueryParams?.profitYear) {
      onSearch();
    }
  }, [contributionsByAgeQueryParams?.profitYear, initialSearchLoaded, onSearch]);

  return (
    <>
      {contributionsByAgeTotal?.response && (
        <>
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <div className="flex sticky top-0 z-10 bg-white">
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
          <Grid2
            size={{ xs: 12 }}
            container>
            <Grid2 size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_Total"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: contributionsByAgeTotal?.response.results,

                  columnDefs: [
                    {
                      headerName: columnDefsTotal.headerName,
                      children: columnDefsTotal.children
                    }
                  ]
                }}
              />
            </Grid2>
            <Grid2 size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_FullTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: contributionsByAgeFullTime?.response.results,
                  theme: "legacy",
                  columnDefs: [
                    {
                      headerName: columnDefsFullTime.headerName,
                      children: columnDefsFullTime.children
                    }
                  ]
                }}
              />
            </Grid2>
            <Grid2 size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_PartTime"}
                isLoading={isLoading}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: contributionsByAgePartTime?.response.results,

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

export default ContributionsByAgeGrid;
