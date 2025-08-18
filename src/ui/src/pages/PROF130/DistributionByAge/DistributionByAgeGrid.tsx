import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetDistributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
import { GetDistributionsByAgeColumns } from "./DistributionByAgeGridColumns";
import { Grid } from "@mui/material";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { numberToCurrency } from "smart-ui-library";

interface DistributionByAgeGridProps {
  initialSearchLoaded: boolean;
}

const DistributionByAgeGrid: React.FC<DistributionByAgeGridProps> = ({ initialSearchLoaded }) => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const {
    distributionsByAgeTotal,
    distributionsByAgeFullTime,
    distributionsByAgePartTime,
    distributionsByAgeQueryParams
  } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetDistributionsByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = useMemo(() => GetDistributionsByAgeColumns(FrozenReportsByAgeRequestType.Total), []);

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

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
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <div className="sticky top-0 z-10 flex bg-white">
            <TotalsGrid
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
            <TotalsGrid
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
            <TotalsGrid
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
              topRowHeaders={["PartTime", "EMPS", "Amount"]}></TotalsGrid>
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
                  rowData: distributionsByAgeTotal?.response?.results ?? [],
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
