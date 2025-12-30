import { Divider, Tabs, Tab } from "@mui/material";
import { Grid } from "@mui/material";
import { useState } from "react";
import { Page } from "smart-ui-library";
import { PageErrorBoundary } from "../../components/PageErrorBoundary";
import CleanUpSummaryCards from "./FrozenSummaryCards";
import CleanUpSummaryGrids from "./FrozenSummaryGrids";

const FrozenSummary = () => {
  const [selectedTab, setSelectedTab] = useState(0);

  const tabs = ["Summary", "Distributions", "Contributions", "Forfeitures", "Balance"];

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setSelectedTab(newValue);
  };

  return (
    <PageErrorBoundary pageName="Frozen Process Summary">
      <Page label="Frozen Process Summary">
        <Grid
          container
          width="100%"
          rowSpacing="24px">
          <Grid width="100%">
            <Divider />
          </Grid>
          <Grid
            container
            width="100%"
            rowSpacing="24px">
            <Grid width="100%">
              <Tabs
                sx={{ marginLeft: "24px" }}
                value={selectedTab}
                onChange={handleTabChange}>
                {tabs.map((tab, index) => (
                  <Tab
                    key={index}
                    label={tab}
                  />
                ))}
              </Tabs>
            </Grid>
            <Grid width="100%">
              {selectedTab === 0 ? (
                <CleanUpSummaryCards setSelectedTab={setSelectedTab} />
              ) : (
                <CleanUpSummaryGrids tabIndex={selectedTab} />
              )}
            </Grid>
          </Grid>
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default FrozenSummary;
