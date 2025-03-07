import { Divider } from "@mui/material";
import { useState } from "react";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import EmployeesOnMilitaryLeaveSearchFilter from "./EmployeesOnMilitaryLeaveSearchFilter";
import EmployeesOnMilitaryLeaveGrid from "./EmployeesOnMilitaryLeaveGrid";

const EmployeesOnMilitaryLeave = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  return (
    <Page label="Employees on Military Leave">
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
