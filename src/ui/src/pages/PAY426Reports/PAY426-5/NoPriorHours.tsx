import { CAPTIONS } from "../../../constants";
import { DSMAccordion, Page } from "smart-ui-library";
import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import EighteenToTwentySearchFilter from "../PAY426-1/EigteenToTwentySearchFilters";
import NoPriorHoursGrid from "./NoPriorHoursGrid";

const NoPriorHours = () => {
    return (
        <Page label={CAPTIONS.PAY426_ACTIVE_NO_PRIOR}>
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
                    <NoPriorHoursGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default NoPriorHours;