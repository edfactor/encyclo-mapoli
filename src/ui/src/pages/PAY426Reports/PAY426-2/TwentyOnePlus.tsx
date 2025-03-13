import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import EighteenToTwentyGrid from "../PAY426-1/EighteenToTwentyGrid";
import TwentyOnePlusGrid from "./TwentyOnePlusGrid";
import EighteenToTwentySearchFilter from "../PAY426-1/EigteenToTwentySearchFilters";

const TwentyOnePlus = () => {
    return (
        <Page label={CAPTIONS.PAY426_ACTIVE_21_PLUS}>
            <Grid2 container rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2 width={"100%"}>
                    <DSMAccordion title="Filter">
                        <EighteenToTwentySearchFilter />
                    </DSMAccordion>
                </Grid2>
                <Grid2 width="100%">
                    <TwentyOnePlusGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default TwentyOnePlus;