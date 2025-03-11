import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetMilitaryAndRehireProfitSummaryQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { GetMilitaryAndRehireProfitSummaryColumns } from "./MilitaryAndRehireProfitSummaryGridColumns";

interface MilitaryAndRehireForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const MilitaryAndRehireProfitSummaryGrid: React.FC<MilitaryAndRehireForfeituresGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Badge",
    isSortDescending: false
  });

  const { militaryAndRehireProfitSummary, militaryAndRehireProfitSummaryQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [triggerSearch, { isFetching }] = useLazyGetMilitaryAndRehireProfitSummaryQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: militaryAndRehireProfitSummaryQueryParams?.profitYear ?? 0,
      reportingYear: militaryAndRehireProfitSummaryQueryParams?.reportingYear ?? "",
      pagination: { skip: pageNumber * pageSize, take: pageSize }
    };

    await triggerSearch(request, false);
  }, [
    militaryAndRehireProfitSummaryQueryParams?.profitYear,
    militaryAndRehireProfitSummaryQueryParams?.reportingYear,
    pageNumber,
    pageSize,
    triggerSearch
  ]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
  const columnDefs = useMemo(() => GetMilitaryAndRehireProfitSummaryColumns(), []);

  return (
    <>
      {militaryAndRehireProfitSummary?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Military and Rehire Profit Summary(${militaryAndRehireProfitSummary?.response.total || 0})`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"DUPE_SSNS"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: militaryAndRehireProfitSummary?.response.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!militaryAndRehireProfitSummary && militaryAndRehireProfitSummary.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={militaryAndRehireProfitSummary.response.total}
        />
      )}
    </>
  );
};

export default MilitaryAndRehireProfitSummaryGrid;
