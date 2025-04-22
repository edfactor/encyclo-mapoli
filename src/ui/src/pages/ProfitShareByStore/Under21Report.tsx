import { Divider, CircularProgress, Box } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { useLazyGetUnder21TotalsQuery, useLazyGetUnder21BreakdownByStoreQuery } from "reduxstore/api/YearsEndApi";
import Under21Summary from "./Under21/Under21Summary";
import Under21BreakdownGrid from "./Under21/Under21BreakdownGrid";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";

const Under21Report = () => {
  const [fetchUnder21Totals, { isLoading: isTotalsLoading }] = useLazyGetUnder21TotalsQuery();
  const [fetchUnder21Breakdown, { isLoading: isBreakdownLoading }] = useLazyGetUnder21BreakdownByStoreQuery();
  const under21Totals = useSelector((state: RootState) => state.yearsEnd.under21Totals);
  const under21Breakdown = useSelector((state: RootState) => state.yearsEnd.under21BreakdownByStore);
  const [initialLoad, setInitialLoad] = useState(true);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const profitYear = useDecemberFlowProfitYear();
  const isLoading = isTotalsLoading || isBreakdownLoading;
  const hasData = !!under21Totals && !!under21Breakdown;

  useEffect(() => {
    const fetchData = async () => {
      const queryParams = {
        profitYear: profitYear,
        isSortDescending: sortParams.isSortDescending,
        pagination: {
          take: pageSize,
          skip: pageNumber * pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };
      
      await Promise.all([
        fetchUnder21Totals(queryParams),
        fetchUnder21Breakdown(queryParams)
      ]);
      
      setInitialLoad(false);
    };
    
    fetchData();
  }, [fetchUnder21Totals, fetchUnder21Breakdown, pageNumber, pageSize, sortParams]);

  useEffect(() => {
    if (initialSearchLoaded) {
      const queryParams = {
        profitYear: profitYear,
        isSortDescending: sortParams.isSortDescending,
        pagination: {
          take: pageSize,
          skip: pageNumber * pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };
      
      fetchUnder21Totals(queryParams);
      fetchUnder21Breakdown(queryParams);
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, fetchUnder21Totals, fetchUnder21Breakdown]);

  const handleSearch = (profitYear: number, isSortDescending: boolean) => {
    const queryParams = {
      profitYear,
      isSortDescending,
      pagination: {
        take: pageSize,
        skip: pageNumber * pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending
      }
    };
    
    fetchUnder21Totals(queryParams);
    fetchUnder21Breakdown(queryParams);
  };

  return (
    <Page label={CAPTIONS.QPAY066_UNDER21}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>

        {initialLoad ? (
          <Grid2 width="100%">
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
              <CircularProgress />
            </Box>
          </Grid2>
        ) : (
          <>
            {hasData && !isLoading && (
              <Grid2 width="100%" paddingX="24px">
                <Under21Summary 
                  totals={under21Totals} 
                  isLoading={isTotalsLoading} 
                  title="UNDER 21 REPORT (QPAY066-UNDR21)"
                />
              </Grid2>
            )}

            <Grid2 width="100%">
              <Under21BreakdownGrid 
                isLoading={isBreakdownLoading}
                initialSearchLoaded={initialSearchLoaded}
                setInitialSearchLoaded={setInitialSearchLoaded}
                pageNumber={pageNumber}
                setPageNumber={setPageNumber}
                pageSize={pageSize}
                setPageSize={setPageSize}
                sortParams={sortParams}
                setSortParams={setSortParams}
              />
            </Grid2>
          </>
        )}
      </Grid2>
    </Page>
  );
};

export default Under21Report;
