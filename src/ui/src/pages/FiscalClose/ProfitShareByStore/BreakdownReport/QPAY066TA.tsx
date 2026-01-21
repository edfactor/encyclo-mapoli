import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Divider, Grid } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../../constants";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { useLazyGetBreakdownGrandTotalsQuery } from "../../../../reduxstore/api/AdhocApi";
import { RootState } from "../../../../reduxstore/store";
import QPAY066TABreakdownParameters from "./QPAY066TABreakdownParameters";
import AllEmployeesContent from "./AllEmployeesContent";
import Under21Content from "./Under21Content";
import StoreContent from "./StoreContent";
import TotalsContent from "./TotalsContent";

const QPAY066TA = () => {
  const [store, setStore] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [refetchTrigger, setRefetchTrigger] = useState(0);
  const [isAnyGridExpanded, setIsAnyGridExpanded] = useState(false);
  
  const profitYear = useDecemberFlowProfitYear();
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [getBreakdownGrandTotals] = useLazyGetBreakdownGrandTotalsQuery();

  // Fetch grand totals when store === -1 (for the grids)
  // TotalsContent will fetch breakdown totals after this completes
  useEffect(() => {
    if (hasToken && store === -1) {
      getBreakdownGrandTotals({
        profitYear: profitYear
      });
    }
  }, [store, profitYear, getBreakdownGrandTotals, hasToken]);

  const handleReset = useCallback(() => {
    setStore(null);
    setRefetchTrigger(0);
  }, []);

  const handleLoadingChange = useCallback((loading: boolean) => {
    setIsLoading(loading);
  }, []);
  
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <PageErrorBoundary pageName="QPAY066TA Breakdown Report">
      <Page
        label={isAnyGridExpanded ? "" : CAPTIONS.BREAKDOWN_REPORT}
        actionNode={isAnyGridExpanded ? undefined : renderActionNode()}>
        <Grid
          container
          rowSpacing="24px">
          {!isAnyGridExpanded && (
            <Grid width="100%">
              <Divider />
            </Grid>
          )}

          {!isAnyGridExpanded && (
            <Grid width="100%">
              <DSMAccordion title="Filter">
                <QPAY066TABreakdownParameters
                  onStoreChange={(newStore) => setStore(newStore)}
                  onReset={handleReset}
                  isLoading={isLoading}
                  onSearch={() => setRefetchTrigger(prev => prev + 1)}
                  initialStore={store}
                />
              </DSMAccordion>
            </Grid>
          )}

          {!isAnyGridExpanded && store !== null && (store === -1 || store > 0) && (
            <TotalsContent store={store} onLoadingChange={handleLoadingChange} />
          )}

          {!isAnyGridExpanded && store === -1 && (
            <>
              <AllEmployeesContent />
              <Under21Content />
            </>
          )}

          {store !== null && store > 0 && (
            <StoreContent
              store={store}
              onLoadingChange={handleLoadingChange}
              refetchTrigger={refetchTrigger}
              onGridExpandChange={setIsAnyGridExpanded}
            />
          )}
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default QPAY066TA;
