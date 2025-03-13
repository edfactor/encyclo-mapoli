import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ProfitShareGrossReportGrid from "./ProfitShareGrossReportGrid";
import ProfitShareGrossReportParameters from "./ProifitShareGrossReportParameters";

const ProfitShareGrossReport = () => {
    return (
        <Page label={CAPTIONS.PROFIT_SHARE_GROSS_REPORT}>
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2
                    width={"100%"}>
                    <DSMAccordion title="Create">
                        <ProfitShareGrossReportParameters />
                    </DSMAccordion>
                </Grid2>

                <Grid2 width="100%">
                    <ProfitShareGrossReportGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default ProfitShareGrossReport;