import { Typography } from "@mui/material";
import { useState, useMemo, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetHistoricalFrozenStateResponseQuery } from "reduxstore/api/ItOperations";
import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import { GetFreezeColumns } from "./DemographicFreezeGridColumns";

interface DemoFreezeSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const DemographicFreeze: React.FC<DemoFreezeSearchProps> = ({initialSearchLoaded, setInitialSearchLoaded }) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);

  const freezeResults = useSelector(
    (state: RootState) => state.frozen.frozenStateCollectionData
  );

  const [triggerSearch, { isFetching }] = useLazyGetHistoricalFrozenStateResponseQuery();

  const onSearch = useCallback(async () => {
    const request = {
      skip: pageNumber * pageSize, 
      take: pageSize, 
      sortBy: "createdDateTime", 
      isSortDescending: true
    };

    await triggerSearch(request, false);
  }, [pageNumber, pageSize, triggerSearch]);

  // First useEffect to trigger the search on initial render
  useEffect(() => {
    if (!hasToken) return;
    onSearch();
  }, [onSearch, hasToken]); // Only depends on onSearch, will execute once when component mounts

  // Second useEffect to handle pagination changes
  useEffect(() => {
    // Skip the initial render since we already triggered the search
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [pageNumber, pageSize, initialSearchLoaded, onSearch]);

  const columnDefs = useMemo(() => GetFreezeColumns(), []);

  return (
    <>
      {freezeResults && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`Previous Freezes`}
            </Typography>
          </div>
          <DSMGrid
            preferenceKey={"FREEZE"}
            isLoading={isFetching}
            providedOptions={{
              rowData: freezeResults?.results,
              columnDefs: columnDefs
            }}
          />
        </>
      )}
      {!!freezeResults && freezeResults.results.length > 0 && (
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
          recordCount={freezeResults.total}
        />
      )}
    </>
  );
};

export default DemographicFreeze;