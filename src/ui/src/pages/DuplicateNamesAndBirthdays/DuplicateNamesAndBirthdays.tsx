import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import DuplicateNamesAndBirthdaysSearchFilter from "./DuplicateNamesAndBirthdaysSearchFilter";
import DuplicateNamesAndBirthdaysGrid from "./DuplicateNamesAndBirthdaysGrid";

const DuplicateNamesAndBirthdays = () => {
  return (
    <Page label="Duplicate Names and Birthdays">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
             <DSMAccordion title="Filter">
              <DuplicateNamesAndBirthdaysSearchFilter />
             </DSMAccordion>
          </Grid2>

          <Grid2 width="100%">
            <DuplicateNamesAndBirthdaysGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default DuplicateNamesAndBirthdays;
