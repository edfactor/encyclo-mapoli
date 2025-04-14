import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import EighteenToTwentySearchFilter from "../PAY426-1/EigteenToTwentySearchFilters";
import UnderEighteenGrid from "./UnderEighteenGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const renderActionNode = () => {
    return (
        <StatusDropdownActionNode />
    );
};

const UnderEighteen = () => {
    return (
        <Page 
            label={CAPTIONS.PAY426_ACTIVE_UNDER_18}
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
                    <UnderEighteenGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default UnderEighteen;