import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { ProfitShareUpdateGridColumns } from "./ProfitShareUpdateGridColumns";
import { useLazyGetProfitShareUpdateQuery } from "reduxstore/api/YearsEndApi";
import { ProfitShareUpdateRequest } from "reduxstore/types";

interface ProfitShareEditUpdateGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const ProfitShareEditUpdateGrid = ({ initialSearchLoaded, setInitialSearchLoaded }: ProfitShareEditUpdateGridProps) => {
  const [pageNumber, setPageNumber] = useState(0);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "Name",
    isSortDescending: false
  });
  const columnDefs = useMemo(() => ProfitShareUpdateGridColumns(), []);
  const { profitSharingUpdate, profitSharingUpdateQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearchUpdate, { isFetching }] = useLazyGetProfitShareUpdateQuery();

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  const onSearch = useCallback(async () => {
    const request: ProfitShareUpdateRequest = {
      pagination: {
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending,
        skip: pageNumber * pageSize,
        take: pageSize
      },
      profitYear: profitSharingUpdateQueryParams?.profitYear.getFullYear() ?? 0,
      contributionPercent: profitSharingUpdateQueryParams?.contributionPercent ?? 0,
      earningsPercent: profitSharingUpdateQueryParams?.earningsPercent ?? 0,
      incomingForfeitPercent: profitSharingUpdateQueryParams?.incomingForfeitPercent ?? 0,
      secondaryEarningsPercent: profitSharingUpdateQueryParams?.secondaryEarningsPercent ?? 0,
      maxAllowedContributions: profitSharingUpdateQueryParams?.maxAllowedContributions ?? 0,
      badgeToAdjust: profitSharingUpdateQueryParams?.badgeToAdjust ?? 0,
      adjustContributionAmount: profitSharingUpdateQueryParams?.adjustContributionAmount ?? 0,
      adjustEarningsAmount: profitSharingUpdateQueryParams?.adjustEarningsAmount ?? 0,
      adjustIncomingForfeitAmount: profitSharingUpdateQueryParams?.adjustEarningsSecondaryAmount ?? 0,
      badgeToAdjust2: profitSharingUpdateQueryParams?.badgeToAdjust2 ?? 0,
      adjustEarningsSecondaryAmount: profitSharingUpdateQueryParams?.adjustEarningsSecondaryAmount ?? 0
    };

    await triggerSearchUpdate(request, false);
  }, [pageNumber, pageSize, sortParams, triggerSearchUpdate, profitSharingUpdateQueryParams]);

  useEffect(() => {
    if (initialSearchLoaded && hasToken) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch, hasToken]);

  return (
    <>
      <div className="px-[24px]">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`Profit Share Update (PAY447)`}
        </Typography>
      </div>
      {!!profitSharingUpdate && (
        <>
          <DSMGrid
            preferenceKey={"ProfitShareUpdateGrid"}
            isLoading={false}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: "response" in profitSharingUpdate ? profitSharingUpdate.response?.results : [],
              columnDefs: columnDefs
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
            recordCount={profitSharingUpdate?.response.total ?? 0}
          />
        </>
      )}
    </>
  );
};

export default ProfitShareEditUpdateGrid;
