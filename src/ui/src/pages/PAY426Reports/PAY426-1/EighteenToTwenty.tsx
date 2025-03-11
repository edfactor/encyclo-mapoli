import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import EighteenToTwentySearchFilter from "./EigteenToTwentySearchFilters";
import EighteenToTwentyGrid from "./EighteenToTwentyGrid";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';

const EighteenToTwenty = () => {

    return (<Page label={CAPTIONS.PAY426_ACTIVE_18_20}>
        <Grid2
            container
            rowSpacing="24px">
            <Grid2 width={"100%"}>
                <Divider />
            </Grid2>
            <Grid2
                width={"100%"}>
                <DSMAccordion title="Filter">
                    <EighteenToTwentySearchFilter />
                </DSMAccordion>
            </Grid2>

            <Grid2 width="100%">
                <EighteenToTwentyGrid />
            </Grid2>
        </Grid2>
    </Page>
    )
}

export default EighteenToTwenty;