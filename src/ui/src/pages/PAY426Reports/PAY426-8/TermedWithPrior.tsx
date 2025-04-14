import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import EighteenToTwentySearchFilter from "../PAY426-1/EigteenToTwentySearchFilters";
import TermedWithPriorGrid from "./TermedWithPriorGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const renderActionNode = () => {
    return (
        <StatusDropdownActionNode />
    );
};

const TermedWithPrior = () => {
    return (
        <Page 
            label={CAPTIONS.PAY426_TERMINATED_PRIOR}
            actionNode={renderActionNode()}>
            <Grid2 container rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2 width={"100%"}>
                    <DSMAccordion title="Filter">
                        <EighteenToTwentySearchFilter />
                    </DSMAccordion>
                </Grid2>
                <Grid2 width="100%">
                    <TermedWithPriorGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default TermedWithPrior;