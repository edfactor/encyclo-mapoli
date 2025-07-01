import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitShareEditQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { ProfitShareUpdateRequest } from "reduxstore/types";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { ProfitShareEditUpdateGridColumns } from "./ProfitShareEditGridColumns";

interface ProfitShareEditGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const ProfitShareEditGrid = ({ initialSearchLoaded, setInitialSearchLoaded }: ProfitShareEditGridProps) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "name",
    isSortDescending: false
  });

  const editColumnDefs = useMemo(() => ProfitShareEditUpdateGridColumns(), []);
  const { profitSharingEdit, profitSharingEditQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearchUpdate, { isFetching }] = useLazyGetProfitShareEditQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const onSearch = useCallback(async () => {
    const request: ProfitShareUpdateRequest = {
      pagination: {
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending,
        skip: pageNumber * pageSize,
        take: pageSize
      },
      profitYear: profitSharingEditQueryParams?.profitYear.getFullYear() ?? 0,
      contributionPercent: profitSharingEditQueryParams?.contributionPercent ?? 0,
      earningsPercent: profitSharingEditQueryParams?.earningsPercent ?? 0,
      incomingForfeitPercent: profitSharingEditQueryParams?.incomingForfeitPercent ?? 0,
      secondaryEarningsPercent: profitSharingEditQueryParams?.secondaryEarningsPercent ?? 0,
      maxAllowedContributions: profitSharingEditQueryParams?.maxAllowedContributions ?? 0,
      badgeToAdjust: profitSharingEditQueryParams?.badgeToAdjust ?? 0,
      adjustContributionAmount: profitSharingEditQueryParams?.adjustContributionAmount ?? 0,
      adjustEarningsAmount: profitSharingEditQueryParams?.adjustEarningsAmount ?? 0,
      adjustIncomingForfeitAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0,
      badgeToAdjust2: profitSharingEditQueryParams?.badgeToAdjust2 ?? 0,
      adjustEarningsSecondaryAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0
    };

    await triggerSearchUpdate(request, false);
  }, [pageNumber, pageSize, sortParams, triggerSearchUpdate, profitSharingEditQueryParams]);

  useEffect(() => {
    if (initialSearchLoaded && hasToken) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch, hasToken]);

  // Need a useEffect to reset the page number when data changes
  const prevProfitSharingEdit = useRef<any>(null);
  useEffect(() => {
    if (profitSharingEdit?.response?.results && profitSharingEdit.response.results.length > 0 &&
        (prevProfitSharingEdit.current === null || 
         profitSharingEdit.response.results.length !== prevProfitSharingEdit.current.response.results.length)) {
      setPageNumber(0);
    }
    prevProfitSharingEdit.current = profitSharingEdit;
  }, [profitSharingEdit]);

  return (
    <>
      <div className="px-[24px]">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`Profit Share Edit (PAY447)`}
        </Typography>
      </div>
      {!!profitSharingEdit && (
        <>
          <DSMGrid
            preferenceKey={"ProfitShareEditGrid"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: "response" in profitSharingEdit ? profitSharingEdit.response?.results : [],
              columnDefs: editColumnDefs
            }}
          />
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
            recordCount={profitSharingEdit?.response.total ?? 0}
          />
        </>
      )}
    </>
  );
};

export default ProfitShareEditGrid;
