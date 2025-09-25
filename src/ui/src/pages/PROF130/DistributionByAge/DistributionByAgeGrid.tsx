import { Grid } from "@mui/material";
import { TotalsGrid } from "components/TotalsGrid/TotalsGrid";
import { useCallback, useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDistributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, numberToCurrency } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { GetDistributionsByAgeColumns } from "./DistributionByAgeGridColumns";
import { useGridPagination } from "../../../hooks/useGridPagination";

interface DistributionByAgeGridProps {
  initialSearchLoaded: boolean;
}

const DistributionByAgeGrid: React.FC<DistributionByAgeGridProps> = ({ initialSearchLoaded }) => {
  const {
    distributionsByAgeTotal,
    distributionsByAgeFullTime,
    distributionsByAgePartTime,
    distributionsByAgeQueryParams
  } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetDistributionsByAgeQuery();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  const { handleSortChange } = useGridPagination({
    initialPageSize: 255,
    initialSortBy: "badgeNumber",
    initialSortDescending: false,
    onPaginationChange: useCallback(() => {
      // This component doesn't use pagination, only sorting
    }, [])
  });

  const columnDefsTotal = useMemo(() => GetDistributionsByAgeColumns(FrozenReportsByAgeRequestType.Total), []);

  const onSearch = useCallback(async () => {
    await triggerSearch(
      {
        profitYear: distributionsByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.Total,
        pagination: { skip: 0, take: 255 }
      },
      false
    );
    await triggerSearch(
      {
        profitYear: distributionsByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.FullTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
    await triggerSearch(
      {
        profitYear: distributionsByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.PartTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    );
  }, [triggerSearch, distributionsByAgeQueryParams?.profitYear]);

  useEffect(() => {
    if (hasToken && initialSearchLoaded && distributionsByAgeQueryParams?.profitYear) {
      onSearch();
    }
  }, [distributionsByAgeQueryParams?.profitYear, hasToken, initialSearchLoaded, onSearch]);

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
                isLoading={isFetching}
                handleSortChanged={handleSortChange}
                providedOptions={{
                  rowData: distributionsByAgeTotal?.response?.results ?? [],
                  columnDefs: columnDefsTotal ?? []
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
                isLoading={isFetching}
                handleSortChanged={handleSortChange}
                providedOptions={{
                  rowData: distributionsByAgeFullTime?.response?.results ?? [],
                  columnDefs: columnDefsTotal ?? []
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
                isLoading={isFetching}
                handleSortChanged={handleSortChange}
                providedOptions={{
                  rowData: distributionsByAgePartTime?.response?.results ?? [],
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

export default DistributionByAgeGrid;
