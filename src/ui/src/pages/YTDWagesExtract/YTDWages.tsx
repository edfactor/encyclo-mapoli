import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import YTDWagesSearchFilter from "./YTDWagesSearchFilter";
import YTDWagesGrid from "./YTDWagesGrid";
import { CAPTIONS } from "../../constants";
import { Download, Print } from "@mui/icons-material";

const YTDWages = () => {
  const renderActionNode = () => {
    return (
      <div className="flex items-center gap-2 h-10">
        <Button
          onClick={() => {}}
          variant="outlined"
          startIcon={<Download color={"primary"} />}
          className="h-10 whitespace-nowrap min-w-fit">
          Download
        </Button>
        <Button
          onClick={() => {}}
          variant="outlined"
          startIcon={<Print color={"primary"} />}
          className="h-10 whitespace-nowrap min-w-fit">
          Print
        </Button>
      </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.YTD_WAGES_EXTRACT}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <YTDWagesSearchFilter />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <YTDWagesGrid />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default YTDWages;
