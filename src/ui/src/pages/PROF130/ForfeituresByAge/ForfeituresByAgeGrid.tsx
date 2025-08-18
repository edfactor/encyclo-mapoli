import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetForfeituresByAgeQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, TotalsGrid } from "smart-ui-library";
import { GetForfeituresByAgeColumns } from "./ForfeituresByAgeGridColumns";
import { Grid } from "@mui/material";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { numberToCurrency } from "smart-ui-library";

interface ForfeituresByAgeGridProps {
  initialSearchLoaded: boolean;
}

const ForfeituresByAgeGrid: React.FC<ForfeituresByAgeGridProps> = ({ initialSearchLoaded }) => {
  const [_discard0, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { forfeituresByAgeTotal, forfeituresByAgeFullTime, forfeituresByAgePartTime, forfeituresByAgeQueryParams } =
    useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetForfeituresByAgeQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const columnDefsTotal = GetForfeituresByAgeColumns(FrozenReportsByAgeRequestType.Total);
  const columnDefsFullTime = GetForfeituresByAgeColumns(FrozenReportsByAgeRequestType.FullTime);
  const columnDefsPartTime = GetForfeituresByAgeColumns(FrozenReportsByAgeRequestType.PartTime);

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  const onSearch = useCallback(async () => {
    await triggerSearch(
      {
        profitYear: forfeituresByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.Total,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
    await triggerSearch(
      {
        profitYear: forfeituresByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.FullTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
    await triggerSearch(
      {
        profitYear: forfeituresByAgeQueryParams?.profitYear || 0,
        reportType: FrozenReportsByAgeRequestType.PartTime,
        pagination: { skip: 0, take: 255 }
      },
      false
    ).unwrap();
  }, [triggerSearch, forfeituresByAgeQueryParams?.profitYear]);

  useEffect(() => {
    if (hasToken && initialSearchLoaded && forfeituresByAgeQueryParams?.profitYear) {
      onSearch();
    }
  }, [forfeituresByAgeQueryParams?.profitYear, hasToken, initialSearchLoaded, onSearch]);

  return (
    <>
      {forfeituresByAgeTotal?.response && (
        <>
          <div className="px-[24px]">
            <h2 className="text-dsm-secondary">Summary</h2>
          </div>
          <div className="sticky top-0 z-10 flex bg-white">
            <TotalsGrid
              displayData={[
                [forfeituresByAgeTotal?.totalEmployees || 0, numberToCurrency(forfeituresByAgeTotal?.totalAmount)]
              ]}
              leftColumnHeaders={["All"]}
              topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  forfeituresByAgeFullTime?.totalEmployees || 0,
                  numberToCurrency(forfeituresByAgeFullTime?.totalAmount || 0)
                ]
              ]}
              leftColumnHeaders={["FullTime"]}
              topRowHeaders={["", "EMPS", "Amount"]}></TotalsGrid>
            <TotalsGrid
              displayData={[
                [
                  forfeituresByAgePartTime?.totalEmployees || 0,
                  numberToCurrency(forfeituresByAgePartTime?.totalAmount || 0)
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
                  rowData: forfeituresByAgeTotal?.response.results || [],
                  columnDefs: columnDefsTotal || []
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_FullTime"}
                isLoading={isFetching}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: forfeituresByAgeFullTime?.response.results || [],
                  columnDefs: columnDefsFullTime || []
                }}
              />
            </Grid>
            <Grid size={{ xs: 4 }}>
              <DSMGrid
                preferenceKey={"AGE_PartTime"}
                isLoading={isFetching}
                handleSortChanged={sortEventHandler}
                providedOptions={{
                  rowData: forfeituresByAgePartTime?.response.results || [],
                  columnDefs: columnDefsPartTime || []
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
