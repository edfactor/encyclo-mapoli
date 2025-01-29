import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareReportInputPanel from "./ProfitShareUpdateInputPanel";
import ProfitShareUpdateGrid from "./ProfitShareUpdateGrid";

const ProfitShareUpdate = () => {
    return (
        <Page label="Profit Share Update / Edit / Master">
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2
                    width={"100%"}>
                    <DSMAccordion title="Parameters">
                        <ProfitShareReportInputPanel />
                    </DSMAccordion>
                </Grid2>
                <Grid2 width="100%">
                    <ProfitShareUpdateGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default ProfitShareUpdate;
