import { Divider, Tabs, Tab, Button, Stack, Breadcrumbs, Link } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { Page } from "smart-ui-library";
import CleanUpSummaryCards from "./CleanUpSummaryCards";
import CleanUpSummaryGrids from "./CleanUpSummaryGrids";
import { useNavigate } from "react-router";
import StatusDropdown, { ProcessStatus } from "components/StatusDropdown";
import { MENU_LABELS } from "../../constants";

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

  const handleStatusChange = async (newStatus: ProcessStatus) => {
    console.info("Logging new status: ", newStatus);
  };

  const renderActionNode = () => {
    if (selectedTab === 0) {
      return (
        <div className="flex items-center gap-2 h-10">
          <StatusDropdown onStatusChange={handleStatusChange} />
          <Button
            onClick={() => navigate('/december-process-accordion')}
            variant="outlined"
            className="h-10 whitespace-nowrap min-w-fit"
          >
            {MENU_LABELS.DECEMBER_ACTIVITIES}
          </Button>
        </div>
      );
    }
    return null;
  };

  return (
    <Page label={selectedTab == 0 ? "Clean Up Process Summary" : tabs[selectedTab]} actionNode={renderActionNode()}>
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
