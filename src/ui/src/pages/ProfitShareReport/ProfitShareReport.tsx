import { useState } from "react";
import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareReportSearchFilter from "./ProfitShareReportSearchFilter";
import ProfitShareReportGrid from "./ProfitShareReportGrid";
import StatusDropdown, { ProcessStatus } from "components/StatusDropdown";
import { useNavigate } from "react-router";
import { MENU_LABELS } from "../../constants";

const ProfitShareReport = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const navigate = useNavigate();

  const handleStatusChange = async (newStatus: ProcessStatus) => {
    console.info("Logging new status: ", newStatus);
  };

  const renderActionNode = () => {
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
  };

  return (
    <Page label="Profit Share Report" actionNode={renderActionNode()}>
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <ProfitShareReportSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <ProfitShareReportGrid
              initialSearchLoaded={initialSearchLoaded}
              setInitialSearchLoaded={setInitialSearchLoaded}
             />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default ProfitShareReport;
