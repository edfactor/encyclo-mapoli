import { Divider, Tabs, Tab } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { Page } from "smart-ui-library";
import CleanUpSummaryCards from "./CleanUpSummaryCards";
import CleanUpSummaryGrids from "./CleanUpSummaryGrids";

const CleanUpSummary = () => {
  const [selectedTab, setSelectedTab] = useState(0);

  const tabs = [
    "Summary",
    "Negative ETVA",
    "Payroll Duplicate SSNs on Demographics",
    "Demographic Badges Not On Payprofit",
    "Duplicate Names and Birthdays"
  ];

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setSelectedTab(newValue);
  };

  return (
    <Page label="Clean Up Process Summary">
      <Grid2
        container
        width="100%"
        rowSpacing="24px">
        <Grid2 width="100%">
          <Divider />
        </Grid2>
        <Grid2
          container
          width="100%"
          rowSpacing="24px">
          <Grid2 width="100%">
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
          </Grid2>
          <Grid2 width="100%">
            {selectedTab === 0 ? (
              <CleanUpSummaryCards setSelectedTab={setSelectedTab} />
            ) : (
              <CleanUpSummaryGrids tabIndex={selectedTab} />
            )}
          </Grid2>
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default CleanUpSummary;
