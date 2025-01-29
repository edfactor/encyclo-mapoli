import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";

const ManageExecutiveHoursAndDollars = () => {
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
          December Flow
        </Button>
      </div>
    );
  };

  return (
    <Page label="Manage Executive Hours And Dollars" actionNode={renderActionNode()}>
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <ManageExecutiveHoursAndDollarsSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <ManageExecutiveHoursAndDollarsGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default ManageExecutiveHoursAndDollars;
