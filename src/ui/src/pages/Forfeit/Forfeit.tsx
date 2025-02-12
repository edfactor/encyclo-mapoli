import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ForfeitSearchParameters from "./ForfeitSearchParameters";
import ForfeitGrid from "./ForfeitGrid";

const Forfeit = () => {
    return (
        <Page 
            label={CAPTIONS.FORFEIT}
            actionNode={
                <Button
                    variant="outlined"
                    onClick={() => {/* TODO: Implement download */}}
                >
                    DOWNLOAD
                </Button>
            }
        >
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2 width={"100%"}>
                    <DSMAccordion title="Filter">
                        <ForfeitSearchParameters />
                    </DSMAccordion>
                </Grid2>

                <Grid2 width="100%">
                    <ForfeitGrid />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default Forfeit;