import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import EighteenToTwentySearchFilter from "../PAY426-1/EigteenToTwentySearchFilters";
import TermedWithHoursGrid from "./TermedWithHoursGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

const renderActionNode = () => {
    return (
        <StatusDropdownActionNode />
    );
};

const TermedWithHours = () => {
    return (
        <Page 
            label={CAPTIONS.PAY426_TERMINATED_1000_PLUS}
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
                    <TermedWithHoursGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default TermedWithHours;