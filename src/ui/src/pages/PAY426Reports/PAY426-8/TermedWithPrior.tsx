import { CAPTIONS } from "../../../constants";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import TermedWithPriorGrid from "./TermedWithPriorGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";

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
                <Grid2 width="100%">
                    <TermedWithPriorGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default TermedWithPrior;