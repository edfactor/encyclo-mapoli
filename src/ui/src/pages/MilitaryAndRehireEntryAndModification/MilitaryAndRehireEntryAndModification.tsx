import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireSearchFilter from "./MilitaryAndRehireEntryAndModificationSearchFilter";
import MilitaryAndRehireEntryAndModificationGrid from "./MilitaryAndRehireEntryAndModificationGrid";
import { useNavigate } from "react-router";
import StatusDropdown, { ProcessStatus } from "components/StatusDropdown";
import { MENU_LABELS } from "../../constants";

const MilitaryAndRehireEntryAndModification = () => {
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
    <Page label="Military Entry and Modification (008-13)" actionNode={renderActionNode()}>
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <MilitaryAndRehireSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <MilitaryAndRehireEntryAndModificationGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default MilitaryAndRehireEntryAndModification;
