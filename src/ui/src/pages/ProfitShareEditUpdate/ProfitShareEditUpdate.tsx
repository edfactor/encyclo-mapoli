import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareReportInputPanel from "./ProfitShareEditUpdateSearchFilter";
import ProfitShareEditUpdateGrid from "./ProfitShareEditUpdateGrid";

const developmentNoteStyle = {
  backgroundColor: "#FFFFE0", // Light yellow
  padding: "10px",
  margin: "10px"
};

const ProfitShareEditUpdate = () => {
  return (
    <Page label="Profit Share Update / Edit / Master (PAY444 PAY447 PAY460 PROFTLD)">
      <div style={developmentNoteStyle}>
        DevNote: This page is functional but incomplete; needs Totals block, needs Adjustment Details, and needs Paging.{" "}
        <a
          style={{ color: "blue" }}
          href="https://demoulas.atlassian.net/browse/PS-945">
          PS-945
        </a>
      </div>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Parameters">
            <ProfitShareReportInputPanel />
          </DSMAccordion>
        </Grid2>
        <Grid2 width="100%">
          <ProfitShareEditUpdateGrid />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareEditUpdate;
