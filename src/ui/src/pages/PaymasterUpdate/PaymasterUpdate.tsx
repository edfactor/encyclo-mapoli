import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { CAPTIONS } from "../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import PaymasterUpdateParameters from "./PaymasterUpdateParameters";
import PaymasterUpdateResults from "./PaymasterUpdateResults";

const PaymasterUpdate = () => {
    return (
        <Page label={CAPTIONS.PAYMASTER_UPDATE}>
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2
                    width={"100%"}>
                    <DSMAccordion title="Filter">
                        <PaymasterUpdateParameters />
                    </DSMAccordion>
                </Grid2>

                <Grid2 width="100%">
                    <PaymasterUpdateResults />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default PaymasterUpdate;