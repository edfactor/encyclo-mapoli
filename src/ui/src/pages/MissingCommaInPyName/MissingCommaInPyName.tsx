import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { Page } from "smart-ui-library";
import MissingCommaInPyNameGrid from "./MissingCommaInPyNameGrid";

const MissingCommaInPyName = () => {
  return (
    <Page label="Missing Comma in Full Name">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <MissingCommaInPyNameGrid />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default MissingCommaInPyName;
