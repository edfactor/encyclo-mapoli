import { CAPTIONS } from "../../../constants";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import UnderEighteenGrid from "./UnderEighteenGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";

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
                <Grid2 width="100%">
                    <UnderEighteenGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default UnderEighteen;