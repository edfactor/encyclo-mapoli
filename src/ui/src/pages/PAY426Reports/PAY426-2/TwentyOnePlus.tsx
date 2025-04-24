import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import TwentyOnePlusGrid from "./TwentyOnePlusGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";

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