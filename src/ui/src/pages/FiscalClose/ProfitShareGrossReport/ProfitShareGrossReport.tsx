import { Alert, Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import { useLazyGetFrozenStateResponseQuery } from "reduxstore/api/ItOperationsApi";
import { RootState } from "reduxstore/store";
import { CAPTIONS } from "../../../constants";
import ProfitShareGrossReportGrid from "./ProfitShareGrossReportGrid";
import ProfitShareGrossReportParameters from "./ProfitShareGrossReportSearchFilter";

const ProfitShareGrossReport = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const [frozenStateNotAvailable, setFrozenStateNotAvailable] = useState(false);
  const { grossWagesReportQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [getFrozenState] = useLazyGetFrozenStateResponseQuery();

  // Check if frozen state is available for the selected profit year
  useEffect(() => {
    const checkFrozenState = async () => {
      if (grossWagesReportQueryParams?.profitYear) {
        try {
          const result = await getFrozenState();
          if (result.data) {
            // Check if the frozen state is active and matches the requested year
            const isFrozenStateAvailable =
              result.data.isActive && result.data.profitYear === grossWagesReportQueryParams.profitYear;
            setFrozenStateNotAvailable(!isFrozenStateAvailable);
          }
        } catch (error) {
          console.error("Error checking frozen state:", error);
        }
      }
    };

    checkFrozenState();
  }, [grossWagesReportQueryParams?.profitYear, getFrozenState]);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={CAPTIONS.PROFIT_SHARE_GROSS_REPORT}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        {frozenStateNotAvailable && (
          <Grid width={"100%"}>
            <Alert severity="warning">
              Frozen data for profit year {grossWagesReportQueryParams?.profitYear} is not available. 
              Year-end close processing may not be complete for this year. Please contact IT Operations 
              if this is unexpected.
            </Alert>
          </Grid>
        )}

        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <ProfitShareGrossReportParameters setPageReset={setPageNumberReset} />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <ProfitShareGrossReportGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default ProfitShareGrossReport;
