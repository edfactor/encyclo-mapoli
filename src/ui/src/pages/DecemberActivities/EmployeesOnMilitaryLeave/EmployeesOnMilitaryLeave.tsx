import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { Page } from "smart-ui-library";
import EmployeesOnMilitaryLeaveGrid from "./EmployeesOnMilitaryLeaveGrid";
import { CAPTIONS } from "../../../constants";

const EmployeesOnMilitaryLeave = () => {
  return (
    <Page label={`${CAPTIONS.EMPLOYEES_MILITARY}`}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <EmployeesOnMilitaryLeaveGrid />
        </Grid>
      </Grid>
    </Page>
  );
};

export default EmployeesOnMilitaryLeave;
