import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareReportEditRunParameters from "./ProfitShareReportEditRunParameters";
import { CAPTIONS } from "../../../constants";
import ProfitShareReportEditRunResults from "./ProfitShareReportEditRunResults";

const ProfitShareReportEditRun = () => {

    return (
        <Page label={CAPTIONS.PROFIT_SHARE_REPORT_EDIT_RUN}>
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2
                    width={"100%"}>
                    <DSMAccordion title="Filter">
                        <ProfitShareReportEditRunParameters />
                    </DSMAccordion>
                </Grid2>

                <Grid2 width="100%">
                    <ProfitShareReportEditRunResults />
                </Grid2>
            </Grid2>
        </Page>
    )


};

export default ProfitShareReportEditRun;