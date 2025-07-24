import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
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
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <DuplicateNamesAndBirthdaysGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DuplicateNamesAndBirthdays;
