import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import EighteenToTwentyGrid from "./EighteenToTwentyGrid";
import { useState } from "react";

const renderActionNode = () => {
    return (
        <StatusDropdownActionNode />
    );
};

const EighteenToTwenty = () => {
    const [pageNumberReset, setPageNumberReset] = useState(false);

    return (
    <Page label={CAPTIONS.PAY426_ACTIVE_18_20} actionNode={renderActionNode()}>
        <Grid2
            container
            rowSpacing="24px">
            <Grid2 width={"100%"}>
                <Divider />
            </Grid2>
            <Grid2 width="100%">
                <EighteenToTwentyGrid 
                    pageNumberReset={pageNumberReset}
                    setPageNumberReset={setPageNumberReset}
                />
            </Grid2>
        </Grid2>
    </Page>
    )
}

export default EighteenToTwenty;