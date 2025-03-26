import { Divider, CircularProgress, Box } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { useLazyGetUnder21TotalsQuery, useLazyGetUnder21InactiveQuery } from "reduxstore/api/YearsEndApi";
import Under21SearchFilters from "./Under21SearchFilters";
import Under21Summary from "./Under21/Under21Summary";
import Under21InactiveGrid from "./Under21/Under21InactiveGrid";

const Under21TA = () => {
  const [fetchUnder21Totals, { isLoading: isTotalsLoading }] = useLazyGetUnder21TotalsQuery();
  const [fetchUnder21Inactive, { isLoading: isInactiveLoading }] = useLazyGetUnder21InactiveQuery();
  const under21Totals = useSelector((state: RootState) => state.yearsEnd.under21Totals);
  const under21Inactive = useSelector((state: RootState) => state.yearsEnd.under21Inactive);
  const [initialLoad, setInitialLoad] = useState(true);

  const isLoading = isTotalsLoading || isInactiveLoading;

  const hasData = !!under21Totals && !!under21Inactive;

  useEffect(() => {
    const fetchData = async () => {
      const queryParams = {
        profitYear: 2024,
        isSortDescending: true,
        pagination: {
          take: 255,
          skip: 0
        }
      };
      
      await Promise.all([
        fetchUnder21Totals(queryParams),
        fetchUnder21Inactive(queryParams)
      ]);
      
      setInitialLoad(false);
    };
    
    fetchData();
  }, [fetchUnder21Totals, fetchUnder21Inactive]);

  const handleSearch = (profitYear: number, isSortDescending: boolean) => {
    const queryParams = {
      profitYear,
      isSortDescending,
      pagination: {
        take: 255,
        skip: 0
      }
    };
    
    fetchUnder21Totals(queryParams);
    fetchUnder21Inactive(queryParams);
  };

  return (
    <Page label={CAPTIONS.QPAY066TA_UNDER21}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <Under21SearchFilters onSearch={handleSearch} isLoading={isLoading} />
          </DSMAccordion>
        </Grid2>

        {initialLoad || isLoading ? (
          <Grid2 width="100%">
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
              <CircularProgress />
            </Box>
          </Grid2>
        ) : hasData && (
          <>
            <Grid2 width="100%" paddingX="24px">
              <Under21Summary 
                totals={under21Totals} 
                isLoading={isTotalsLoading}
                title="UNDER 21 INACTIVE (QPAY066TA-UNDR21)"
              />
            </Grid2>

            <Grid2 width="100%">
              <Under21InactiveGrid />
            </Grid2>
          </>
        )}
      </Grid2>
    </Page>
  );
};

export default Under21TA;