import { Button, Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ForfeitSearchFilter from "./ForfeitSearchFilter";
import ForfeitGrid from "./ForfeitGrid";

const Forfeit = () => {
  return (
    <Page
      label={CAPTIONS.FORFEIT}
      actionNode={
        <Button
          variant="outlined"
          disabled={true}
          onClick={() => {
            /* TODO: Implement download */
          }}>
          DOWNLOAD
        </Button>
      }>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ForfeitSearchFilter />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <ForfeitGrid />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default Forfeit;
