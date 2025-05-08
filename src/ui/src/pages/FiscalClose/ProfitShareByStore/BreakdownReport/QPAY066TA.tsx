import { Divider, Tab, Tabs } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import QPAY066TABreakdownParameters from "./QPAY066TABreakdownParameters";
import StoreContent from "./StoreContent";
import SummariesContent from "./SummariesContent";
import TotalsContent from "./TotalsContent";

const QPAY066TA = () => {
  const [tabValue, setTabValue] = useState(0);
  const [store, setStore] = useState(700);

  const tabs = ["ALL", "STORES", "SUMMARIES", "TOTALS"];

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
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
          <Grid2
            container
            width="100%"
            direction="column"
            spacing={4}>
            <StoreContent store={store} />
            <TotalsContent store={store} />
            <SummariesContent store={store} />
          </Grid2>
        );
      case 1:
        return <StoreContent store={store} />;
      case 2:
        return <SummariesContent store={store} />;
      case 3:
        return <TotalsContent store={store} />;
      default:
        return null;
    }
  };

  return (
    <Page label={CAPTIONS.BREAKDOWN_REPORT}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width="100%">
          <Divider />
        </Grid2>

        <Grid2 width="100%">
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
        </Grid2>

        <Grid2 width="100%">
          <DSMAccordion title="Filter">
            <QPAY066TABreakdownParameters
              activeTab={getActiveTab()}
              onStoreChange={(newStore) => setStore(newStore)}
            />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">{renderContent()}</Grid2>
      </Grid2>
    </Page>
  );
};

export default QPAY066TA;
