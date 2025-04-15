import { CAPTIONS } from "../../../constants";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import BeneficiariesGrid from "./BeneficiariesGrid";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { Page } from "smart-ui-library";

const renderActionNode = () => {
    return (
        <StatusDropdownActionNode />
    );
};

const Beneficiaries = () => {
    return (
        <Page 
            label={CAPTIONS.PAY426_NON_EMPLOYEE}
            actionNode={renderActionNode()}>
            <Grid2 container rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2 width="100%">
                    <BeneficiariesGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default Beneficiaries;