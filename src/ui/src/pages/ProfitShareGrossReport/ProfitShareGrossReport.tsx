import { Divider } from "@mui/material";
import { useState } from "react";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ProfitShareGrossReportGrid from "./ProfitShareGrossReportGrid";
import ProfitShareGrossReportParameters from "./ProifitShareGrossReportParameters";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const ProfitShareGrossReport = () => {
    const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

    const renderActionNode = () => {
        return (
            <StatusDropdownActionNode />
        );
    };
    
    return (
        <Page label={CAPTIONS.PROFIT_SHARE_GROSS_REPORT} actionNode={renderActionNode()}>
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2
                    width={"100%"}>
                    <DSMAccordion title="Filter">
                        <ProfitShareGrossReportParameters />
                    </DSMAccordion>
                </Grid2>

                <Grid2 width="100%">
                    <ProfitShareGrossReportGrid initialSearchLoaded={initialSearchLoaded} setInitialSearchLoaded={setInitialSearchLoaded}/>
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default ProfitShareGrossReport;