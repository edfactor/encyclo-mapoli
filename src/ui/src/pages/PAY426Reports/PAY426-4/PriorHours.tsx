import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import EighteenToTwentySearchFilter from "../PAY426-1/EigteenToTwentySearchFilters";
import PriorHoursGrid from "./PriorHoursGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const renderActionNode = () => {
    return (
        <StatusDropdownActionNode />
    );
};

const PriorHours = () => {
    return (
        <Page 
            label={CAPTIONS.PAY426_ACTIVE_PRIOR_SHARING}
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
                    <PriorHoursGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default PriorHours;