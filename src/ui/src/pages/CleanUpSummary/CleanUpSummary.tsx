import { Divider, Tabs, Tab, Button } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { Page } from "smart-ui-library";
import CleanUpSummaryCards from "./CleanUpSummaryCards";
import CleanUpSummaryGrids from "./CleanUpSummaryGrids";
import { useNavigate } from "react-router";

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
  const navigate = useNavigate();

  return (
    <Page label={selectedTab == 0 ? "Clean Up Process Summary" : tabs[selectedTab]} actionNode={selectedTab == 0 ? <Button onClick={() => navigate('/december-process-accordion')} variant="outlined">December Flow</Button> : null}>
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
