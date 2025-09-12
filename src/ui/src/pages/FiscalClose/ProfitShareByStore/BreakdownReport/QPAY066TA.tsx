import { Divider, Tab, Tabs } from "@mui/material";
import { Grid } from "@mui/material";
import { useState, useCallback } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../../constants";
import QPAY066TABreakdownParameters from "./QPAY066TABreakdownParameters";
import StoreContent from "./StoreContent";
import SummariesContent from "./SummariesContent";
import TotalsContent from "./TotalsContent";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const QPAY066TA = () => {
  const [tabValue, setTabValue] = useState(0);
  const [store, setStore] = useState<number | null>(null);
  const tabs = ["ALL", "STORE", "TOTALS", "SUMMARIES"];

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleReset = useCallback(() => {
    setStore(null);
    setTabValue(0);
  }, []);
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const getActiveTab = (): "all" | "stores" | "summaries" | "totals" => {
    switch (tabValue) {
      case 0:
        return "all";
      case 1:
        return "stores";
      case 2:
        return "summaries";
      case 3:
        return "totals";
      default:
        return "all";
    }
  };

  const renderContent = () => {
    switch (tabValue) {
      case 0:
        return (
          <Grid
            container
            width="100%"
            direction="column"
            spacing={4}>
            <StoreContent store={store} />
            <TotalsContent store={store} />
            <SummariesContent />
          </Grid>
        );
      case 1:
        return <StoreContent store={store} />;
      case 2:
        return <TotalsContent store={store} />;
      case 3:
        return <SummariesContent />;
      default:
        return null;
    }
  };

  return (
    <Page
      label={CAPTIONS.BREAKDOWN_REPORT}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width="100%">
          <Divider />
        </Grid>

        <Grid width="100%">
          <DSMAccordion title="Filter">
            <QPAY066TABreakdownParameters
              activeTab={getActiveTab()}
              onStoreChange={(newStore) => setStore(newStore)}
              onReset={handleReset}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <Tabs
            value={tabValue}
            onChange={handleTabChange}
            aria-label="pay share by store tabs"
            sx={{ marginLeft: "24px" }}>
            {tabs.map((tab, index) => (
              <Tab
                key={index}
                label={tab}
                id={`tab-${index}`}
                aria-controls={`tabpanel-${index}`}
              />
            ))}
          </Tabs>
        </Grid>

        {store && <Grid width="100%">{renderContent()}</Grid>}
      </Grid>
    </Page>
  );
};

export default QPAY066TA;
