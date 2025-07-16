import { CAPTIONS } from "../../../constants";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import TermedWithHoursGrid from "./TermedWithHoursGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";
import { useState } from "react";

const renderActionNode = () => {
    return (
        <StatusDropdownActionNode />
    );
};

const TermedWithHours = () => {
    const [pageNumberReset, setPageNumberReset] = useState(false);
    
    return (
        <Page 
            label={CAPTIONS.PAY426_TERMINATED_1000_PLUS}
            actionNode={renderActionNode()}>
            <Grid2 container rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>               
                <Grid2 width="100%">
                    <TermedWithHoursGrid 
                        pageNumberReset={pageNumberReset}
                        setPageNumberReset={setPageNumberReset}
                    />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default TermedWithHours;