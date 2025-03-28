import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DuplicateNamesAndBirthdaysGrid from "pages/DecemberActivities/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdaysGrid";
import { useState } from "react";

export const DupeNames = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
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
  );
};

export default DupeNames;
