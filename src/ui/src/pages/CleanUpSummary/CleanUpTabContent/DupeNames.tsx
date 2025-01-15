import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import DuplicateNamesAndBirthdaysGrid from "pages/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdaysGrid";
import DuplicateNamesAndBirthdaysSearchFilter from "pages/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdaysSearchFilter";
import { DSMAccordion } from "smart-ui-library";

export const DupeNames = () => {
  return (
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
  );
};

export default DupeNames;
