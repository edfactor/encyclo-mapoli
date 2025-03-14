import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Page } from "smart-ui-library";
import EmployeesOnMilitaryLeaveGrid from "./EmployeesOnMilitaryLeaveGrid";
import { CAPTIONS } from "../../constants";

const EmployeesOnMilitaryLeave = () => {
  return (
    <Page label={`${CAPTIONS.EMPLOYEES_MILITARY}`}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <EmployeesOnMilitaryLeaveGrid />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default EmployeesOnMilitaryLeave;
