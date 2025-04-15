import { CAPTIONS } from "../../../constants";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import NoPriorHoursGrid from "./NoPriorHoursGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";

const renderActionNode = () => {
    return (
        <StatusDropdownActionNode />
    );
};

const NoPriorHours = () => {
    return (
        <Page 
            label={CAPTIONS.PAY426_ACTIVE_NO_PRIOR}
            actionNode={renderActionNode()}>
            <Grid2 container rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2 width="100%">
                    <NoPriorHoursGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default NoPriorHours;