import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { useState } from "react";
import { Page } from "smart-ui-library";
import DuplicateNamesAndBirthdaysGrid from "./DuplicateNamesAndBirthdaysGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const DuplicateNamesAndBirthdays = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };
  return (
    <Page
      label="Duplicate Names and Birthdays"
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <DuplicateNamesAndBirthdaysGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DuplicateNamesAndBirthdays;
