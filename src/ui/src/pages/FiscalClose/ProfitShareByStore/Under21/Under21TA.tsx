import { Divider, CircularProgress, Box } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../../constants";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { useLazyGetUnder21TotalsQuery, useLazyGetUnder21InactiveQuery } from "reduxstore/api/YearsEndApi";
import Under21Summary from "./Under21Summary";
import Under21InactiveGrid from "./Under21InactiveGrid";
import useFiscalCloseProfitYear from "../../../../hooks/useFiscalCloseProfitYear";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const Under21TA = () => {
  const [fetchUnder21Totals, { isLoading: isTotalsLoading }] = useLazyGetUnder21TotalsQuery();
  const [fetchUnder21Inactive, { isLoading: isInactiveLoading }] = useLazyGetUnder21InactiveQuery();
  const under21Totals = useSelector((state: RootState) => state.yearsEnd.under21Totals);
  const under21Inactive = useSelector((state: RootState) => state.yearsEnd.under21Inactive);
  const [initialLoad, setInitialLoad] = useState(true);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const profitYear = useFiscalCloseProfitYear();

  const isLoading = isTotalsLoading || isInactiveLoading;

  const hasData = !!under21Totals && !!under21Inactive;
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  useEffect(() => {
    const fetchData = async () => {
      const queryParams = {
        profitYear,
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
        fetchUnder21Inactive(queryParams)
      ]);
      
      setInitialLoad(false);
    };
    
    fetchData();
  }, [fetchUnder21Totals, fetchUnder21Inactive, pageNumber, pageSize, sortParams, profitYear]);

  useEffect(() => {
    if (initialSearchLoaded) {
      const queryParams = {
        profitYear,
        isSortDescending: sortParams.isSortDescending,
        pagination: {
          take: pageSize,
          skip: pageNumber * pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };
      
      fetchUnder21Totals(queryParams);
      fetchUnder21Inactive(queryParams);
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, fetchUnder21Totals, fetchUnder21Inactive, profitYear]);

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
    fetchUnder21Inactive(queryParams);
  };

  return (
    <Page label={CAPTIONS.QPAY066TA_UNDER21} actionNode={renderActionNode()}>
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
                  title="UNDER 21 INACTIVE (QPAY066TA-UNDR21)"
                />
              </Grid2>
            )}

            <Grid2 width="100%">
              <Under21InactiveGrid 
                isLoading={isInactiveLoading}
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

export default Under21TA;