import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import TwentyOnePlusGrid from "./TwentyOnePlusGrid";

const renderActionNode = () => {
    return (
        <StatusDropdownActionNode />
    );
};

const TwentyOnePlus = () => {
    return (
        <Page 
            label={CAPTIONS.PAY426_ACTIVE_21_PLUS}
            actionNode={renderActionNode()}>
            <Grid2 container rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>               
                <Grid2 width="100%">
                    <TwentyOnePlusGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default TwentyOnePlus;