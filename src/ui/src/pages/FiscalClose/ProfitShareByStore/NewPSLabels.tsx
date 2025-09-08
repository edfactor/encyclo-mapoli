import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { Page, TotalsGrid } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import Under21ReportGrid from "./Under21ReportGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const NewPSLabels = () => {
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };
  return (
    <Page
      label={CAPTIONS.NEW_PS_LABELS}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <div className="sticky top-0 z-10 flex bg-white">
          <TotalsGrid
            displayData={[
              ["Under 21 100% Vested", "1"],
              ["Under 21 Partially Vested", "0"],
              ["Under 21 with 1-2 PS Years", "20"]
            ]}
            leftColumnHeaders={[]}
            topRowHeaders={["", "Active Employees"]}
          />

          <TotalsGrid
            displayData={[
              ["Under 21 100% Vested", "0"],
              ["Under 21 Partially Vested", "0"],
              ["Under 21 with 1-2 PS Years", "20"]
            ]}
            leftColumnHeaders={[]}
            topRowHeaders={["", "Terminated Employees"]}
          />

          <TotalsGrid
            displayData={[
              ["Under 21 100% Vested", "1"],
              ["Under 21 Partially Vested", "0"],
              ["Under 21 with 1-2 PS Years", "0"]
            ]}
            leftColumnHeaders={[]}
            topRowHeaders={["", "Inactive Employees"]}
          />
        </div>

        <Grid width="100%">
          <Under21ReportGrid />
        </Grid>
      </Grid>
    </Page>
  );
};

export default NewPSLabels;
